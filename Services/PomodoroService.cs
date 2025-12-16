using System;
using System.Media;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.Services;

public class PomodoroTimerService : IPomodoroTimerService
{
    // --- 事件定义 ---
    public event EventHandler<PomodoroTimerStateChangedEventArgs>? PomodoroTimerStateChanged;
    public event EventHandler<PomodoroCompletedEventArgs>? PomodoroCompleted;
    public event EventHandler<PomodoroBreakStartedEventArgs>? PomodoroBreakStarted;
    public event EventHandler? PomodoroBreakSkipped;
    public event EventHandler? TimeUpdated;

    // --- 核心字段 ---
    private readonly System.Timers.Timer _timer;
    private readonly ITimerService? _timerService;
    private readonly IAppSettings? _appSettings;

    // --- 状态数据 ---
    private TimeSpan _timeLeft;
    private TimerStatus _status = TimerStatus.Stopped; // 使用枚举管理内部状态
    private int _completedPomodoros = 0;
    private PomodoroTimerState _currentState = PomodoroTimerState.Work;
    private PomodoroTimerState _previousState = PomodoroTimerState.Work;

    // 设置
    public PomodoroTimeSettings Settings { get; set; } = new();

    // --- 属性 ---

    // 为了兼容接口 (UI绑定)，将 Running 状态映射为 bool
    public bool IsRunning => _status == TimerStatus.Running;

    // 暴露具体的枚举状态 (供内部或高级UI逻辑判断)
    public TimerStatus Status => _status;

    public TimeSpan TimeLeft => _timeLeft;
    public int CompletedPomodoros => _completedPomodoros;
    public PomodoroTimerState CurrentState => _currentState;
    public PomodoroTimerState PreviousState => _previousState;

    // 仅在非工作时间（即休息时间）允许查看统计
    public bool CanShowStats => _currentState != PomodoroTimerState.Work;
    public DateTime PomodoroCycleStartTime { get; private set; } = DateTime.Now;
    public DateTime FullPomodoroCycleStartTime { get; private set; } = DateTime.Now;

    public PomodoroTimerService(ITimerService? timerService, IAppSettings? appSettings)
    {
        _timerService = timerService;
        _appSettings = appSettings;

        _timer = new System.Timers.Timer(100)
        {
            AutoReset = true // 确保它会循环触发
        };
        _timer.Elapsed += OnTimerTick; // 注意：事件名从 Tick 变成了 Elapse

        // 显式初始化为停止状态，防止自动运行
        _status = TimerStatus.Stopped;

        // 加载设置（会调用 Reset 更新时间，但保持 Stopped）
        LoadSettings();

        // 订阅外部计时器事件
        if (_timerService != null)
        {
            _timerService.TimerRunningStateChangedEvent += OnGameTimerRunningStateChanged;
            _timerService.TimerPauseStateChangedEvent += OnGameTimerPauseStateChanged;
        }
    }

    #region 外部控制接口 (Start/Pause/Reset/Skip)

    public void Start()
    {
        // 场景1：如果在“等待触发”状态 (Paused 且时间为0)，点击开始意味着“进入下一阶段”
        // 对应：半自动/全手动模式下，工作/休息结束后的等待
        if (_status == TimerStatus.Paused && _timeLeft <= TimeSpan.Zero)
        {
            TransitionToNextStage();
            return;
        }

        // 场景2：普通暂停后的恢复，或从停止状态开始
        if (_status != TimerStatus.Running)
        {
            if (_status == TimerStatus.Stopped)
            {
                PomodoroCycleStartTime = DateTime.Now;
                FullPomodoroCycleStartTime = DateTime.Now;
            }
            _status = TimerStatus.Running;
            _timer.Start();
            NotifyStateChanged();
        }
    }

    public void Pause()
    {
        if (_status == TimerStatus.Running)
        {
            _status = TimerStatus.Paused;
            _timer.Stop();
            NotifyStateChanged();
        }
    }

    public void Reset()
    {
        _timer.Stop();
        _status = TimerStatus.Stopped; // 回到停止状态
        _completedPomodoros = 0;
        _currentState = PomodoroTimerState.Work;
        _previousState = PomodoroTimerState.Work;

        // 重置时间为工作时间
        _timeLeft = GetDurationForState(PomodoroTimerState.Work);

        NotifyStateChanged();
        NotifyTimeUpdated();
    }

    // 强制进入下一阶段 (用于手动跳过，或半自动模式下的触发)
    public void SwitchToNextState()
    {
        if (!IsRunning)
            return;

        TransitionToNextStage();
    }

    public void SkipBreak()
    {
        if (_currentState == PomodoroTimerState.Work)
            return;

        TransitionToNextStage();
        PomodoroBreakSkipped?.Invoke(this, EventArgs.Empty);
        Toast.Success(LanguageManager.GetString("PomodoroBreakSkipped", "Break skipped"));
    }

    public void AddMinutes(int minutes)
    {
        _timeLeft = _timeLeft.Add(TimeSpan.FromMinutes(minutes));
        NotifyTimeUpdated();
    }

    #endregion

    #region 核心状态机逻辑

