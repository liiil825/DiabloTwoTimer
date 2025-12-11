using System;

namespace DiabloTwoMFTimer.Models;

// 枚举和事件参数类
public enum PomodoroTimerState
{
    Work,
    ShortBreak,
    LongBreak,
}

public enum PomodoroBreakType
{
    ShortBreak,
    LongBreak,
}

public enum PomodoroMode
{
    Automatic, // 严格模式：全自动
    SemiAuto,   // 半自动：工作->休息(手动)，休息->工作(自动)
    Manual,    // 手动模式：双向都需要手动触发
}

public class PomodoroTimeSettings
{
    public int WorkTimeMinutes { get; set; } = 25;
    public int WorkTimeSeconds { get; set; } = 0;
    public int ShortBreakMinutes { get; set; } = 5;
    public int ShortBreakSeconds { get; set; } = 0;
    public int LongBreakMinutes { get; set; } = 15;
    public int LongBreakSeconds { get; set; } = 0;
}

// 番茄钟事件参数类
public class PomodoroTimerStateChangedEventArgs(
    PomodoroTimerState state,
    PomodoroTimerState previousState,
    bool isRunning,
    TimeSpan timeLeft
) : EventArgs
{
    public PomodoroTimerState State { get; } = state;
    public PomodoroTimerState PreviousState { get; } = previousState;
    public bool IsRunning { get; } = isRunning;
    public TimeSpan TimeLeft { get; } = timeLeft;
}

public class PomodoroCompletedEventArgs(int completedPomodoros) : EventArgs
{
    public int CompletedPomodoros { get; } = completedPomodoros;
}

public class PomodoroBreakStartedEventArgs(PomodoroBreakType breakType) : EventArgs
{
    public PomodoroBreakType BreakType { get; } = breakType;
}
