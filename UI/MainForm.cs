using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.UI.Form;
using DiabloTwoMFTimer.UI.Pomodoro;
using DiabloTwoMFTimer.UI.Settings;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.UI.Timer;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI;

public partial class MainForm : System.Windows.Forms.Form
{
    private readonly IMainService _mainService = null!;
    private readonly IAppSettings _appSettings = null!;
    private readonly IProfileService _profileService = null!;
    private readonly ITimerHistoryService _timerHistoryService = null!;
    private readonly IMessenger _messenger = null!;
    private readonly ISceneService _sceneService = null!;

    private readonly TimerControl _timerControl = null!;
    private readonly PomodoroControl _pomodoroControl = null!;
    private NotifyIcon _notifyIcon = null!;
    private ContextMenuStrip _trayMenu = null!;
    private readonly ICommandDispatcher _commandDispatcher = null!;
    private readonly IKeyMapRepository _keyMapRepository = null!;
    private readonly CommandInitializer _commandInitializer = null!;
    private readonly LeaderKeyForm _leaderKeyForm = null!;
    private System.ComponentModel.IContainer _components = null!;
    private readonly IAudioService _audioService = null!;

    public MainForm()
    {
        InitializeComponent();
        this.tlpNavigation.Height = Theme.UISizeConstants.TabItemHeight;
        UpdateNavButtonStyles(btnNavTimer);
    }

    public MainForm(
        IMainService mainService,
        IAppSettings settings,
        IProfileService profileService,
        ITimerHistoryService timerHistoryService,
        ISceneService sceneService,
        IMessenger messenger,
        TimerControl timerControl,
        PomodoroControl pomodoroControl,
        ICommandDispatcher commandDispatcher,
        CommandInitializer commandInitializer,
        IKeyMapRepository keyMapRepository,
        IAudioService audioService
    )
        : this()
    {
        _mainService = mainService;
        _appSettings = settings;
        _profileService = profileService;
        _timerHistoryService = timerHistoryService;
        _sceneService = sceneService;
        _timerControl = timerControl;
        _pomodoroControl = pomodoroControl;
        _keyMapRepository = keyMapRepository;
        _commandDispatcher = commandDispatcher;
        _commandInitializer = commandInitializer;
        _messenger = messenger;
        _audioService = audioService;

        _commandInitializer.Initialize();
        _leaderKeyForm = new LeaderKeyForm(_commandDispatcher, _keyMapRepository);
        InitializeChildControls();
        InitializeForm();
        InitializeSystemTray();
        InitializeAudio();
        SubscribeToEvents();
        SubscribeToMessages();
        Utils.Toast.RegisterUiInvoker(action => this.SafeInvoke(action));
        this.Shown += OnMainForm_Shown;
    }

    private void InitializeAudio()
    {
        try
        {
            _audioService.Initialize();
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("MainForm", "初始化音频服务失败", ex);
        }
    }

    internal TabControl? TabControl => tabControl;

    private void OnMainForm_Shown(object? sender, EventArgs e)
    {
        _mainService.InitializeApplication(this.Handle);
        this.MoveWindowToPosition(this);

        // 核心逻辑：启动时自动加载上次的配置
        LoadLastRunSettings();

        // 核心逻辑：如果没有角色档案，自动触发创建
        CheckAndCreateInitialProfile();

        _mainService.SetActiveTabPage(Models.TabPage.Timer);

        this.Opacity = _appSettings.Opacity;
    }

    // 检查并创建初始角色
    private void CheckAndCreateInitialProfile()
    {
        var profiles = _profileService.GetAllProfiles();
        if (profiles == null || profiles.Count == 0)
        {
            LogManager.WriteDebugLog("MainForm", "检测到无角色档案，自动触发创建流程");
            // 延迟调用以确保窗体完全加载
            this.BeginInvoke(new Action(() =>
            {
                // 手动触发创建请求
                OnCreateCharacterRequested(new CreateCharacterMessage());
            }));
        }
    }

    private void LoadLastRunSettings()
    {
        string lastUsedProfileName = _appSettings.LastUsedProfile;

        if (!string.IsNullOrWhiteSpace(lastUsedProfileName))
        {
            var profile = _profileService.FindProfileByName(lastUsedProfileName);
            if (profile != null)
            {
                _profileService.SwitchCharacter(profile);

                if (!string.IsNullOrEmpty(profile.LastRunScene))
                {
                    _profileService.CurrentScene = profile.LastRunScene;
                }

                _profileService.CurrentDifficulty = profile.LastRunDifficulty;
                LogManager.WriteDebugLog("MainForm", $"已恢复上次状态: {profile.Name}, {profile.LastRunScene}, {profile.LastRunDifficulty}");
            }
        }
    }