    private void OnTimerTick(object? sender, EventArgs e)
    {
        // 双重检查：只有 Running 状态才走字
        if (_status != TimerStatus.Running)
            return;

        // 扣减时间
        _timeLeft = _timeLeft.Subtract(TimeSpan.FromMilliseconds(100));

        // 检查倒计时结束
        if (_timeLeft <= TimeSpan.Zero)
        {
            _timeLeft = TimeSpan.Zero;
            HandleTimerExpiration();
        }
        else
        {
            // 检查剩余时间提示（仅工作状态）
            CheckForWarnings();
        }

        NotifyTimeUpdated();
    }

    /// <summary>
    /// 处理当前阶段时间耗尽
    /// </summary>
    private void HandleTimerExpiration()
    {
        // 1. 先暂停计时器
        _timer.Stop();
        SystemSounds.Beep.Play();

        // 2. 判断当前模式是否需要“等待”
        bool shouldWait = CheckIfShouldWait(_appSettings?.PomodoroMode ?? PomodoroMode.Automatic);

        if (shouldWait)
        {
            // --- 进入“等待触发”状态 ---
            // 状态设为 Paused，时间保持 00:00
            // 此时界面显示 00:00，按钮显示“开始/跳过”，等待外部触发
            _status = TimerStatus.Paused;
            NotifyStateChanged();
        }
        else
        {
            // --- 全自动模式 ---
            // 直接流转到下一阶段
            TransitionToNextStage();
        }
    }

    /// <summary>
    /// 核心流转逻辑：执行从当前阶段到下一阶段的切换
    /// </summary>
    private void TransitionToNextStage()
    {
        // 1. 记录旧状态
        _previousState = _currentState;

        // 2. 根据旧状态决定新状态和动作
        if (_currentState == PomodoroTimerState.Work)
        {
            // >>> 工作 -> 休息 <<<
            _completedPomodoros++;

            PomodoroCompleted?.Invoke(this, new PomodoroCompletedEventArgs(_completedPomodoros));

            bool isLongBreak = (_completedPomodoros % 4 == 0);
            var breakType = isLongBreak ? PomodoroBreakType.LongBreak : PomodoroBreakType.ShortBreak;

            _currentState = isLongBreak ? PomodoroTimerState.LongBreak : PomodoroTimerState.ShortBreak;

            // 先重置时间
            _timeLeft = GetDurationForState(_currentState);
            TryPauseGameTimer();
            // 再触发UI弹窗 (UI会读取到新的休息时间)
            PomodoroBreakStarted?.Invoke(this, new PomodoroBreakStartedEventArgs(breakType));
        }
        else
        {
            // >>> 休息 -> 工作 <<<
            PomodoroCycleStartTime = DateTime.Now;
            if (_currentState == PomodoroTimerState.LongBreak)
                FullPomodoroCycleStartTime = DateTime.Now;
            _currentState = PomodoroTimerState.Work;

            _timeLeft = GetDurationForState(_currentState);
            TryResumeGameTimer();
        }

        // 3. 进入新阶段后，状态自动变为 Running 并开始计时
        // 你的需求："进入休息时间并计时开始"
        _status = TimerStatus.Running;
        _timer.Start();

        NotifyStateChanged();
        NotifyTimeUpdated();
    }

    /// <summary>
    /// 判断是否应该在当前阶段结束时暂停等待
    /// </summary>
    private bool CheckIfShouldWait(PomodoroMode mode)
    {
        // 模式 3: 全手动 (工作结束等待；休息结束等待)
        if (mode == PomodoroMode.Manual)
            return true;

        // 模式 2: 半自动 (工作->休息：暂停等待；休息->工作：自动)
        if (mode == PomodoroMode.SemiAuto)
        {
            if (_currentState == PomodoroTimerState.Work)
                return true; // 工作结束，停下来等触发
            return false; // 休息结束，自动进工作
        }

        // 模式 1: 全自动 (双向都自动)
        return false;
    }

    #endregion

    #region 辅助逻辑

    private TimeSpan GetDurationForState(PomodoroTimerState state)
    {
        return state switch
        {
            PomodoroTimerState.ShortBreak => new TimeSpan(0, Settings.ShortBreakMinutes, Settings.ShortBreakSeconds),
            PomodoroTimerState.LongBreak => new TimeSpan(0, Settings.LongBreakMinutes, Settings.LongBreakSeconds),
            _ => new TimeSpan(0, Settings.WorkTimeMinutes, Settings.WorkTimeSeconds), // Work
        };
    }

