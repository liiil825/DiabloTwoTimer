using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.UI.Components;
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

    // 统一的点击事件处理
    private void NavButton_Click(object sender, EventArgs e)
    {
        if (sender == btnNavProfile) tabControl.SelectedIndex = 0;
        else if (sender == btnNavTimer) tabControl.SelectedIndex = 1;
        else if (sender == btnNavPomodoro) tabControl.SelectedIndex = 2;
        else if (sender == btnNavSettings) tabControl.SelectedIndex = 3;
        else if (sender == btnNavMinimize) // 新增判断
        {
            // 最小化前先切换到计时界面
            if (tabControl.SelectedIndex != (int)Models.TabPage.Timer)
            {
                tabControl.SelectedIndex = (int)Models.TabPage.Timer;
            }
            this.WindowState = FormWindowState.Minimized;
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
        using var lootForm = new UI.Timer.RecordLootForm(_profileService, _timerHistoryService, _sceneService);
        lootForm.LootRecordSaved += (s, e) => _timerControl.HandleLootAdded();
        lootForm.ShowDialog(this);
    }

    private void AdjustWindowHeight()
    {
        bool showLoot = _appSettings.TimerShowLootDrops;
        int targetHeight = showLoot ? Theme.UISizeConstants.ClientHeightWithLoot : Theme.UISizeConstants.ClientHeightWithoutLoot;

        if (this.ClientSize.Height != targetHeight)
        {
            this.ClientSize = new Size(Theme.UISizeConstants.ClientWidth, targetHeight);
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