    private void NavButton_Click(object sender, EventArgs e)
    {
        if (sender == btnNavTimer)
            tabControl.SelectedIndex = (int)Models.TabPage.Timer;
        else if (sender == btnNavPomodoro)
            tabControl.SelectedIndex = (int)Models.TabPage.Pomodoro;
        else if (sender == btnNavSettings)
        {
            _mainService.RequestShowSettings();
            return;
        }
        else if (sender == btnNavMinimize)
        {
            HandleCloseOption();
            return;
        }

        UpdateNavButtonStyles((ThemedButton)sender);
    }

    private void HandleCloseOption()
    {
        using var form = new CloseOptionForm();
        var result = form.ShowDialog(this);

        if (result == DialogResult.OK)
        {
            if (form.IsCloseAppSelected)
            {
                _mainService.HandleApplicationClosing();
                Application.Exit();
            }
            else
            {
                if (tabControl.SelectedIndex != (int)Models.TabPage.Timer)
                {
                    tabControl.SelectedIndex = (int)Models.TabPage.Timer;
                    UpdateNavButtonStyles(btnNavTimer);
                }
                else
                {
                    UpdateNavButtonStyles(btnNavTimer);
                }
                this.WindowState = FormWindowState.Minimized;
            }
        }
    }

    private void UpdateNavButtonStyles(ThemedButton activeBtn)
    {
        var buttons = new[] { btnNavTimer, btnNavPomodoro, btnNavSettings };
        foreach (var btn in buttons)
        {
            if (btn != null)
                btn.IsSelected = (btn == activeBtn);
        }

        if (btnNavProfile != null) btnNavProfile.IsSelected = false;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using var pen = new Pen(DiabloTwoMFTimer.UI.Theme.AppTheme.AccentColor, 1);
        e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
    }

    private void InitializeChildControls()
    {
        static void AddControlToTab(System.Windows.Forms.TabPage page, Control control)
        {
            control.Dock = DockStyle.Fill;
            page.Controls.Add(control);
        }

        AddControlToTab(tabTimerPage, _timerControl);
        AddControlToTab(tabPomodoroPage, _pomodoroControl);
    }

    private void InitializeForm()
    {
        AdjustWindowHeight();
        this.StartPosition = FormStartPosition.Manual;
        this.ShowInTaskbar = true;
        this.Opacity = 0;
        this.TopMost = _appSettings.AlwaysOnTop;
        tlpNavigation.Visible = _appSettings.ShowNavigation;

        if (btnNavProfile != null)
        {
            btnNavProfile.Visible = false;
        }
    }

    private void InitializeSystemTray()
    {
        _components = new Container();

        _trayMenu = new ContextMenuStrip(_components);
        var exitItem = new ToolStripMenuItem("退出程序");
        exitItem.Click += (s, e) =>
        {
            _mainService.HandleApplicationClosing();
            Application.Exit();
        };
        _trayMenu.Items.Add(exitItem);

        _notifyIcon = new NotifyIcon(_components)
        {
            Text = "D2R Timer",
            Icon = new Icon("Resources\\d2r.ico"),
            Visible = true,
            ContextMenuStrip = _trayMenu,
        };

        _notifyIcon.MouseDoubleClick += (s, e) =>
        {
            RestoreFromTray();
        };

        this.Resize += OnMainForm_Resize;
    }

    private void OnMainForm_Resize(object? sender, EventArgs e)
    {
        if (this.WindowState == FormWindowState.Minimized)
        {
            this.ShowInTaskbar = false;
            this.Hide();
            _notifyIcon.Visible = true;
        }
    }

