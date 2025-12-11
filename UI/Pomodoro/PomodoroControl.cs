using System;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Pomodoro;

public partial class PomodoroControl : UserControl
{
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
        IPomodoroTimerService timerService,
        IAppSettings appSettings,
        IProfileService profileService,
        IStatisticsService statsService
    )
        : this()
    {
        _timerService = timerService;
        _appSettings = appSettings;
        _profileService = profileService;
        _statsService = statsService;

        LoadSettings();
        lblPomodoroTime.BindService(_timerService);
        SubscribeEvents();

        // 绑定新增按钮事件
        this.btnNextState.Click += (s, e) => _timerService.SwitchToNextState();
        this.btnAddMinute.Click += (s, e) => _timerService.AddMinutes(1);
        this.btnShowStats.Click += BtnShowStats_Click;

        UpdateUI();
    }

    private void SubscribeEvents()
    {
        _timerService.PomodoroTimerStateChanged += TimerService_PomodoroTimerStateChanged;
        _timerService.PomodoroCompleted += TimerService_PomodoroCompleted;
        _timerService.PomodoroBreakStarted += TimerService_PomodoroBreakStarted;
        _timerService.PomodoroBreakSkipped += TimerService_PomodoroBreakSkipped;
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        _timerService.PomodoroTimerStateChanged -= TimerService_PomodoroTimerStateChanged;
        _timerService.PomodoroCompleted -= TimerService_PomodoroCompleted;
        _timerService.PomodoroBreakStarted -= TimerService_PomodoroBreakStarted;
        _timerService.PomodoroBreakSkipped -= TimerService_PomodoroBreakSkipped;
        base.OnHandleDestroyed(e);
    }

    #region 事件处理

    private void TimerService_PomodoroTimerStateChanged(object? sender, PomodoroTimerStateChangedEventArgs e)
    {
        this.SafeInvoke(UpdateUI);
    }

    private void TimerService_PomodoroBreakStarted(object? sender, PomodoroBreakStartedEventArgs e)
    {
        this.SafeInvoke(() => ShowBreakForm(e.BreakType));
    }

    private void TimerService_PomodoroCompleted(object? sender, PomodoroCompletedEventArgs e)
    {
    }

    private void TimerService_PomodoroBreakSkipped(object? sender, EventArgs e)
    {
    }

    #endregion

    #region UI 更新逻辑

    private void UpdateUI()
    {
        // 更新按钮文字
        btnStartPomodoro.Text = _timerService.IsRunning
            ? (LanguageManager.GetString("PausePomodoro") ?? "暂停")
            : (LanguageManager.GetString("StartPomodoro") ?? "开始");

        btnPomodoroReset.Text = LanguageManager.GetString("ResetPomodoro") ?? "重置";
        btnPomodoroSettings.Text = LanguageManager.GetString("Settings") ?? "设置";

        // 更新新按钮文字
        btnNextState.Text = LanguageManager.GetString("PomodoroSkip") ?? "Skip";
        btnAddMinute.Text = LanguageManager.GetString("PomodoroAddMin") ?? "+1 Min";
        btnShowStats.Text = LanguageManager.GetString("Statistics") ?? "Stats";

        // 仅在休息时间启用统计按钮
        btnShowStats.Enabled = _timerService.CanShowStats;

        // 更新计数显示
        UpdateCountDisplay();
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
            Toast.Success(LanguageManager.GetString("PomodoroPaused", "Pomodoro timer paused"));
        }
        else
        {
            _timerService.Start();
            Toast.Success(LanguageManager.GetString("PomodoroStarted", "Pomodoro timer started"));
        }
    }

    private void BtnPomodoroReset_Click(object sender, EventArgs e)
    {
        _timerService?.Reset();
        Toast.Success(LanguageManager.GetString("PomodoroReset", "Pomodoro timer reset successfully"));
    }

    private void BtnShowStats_Click(object? sender, EventArgs e)
    {
        // 获取当前的 BreakType (如果不是休息时间，默认 Short)
        var breakType = (_timerService.CompletedPomodoros % 4 == 0)
            ? PomodoroBreakType.LongBreak
            : PomodoroBreakType.ShortBreak;

        ShowBreakForm(breakType);
    }

    private void BtnPomodoroSettings_Click(object sender, EventArgs e)
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
            // 更新 Service 配置
            _timerService.Settings.WorkTimeMinutes = settingsForm.WorkTimeMinutes;
            _timerService.Settings.WorkTimeSeconds = settingsForm.WorkTimeSeconds;
            _timerService.Settings.ShortBreakMinutes = settingsForm.ShortBreakMinutes;
            _timerService.Settings.ShortBreakSeconds = settingsForm.ShortBreakSeconds;
            _timerService.Settings.LongBreakMinutes = settingsForm.LongBreakMinutes;
            _timerService.Settings.LongBreakSeconds = settingsForm.LongBreakSeconds;

            // 更新警告时间设置
            _appSettings.PomodoroWarningLongTime = settingsForm.WarningLongTime;
            _appSettings.PomodoroWarningShortTime = settingsForm.WarningShortTime;

            // 更新模式设置 (SettingsForm 内部已直接更新 _appSettings)
            // 确保加载一次新模式
            _timerService.LoadSettings();

            SaveSettings();
            _timerService.Reset();
            Toast.Success(LanguageManager.GetString("PomodoroSettingsSaved", "Pomodoro settings saved successfully"));
        }
    }

    private void ShowBreakForm(PomodoroBreakType breakType)
    {
        if (_breakForm != null && !_breakForm.IsDisposed)
        {
            _breakForm.Close();
        }

        _breakForm = new BreakForm(
            _timerService,
            _appSettings,
            _profileService,
            _statsService,
            BreakFormMode.PomodoroBreak,
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