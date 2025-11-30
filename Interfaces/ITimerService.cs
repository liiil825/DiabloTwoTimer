using System;
using DiabloTwoMFTimer.Models;

namespace DiabloTwoMFTimer.Interfaces;

public interface ITimerService : IDisposable
{
    // 事件
    event Action<string>? TimeUpdatedEvent;
    event Action<bool>? TimerRunningStateChangedEvent;
    event Action<bool>? TimerPauseStateChangedEvent;
    event Action? TimerResetEvent;
    event Action<TimeSpan>? RunCompletedEvent;

    // 属性
    bool IsRunning { get; }
    bool IsPaused { get; }
    bool IsStopped { get; }
    TimerStatus Status { get; }
    DateTime StartTime { get; }
    TimeSpan PausedDuration { get; }
    DateTime PauseStartTime { get; }
    TimerStatus PreviousStatusBeforePause { get; }

    // 方法
    void Start();
    void CompleteRun(bool autoStartNext = false);
    void StartOrNextRun();
    void TogglePause();
    void Pause();
    void Resume();
    void Reset();
    void HandleStartFarm();
    TimeSpan GetElapsedTime();
    string GetFormattedTime();
    void HandleApplicationClosing();
    void RestoreIncompleteRecord();
    void ResetTimerRequested();
}
