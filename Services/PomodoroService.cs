using System;
using System.Media;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.Services;

public class PomodoroTimerService : IPomodoroTimerService
{
    // 事件定义
    public event EventHandler<PomodoroTimerStateChangedEventArgs>? PomodoroTimerStateChanged;
    public event EventHandler<PomodoroCompletedEventArgs>? PomodoroCompleted;
    public event EventHandler<PomodoroBreakStartedEventArgs>? PomodoroBreakStarted;
    public event EventHandler? PomodoroBreakSkipped;
    public event EventHandler? TimeUpdated;

    // 番茄时钟相关字段
    private TimeSpan _timeLeft;
    private bool _isRunning = false;
    private int _completedPomodoros = 0;
    private PomodoroTimerState _currentState = PomodoroTimerState.Work;
    private PomodoroTimerState _previousState = PomodoroTimerState.Work; // 记录之前的状态
    private readonly System.Windows.Forms.Timer _timer;
    private readonly ITimerService? _timerService; // 引用计时器服务

    // 标记是否在休息前暂停了计时器
    private bool _timerWasPausedBeforeBreak = false;
    private IAppSettings? _appSettings;

    // 时间设置
    public PomodoroTimeSettings Settings { get; set; }

    public PomodoroTimerService()
        : this(null) { }

    public PomodoroTimerService(ITimerService? timerService)
    {
        _timerService = timerService;
        Settings = new PomodoroTimeSettings();
        _timer = new System.Windows.Forms.Timer { Interval = 100 };
        _timer.Tick += Timer_Tick;
        InitializeTimer();

        // 订阅计时器服务的事件
        if (_timerService != null)
        {
            _timerService.TimerRunningStateChangedEvent += OnTimerRunningStateChanged;
            _timerService.TimerPauseStateChangedEvent += OnTimerPauseStateChanged;
        }
    }

    private void TriggerStartFromTimer()
    {
        if (!_isRunning && _timerService != null && _timerService.IsRunning)
        {
            Start();
        }
    }

    // 处理计时器运行状态变化事件
    private void OnTimerRunningStateChanged(bool isRunning)
    {
        // 当计时器开启时，如果番茄时钟启动则启动番茄时钟（如果番茄时钟已经开启则不处理）
        TriggerStartFromTimer();
    }

    // 处理计时器暂停状态变化事件
    private void OnTimerPauseStateChanged(bool isPaused)
    {
        if (isPaused)
        {
            // 当计时器暂停时，如果番茄钟正在运行且设置了同步暂停，则暂停番茄钟
            if (_appSettings?.TimerSyncPausePomodoro == true && _isRunning)
            {
                Pause();
            }
        }
        else
        {
            TriggerStartFromTimer();
        }
    }

    public void Start()
    {
        if (!_isRunning)
        {
            _isRunning = true;
            _timer.Start();
            OnTimerStateChanged();
        }
    }

    public void Pause()
    {
        if (_isRunning)
        {
            _isRunning = false;
            _timer.Stop();
            OnTimerStateChanged();
        }
    }

    public void Reset()
    {
        _isRunning = false;
        _timer.Stop();
        _completedPomodoros = 0;
        _currentState = PomodoroTimerState.Work;
        _timeLeft = GetWorkTime();
        OnTimerStateChanged();
    }

    public void LoadSettings(IAppSettings appSettings)
    {
        _appSettings = appSettings;
        Settings.WorkTimeMinutes = appSettings.WorkTimeMinutes;
        Settings.WorkTimeSeconds = appSettings.WorkTimeSeconds;
        Settings.ShortBreakMinutes = appSettings.ShortBreakMinutes;
        Settings.ShortBreakSeconds = appSettings.ShortBreakSeconds;
        Settings.LongBreakMinutes = appSettings.LongBreakMinutes;
        Settings.LongBreakSeconds = appSettings.LongBreakSeconds;
        Reset();
    }

