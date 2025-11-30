namespace DiabloTwoMFTimer.Models;

/// <summary>
/// 计时器状态枚举
/// </summary>
public enum TimerStatus
{
    /// <summary>
    /// 未运行
    /// </summary>
    Stopped,

    /// <summary>
    /// 正在运行
    /// </summary>
    Running,

    /// <summary>
    /// 已暂停
    /// </summary>
    Paused,
}
