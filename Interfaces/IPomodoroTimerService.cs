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

    // 【新增】当前番茄周期（工作+休息）的起始时间
    DateTime PomodoroCycleStartTime { get; }
    DateTime FullPomodoroCycleStartTime { get; }

    // 方法
    void Start();
    void Pause();
    void Reset();
    void LoadSettings();
    void SkipBreak();

    // 【新增】
    void SwitchToNextState(); // 立即结束当前状态，进入下一个
    void AddMinutes(int minutes); // 增加时间
    bool CanShowStats { get; } // 是否允许显示统计（仅休息时间）
}