    private void CheckForWarnings()
    {
        if (_currentState != PomodoroTimerState.Work)
            return;

        double totalSeconds = _timeLeft.TotalSeconds;
        int warnLong = _appSettings?.PomodoroWarningLongTime ?? 60;
        int warnShort = _appSettings?.PomodoroWarningShortTime ?? 3;
        var pomodoroMode = _appSettings?.PomodoroMode ?? PomodoroMode.Automatic;

        // 只有在整秒附近才触发，避免多次触发
        if (Math.Abs(totalSeconds - warnLong) < 0.05)
        {
            LogManager.WriteDebugLog("Pomodoro", $"工作时间警告：{warnLong}秒");
            var message = LanguageManager.GetString("PomodoroWorkEndingLong", warnLong);
            if (pomodoroMode == PomodoroMode.Automatic)
                Toast.Warning(message);
            else
                Toast.Info(message);
        }
        else if (Math.Abs(totalSeconds - warnShort) < 0.05)
        {
            LogManager.WriteDebugLog("Pomodoro", $"工作时间警告：{warnShort}秒");
            var message = LanguageManager.GetString("PomodoroWorkEndingShort", warnShort);
            if (pomodoroMode == PomodoroMode.Automatic)
                Toast.Error(message);
            else
                Toast.Info(message);
        }
    }

    private void TryPauseGameTimer()
    {
        _timerService!.Pause();
    }

    private void TryResumeGameTimer()
    {
        if (
            _timerService != null
            && !_timerService.IsRunning
            && _timerService.PreviousStatusBeforePause == TimerStatus.Running
        )
        {
            _timerService.Resume();
        }
    }

    #endregion

    #region 外部事件响应 (Game Timer Events)

    // 当游戏开始 (Timer Running = true)
    private void OnGameTimerRunningStateChanged(bool isGameRunning)
    {
        if (!isGameRunning)
            return;

        // 【核心修复】增加这一行：检查游戏计时器是否真的在运行。
        // 因为程序启动恢复记录时，会触发 isGameRunning=true，但状态其实是 Paused。
        // 这时 _timerService.IsRunning 会返回 false。
        if (_timerService != null && !_timerService.IsRunning)
        {
            // 如果收到运行信号，但实际上服务状态不是运行中（比如是恢复的暂停状态），则忽略
            return;
        }

        // 场景 A: 处于“等待触发”状态 (Paused && Time=0)
        // 解释：比如半自动模式下，工作结束了，番茄钟停在 00:00 (Paused)。
        // 此时用户开始游戏 (Game Start)，这被视为“触发信号”，进入下一阶段并开始计时。
        if (_status == TimerStatus.Paused && _timeLeft <= TimeSpan.Zero)
        {
            TransitionToNextStage();
        }
        // 场景 B: 同步开启 (Stopped -> Start)
        // 解释：番茄钟完全没跑 (Stopped)，用户开始了游戏，且开启了同步启动设置。
        else if (_appSettings?.TimerSyncStartPomodoro == true && _status == TimerStatus.Stopped)
        {
            Start();
        }
    }

    // 当游戏暂停/恢复
    private void OnGameTimerPauseStateChanged(bool isGamePaused)
    {
        // 需求：同步暂停 (TimerSyncPausePomodoro)
        if (_appSettings?.TimerSyncPausePomodoro == true)
        {
            if (isGamePaused)
            {
                // 只有在【工作状态】且【正在运行】时才同步暂停。
                // 休息时间是现实时间，不应该因为游戏暂停了就停止休息倒计时。
                if (_currentState == PomodoroTimerState.Work && _status == TimerStatus.Running)
                {
                    Pause();
                }
            }
            else // Game Resumed (暂停恢复)
            {
                // 如果番茄钟处于【工作状态】且【被暂停】(非等待触发状态)，则恢复
                if (
                    _currentState == PomodoroTimerState.Work
                    && _status == TimerStatus.Paused
                    && _timeLeft > TimeSpan.Zero
                )
                {
                    Start();
                }
            }
        }

        if (_appSettings?.TimerSyncStartPomodoro == true && !isGamePaused && _status == TimerStatus.Stopped)
        {
            Start();
        }
        // 特殊情况补漏：如果游戏恢复了 (Paused -> Running)，且我们处于等待触发状态 (Time=0)
        // 这通常由 OnGameTimerRunningStateChanged 处理，但以防万一
        if (!isGamePaused && _status == TimerStatus.Paused && _timeLeft <= TimeSpan.Zero)
        {
            TransitionToNextStage();
        }
    }

    #endregion

    #region Helper & Loaders

    public void LoadSettings()
    {
        if (_appSettings == null)
            return;
        Settings.WorkTimeMinutes = _appSettings.WorkTimeMinutes;
        Settings.WorkTimeSeconds = _appSettings.WorkTimeSeconds;
        Settings.ShortBreakMinutes = _appSettings.ShortBreakMinutes;
        Settings.ShortBreakSeconds = _appSettings.ShortBreakSeconds;
        Settings.LongBreakMinutes = _appSettings.LongBreakMinutes;
        Settings.LongBreakSeconds = _appSettings.LongBreakSeconds;

        // 加载设置时重置，确保状态回到 Stopped
        Reset();
    }

    private void NotifyStateChanged()
    {
        // 转换 TimerStatus 为 bool isRunning 传递给 UI
        bool isRunningBool = (_status == TimerStatus.Running);

        PomodoroTimerStateChanged?.Invoke(
            this,
            new PomodoroTimerStateChangedEventArgs(_currentState, _previousState, isRunningBool, _timeLeft)
        );
    }

    private void NotifyTimeUpdated()
    {
        TimeUpdated?.Invoke(this, EventArgs.Empty);
    }

    #endregion
}
