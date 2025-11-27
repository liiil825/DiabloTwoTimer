using System;

namespace DTwoMFTimerHelper.Services;

public interface IPomodoroTimerService
{
    // 事件定义
    event EventHandler<TimerStateChangedEventArgs>? TimerStateChanged;
    event EventHandler<PomodoroCompletedEventArgs>? PomodoroCompleted;
    event EventHandler<BreakStartedEventArgs>? BreakStarted;
    event EventHandler? BreakSkipped;
    event EventHandler? TimeUpdated;

    // 属性
    TimeSettings Settings { get; set; }
    bool IsRunning { get; }
    TimeSpan TimeLeft { get; }
    int CompletedPomodoros { get; }
    TimerState CurrentState { get; }
    TimerState PreviousState { get; }

    // 方法
    void Start();
    void Pause();
    void Reset();
    void LoadSettings(IAppSettings appSettings);
    void SkipBreak();
}