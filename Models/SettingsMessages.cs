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

public class SuspendHotkeysMessage { }

public class ResumeHotkeysMessage { }

// 【新增】请求隐藏主窗口消息
public class HideMainWindowMessage { }

// 请求显示主窗口消息
public class ShowMainWindowMessage { }

public class OpacityChangedMessage { }

// 请求截图消息 (携带掉落名称)
public class ScreenshotRequestedMessage(string lootName)
{
    public string LootName { get; } = lootName;
}

public class ShowLeaderKeyFormMessage { }

/// <summary>
/// 请求打开番茄钟设置界面
/// </summary>
public class ShowPomodoroSettingsMessage { }

/// <summary>
/// 请求打开番茄钟休息界面
/// </summary>
public class ShowPomodoroBreakFormMessage { }

/// <summary>
/// 请求最小化到系统托盘
/// </summary>
public class MinimizeToTrayMessage { }

/// <summary>
/// 请求从系统托盘恢复
/// </summary>
public class RestoreFromTrayMessage { }

/// <summary>
/// 请求切换窗口可见性（最小化到托盘或从托盘恢复）
/// </summary>
public class ToggleWindowVisibilityMessage { }

public class ShowRecordLootFormMessage { }

// 番茄钟模式变更消息
public class PomodoroModeChangedMessage(Models.PomodoroMode newMode)
{
    public Models.PomodoroMode NewMode { get; } = newMode;
}

// 请求切换掉落记录可见性消息
public class ToggleLootVisibilityMessage { }

// 请求创建角色消息
public class CreateCharacterMessage { }

// 请求切换角色消息
public class SwitchCharacterMessage { }

// 请求导出角色消息
public class ExportCharacterMessage { }

// 请求显示掉落历史消息
public class ShowLootHistoryMessage { }

// 请求删除角色消息
public class DeleteCharacterMessage { }

// 番茄钟设置变更消息
public class PomodoroSettingsChangedMessage
{
    public bool IsTimerDataChanged { get; set; }
}

// 导航栏可见性变更消息
public class NavigationVisibilityChangedMessage { }