    public void SkipBreak()
    {
        if (_currentState != PomodoroTimerState.Work)
        {
            // 如果跳过休息，也需要检查是否需要恢复计时器
            if (_timerService != null)
            {
                if (!_timerWasPausedBeforeBreak && _timerService.PreviousStatusBeforePause == TimerStatus.Running)
                {
                    _timerService.Resume();
                }
                _timerWasPausedBeforeBreak = false;
            }

            _previousState = _currentState;
            _currentState = PomodoroTimerState.Work;
            _timeLeft = GetWorkTime();
            PomodoroBreakSkipped?.Invoke(this, EventArgs.Empty);
            OnTimerStateChanged();
            Toast.Success(LanguageManager.GetString("PomodoroBreakSkipped", "Break skipped"));
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (_isRunning)
        {
            // 记录当前时间，用于比较
            var currentTime = _timeLeft;
            _timeLeft = _timeLeft.Subtract(TimeSpan.FromMilliseconds(100));

            // 检查是否需要显示提示
            if (_currentState == PomodoroTimerState.Work)
            {
                // 获取配置的提示时间，默认值分别为60秒和3秒
                int warningLongTime = _appSettings?.PomodoroWarningLongTime ?? 60;
                int warningShortTime = _appSettings?.PomodoroWarningShortTime ?? 3;

                // 当时间从warningLongTime+1秒变为warningLongTime秒时显示提示
                if (
                    currentTime.TotalSeconds > warningLongTime
                    && _timeLeft.TotalSeconds <= warningLongTime
                    && _timeLeft.TotalSeconds > warningLongTime - 0.1
                )
                {
                    // 自定义秒数提示
                    Toast.Info(LanguageManager.GetString("PomodoroWorkEndingLong", warningLongTime));
                }
                // 当时间从warningShortTime+1秒变为warningShortTime秒时显示提示
                else if (
                    currentTime.TotalSeconds > warningShortTime
                    && _timeLeft.TotalSeconds <= warningShortTime
                    && _timeLeft.TotalSeconds > warningShortTime - 0.1
                )
                {
                    // 自定义秒数提示
                    Toast.Info(LanguageManager.GetString("PomodoroWorkEndingShort", warningShortTime));
                }
            }

            if (_timeLeft <= TimeSpan.Zero)
            {
                HandleTimerCompletion();
            }

            TimeUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    private void HandleTimerCompletion()
    {
        _timeLeft = TimeSpan.Zero;
        SystemSounds.Beep.Play();

        _previousState = _currentState; // 保存之前的状态

        switch (_currentState)
        {
            case PomodoroTimerState.Work:
                _completedPomodoros++;
                var breakType =
                    (_completedPomodoros % 4 == 0) ? PomodoroBreakType.LongBreak : PomodoroBreakType.ShortBreak;

                // 触发完成事件
                PomodoroCompleted?.Invoke(this, new PomodoroCompletedEventArgs(_completedPomodoros));

                // 如果有计时器服务，检查并暂停计时器
                if (_timerService != null)
                {
                    _timerWasPausedBeforeBreak = _timerService.IsPaused;
                    // 只有当计时器正在运行时才暂停它
                    if (_timerService.IsRunning)
                    {
                        _timerService.Pause();
                    }
                }

                // 触发休息开始事件
                PomodoroBreakStarted?.Invoke(this, new PomodoroBreakStartedEventArgs(breakType));

                // 自动开始休息计时
                _currentState =
                    (breakType == PomodoroBreakType.ShortBreak)
                        ? PomodoroTimerState.ShortBreak
                        : PomodoroTimerState.LongBreak;
                _timeLeft = GetBreakTime(breakType);
                break;

            case PomodoroTimerState.ShortBreak:
            case PomodoroTimerState.LongBreak:
                // 休息结束，检查是否需要恢复计时器
                if (_timerService != null)
                {
                    // 只有当计时器不是之前就暂停的，并且暂停前的状态是Running时才恢复
                    if (!_timerWasPausedBeforeBreak && _timerService.PreviousStatusBeforePause == TimerStatus.Running)
                    {
                        _timerService.Resume();
                    }
                    // 重置标记
                    _timerWasPausedBeforeBreak = false;
                }

                // 自动开始下一个工作周期
                _currentState = PomodoroTimerState.Work;
                _timeLeft = GetWorkTime();
                SystemSounds.Beep.Play();
                break;
        }

        OnTimerStateChanged();
    }

    private void InitializeTimer()
    {
        _currentState = PomodoroTimerState.Work;
        _previousState = PomodoroTimerState.Work;
        _timeLeft = GetWorkTime();
        _isRunning = false;
        _timerWasPausedBeforeBreak = false;
    }

    private TimeSpan GetWorkTime()
    {
        return new TimeSpan(0, Settings.WorkTimeMinutes, Settings.WorkTimeSeconds);
    }

    private TimeSpan GetBreakTime(PomodoroBreakType breakType)
    {
        return breakType == PomodoroBreakType.ShortBreak
            ? new TimeSpan(0, Settings.ShortBreakMinutes, Settings.ShortBreakSeconds)
            : new TimeSpan(0, Settings.LongBreakMinutes, Settings.LongBreakSeconds);
    }

    private void OnTimerStateChanged()
    {
        PomodoroTimerStateChanged?.Invoke(
            this,
            new PomodoroTimerStateChangedEventArgs(_currentState, _previousState, _isRunning, _timeLeft)
        );
    }

    // 公共属性
    public bool IsRunning => _isRunning;
    public TimeSpan TimeLeft => _timeLeft;
    public int CompletedPomodoros => _completedPomodoros;
    public PomodoroTimerState CurrentState => _currentState;
    public PomodoroTimerState PreviousState => _previousState;
}
