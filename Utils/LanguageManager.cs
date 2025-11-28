using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using DiabloTwoMFTimer.Models;

namespace DiabloTwoMFTimer.Utils;

public static class LanguageManager
{
    // 语言常量
    public const string Chinese = "zh-CN";
    public const string English = "en-US";

    // 当前语言
    private static CultureInfo currentCulture = CultureInfo.CurrentCulture;
    private static Dictionary<string, string> translations = [];

    // 语言变更事件
    public static event EventHandler? OnLanguageChanged;

    static LanguageManager()
    {
        LoadTranslations(currentCulture);
    }

    /// <summary>
    /// 切换语言
    /// </summary>
    /// <param name="languageCode">语言代码</param>
    public static void SwitchLanguage(string languageCode)
    {
        if (!string.IsNullOrEmpty(languageCode) && languageCode != currentCulture.Name)
        {
            currentCulture = new CultureInfo(languageCode);
            Thread.CurrentThread.CurrentCulture = currentCulture;
            Thread.CurrentThread.CurrentUICulture = currentCulture;
            LoadTranslations(currentCulture);
            OnLanguageChanged?.Invoke(typeof(LanguageManager), EventArgs.Empty);
        }
    }

    private static void LoadTranslations(CultureInfo? culture)
    {
        translations.Clear();

        // 确定要加载的语言文件
        string langCode = culture?.Name.StartsWith("zh") ?? false ? "zh" : "en";
        string jsonFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources",
            $"strings_{langCode}.json"
        );

        // 如果文件不存在，尝试使用相对路径
        if (!File.Exists(jsonFilePath))
        {
            // 尝试直接在Resources目录中查找
            jsonFilePath = Path.Combine("Resources", $"strings_{langCode}.json");
        }

        // 加载JSON文件中的翻译
        if (File.Exists(jsonFilePath))
        {
            try
            {
                string jsonContent = File.ReadAllText(jsonFilePath);
                var deserialized = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
                translations = deserialized ?? [];
            }
            catch (Exception ex)
            {
                LogManager.WriteErrorLog("LanguageManager", $"Error loading translations: {ex.Message}");
            }
        }

        // 添加必要的默认翻译字符串作为后备
        AddDefaultTranslations(langCode);
    }

    private static void AddDefaultTranslations(string langCode)
    {
        // 英文默认翻译
        var defaultTranslations = new Dictionary<string, string>()
        {
            { "HotkeySettings", "Hotkey Settings" },
            { "StartStop", "Start/Stop" },
            { "Pause", "Pause" },
            { "Set", "Set" },
            { "General", "General" },
            { "Hotkeys", "Hotkeys" },
        };

        // 如果是中文，使用中文翻译
        if (langCode == "zh")
        {
            defaultTranslations = new Dictionary<string, string>()
            {
                { "HotkeySettings", "快捷键设置" },
                { "StartStop", "开始/结束" },
                { "Pause", "暂停" },
                { "Set", "设置" },
                { "General", "通用" },
                { "Hotkeys", "快捷键" },
            };
        }

        // 添加或更新默认翻译
        foreach (var pair in defaultTranslations)
        {
            if (!translations.ContainsKey(pair.Key))
            {
                translations[pair.Key] = pair.Value;
            }
        }
    }

    /// <summary>
    /// 根据键获取本地化字符串
    /// </summary>
    /// <param name="key">字符串键</param>
    /// <param name="args">格式化参数</param>
    /// <returns>本地化后的字符串</returns>
    public static string GetString(string key, params object[] args)
    {
        if (key == null)
            return string.Empty;
        string value;

        // 首先尝试直接从翻译字典中获取
        if (translations != null && translations.TryGetValue(key, out string? tempValue) && tempValue != null)
        {
            value = tempValue;
        }
        else
        {
            return key;
        }

        // 处理格式化参数
        if (args != null && args.Length > 0)
        {
            // 使用正则表达式替换{{0}}, {{1}}等占位符
            string formattedValue = value;
            for (int i = 0; i < args.Length; i++)
            {
                string replacement = args[i]?.ToString() ?? string.Empty;
                formattedValue = Regex.Replace(formattedValue, $"\\{{\\{{{i}\\}}\\}}", replacement);
            }
            return formattedValue;
        }
        return value;
    }

    /// <summary>
    /// 获取本地化的职业名称
    /// </summary>
    /// <param name="charClass">角色职业枚举</param>
    /// <returns>本地化的职业名称</returns>
    public static string GetLocalizedClassName(CharacterClass charClass) =>
        charClass switch
        {
            CharacterClass.Barbarian => GetString("CharacterClass_Barbarian") ?? "野蛮人",
            CharacterClass.Sorceress => GetString("CharacterClass_Sorceress") ?? "法师",
            CharacterClass.Assassin => GetString("CharacterClass_Assassin") ?? "刺客",
            CharacterClass.Druid => GetString("CharacterClass_Druid") ?? "德鲁伊",
            CharacterClass.Paladin => GetString("CharacterClass_Paladin") ?? "圣骑士",
            CharacterClass.Amazon => GetString("CharacterClass_Amazon") ?? "亚马逊",
            CharacterClass.Necromancer => GetString("CharacterClass_Necromancer") ?? "死灵法师",
            _ => GetString("CharacterClass_Unknown") ?? "未知",
        };
}
