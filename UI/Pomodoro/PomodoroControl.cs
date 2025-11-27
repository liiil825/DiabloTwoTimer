using System;
using System.Windows.Forms;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Pomodoro;

public partial class PomodoroControl : UserControl
{
    // 注意：这里我们声明为可空，因为无参构造函数中它尚未赋值
    // 但在运行时使用带参构造后，它将不为空
    private readonly IPomodoroTimerService _timerService = null!;
    private readonly IAppSettings _appSettings = null!;
    private readonly IProfileService _profileService = null!;

    private BreakForm? _breakForm;

    // 1. 无参构造函数 (用于 VS 设计器预览)
    public PomodoroControl()
    {
        InitializeComponent();
    }

    // 2. 依赖注入构造函数 (实际运行时使用)
    public PomodoroControl(
        IPomodoroTimerService timerService,
        IAppSettings appSettings,
        IProfileService profileService
    )
        : this()
    {
        _timerService = timerService;
        _appSettings = appSettings;
        _profileService = profileService;

        // 加载设置并刷新一次静态UI
        LoadSettings();
        // --- 关键点：将 Service 绑定给子组件 ---
        // TimerDisplayLabel 会自己处理时间更新，主控件不用管了
        lblPomodoroTime.BindService(_timerService);

        // --- 订阅主控件关心的事件 (按钮状态、弹窗) ---
        SubscribeEvents();

        UpdateUI();
    }

    private void SubscribeEvents()
    {
        _timerService.TimerStateChanged += TimerService_TimerStateChanged;
        _timerService.PomodoroCompleted += TimerService_PomodoroCompleted;
        _timerService.BreakStarted += TimerService_BreakStarted;
        _timerService.BreakSkipped += TimerService_BreakSkipped;
        // 注意：不需要订阅 TimeUpdated，那是 Label 的事
    }

    // 清理资源：重写 OnHandleDestroyed 或 Dispose 来取消订阅
    protected override void OnHandleDestroyed(EventArgs e)
    {
        _timerService.TimerStateChanged -= TimerService_TimerStateChanged;
        _timerService.PomodoroCompleted -= TimerService_PomodoroCompleted;
        _timerService.BreakStarted -= TimerService_BreakStarted;
        _timerService.BreakSkipped -= TimerService_BreakSkipped;
        base.OnHandleDestroyed(e);
    }

    #region 事件处理

    private void TimerService_TimerStateChanged(object? sender, TimerStateChangedEventArgs e)
    {
        // Service 可能在后台线程触发，必须 Invoke
        if (InvokeRequired)
        {
            BeginInvoke(new Action(UpdateUI));
            return;
        }
        UpdateUI();
    }

    private void TimerService_BreakStarted(object? sender, BreakStartedEventArgs e)
    {
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => ShowBreakForm(e.BreakType)));
            return;
        }
        ShowBreakForm(e.BreakType);
    }

    private void TimerService_PomodoroCompleted(object? sender, PomodoroCompletedEventArgs e)
    {
        // 如果需要在番茄钟完成时播放音效或弹通知，写在这里
    }

    private void TimerService_BreakSkipped(object? sender, EventArgs e)
    {
        // 跳过休息的逻辑
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

            SaveSettings();
            _timerService.Reset();
            Toast.Success(
                LanguageManager.GetString("PomodoroSettingsSaved", "Pomodoro settings saved successfully")
            );
        }
    }

    private void ShowBreakForm(BreakType breakType)
    {
        if (_breakForm != null && !_breakForm.IsDisposed)
        {
            _breakForm.Close();
        }

        _breakForm = new BreakForm(
            _timerService,
            _appSettings,
            _profileService,
            BreakFormMode.PomodoroBreak,
            breakType
        );
        _breakForm.Show(this.FindForm());
    }

    #endregion

    #region 设置加载/保存
    private void LoadSettings()
    {
        // 使用注入的_appSettings获取设置
        _timerService.Settings.WorkTimeMinutes = _appSettings.WorkTimeMinutes;
        _timerService.Settings.WorkTimeSeconds = _appSettings.WorkTimeSeconds;
        _timerService.Settings.ShortBreakMinutes = _appSettings.ShortBreakMinutes;
        _timerService.Settings.ShortBreakSeconds = _appSettings.ShortBreakSeconds;
        _timerService.Settings.LongBreakMinutes = _appSettings.LongBreakMinutes;
        _timerService.Settings.LongBreakSeconds = _appSettings.LongBreakSeconds;
        _timerService.Reset();
    }

    private void SaveSettings()
    {
        // 使用注入的_appSettings保存设置
        _appSettings.WorkTimeMinutes = _timerService.Settings.WorkTimeMinutes;
        _appSettings.WorkTimeSeconds = _timerService.Settings.WorkTimeSeconds;
        _appSettings.ShortBreakMinutes = _timerService.Settings.ShortBreakMinutes;
        _appSettings.ShortBreakSeconds = _timerService.Settings.ShortBreakSeconds;
        _appSettings.LongBreakMinutes = _timerService.Settings.LongBreakMinutes;
        _appSettings.LongBreakSeconds = _timerService.Settings.LongBreakSeconds;
        SettingsManager.SaveSettings(_appSettings);
    }

    public void RefreshUI()
    {
        UpdateUI();
    }

    #endregion
}
