using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Pomodoro;

public partial class PomodoroControl : UserControl
{
    private readonly IMainService _mainService = null!;
    private readonly IMessenger _messenger = null!;
    private readonly IPomodoroTimerService _timerService = null!;
    private readonly IAppSettings _appSettings = null!;
    private readonly IProfileService _profileService = null!;
    private readonly IStatisticsService _statsService = null!;

    private BreakForm? _breakForm;

    public PomodoroControl()
    {
        InitializeComponent();
    }

    public PomodoroControl(
        IMainService mainService,
        IPomodoroTimerService timerService,
        IAppSettings appSettings,
        IProfileService profileService,
        IMessenger messenger,
        IStatisticsService statsService
    )
        : this()
    {
        _mainService = mainService;
        _timerService = timerService;
        _appSettings = appSettings;
        _profileService = profileService;
        _statsService = statsService;
        _messenger = messenger;

        InitializeModeMark();
        LoadSettings();
        _timerService.Reset();
        lblPomodoroTime.BindService(_timerService);
        SubscribeEvents();
        SubscribeToMessages();

        // 绑定新增按钮事件
        this.btnNextState.Click += (s, e) => _timerService.SwitchToNextState();
        this.btnAddMinute.Click += (s, e) => _timerService.AddMinutes(1);
        this.btnShowStats.Click += BtnShowStats_Click;

        UpdateUI();
    }

    private void InitializeModeMark()
    {
        _lblModeMark = new Label
        {
            // 使用衬线体显示罗马数字更有质感，或者用 Consolas 保持代码风
            // 这里推荐 Times New Roman 粗体，更有 Diablo/极简 风格
            Font = AppTheme.Fonts.RomanTimer,
            ForeColor = AppTheme.Colors.Primary, // 使用您的砂金色
            BackColor = Color.Transparent,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "I", // 默认值
        };

        // 布局定位：建议放在左上角，或者计时器的正上方/正下方
        // 这里示例放在右上角，作为状态角标
        _lblModeMark.Location = new Point(this.Width - 60, 10);
        _lblModeMark.Anchor = AnchorStyles.Top | AnchorStyles.Right;

        this.Controls.Add(_lblModeMark);
        _lblModeMark.BringToFront();
    }

    private void SubscribeEvents()
    {
        _timerService.PomodoroTimerStateChanged += TimerService_PomodoroTimerStateChanged;
        _timerService.PomodoroBreakStarted += TimerService_PomodoroBreakStarted;
    }

    private void UnSubscribeEvents()
    {
        _timerService.PomodoroTimerStateChanged -= TimerService_PomodoroTimerStateChanged;
        _timerService.PomodoroBreakStarted -= TimerService_PomodoroBreakStarted;
    }

    /// <summary>
    /// 【重构】集中订阅全局消息
    /// </summary>
    private void SubscribeToMessages()
    {
        // 注意：这里使用命名方法 OnShowSettings，而不是 Lambda 表达式
        // 这样在 Unsubscribe 时才能正确移除
        _messenger.Subscribe<ShowPomodoroSettingsMessage>(OnShowSettings);
        _messenger.Subscribe<ShowPomodoroBreakFormMessage>(OnShowBreakForm);
        _messenger.Subscribe<PomodoroModeChangedMessage>(OnPomodoroModeChanged);
        _messenger.Subscribe<PomodoroSettingsChangedMessage>(OnPomodoroSettingsChanged);
    }

    /// <summary>
    /// 【重构】集中取消订阅全局消息
    /// </summary>
    private void UnSubscribeToMessages()
    {
        _messenger.Unsubscribe<ShowPomodoroSettingsMessage>(OnShowSettings);
        _messenger.Unsubscribe<ShowPomodoroBreakFormMessage>(OnShowBreakForm);
        _messenger.Unsubscribe<PomodoroModeChangedMessage>(OnPomodoroModeChanged);
        _messenger.Unsubscribe<PomodoroSettingsChangedMessage>(OnPomodoroSettingsChanged);
    }

    /// <summary>
    /// 处理番茄钟模式变更消息
    /// </summary>
    /// <param name="message">模式变更消息</param>
    private void OnPomodoroModeChanged(PomodoroModeChangedMessage message)
    {
        this.SafeInvoke(() => UpdateModeMark(message.NewMode));
    }

    private void OnShowSettings(ShowPomodoroSettingsMessage msg)
    {
        this.SafeInvoke(ShowSettingsDialog);
    }

