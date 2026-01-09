using System;
using System.ComponentModel;
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
    public bool TimerShowPomodoro { get; set; }
    public bool TimerSyncStartPomodoro { get; set; }
    public bool TimerSyncPausePomodoro { get; set; }
    public bool ScreenshotOnLoot { get; set; }
    public bool HideWindowOnScreenshot { get; set; }
    public bool ShowNavigation { get; set; }

    // 番茄钟提示时间设置
    public int PomodoroWarningLongTime { get; set; }
    public int PomodoroWarningShortTime { get; set; }
    public bool GenerateRoomName { get; set; }

    public Models.PomodoroMode PomodoroMode { get; set; }

    // 热键设置
    public Keys HotkeyLeader { get; set; }
    public Keys HotkeyStartOrNext { get; set; }
    public Keys HotkeyPause { get; set; }
    public Keys HotkeyDeleteHistory { get; set; }
    public Keys HotkeyRecordLoot { get; set; }

    bool AudioEnabled { get; set; }
    int AudioVolume { get; set; } // 0-100
    string SoundTimerStart { get; set; }
    string SoundTimerPause { get; set; }
    string SoundBreakStart { get; set; } // 工作结束 -> 休息开始
    string SoundBreakEnd { get; set; }   // 休息结束 -> 准备工作
    bool SoundTimerStartEnabled { get; set; }
    bool SoundTimerPauseEnabled { get; set; }
    bool SoundBreakStartEnabled { get; set; }
    bool SoundBreakEndEnabled { get; set; }

    // --- 新增：计时界面模块显示设置 ---
    bool TimerShowTimerTime { get; set; }       // 是否显示计时器时间数字 (不影响高度)
    bool TimerShowStatistics { get; set; }      // 是否显示统计信息
    bool TimerShowHistory { get; set; }         // 是否显示计时历史
    bool TimerShowLootDrops { get; set; }       // 是否显示掉落 (原 TimerShowLootDrops，保持兼容)
    bool TimerShowAccountInfo { get; set; }     // 是否显示底部账户信息

    int TimerAverageRunCount { get; set; }      // 平均时间计算样本量 (0=全部, >0=最近N场)

    // 计算当前选中的模块数量 (用于高度计算和番茄钟按钮显示)
    int VisibleModuleCount { get; }
    // 方法
    void Save();
}
