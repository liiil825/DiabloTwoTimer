using System.Windows.Forms;

namespace DiabloTwoMFTimer.Interfaces;

public interface IAppSettings
{
    public string WindowPosition { get; set; }
    double Opacity { get; set; } // 新增：透明度 (0.1 ~ 1.0)
    public float UiScale { get; set; }
    public string Language { get; set; }
    public bool AlwaysOnTop { get; set; }

    // 角色档案设置
    public string LastUsedProfile { get; set; }
    public string LastRunScene { get; set; }
    public string LastUsedDifficulty { get; set; }
    public int WorkTimeMinutes { get; set; }
    public int WorkTimeSeconds { get; set; }
    public int ShortBreakMinutes { get; set; }
    public int ShortBreakSeconds { get; set; }
    public int LongBreakMinutes { get; set; }
    public int LongBreakSeconds { get; set; }

    // 界面设置
    public bool TimerShowLootDrops { get; set; }
    public bool TimerShowPomodoro { get; set; }
    public bool TimerSyncStartPomodoro { get; set; }
    public bool TimerSyncPausePomodoro { get; set; }
    public bool ScreenshotOnLoot { get; set; }
    public bool HideWindowOnScreenshot { get; set; }

    // 番茄钟提示时间设置
    public int PomodoroWarningLongTime { get; set; }
    public int PomodoroWarningShortTime { get; set; }
    public bool GenerateRoomName { get; set; }

    public Models.PomodoroMode PomodoroMode { get; set; }

    // 热键设置
    public Keys HotkeyStartOrNext { get; set; }
    public Keys HotkeyPause { get; set; }
    public Keys HotkeyDeleteHistory { get; set; }
    public Keys HotkeyRecordLoot { get; set; }

    // 方法
    void Save();
}