    /// <summary>
    /// 处理番茄钟设置变更消息
    /// </summary>
    private void OnPomodoroSettingsChanged(PomodoroSettingsChangedMessage message)
    {
        this.SafeInvoke(() =>
        {
            // 重新加载计时器设置
            _timerService.LoadSettings();

            // 只有在修改了番茄数据时才重置计时器
            if (message.IsTimerDataChanged)
            {
                _timerService.Reset();
            }

            // 更新UI
            UpdateUI();
        });
    }

    private void OnShowBreakForm(ShowPomodoroBreakFormMessage msg)
    {
        var breakType =
            (_timerService.CompletedPomodoros % 4 == 0) ? PomodoroBreakType.LongBreak : PomodoroBreakType.ShortBreak;
        var mode = BreakFormMode.PomodoroBreak;
        if (_timerService.CurrentState == PomodoroTimerState.Work)
        {
            //    breakType = PomodoroBreakType.
            mode = BreakFormMode.StatisticsView;
        }

        AsyncShowBreakForm(breakType, mode);
    }

    #region 事件处理

    // 提取出的公用方法
    private void ShowSettingsDialog()
    {
        using var settingsForm = new PomodoroSettingsForm(
            _appSettings,
            _timerService.Settings.WorkTimeMinutes,
            _timerService.Settings.WorkTimeSeconds,
            _timerService.Settings.ShortBreakMinutes,
            _timerService.Settings.ShortBreakSeconds,
            _timerService.Settings.LongBreakMinutes,
            _timerService.Settings.LongBreakSeconds,
            _appSettings.PomodoroWarningLongTime,
            _appSettings.PomodoroWarningShortTime
        );

        if (settingsForm.ShowDialog(this.FindForm()) == DialogResult.OK)
        {
            // 比较当前设置和新设置，判断是否修改了番茄数据
            bool isTimerDataChanged = false;

            if (_timerService.Settings.WorkTimeMinutes != settingsForm.WorkTimeMinutes ||
                _timerService.Settings.WorkTimeSeconds != settingsForm.WorkTimeSeconds ||
                _timerService.Settings.ShortBreakMinutes != settingsForm.ShortBreakMinutes ||
                _timerService.Settings.ShortBreakSeconds != settingsForm.ShortBreakSeconds ||
                _timerService.Settings.LongBreakMinutes != settingsForm.LongBreakMinutes ||
                _timerService.Settings.LongBreakSeconds != settingsForm.LongBreakSeconds)
            {
                isTimerDataChanged = true;
            }

            _timerService.Settings.WorkTimeMinutes = settingsForm.WorkTimeMinutes;
            _timerService.Settings.WorkTimeSeconds = settingsForm.WorkTimeSeconds;
            _timerService.Settings.ShortBreakMinutes = settingsForm.ShortBreakMinutes;
            _timerService.Settings.ShortBreakSeconds = settingsForm.ShortBreakSeconds;
            _timerService.Settings.LongBreakMinutes = settingsForm.LongBreakMinutes;
            _timerService.Settings.LongBreakSeconds = settingsForm.LongBreakSeconds;
            // _timerService.Settings.PomodoroMode = settingsForm;

            _appSettings.PomodoroWarningLongTime = settingsForm.WarningLongTime;
            _appSettings.PomodoroWarningShortTime = settingsForm.WarningShortTime;

            _timerService.LoadSettings(); // 确保重新加载模式等设置

            SaveSettings();

            // 只有在修改了番茄数据时才重置计时器
            if (isTimerDataChanged)
            {
                _timerService.Reset();
            }

            Toast.Success(LanguageManager.GetString("PomodoroSettingsSaved", "Pomodoro settings saved successfully"));
        }
    }

    private void TimerService_PomodoroTimerStateChanged(object? sender, PomodoroTimerStateChangedEventArgs e)
    {
        this.SafeInvoke(UpdateUI);
    }

    // 【核心修复】异步处理弹窗，防止卡死 UI
    private void TimerService_PomodoroBreakStarted(object? sender, PomodoroBreakStartedEventArgs e)
    {
        AsyncShowBreakForm(e.BreakType, BreakFormMode.PomodoroBreak);
    }

    private void AsyncShowBreakForm(PomodoroBreakType breakType, BreakFormMode mode = BreakFormMode.PomodoroBreak)
    {
        // 使用 BeginInvoke 脱离当前执行栈（特别是从 Hook/Hotkey 进来的调用）
        this.BeginInvoke(
            new Action(async () =>
            {
                try
                {
                    // 延迟 100ms：让出 CPU 给游戏和系统消息循环，避免资源竞争导致卡死
                    await Task.Delay(100);
                    ShowBreakForm(breakType, mode);
                }
                catch (Exception ex)
                {
                    LogManager.WriteErrorLog("PomodoroControl", "ShowBreakForm error", ex);
                }
            })
        );
    }

