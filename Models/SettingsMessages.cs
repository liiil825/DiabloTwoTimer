namespace DiabloTwoMFTimer.Models;

// 1. 热键变更消息
public class HotkeysChangedMessage { }

// 2. 窗口位置变更消息
public class WindowPositionChangedMessage { }

// 3. 始终置顶变更消息
public class AlwaysOnTopChangedMessage { }

public class LanguageChangedMessage(string code)
{
    public string LanguageCode { get; } = code;
}

public class TimerShowLootDropsChangedMessage { }
