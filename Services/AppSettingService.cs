using System;
using System.IO;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Utils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DiabloTwoMFTimer.Services;

public class AppSettings : IAppSettings
{
    private static readonly ISerializer serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    private static readonly IDeserializer deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    // 窗口设置
    public string WindowPosition { get; set; } = "TopRight";
    public double Opacity { get; set; } = 1.0;
    public float UiScale { get; set; } = 0f;
    public bool AlwaysOnTop { get; set; } = true;
    public string Language { get; set; } = "Chinese";

    // 角色档案设置
    public string LastUsedProfile { get; set; } = "";
    public string LastRunScene { get; set; } = "";
    public string LastUsedDifficulty { get; set; } = "";

    // 番茄时钟设置
    public int WorkTimeMinutes { get; set; } = 25;
    public int WorkTimeSeconds { get; set; } = 0;
    public int ShortBreakMinutes { get; set; } = 5;
    public int ShortBreakSeconds { get; set; } = 0;
    public int LongBreakMinutes { get; set; } = 15;
    public int LongBreakSeconds { get; set; } = 0;

    public Models.PomodoroMode PomodoroMode { get; set; } = Models.PomodoroMode.Automatic;

    // 界面设置
    public bool TimerShowLootDrops { get; set; } = false; // 是否显示掉落记录
    public bool TimerShowPomodoro { get; set; } = true; // 是否显示番茄钟
    public bool TimerSyncStartPomodoro { get; set; } = false; // 开启计时器时是否同步开启番茄钟
    public bool TimerSyncPausePomodoro { get; set; } = false; // 暂停计时器时是否同步暂停番茄钟
    public bool ScreenshotOnLoot { get; set; } = false;
    public bool HideWindowOnScreenshot { get; set; } = false;
    public int PomodoroWarningLongTime { get; set; } = 60; // 番茄钟长时间提示（实际值）
    public int PomodoroWarningShortTime { get; set; } = 3; // 番茄钟短时间提示（实际值）
    public bool GenerateRoomName { get; set; } = true; // 是否生成房间名称
    public bool ShowNavigation { get; set; } = true; // 是否显示导航栏

    public Keys HotkeyLeader { get; set; } = Keys.Space | Keys.Control;
    public Keys HotkeyStartOrNext { get; set; } = Keys.None;
    public Keys HotkeyPause { get; set; } = Keys.None;
    public Keys HotkeyDeleteHistory { get; set; } = Keys.None;
    public Keys HotkeyRecordLoot { get; set; } = Keys.None;

    public bool AudioEnabled { get; set; } = true;
    public int AudioVolume { get; set; } = 100;
    public string SoundTimerStart { get; set; } = "timer_start.mp3";
    public string SoundTimerPause { get; set; } = "timer_pause.mp3";
    public string SoundBreakStart { get; set; } = "break_start.mp3";
    public string SoundBreakEnd { get; set; } = "break_end.mp3";

    // 保存设置
    public void Save()
    {
        var configFilePath = FolderManager.ConfigFilePath;
        try
        {
            // 确保目录存在 - 添加null检查以修复CS8604警告
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

    // 加载设置
    public static IAppSettings Load()
    {
        try
        {
            var configFilePath = FolderManager.ConfigFilePath;
            // 确保目录存在 - 添加null检查以修复CS8604警告
            string? directory = Path.GetDirectoryName(configFilePath);
            if (directory != null)
            {
                Directory.CreateDirectory(directory);
            }
            if (File.Exists(configFilePath))
            {
                var yaml = File.ReadAllText(configFilePath);
                var settings = deserializer.Deserialize<AppSettings>(yaml);
                return settings;
            }
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("AppSettings", $"加载设置失败", ex);
        }

        // 返回默认设置
        LogManager.WriteDebugLog("AppSettings", "返回默认设置");
        return new AppSettings();
    }

    // UI相关转换方法
    // 将设置窗口的位置枚举转换为字符串
    public static string WindowPositionToString(Models.WindowPosition position)
    {
        return position.ToString();
    }

    // 将字符串转换为设置窗口的位置枚举
    public static Models.WindowPosition StringToWindowPosition(string positionStr)
    {
        if (Enum.TryParse<Models.WindowPosition>(positionStr, out var position))
        {
            return position;
        }
        return Models.WindowPosition.TopLeft;
    }

    // 将语言选项转换为字符串
    public static string LanguageToString(Models.LanguageOption language)
    {
        return language.ToString();
    }

    // 将字符串转换为语言选项
    public static Models.LanguageOption StringToLanguage(string languageStr)
    {
        if (Enum.TryParse<Models.LanguageOption>(languageStr, out var language))
        {
            return language;
        }
        return Models.LanguageOption.Chinese;
    }
}