    #endregion

    #region UI 更新逻辑

    private void UpdateUI()
    {
        btnStartPomodoro.Text = _timerService.IsRunning
            ? (LanguageManager.GetString("PausePomodoro") ?? "暂停")
            : (LanguageManager.GetString("StartPomodoro") ?? "开始");

        btnPomodoroReset.Text = LanguageManager.GetString("ResetPomodoro") ?? "重置";
        btnPomodoroSettings.Text = LanguageManager.GetString("Settings") ?? "设置";

        btnNextState.Text = LanguageManager.GetString("PomodoroSkip") ?? "Skip";
        btnAddMinute.Text = LanguageManager.GetString("PomodoroAddMin") ?? "+1 Min";
        btnShowStats.Text = LanguageManager.GetString("Statistics") ?? "Stats";

        // 仅在休息时间启用统计按钮
        btnShowStats.Enabled = _timerService.CanShowStats;
        btnNextState.Enabled = _timerService.IsRunning;

        UpdateModeMark();
    }

    /// <summary>
    /// 更新模式标记
    /// </summary>
    private void UpdateModeMark()
    {
        UpdateModeMark(_appSettings.PomodoroMode);
    }

    /// <summary>
    /// 更新模式标记
    /// </summary>
    /// <param name="mode">新的模式</param>
    private void UpdateModeMark(Models.PomodoroMode mode)
    {
        switch (mode)
        {
            case Models.PomodoroMode.Automatic:
                _lblModeMark.Text = "I";
                _lblModeMark.ForeColor = AppTheme.Colors.Primary;
                break;

            case Models.PomodoroMode.SemiAuto:
                _lblModeMark.Text = "II";
                _lblModeMark.ForeColor = AppTheme.Colors.Success;
                break;

            case Models.PomodoroMode.Manual:
                _lblModeMark.Text = "III";
                _lblModeMark.ForeColor = AppTheme.Colors.Info;
                break;

            default:
                _lblModeMark.Text = "";
                break;
        }
    }

    private void UpdateCountDisplay()
    {
        int completed = _timerService.CompletedPomodoros;
        pomodoroStatusDisplay1.TotalCompletedCount = completed;
    }

    #endregion

    #region 按钮点击与交互

    private void BtnStartPomodoro_Click(object sender, EventArgs e)
    {
        if (_timerService.IsRunning)
        {
            _timerService.Pause();
        }
        else
        {
            _timerService.Start();
        }
    }

    private void BtnPomodoroReset_Click(object sender, EventArgs e)
    {
        _timerService?.Reset();
        Toast.Success(LanguageManager.GetString("PomodoroReset", "Pomodoro timer reset successfully"));
    }

    private void BtnShowStats_Click(object? sender, EventArgs e)
    {
        var breakType =
            (_timerService.CompletedPomodoros % 4 == 0) ? PomodoroBreakType.LongBreak : PomodoroBreakType.ShortBreak;

        ShowBreakForm(breakType);
    }

    private void BtnPomodoroSettings_Click(object sender, EventArgs e)
    {
        ShowSettingsDialog();
    }

    private void ShowBreakForm(PomodoroBreakType breakType, BreakFormMode mode = BreakFormMode.PomodoroBreak)
    {
        if (_breakForm != null && !_breakForm.IsDisposed)
        {
            _breakForm.Close();
        }

        _breakForm = new BreakForm(
            _mainService,
            _timerService,
            _appSettings,
            _profileService,
            _statsService,
            mode,
            breakType
        );
        _breakForm.Show(this.FindForm());
    }

    #endregion

    #region 设置加载/保存
    private void LoadSettings()
    {
        _timerService.LoadSettings();
    }

    private void SaveSettings()
    {
        _appSettings.WorkTimeMinutes = _timerService.Settings.WorkTimeMinutes;
        _appSettings.WorkTimeSeconds = _timerService.Settings.WorkTimeSeconds;
        _appSettings.ShortBreakMinutes = _timerService.Settings.ShortBreakMinutes;
        _appSettings.ShortBreakSeconds = _timerService.Settings.ShortBreakSeconds;
        _appSettings.LongBreakMinutes = _timerService.Settings.LongBreakMinutes;
        _appSettings.LongBreakSeconds = _timerService.Settings.LongBreakSeconds;
        _appSettings.Save();
    }

    public void RefreshUI()
    {
        UpdateUI();
    }

    #endregion
}
