using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Utils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DiabloTwoMFTimer.Services;

public class AppSettings : IAppSettings
{
    // 静态序列化配置
    private static readonly ISerializer serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    private static readonly IDeserializer deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    // ================== 窗口设置 ==================
    public string WindowPosition { get; set; } = "TopRight";

    public double Opacity { get; set; } = 1.0;
    public float UiScale { get; set; } = 0f;
    public bool AlwaysOnTop { get; set; } = true;
    public string Language { get; set; } = "Chinese";

    // ================== 角色档案设置 ==================
    public string LastUsedProfile { get; set; } = "";
    public string LastRunScene { get; set; } = "";
    public string LastUsedDifficulty { get; set; } = "";

    // ================== 番茄时钟设置 ==================
    public int WorkTimeMinutes { get; set; } = 25;
    public int WorkTimeSeconds { get; set; } = 0;
    public int ShortBreakMinutes { get; set; } = 5;
    public int ShortBreakSeconds { get; set; } = 0;
    public int LongBreakMinutes { get; set; } = 15;
    public int LongBreakSeconds { get; set; } = 0;
    public Models.PomodoroMode PomodoroMode { get; set; } = Models.PomodoroMode.Automatic;

    // ================== 界面设置 ==================
    public bool TimerShowTimerTime { get; set; } = true;
    public bool TimerShowStatistics { get; set; } = true;
    public bool TimerShowHistory { get; set; } = false;
    public bool TimerShowLootDrops { get; set; } = false;
    public bool TimerShowAccountInfo { get; set; } = true;
    public int TimerAverageRunCount { get; set; } = 0;

    [YamlIgnore]
    public int VisibleModuleCount
    {
        get
        {
            int count = 1; // 基础值
            if (TimerShowStatistics) count++;
            if (TimerShowHistory) count++;
            if (TimerShowLootDrops) count++;
            if (TimerShowAccountInfo) count++;
            return count;
        }
    }

    public bool TimerShowPomodoro { get; set; } = true;
    public bool TimerSyncStartPomodoro { get; set; } = false;
    public bool TimerSyncPausePomodoro { get; set; } = false;
    public bool ScreenshotOnLoot { get; set; } = false;
    public bool HideWindowOnScreenshot { get; set; } = false;
    public int PomodoroWarningLongTime { get; set; } = 60;
    public int PomodoroWarningShortTime { get; set; } = 3;
    public bool GenerateRoomName { get; set; } = true;
    public bool ShowNavigation { get; set; } = true;

    // ================== 热键设置 ==================
    public Keys HotkeyLeader { get; set; } = Keys.Space | Keys.Control;
    public Keys HotkeyStartOrNext { get; set; } = Keys.None;
    public Keys HotkeyPause { get; set; } = Keys.None;
    public Keys HotkeyDeleteHistory { get; set; } = Keys.None;
    public Keys HotkeyRecordLoot { get; set; } = Keys.None;

    // ================== 音频设置 ==================
    public bool AudioEnabled { get; set; } = true;
    public int AudioVolume { get; set; } = 100;
    public string SoundTimerStart { get; set; } = "timer_start.mp3";
    public string SoundTimerPause { get; set; } = "timer_pause.mp3";
    public string SoundBreakStart { get; set; } = "break_start.mp3";
    public string SoundBreakEnd { get; set; } = "break_end.mp3";
    public bool SoundTimerStartEnabled { get; set; } = true;
    public bool SoundTimerPauseEnabled { get; set; } = true;
    public bool SoundBreakStartEnabled { get; set; } = true;
    public bool SoundBreakEndEnabled { get; set; } = true;

    // ================== 方法 ==================

    public void Save()
    {
        var configFilePath = FolderManager.ConfigFilePath;
        try
        {
            string? directory = Path.GetDirectoryName(configFilePath);
            if (directory != null)
            {
                Directory.CreateDirectory(directory);
            }

            var yaml = serializer.Serialize(this);
            File.WriteAllText(configFilePath, yaml);
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("AppSettings", $"保存设置失败", ex);
        }
    }

    public static IAppSettings Load()
    {
        try
        {
            var configFilePath = FolderManager.ConfigFilePath;
            string? directory = Path.GetDirectoryName(configFilePath);
            if (directory != null)
            {
                Directory.CreateDirectory(directory);
            }
            if (File.Exists(configFilePath))
            {
                var yaml = File.ReadAllText(configFilePath);
                var settings = deserializer.Deserialize<AppSettings>(yaml);

                // 加载后手动触发一次 VisibleModuleCount 相关的逻辑可能需要，
                // 但通常 UI 初始化时会读取属性，所以没关系。
                return settings;
            }
        }
        catch (Exception ex)
        {
            // 如果加载失败（比如旧配置文件不兼容严重错误），记录日志并返回默认
            LogManager.WriteErrorLog("AppSettings", $"加载设置失败", ex);
        }

        LogManager.WriteDebugLog("AppSettings", "返回默认设置");
        return new AppSettings();
    }

    public static string WindowPositionToString(Models.WindowPosition position)
    {
        return position.ToString();
    }

    public static Models.WindowPosition StringToWindowPosition(string positionStr)
    {
        if (Enum.TryParse<Models.WindowPosition>(positionStr, out var position))
        {
            return position;
        }
        return Models.WindowPosition.TopLeft;
    }

    public static string LanguageToString(Models.LanguageOption language)
    {
        return language.ToString();
    }

    public static Models.LanguageOption StringToLanguage(string languageStr)
    {
        if (Enum.TryParse<Models.LanguageOption>(languageStr, out var language))
        {
            return language;
        }
        return Models.LanguageOption.Chinese;
    }
}