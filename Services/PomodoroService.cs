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
    private readonly ITimerService? _timerService = null!; // 引用计时器服务
    private readonly IAppSettings? _appSettings = null!;

    // 标记是否在休息前暂停了计时器
    private bool _timerWasPausedBeforeBreak = false;

    // 【新增】标记是否正在等待手动触发进入下一阶段
    private bool _isWaitingForManualTrigger = false;

    // 时间设置
    public PomodoroTimeSettings Settings { get; set; }

    public PomodoroTimerService(ITimerService? timerService, IAppSettings? appSettings)
    {
        _timerService = timerService;
        _appSettings = appSettings;
        Settings = new PomodoroTimeSettings();
        _timer = new System.Windows.Forms.Timer { Interval = 1000 }; // 修正为 1000ms，之前的 60ms 太快且 Tick 逻辑是减 100ms
        // 注意：原代码 Timer_Tick 中是 Subtract(TimeSpan.FromMilliseconds(100))，Interval 应配合调整。
        // 这里保持原逻辑修正：原代码 Interval=60, Tick 减 100ms，会导致时间走得比实际快。
        // 建议改为标准秒级：Interval=1000, Tick 减 1秒。或者 Interval=100, Tick 减 100ms。
        // 为了平滑，这里设为 100ms
        _timer.Interval = 100;

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
        // 当计时器开启时，如果番茄时钟启动则启动番茄时钟（如果番茄时钟已经开启则不处理）
        if (_timerService?.IsRunning == true
                && _appSettings?.TimerSyncStartPomodoro == true)
        {
            Start();
        }
    }

    // 处理计时器运行状态变化事件
    private void OnTimerRunningStateChanged(bool isRunning)
    {
        if (isRunning)
        {
            // 如果正在等待手动触发，且检测到游戏开始，视为触发信号
            if (_isWaitingForManualTrigger)
            {
                SwitchToNextState();
            }
            else
            {
                TriggerStartFromTimer();
            }
        }
    }

    // 处理计时器暂停状态变化事件
    private void OnTimerPauseStateChanged(bool isPaused)
    {
        if (!isPaused)
        {
            // 恢复时，如果是等待状态，则触发进入下一阶段
            if (_isWaitingForManualTrigger)
            {
                SwitchToNextState();
            }
            else
            {
                TriggerStartFromTimer();
            }
        }
        // 当计时器暂停时，如果番茄钟正在运行且设置了同步暂停，则暂停番茄钟
        else if (isPaused &&
        _appSettings?.TimerSyncPausePomodoro == true && _isRunning)
        {
            Pause();
        }
    }

    public void Start()
    {
        // 如果是等待状态，点击开始意味着进入下一阶段
        if (_isWaitingForManualTrigger)
        {
            SwitchToNextState();
            return;
        }

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
        _isWaitingForManualTrigger = false;
        OnTimerStateChanged();
    }

    public void LoadSettings()
    {
        Settings.WorkTimeMinutes = _appSettings?.WorkTimeMinutes ?? 25;
        Settings.WorkTimeSeconds = _appSettings?.WorkTimeSeconds ?? 0;
        Settings.ShortBreakMinutes = _appSettings?.ShortBreakMinutes ?? 5;
        Settings.ShortBreakSeconds = _appSettings?.ShortBreakSeconds ?? 0;
        Settings.LongBreakMinutes = _appSettings?.LongBreakMinutes ?? 15;
        Settings.LongBreakSeconds = _appSettings?.LongBreakSeconds ?? 0;
        Reset();
    }

    // 【新增】增加时间
    public void AddMinutes(int minutes)
    {
        _timeLeft = _timeLeft.Add(TimeSpan.FromMinutes(minutes));
        TimeUpdated?.Invoke(this, EventArgs.Empty);
    }

    // 【新增】切换到下一个状态 (Manual Trigger / Skip)
    public void SwitchToNextState()
    {
        _isWaitingForManualTrigger = false; // 清除等待标记
        HandleTimerCompletion(forceNext: true);
    }

    public void SkipBreak()
    {
        // SkipBreak 本质上就是从休息状态跳到工作状态，可以用 SwitchToNextState 实现
        // 但为了兼容旧逻辑（如果不在休息状态跳过可能会有副作用），保留特定检查
        if (_currentState != PomodoroTimerState.Work)
        {
            SwitchToNextState();
            Toast.Success(LanguageManager.GetString("PomodoroBreakSkipped", "Break skipped"));
            PomodoroBreakSkipped?.Invoke(this, EventArgs.Empty);
        }
    }

    // 【新增】是否允许显示统计
    public bool CanShowStats => _currentState != PomodoroTimerState.Work;

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (_isRunning)
        {
            // 记录当前时间，用于比较
            var currentTime = _timeLeft;
            // 使用 100ms 步进
            _timeLeft = _timeLeft.Subtract(TimeSpan.FromMilliseconds(100));

            // 检查是否需要显示提示
            if (_currentState == PomodoroTimerState.Work)
            {
                // 获取配置的提示时间
                int warningLongTime = _appSettings?.PomodoroWarningLongTime ?? 60;
                int warningShortTime = _appSettings?.PomodoroWarningShortTime ?? 3;

                // 简单的提示逻辑...
                if (currentTime.TotalSeconds > warningLongTime && _timeLeft.TotalSeconds <= warningLongTime)
                {
                    Toast.Info(LanguageManager.GetString("PomodoroWorkEndingLong", warningLongTime));
                }
                else if (currentTime.TotalSeconds > warningShortTime && _timeLeft.TotalSeconds <= warningShortTime)
                {
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

    private void HandleTimerCompletion(bool forceNext = false)
    {
        // 如果不是强制切换（即时间自然走完），需要检查模式
        if (!forceNext && !_isWaitingForManualTrigger)
        {
            var mode = _appSettings?.PomodoroMode ?? PomodoroMode.Automatic;
            bool shouldWait = false;

            if (mode == PomodoroMode.Manual)
            {
                shouldWait = true; // 手动模式：任何阶段结束都等待
            }
            else if (mode == PomodoroMode.SemiAuto)
            {
                // 半自动：工作->休息(手动)，休息->工作(自动)
                // "1个是工作时间需要手动" -> 意味着工作结束，进入休息需要手动确认
                if (_currentState == PomodoroTimerState.Work)
                    shouldWait = true;
            }

            if (shouldWait)
            {
                _isRunning = false;
                _timer.Stop();
                _timeLeft = TimeSpan.Zero;
                _isWaitingForManualTrigger = true;
                SystemSounds.Beep.Play(); // 提示结束
                OnTimerStateChanged();
                return; // 暂停在这里，等待手动触发
            }
        }

        // --- 进入正常的切换逻辑 ---
        _isWaitingForManualTrigger = false;
        _timeLeft = TimeSpan.Zero;
        if (!forceNext) SystemSounds.Beep.Play(); // 自然结束才响铃

        _previousState = _currentState; // 保存之前的状态

        switch (_currentState)
        {
            case PomodoroTimerState.Work:
                _completedPomodoros++;
                var breakType =
                    (_completedPomodoros % 4 == 0) ? PomodoroBreakType.LongBreak : PomodoroBreakType.ShortBreak;

                // 触发完成事件
                PomodoroCompleted?.Invoke(this, new PomodoroCompletedEventArgs(_completedPomodoros));

                // 暂停主计时器逻辑 (如果需要)
                if (_timerService != null)
                {
                    _timerWasPausedBeforeBreak = _timerService.IsPaused;
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
                    if (!_timerWasPausedBeforeBreak && _timerService.PreviousStatusBeforePause == TimerStatus.Running)
                    {
                        _timerService.Resume();
                    }
                    _timerWasPausedBeforeBreak = false;
                }

                // 自动开始下一个工作周期
                _currentState = PomodoroTimerState.Work;
                _timeLeft = GetWorkTime();
                if (!forceNext) SystemSounds.Beep.Play(); // 如果是强制跳过休息，这里可能不响铃比较好
                break;
        }

        // 切换后自动开始运行新阶段
        _isRunning = true;
        _timer.Start();
        OnTimerStateChanged();
    }

    private void InitializeTimer()
    {
        _currentState = PomodoroTimerState.Work;
        _previousState = PomodoroTimerState.Work;
        _timeLeft = GetWorkTime();
        _isRunning = false;
        _timerWasPausedBeforeBreak = false;
        _isWaitingForManualTrigger = false;
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