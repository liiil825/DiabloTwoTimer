using System;
using DiabloTwoMFTimer.Models;

namespace DiabloTwoMFTimer.Interfaces;

public interface IPomodoroTimerService
{
    // 事件定义
    event EventHandler<PomodoroTimerStateChangedEventArgs>? PomodoroTimerStateChanged;
    event EventHandler<PomodoroCompletedEventArgs>? PomodoroCompleted;
    event EventHandler<PomodoroBreakStartedEventArgs>? PomodoroBreakStarted;
    event EventHandler? PomodoroBreakSkipped;
    event EventHandler? TimeUpdated;

    // 属性
    PomodoroTimeSettings Settings { get; set; }
    bool IsRunning { get; }
    TimeSpan TimeLeft { get; }
    int CompletedPomodoros { get; }
    PomodoroTimerState CurrentState { get; }
    PomodoroTimerState PreviousState { get; }

    // 方法
    void Start();
    void Pause();
    void Reset();
    void LoadSettings(IAppSettings appSettings);
    void SkipBreak();
}
