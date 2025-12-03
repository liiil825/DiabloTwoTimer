using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.UI.Pomodoro;
using DiabloTwoMFTimer.UI.Profiles;
using DiabloTwoMFTimer.UI.Settings;
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

    public MainForm()
    {
        InitializeComponent();
        ConfigureTabControl();
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
        SettingsControl settingsControl
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
        _settingsControl = settingsControl;
        _messenger = messenger;

        InitializeChildControls();
        InitializeForm();
        SubscribeToEvents();
        SubscribeToMessages();

        this.Shown += OnMainForm_Shown;
    }

    internal TabControl? TabControl => tabControl;

    private void OnMainForm_Shown(object? sender, EventArgs e)
    {
        _mainService.InitializeApplication(this.Handle);
        _mainService.ApplyWindowSettings(this);
    }

    private void ConfigureTabControl()
    {
        // 指定索引 4 (即最后一个 Tab) 为功能按钮
        this.tabControl.ActionTabIndex = 4;

        // 绑定点击事件：先切换到计时界面，再最小化窗口
        this.tabControl.ActionTabClicked += (s, e) =>
        {
            // 最小化前先切换到计时界面
            if (tabControl.SelectedIndex != (int)Models.TabPage.Timer)
            {
                tabControl.SelectedIndex = (int)Models.TabPage.Timer;
            }
            this.WindowState = FormWindowState.Minimized;
        };
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
        this.TopMost = _appSettings.AlwaysOnTop;
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

        _mainService.OnRequestRecordLoot += () => this.SafeInvoke(ShowRecordLootDialog);
    }

    private void SubscribeToMessages()
    {
        _messenger.Subscribe<WindowPositionChangedMessage>(_ =>
            this.SafeInvoke(() => _mainService.ApplyWindowSettings(this))
        );
        _messenger.Subscribe<AlwaysOnTopChangedMessage>(_ =>
            this.SafeInvoke(() => this.TopMost = _appSettings.AlwaysOnTop)
        );
        _messenger.Subscribe<TimerSettingsChangedMessage>(msg => this.SafeInvoke(() => AdjustWindowHeight()));
    }

    private void UpdateFormTitleAndTabs()
    {
        this.Text = LanguageManager.GetString("FormTitle");

        if (tabControl != null && tabControl.TabPages.Count >= 4)
        {
            tabControl.TabPages[(int)Models.TabPage.Profile].Text = LanguageManager.GetString("TabProfile");
            tabControl.TabPages[(int)Models.TabPage.Timer].Text = LanguageManager.GetString("TabTimer");
            tabControl.TabPages[(int)Models.TabPage.Pomodoro].Text = LanguageManager.GetString("TabPomodoro");
            tabControl.TabPages[(int)Models.TabPage.Settings].Text = LanguageManager.GetString("TabSettings");
            // Tab 4 始终是 "_"，不需要本地化
        }

        _profileManager.RefreshUI();
        _timerControl.RefreshUI();
        _pomodoroControl.RefreshUI();
        _settingsControl.RefreshUI();
    }

    private void ShowRecordLootDialog()
    {
        using var lootForm = new UI.Timer.RecordLootForm(_profileService, _timerHistoryService, _sceneService);
        lootForm.LootRecordSaved += (s, e) => _timerControl.HandleLootAdded();
        lootForm.ShowDialog(this);
    }

    private void AdjustWindowHeight()
    {
        bool showLoot = _appSettings.TimerShowLootDrops;
        int targetHeight = showLoot ? UISizeConstants.ClientHeightWithLoot : UISizeConstants.ClientHeightWithoutLoot;

        if (this.ClientSize.Height != targetHeight)
        {
            this.ClientSize = new Size(UISizeConstants.ClientWidth, targetHeight);
            _mainService.ApplyWindowSettings(this);
        }
    }

    private void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
    {
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
}
