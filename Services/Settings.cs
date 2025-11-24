using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using DTwoMFTimerHelper.UI.Settings;
using DTwoMFTimerHelper.Utils;
using System.Windows.Forms;

namespace DTwoMFTimerHelper.Services {
    public interface IAppSettings {
        public string WindowPosition {
            get;
            set;
        }
        public bool AlwaysOnTop {
            get;
            set;
        }
        public string Language {
            get;
            set;
        }
        // 角色档案设置
        public string LastUsedProfile {
            get;
            set;
        }
        public int WorkTimeMinutes {
            get;
            set;
        }
        public int WorkTimeSeconds {
            get;
            set;
        }
        public int ShortBreakMinutes {
            get;
            set;
        }
        public int ShortBreakSeconds {
            get;
            set;
        }
        public int LongBreakMinutes {
            get;
            set;
        }
        public int LongBreakSeconds {
            get;
            set;
        }
        // 界面设置
        public bool TimerShowLootDrops {
            get;
            set;
        }
        public bool TimerShowPomodoro {
            get;
            set;
        }
        public bool TimerSyncStartPomodoro {
            get;
            set;
        }

    }
    public class AppSettings : IAppSettings {
        // 窗口设置
        public string WindowPosition { get; set; } = "TopLeft";
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

        // 界面设置
        public bool TimerShowLootDrops { get; set; } = false; // 是否显示掉落记录
        public bool TimerShowPomodoro { get; set; } = true; // 是否显示番茄钟
        public bool TimerSyncStartPomodoro { get; set; } = false; // 开启计时器时是否同步开启番茄钟

        public Keys HotkeyStartOrNext { get; set; } = Keys.Q | Keys.Alt;
        public Keys HotkeyPause { get; set; } = Keys.Space | Keys.Control;
        public Keys HotkeyDeleteHistory { get; set; } = Keys.D | Keys.Control;
        public Keys HotkeyRecordLoot { get; set; } = Keys.A | Keys.Alt;
    }

    public static class SettingsManager {
        private static readonly string ConfigFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "mf-time-helper",
            "config.yaml");

        private static readonly ISerializer serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        private static readonly IDeserializer deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        // 加载设置
        public static AppSettings LoadSettings() {
            try {
                // 确保目录存在 - 添加null检查以修复CS8604警告
                string? directory = Path.GetDirectoryName(ConfigFilePath);
                if (directory != null) {
                    Directory.CreateDirectory(directory);
                }

                if (File.Exists(ConfigFilePath)) {
                    var yaml = File.ReadAllText(ConfigFilePath);
                    return deserializer.Deserialize<AppSettings>(yaml);
                }
            }
            catch (Exception ex) {
                LogManager.WriteErrorLog("SettingsManager", $"加载设置失败", ex);
            }

            // 返回默认设置
            return new AppSettings();
        }

        // 保存设置
        public static void SaveSettings(AppSettings settings) {
            try {
                // 确保目录存在 - 添加null检查以修复CS8604警告
                string? directory = Path.GetDirectoryName(ConfigFilePath);
                if (directory != null) {
                    Directory.CreateDirectory(directory);
                }

                var yaml = serializer.Serialize(settings);
                File.WriteAllText(ConfigFilePath, yaml);
            }
            catch (Exception ex) {
                LogManager.WriteErrorLog("SettingsManager", $"保存设置失败", ex);
            }
        }

        // 将设置窗口的位置枚举转换为字符串
        public static string WindowPositionToString(SettingsControl.WindowPosition position) {
            return position.ToString();
        }

        // 将字符串转换为设置窗口的位置枚举
        public static SettingsControl.WindowPosition StringToWindowPosition(string positionStr) {
            if (Enum.TryParse<SettingsControl.WindowPosition>(positionStr, out var position)) {
                return position;
            }
            return SettingsControl.WindowPosition.TopLeft;
        }

        // 将语言选项转换为字符串
        public static string LanguageToString(SettingsControl.LanguageOption language) {
            return language.ToString();
        }

        // 将字符串转换为语言选项
        public static SettingsControl.LanguageOption StringToLanguage(string languageStr) {
            if (Enum.TryParse<SettingsControl.LanguageOption>(languageStr, out var language)) {
                return language;
            }
            return SettingsControl.LanguageOption.Chinese;
        }
    }
}