    private void RestoreFromTray()
    {
        if (!this.Visible)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Activate();
            _mainService.ReloadHotkeys();
        }
    }

    private void SubscribeToEvents()
    {
        _mainService.OnRequestTabChange += (tabPage) =>
        {
            if (tabControl != null && (int)tabPage < tabControl.TabCount)
            {
                this.SafeInvoke(() =>
                {
                    tabControl.SelectedIndex = (int)tabPage;
                    if ((int)tabPage == 1) UpdateNavButtonStyles(btnNavTimer);
                    else if ((int)tabPage == 2) UpdateNavButtonStyles(btnNavPomodoro);
                });
            }
        };

        _mainService.OnRequestRefreshUI += () => this.SafeInvoke(UpdateFormTitleAndTabs);

        _mainService.OnRequestDeleteHistory += () =>
        {
            this.SafeInvoke(() =>
            {
                if (tabControl.SelectedIndex == (int)Models.TabPage.Timer)
                {
                    _ = _timerControl.DeleteSelectedRecordAsync();
                }
            });
        };

        _mainService.OnRequestDeleteLastHistory += () =>
        {
            this.SafeInvoke(() =>
            {
                if (tabControl.SelectedIndex == (int)Models.TabPage.Timer)
                {
                    _ = _timerControl.DeleteLastHistoryRecordAsync();
                }
            });
        };

        _mainService.OnRequestDeleteLastLoot += () =>
        {
            this.SafeInvoke(() =>
            {
                if (tabControl.SelectedIndex == (int)Models.TabPage.Timer)
                {
                    _ = _timerControl.DeleteLastLootRecordAsync();
                }
            });
        };
    }

    private void SubscribeToMessages()
    {
        _messenger.Subscribe<WindowPositionChangedMessage>(_ =>
            this.SafeInvoke(() => this.MoveWindowToPosition(this))
        );
        _messenger.Subscribe<AlwaysOnTopChangedMessage>(_ =>
            this.SafeInvoke(() => this.TopMost = _appSettings.AlwaysOnTop)
        );
        _messenger.Subscribe<OpacityChangedMessage>(_ => this.SafeInvoke(() => this.Opacity = _appSettings.Opacity));
        _messenger.Subscribe<HideMainWindowMessage>(_ => this.SafeInvoke(() => this.Opacity = 0));
        _messenger.Subscribe<ShowMainWindowMessage>(_ => this.SafeInvoke(() => this.Opacity = _appSettings.Opacity));
        _messenger.Subscribe<AdjustWindowHeightMessage>(msg => this.SafeInvoke(() => AdjustWindowHeight()));
        _messenger.Subscribe<ScreenshotRequestedMessage>(OnScreenshotRequested);
        _messenger.Subscribe<ShowLeaderKeyFormMessage>(_ =>
            this.SafeInvoke(() =>
            {
                if (_leaderKeyForm.Visible)
                {
                    _leaderKeyForm.Hide();
                }
                else
                {
                    _leaderKeyForm.StartPosition = FormStartPosition.Manual;
                    var screen = Screen.FromControl(this);
                    _leaderKeyForm.Location = new Point(screen.Bounds.X, screen.Bounds.Bottom - _leaderKeyForm.Height);
                    _leaderKeyForm.Width = screen.Bounds.Width;

                    _leaderKeyForm.Show();
                    _leaderKeyForm.Activate();
                }
            })
        );

        _messenger.Subscribe<MinimizeToTrayMessage>(_ =>
            this.SafeInvoke(() =>
            {
                this.WindowState = FormWindowState.Minimized;
            })
        );

        _messenger.Subscribe<RestoreFromTrayMessage>(_ =>
            this.SafeInvoke(() =>
            {
                RestoreFromTray();
            })
        );

        _messenger.Subscribe<ToggleWindowVisibilityMessage>(_ =>
            this.SafeInvoke(() =>
            {
                if (this.Visible)
                {
                    this.WindowState = FormWindowState.Minimized;
                }
                else
                {
                    RestoreFromTray();
                }
            })
        );

        _messenger.Subscribe<ShowRecordLootFormMessage>(_ => this.SafeInvoke(ShowRecordLootDialog));
        _messenger.Subscribe<NavigationVisibilityChangedMessage>(_ =>
            this.SafeInvoke(() =>
            {
                tlpNavigation.Visible = _appSettings.ShowNavigation;
            })
        );

        _messenger.Subscribe<CreateCharacterMessage>(OnCreateCharacterRequested);
        _messenger.Subscribe<SwitchCharacterMessage>(OnSwitchCharacterRequested);
        _messenger.Subscribe<ExportCharacterMessage>(OnExportCharacterRequested);
        _messenger.Subscribe<ShowLootHistoryMessage>(OnShowLootHistoryRequested);
        _messenger.Subscribe<DeleteCharacterMessage>(OnDeleteCharacterRequested);

        _messenger.Subscribe<RequestSelectDifficultyMessage>(OnRequestSelectDifficulty);
    }

    private void OnCreateCharacterRequested(CreateCharacterMessage message)
    {
        this.SafeInvoke(() =>
        {
            // 确保传入 _sceneService
            using var form = new DiabloTwoMFTimer.UI.Profiles.CreateCharacterForm(_profileService, _sceneService);

            if (form.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(form.CharacterName))
            {
                Utils.Toast.Success($"角色 '{form.CharacterName}' 创建成功并已选中！");
                _mainService.SetActiveTabPage(Models.TabPage.Timer);
                _messenger.Publish(new TimerSettingsChangedMessage());
            }
        });
    }

    private void OnSwitchCharacterRequested(SwitchCharacterMessage message)
    {
        this.SafeInvoke(() =>
        {
            using var form = new DiabloTwoMFTimer.UI.Profiles.SwitchCharacterForm(_profileService);
            if (form.ShowDialog(this) == DialogResult.OK && form.SelectedProfile != null)
            {
                var selectedProfile = form.SelectedProfile;
                if (_profileService.SwitchCharacter(selectedProfile))
                {
                    _appSettings.LastUsedProfile = selectedProfile.Name;
                    _appSettings.Save();

                    if (!string.IsNullOrEmpty(selectedProfile.LastRunScene))
                        _profileService.CurrentScene = selectedProfile.LastRunScene;
                    _profileService.CurrentDifficulty = selectedProfile.LastRunDifficulty;

                    _mainService.SetActiveTabPage(Models.TabPage.Timer);
                    _messenger.Publish(new TimerSettingsChangedMessage());
                    Utils.Toast.Success($"已切换到角色 '{selectedProfile.Name}'");
                }
            }
        });
    }

    private void OnDeleteCharacterRequested(DeleteCharacterMessage message)
    {
        this.SafeInvoke(() =>
        {
            var currentProfile = _profileService.CurrentProfile;
            if (currentProfile == null)
            {
                Utils.Toast.Warning("当前没有选择角色");
                return;
            }

            string confirmMsg = $"确定要删除角色: {currentProfile.Name}?";
            if (DiabloTwoMFTimer.UI.Components.ThemedMessageBox.Show(
                    confirmMsg,
                    LanguageManager.GetString("DeleteCharacter") ?? "删除角色",
                    MessageBoxButtons.YesNo
                ) == DialogResult.Yes)
            {
                if (_profileService.DeleteCharacter(currentProfile))
                {
                    Utils.Toast.Success($"已成功删除角色");
                    // 核心逻辑：删除后检查是否为空，是则触发创建
                    CheckAndCreateInitialProfile();
                    _messenger.Publish(new TimerSettingsChangedMessage());
                }
            }
        });
    }

    private void OnExportCharacterRequested(ExportCharacterMessage message)
    {
        this.SafeInvoke(() =>
        {
            try
            {
                string dirPath = Utils.FolderManager.AppDataPath;
                if (!System.IO.Directory.Exists(dirPath))
                    System.IO.Directory.CreateDirectory(dirPath);

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = dirPath,
                    UseShellExecute = true,
                    Verb = "open",
                });
            }
            catch (Exception ex)
            {
                LogManager.WriteErrorLog("MainForm", "打开档案文件夹失败", ex);
                Utils.Toast.Error($"无法打开文件夹: {ex.Message}");
            }
        });
    }

    private void OnShowLootHistoryRequested(ShowLootHistoryMessage message)
    {
        this.SafeInvoke(() =>
        {
            // 注意：StatisticsService 的注入仍需您在构造函数中补充，此处保持占位
            var historyForm = new DiabloTwoMFTimer.UI.Timer.LootHistoryForm(
                _mainService,
                _profileService,
                _sceneService,
                 null!,
                _messenger
            );

            _messenger.Publish(new HideMainWindowMessage());
            historyForm.ShowDialog(this);
        });
    }

    private void OnRequestSelectDifficulty(RequestSelectDifficultyMessage message)
    {
        // 难度选择逻辑占位
    }

    public void MoveWindowToPosition(System.Windows.Forms.Form form)
    {
        var position = AppSettings.StringToWindowPosition(_appSettings.WindowPosition);
        Rectangle screenBounds = Screen.GetWorkingArea(form);
        int x, y;
        switch (position)
        {
            case WindowPosition.TopLeft:
                x = screenBounds.Left;
                y = screenBounds.Top;
                break;
            case WindowPosition.TopRight:
                x = screenBounds.Right - form.Width;
                y = screenBounds.Top;
                break;
            case WindowPosition.BottomLeft:
                x = screenBounds.Left;
                y = screenBounds.Bottom - form.Height;
                break;
            case WindowPosition.BottomRight:
                x = screenBounds.Right - form.Width;
                y = screenBounds.Bottom - form.Height;
                break;
            default:
                return;
        }
        form.Location = new Point(x, y);
    }

    private void OnScreenshotRequested(ScreenshotRequestedMessage message)
    {
        this.BeginInvoke(
            new Action(async () =>
            {
                try
                {
                    await Task.Delay(500);
                    await PerformMainFormScreenshotAsync(message.LootName);
                }
                catch (Exception ex)
                {
                    LogManager.WriteErrorLog("MainForm", "延迟截图失败", ex);
                    Utils.Toast.Error(LanguageManager.GetString("ScreenshotFailed") ?? "截图失败");
                }
            })
        );
    }

    private async Task PerformMainFormScreenshotAsync(string lootName)
    {
        bool hideWindow = _appSettings.HideWindowOnScreenshot;
        try
        {
            if (hideWindow)
            {
                this.Opacity = 0;
                await Task.Delay(200);
            }
            string? path = ScreenshotHelper.CaptureAndSave(lootName);
            if (path == null)
            {
                Utils.Toast.Error(LanguageManager.GetString("ScreenshotFailed") ?? "截图失败");
            }
            else
            {
                Utils.Toast.Success(LanguageManager.GetString("ScreenshotSavedSuccessfully") ?? "截图已保存");
            }
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("MainForm", "截图流程异常", ex);
            Utils.Toast.Error(LanguageManager.GetString("ScreenshotFailed") ?? "截图失败");
        }
        finally
        {
            if (hideWindow)
            {
                this.Opacity = _appSettings.Opacity;
            }
        }
    }

    private void UpdateFormTitleAndTabs()
    {
        this.Text = LanguageManager.GetString("FormTitle");
        btnNavTimer.Text = LanguageManager.GetString("TabTimer");
        btnNavPomodoro.Text = LanguageManager.GetString("TabPomodoro");
        btnNavSettings.Text = LanguageManager.GetString("TabSettings");

        _timerControl.RefreshUI();
        _pomodoroControl.RefreshUI();
    }

    private void ShowRecordLootDialog()
    {
        using var lootForm = new UI.Timer.RecordLootForm(
            _profileService,
            _timerHistoryService,
            _sceneService,
            _appSettings,
            _messenger
        );
        lootForm.LootRecordSaved += (s, e) => _timerControl.HandleLootAdded();
        lootForm.ShowDialog(this);
    }

    private void AdjustWindowHeight()
    {
        int baseHeight = 150;
        int navHeight = 50;
        int targetHeight = baseHeight;

        if (_appSettings.TimerShowStatistics) targetHeight += 120;
        if (_appSettings.TimerShowHistory) targetHeight += 150;
        if (_appSettings.TimerShowLootDrops) targetHeight += 150;
        if (_appSettings.TimerShowAccountInfo) targetHeight += 100;

        if (_appSettings.ShowNavigation)
        {
            targetHeight += navHeight;
        }
        if (this.ClientSize.Height != targetHeight)
        {
            this.ClientSize = new Size(Theme.UISizeConstants.ClientWidth, targetHeight);
            this.MoveWindowToPosition(this);
        }
    }

    private void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
    {
        int index = tabControl.SelectedIndex;
        DiabloTwoMFTimer.UI.Components.ThemedButton? targetBtn = index switch
        {
            1 => btnNavTimer,
            2 => btnNavPomodoro,
            3 => btnNavSettings,
            _ => null,
        };

        if (targetBtn != null)
        {
            UpdateNavButtonStyles(targetBtn);
        }

        AdjustWindowHeight();
        if (tabControl.SelectedIndex == (int)Models.TabPage.Timer)
        {
            _timerControl.HandleTabSelected();
        }
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        _mainService.ProcessHotKeyMessage(m);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _mainService.HandleApplicationClosing();
        base.OnFormClosing(e);
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        _mainService?.UpdateWindowHandle(this.Handle);
    }
}