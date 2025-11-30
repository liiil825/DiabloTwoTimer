namespace DiabloTwoMFTimer.Models; // 或者 .Messages

// 定义一个纯数据类 (POCO)，承载需要传递的信息
public class TimerSettingsChangedMessage
{
    public bool ShowPomodoro { get; }
    public bool ShowLootDrops { get; }
    public bool SyncStartPomodoro { get; }
    public bool SyncPausePomodoro { get; }
    public bool GenerateRoomName { get; }

    public TimerSettingsChangedMessage(
        bool showPomodoro,
        bool showLootDrops,
        bool syncStartPomodoro,
        bool syncPausePomodoro,
        bool generateRoomName
    )
    {
        ShowPomodoro = showPomodoro;
        ShowLootDrops = showLootDrops;
        SyncStartPomodoro = syncStartPomodoro;
        SyncPausePomodoro = syncPausePomodoro;
        GenerateRoomName = generateRoomName;
    }
}
