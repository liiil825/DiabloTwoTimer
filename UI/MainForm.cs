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
using DiabloTwoMFTimer.UI.Profiles;
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

    private readonly ProfileManager _profileManager = null!;
    private readonly TimerControl _timerControl = null!;
    private readonly PomodoroControl _pomodoroControl = null!;
    private readonly SettingsControl _settingsControl = null!;
    private NotifyIcon _notifyIcon = null!;
    private ContextMenuStrip _trayMenu = null!;
    private readonly ICommandDispatcher _commandDispatcher = null!;
    private readonly IKeyMapRepository _keyMapRepository = null!;
    private readonly CommandInitializer _commandInitializer = null!;
    private LeaderKeyForm _leaderKeyForm = null!;
    private System.ComponentModel.IContainer _components = null!;

    public MainForm()
    {
        InitializeComponent();
        this.tlpNavigation.Height = Theme.UISizeConstants.TabItemHeight;
        UpdateNavButtonStyles(btnNavProfile);
    }

    public MainForm(
        IMainService mainService,
        IAppSettings settings,
        IProfileService profileService,
        ITimerHistoryService timerHistoryService,
        ISceneService sceneService,
        IMessenger messenger,
        ProfileManager profileManager,
        TimerControl timerControl,
        PomodoroControl pomodoroControl,
        SettingsControl settingsControl,
        ICommandDispatcher commandDispatcher,
        CommandInitializer commandInitializer,
        IKeyMapRepository keyMapRepository
    )
        : this()
    {
        _mainService = mainService;
        _appSettings = settings;
        _profileService = profileService;
        _timerHistoryService = timerHistoryService;
        _sceneService = sceneService;
        _profileManager = profileManager;
        _timerControl = timerControl;
        _pomodoroControl = pomodoroControl;
        _keyMapRepository = keyMapRepository;
        _settingsControl = settingsControl;
        _commandDispatcher = commandDispatcher;
        _commandInitializer = commandInitializer;
        _messenger = messenger;

        _commandInitializer.Initialize();
        _leaderKeyForm = new LeaderKeyForm(_commandDispatcher, _keyMapRepository);
        InitializeChildControls();
        InitializeForm();
        InitializeSystemTray();
        SubscribeToEvents();
        SubscribeToMessages();
        Utils.Toast.RegisterUiInvoker(action => this.SafeInvoke(action));
        this.Shown += OnMainForm_Shown;
    }

    internal TabControl? TabControl => tabControl;

    private void OnMainForm_Shown(object? sender, EventArgs e)
    {
        _mainService.InitializeApplication(this.Handle);
        _mainService.ApplyWindowSettings(this);

        // 检查是否有上次使用的角色档案和场景记录，如果有则自动跳转到计时界面
        if (
            !string.IsNullOrEmpty(_appSettings.LastUsedProfile)
            && !string.IsNullOrEmpty(_profileService.CurrentProfile?.LastRunScene)
        )
        {
            _mainService.SetActiveTabPage(Models.TabPage.Timer);
        }

        this.Opacity = _appSettings.Opacity;
    }

    // 统一的点击事件处理
    private void NavButton_Click(object sender, EventArgs e)
    {
        if (sender == btnNavProfile)
            tabControl.SelectedIndex = 0;
        else if (sender == btnNavTimer)
            tabControl.SelectedIndex = 1;
        else if (sender == btnNavPomodoro)
            tabControl.SelectedIndex = 2;
        else if (sender == btnNavSettings)
            tabControl.SelectedIndex = 3;
        else if (sender == btnNavMinimize) // 现在文字是 'x'
        {
            using var form = new CloseOptionForm();
            // 因为继承了 BaseForm，它已经自带了 ShowDialog 逻辑和 DialogResult 处理
            var result = form.ShowDialog(this);

            // BaseForm 的 "确认" 按钮会返回 DialogResult.OK
            if (result == DialogResult.OK)
            {
                if (form.IsCloseAppSelected)
                {
                    // 用户勾选了退出 -> 彻底关闭
                    _mainService.HandleApplicationClosing();
                    Application.Exit();
                }
                else
                {
                    // 用户默认操作 -> 最小化到托盘

                    // 1. 恢复按钮高亮到 Timer 或当前页 (防止 'x' 按钮一直亮着)
                    if (tabControl.SelectedIndex != (int)Models.TabPage.Timer)
                    {
                        tabControl.SelectedIndex = (int)Models.TabPage.Timer;
                        UpdateNavButtonStyles(btnNavTimer);
                    }
                    else
                    {
                        UpdateNavButtonStyles(btnNavTimer);
                    }

                    // 2. 执行最小化 (这会触发 Resize 事件从而隐藏窗口)
                    this.WindowState = FormWindowState.Minimized;
                }
            }
            // 如果点击 BaseForm 的 "取消" 或 "X"，返回 Cancel，这里什么都不做，直接返回
            return; // 重要：阻止执行函数末尾的 UpdateNavButtonStyles((ThemedButton)sender);
        }

        UpdateNavButtonStyles((ThemedButton)sender);
    }

    // 样式更新：高亮当前，变灰其他
    private void UpdateNavButtonStyles(ThemedButton activeBtn)
    {
        var buttons = new[] { btnNavProfile, btnNavTimer, btnNavPomodoro, btnNavSettings };

        foreach (var btn in buttons)
        {
            // 只需要这一行，样式逻辑全在 Button 内部
            btn.IsSelected = (btn == activeBtn);
        }
    }

    // --- 保持绘制边框，这在无边框窗体中很重要 ---
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        // 绘制一圈暗金色的细边框
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

        AddControlToTab(tabProfilePage, _profileManager);
        AddControlToTab(tabTimerPage, _timerControl);
        AddControlToTab(tabPomodoroPage, _pomodoroControl);
        AddControlToTab(tabSettingsPage, _settingsControl);
    }

    private void InitializeForm()
    {
        AdjustWindowHeight();
        this.StartPosition = FormStartPosition.Manual;
        this.ShowInTaskbar = true;
        this.Opacity = 0;
        this.TopMost = _appSettings.AlwaysOnTop;
        // 根据设置显示或隐藏导航栏
        tlpNavigation.Visible = _appSettings.ShowNavigation;
    }

    // 【新增】初始化托盘图标的具体逻辑
    private void InitializeSystemTray()
    {
        _components = new Container();

        // 1. 创建右键菜单
        _trayMenu = new ContextMenuStrip(_components);
        var exitItem = new ToolStripMenuItem("退出程序");
        exitItem.Click += (s, e) =>
        {
            // 彻底退出程序
            _mainService.HandleApplicationClosing();
            Application.Exit();
        };
        _trayMenu.Items.Add(exitItem);

        // 2. 创建托盘图标
        _notifyIcon = new NotifyIcon(_components)
        {
            Text = "D2R Timer", // 鼠标悬停时显示的文字
            Icon = new Icon("Resources\\d2r.ico"), // 复用你的图标，确保路径正确
            Visible = true, // 初始是否可见，建议设为 true，或者仅在最小化时 true
            ContextMenuStrip = _trayMenu,
        };

        // 3. 绑定双击事件：还原窗口
        _notifyIcon.MouseDoubleClick += (s, e) =>
        {
            RestoreFromTray();
        };

        // 4. 绑定窗体大小改变事件（用于检测最小化）
        this.Resize += OnMainForm_Resize;
    }

    // 【新增】核心逻辑：最小化时隐藏窗口
    private void OnMainForm_Resize(object? sender, EventArgs e)
    {
        if (this.WindowState == FormWindowState.Minimized)
        {
            // 隐藏任务栏图标
            this.ShowInTaskbar = false;
            // 隐藏主窗口
            this.Hide();
            // 确保托盘图标可见
            _notifyIcon.Visible = true;

            // 可选：显示一个气泡提示
            // _notifyIcon.ShowBalloonTip(2000, "D2R Helper", "程序已运行在系统托盘", ToolTipIcon.Info);
        }
    }

    // 【新增】核心逻辑：从托盘还原
    private void RestoreFromTray()
    {
        if (!this.Visible)
        {
            this.Show();
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            this.Activate(); // 激活窗口到最前
            _mainService.ReloadHotkeys(); // 重新注册热键，确保热键正常工作
        }
    }

    private void SubscribeToEvents()
    {
        _mainService.OnRequestTabChange += (tabPage) =>
        {
            if (tabControl != null && (int)tabPage < tabControl.TabCount)
            {
                this.SafeInvoke(() => tabControl.SelectedIndex = (int)tabPage);
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

        // 添加删除最后一个时间记录的事件
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

        // 添加删除最后一个掉落记录的事件
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
            this.SafeInvoke(() => _mainService.ApplyWindowSettings(this))
        );
        _messenger.Subscribe<AlwaysOnTopChangedMessage>(_ =>
            this.SafeInvoke(() => this.TopMost = _appSettings.AlwaysOnTop)
        );
        _messenger.Subscribe<OpacityChangedMessage>(_ => this.SafeInvoke(() => this.Opacity = _appSettings.Opacity));
        _messenger.Subscribe<HideMainWindowMessage>(_ => this.SafeInvoke(() => this.Opacity = 0));
        _messenger.Subscribe<ShowMainWindowMessage>(_ => this.SafeInvoke(() => this.Opacity = _appSettings.Opacity));
        _messenger.Subscribe<TimerSettingsChangedMessage>(msg => this.SafeInvoke(() => AdjustWindowHeight()));
        _messenger.Subscribe<ScreenshotRequestedMessage>(OnScreenshotRequested);
        _messenger.Subscribe<ShowLeaderKeyFormMessage>(_ =>
            this.SafeInvoke(() =>
            {
                if (!_leaderKeyForm.Visible)
                {
                    // 确保显示在当前屏幕 (多屏支持)
                    _leaderKeyForm.StartPosition = FormStartPosition.Manual;
                    var screen = Screen.FromControl(this);
                    _leaderKeyForm.Location = new Point(screen.Bounds.X, screen.Bounds.Bottom - _leaderKeyForm.Height);
                    _leaderKeyForm.Width = screen.Bounds.Width;

                    _leaderKeyForm.Show();
                    _leaderKeyForm.Activate();
                }
            })
        );

        // 【新增】响应最小化和恢复消息
        _messenger.Subscribe<MinimizeToTrayMessage>(_ =>
            this.SafeInvoke(() =>
            {
                // 模拟点击最小化按钮的行为
                this.WindowState = FormWindowState.Minimized;
                // OnMainForm_Resize 会处理后续的 Hide 和 ShowInTaskbar = false
            })
        );

        _messenger.Subscribe<RestoreFromTrayMessage>(_ =>
            this.SafeInvoke(() =>
            {
                RestoreFromTray();
            })
        );

        _messenger.Subscribe<ShowRecordLootFormMessage>(_ => this.SafeInvoke(ShowRecordLootDialog));
        // 响应导航栏可见性变更消息
        _messenger.Subscribe<NavigationVisibilityChangedMessage>(_ =>
            this.SafeInvoke(() =>
            {
                tlpNavigation.Visible = _appSettings.ShowNavigation;
            })
        );
    }

    private void OnScreenshotRequested(ScreenshotRequestedMessage message)
    {
        // 使用 BeginInvoke 确保脱离当前的事件调用栈
        // 使用 async/await 让出 UI 线程，让系统有时间处理 RecordLootForm 关闭后的重绘
        this.BeginInvoke(
            new Action(async () =>
            {
                try
                {
                    // 等待 500ms：这对于让刚刚关闭的 RecordLootForm 彻底消失绰绰有余
                    // 此时 UI 线程是空闲的，可以处理 Paint 消息
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
            // 如果需要隐藏主窗口
            if (hideWindow)
            {
                this.Opacity = 0;
                // 等待 200ms 让主窗口消失的重绘生效
                // 使用 Task.Delay 而不是 Thread.Sleep，避免卡死 UI
                await Task.Delay(200);
            }

            // 执行截图 (这是耗时操作，但在 UI 线程执行没问题，因为我们已经让出了时间片)
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
            // 恢复主窗口显示
            if (hideWindow)
            {
                this.Opacity = _appSettings.Opacity;
            }
        }
    }

    private void UpdateFormTitleAndTabs()
    {
        this.Text = LanguageManager.GetString("FormTitle");
        btnNavProfile.Text = LanguageManager.GetString("TabProfile");
        btnNavTimer.Text = LanguageManager.GetString("TabTimer");
        btnNavPomodoro.Text = LanguageManager.GetString("TabPomodoro");
        btnNavSettings.Text = LanguageManager.GetString("TabSettings");
        // Tab 4 始终是 "_"，不需要本地化

        _profileManager.RefreshUI();
        _timerControl.RefreshUI();
        _pomodoroControl.RefreshUI();
        _settingsControl.RefreshUI();
    }

    private void ShowRecordLootDialog()
    {
        using var lootForm = new UI.Timer.RecordLootForm(
            _profileService,
            _timerHistoryService,
            _sceneService,
            _appSettings,
            _messenger // 传入信使
        );
        lootForm.LootRecordSaved += (s, e) => _timerControl.HandleLootAdded();
        lootForm.ShowDialog(this);
    }

    private void AdjustWindowHeight()
    {
        bool showLoot = _appSettings.TimerShowLootDrops;
        int targetHeight = showLoot
            ? Theme.UISizeConstants.ClientHeightWithLoot
            : Theme.UISizeConstants.ClientHeightWithoutLoot;

        if (this.ClientSize.Height != targetHeight)
        {
            this.ClientSize = new Size(Theme.UISizeConstants.ClientWidth, targetHeight);
            _mainService.ApplyWindowSettings(this);
        }
    }

    private void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // 【新增】核心修复：让按钮状态始终跟随 Tab 索引变化
        // 这样无论是点击顶部按钮，还是程序内部跳转（如StartFarm），样式都会自动同步
        int index = tabControl.SelectedIndex;

        // 根据索引找到对应的按钮
        // (注意：这里使用了之前定义的按钮变量名，确保它们在当前类中可访问)
        DiabloTwoMFTimer.UI.Components.ThemedButton? targetBtn = index switch
        {
            0 => btnNavProfile,
            1 => btnNavTimer,
            2 => btnNavPomodoro,
            3 => btnNavSettings,
            _ => null,
        };

        // 更新高亮样式
        if (targetBtn != null)
        {
            UpdateNavButtonStyles(targetBtn);
        }

        AdjustWindowHeight();
        if (tabControl.SelectedIndex == (int)Models.TabPage.Timer)
        {
            _timerControl.HandleTabSelected();
        }
        else if (tabControl.SelectedIndex == (int)Models.TabPage.Profile)
        {
            _profileManager.RefreshUI();
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
        // 当句柄重建时（例如修改 ShowInTaskbar 后），通知 Service 更新并重注册热键
        _mainService?.UpdateWindowHandle(this.Handle);
    }
}
