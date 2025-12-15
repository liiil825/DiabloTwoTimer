using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DiabloTwoMFTimer.Services;

public class SceneService : ISceneService
{
    private readonly IAppSettings _appSettings;
    private static string FarmingSpotsPath =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "FarmingSpots.yaml");
    private List<FarmingScene> _cachedFarmingSpots = [];

    // 构造函数注入
    public SceneService(IAppSettings appSettings)
    {
        _appSettings = appSettings;
        LoadFarmingSpots(); // 初始化时加载
    }

    public List<FarmingScene> FarmingScenes => _cachedFarmingSpots;

    public void LoadFarmingSpots()
    {
        try
        {
            var path = FarmingSpotsPath; // 私有方法，逻辑同原 Helper
            if (File.Exists(path))
            {
                var yaml = File.ReadAllText(path);
                var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
                var data = deserializer.Deserialize<FarmingSpotsData>(yaml);
                _cachedFarmingSpots = data?.FarmingSpots ?? [];
            }
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("SceneService", "加载场景失败", ex);
        }
    }

    /// <summary>
    /// 获取场景的中英文映射字典
    /// </summary>
    /// <returns>场景名称映射字典</returns>
    public Dictionary<string, string> GetSceneMappings()
    {
        var mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var spot in FarmingScenes)
        {
            // 添加英文到中文的映射
            mapping[spot.EnUS] = spot.ZhCN;
            // 添加中文到英文的映射
            mapping[spot.ZhCN] = spot.EnUS;
        }

        return mapping;
    }

    /// <summary>
    /// 获取场景的ACT值映射字典
    /// </summary>
    /// <returns>场景ACT值映射字典</returns>
    public Dictionary<string, int> GetSceneActMappings()
    {
        var mapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var spot in FarmingScenes)
        {
            // 添加英文和中文场景名称对应的ACT值
            mapping[spot.EnUS] = spot.ACT;
            mapping[spot.ZhCN] = spot.ACT;
        }

        return mapping;
    }

    public string GetSceneDisplayName(FarmingScene scene)
    {
        string actText = $"ACT {scene.ACT}";
        // 使用注入的 AppSettings 获取语言
        string name = scene.GetSceneName(_appSettings.Language);
        return $"{actText}: {name}";
    }

    public string GetLocalizedDifficultyName(GameDifficulty difficulty)
    {
        // 这里依赖 LanguageManager 静态类是 OK 的，因为 LanguageManager 是资源管理器
        return difficulty switch
        {
            GameDifficulty.Normal => LanguageManager.GetString("DifficultyNormal"),
            GameDifficulty.Nightmare => LanguageManager.GetString("DifficultyNightmare"),
            GameDifficulty.Hell => LanguageManager.GetString("DifficultyHell"),
            _ => LanguageManager.GetString("DifficultyUnknown"),
        };
    }

    public string GetLocalizedShortSceneName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
            return sceneName;

        string cleanSceneName = GetPureSceneName(sceneName);
        var scene = FarmingScenes.FirstOrDefault(s => s.EnUS == cleanSceneName || s.ZhCN == cleanSceneName);
        string result = cleanSceneName;

        switch (_appSettings.Language)
        {
            case "English":
                if (scene != null && !string.IsNullOrEmpty(scene.ShortEnName))
                    result = scene.ShortEnName;
                break;
            case "Chinese":
            default:
                if (scene != null && !string.IsNullOrEmpty(scene.ShortZhCN))
                    result = scene.ShortZhCN;
                break;
        }
        return result;
    }

    /// <summary>
    /// 根据索引获取游戏难度
    /// </summary>
    /// <param name="difficultyIndex">难度索引（0=普通，1=噩梦，2=地狱）</param>
    /// <returns>游戏难度枚举值</returns>
    public GameDifficulty GetDifficultyByIndex(int difficultyIndex)
    {
        return difficultyIndex switch
        {
            0 => GameDifficulty.Normal,
            1 => GameDifficulty.Nightmare,
            2 => GameDifficulty.Hell,
            _ => GameDifficulty.Hell, // 默认返回地狱难度
        };
    }

    /// <summary>
    /// 根据场景名称获取对应的英文名称
    /// </summary>
    /// <param name="sceneName">场景名称（中文或英文）</param>
    /// <returns>场景的英文名称</returns>
    public string GetEnglishSceneName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
            return sceneName;

        string cleanSceneName = GetPureSceneName(sceneName);

        // 如果已经是英文，直接返回
        if (!cleanSceneName.Any(c => c >= '\u4e00' && c <= '\u9fff'))
            return cleanSceneName;

        // 查找对应的英文名称
        var mappings = GetSceneMappings();
        if (mappings.TryGetValue(cleanSceneName, out string? tempName))
        {
            if (!string.IsNullOrEmpty(tempName))
            {
                return tempName;
            }
        }

        return cleanSceneName;
    }

    public string GetPureSceneName(string sceneName)
    {
        // 移除ACT前缀（如果有），提取纯场景名称
        string pureSceneName = sceneName;
        pureSceneName = pureSceneName.Trim('"', '\'');
        if (sceneName.StartsWith("ACT ") || sceneName.StartsWith("ACT ") || sceneName.StartsWith("ACT "))
        {
            int colonIndex = sceneName.IndexOf(':');
            if (colonIndex > 0)
            {
                pureSceneName = sceneName[(colonIndex + 1)..].Trim();
            }
        }
        return pureSceneName;
    }

    public string GetSceneName(string sceneName, SceneNameType type)
    {
        if (string.IsNullOrEmpty(sceneName))
            return sceneName;

        string cleanSceneName = GetPureSceneName(sceneName);
        var scene = FarmingScenes.FirstOrDefault(s =>
            string.Equals(s.EnUS, cleanSceneName, StringComparison.OrdinalIgnoreCase)
        );

        return type switch
        {
            SceneNameType.English => cleanSceneName,
            SceneNameType.Chinese => scene?.ZhCN ?? cleanSceneName,
            SceneNameType.ShortEnglish => scene?.ShortEnName ?? cleanSceneName,
            SceneNameType.ShortChinese => scene?.ShortZhCN ?? cleanSceneName,
            _ => cleanSceneName,
        };
    }

    /// <summary>
    /// 通过场景名称返回shortName
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <summary>
    /// 根据场景名称获取场景的短名称
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="characterName">角色名称，用于判断使用中文还是英文短名称</param>
    /// <returns>场景的短名称</returns>
    public string GetSceneShortName(string sceneName, string characterName = "")
    {
        // 先调用getPureSceneName获取纯场景名称
        string pureSceneName = GetPureSceneName(sceneName);

        // 查找匹配的场景
        var scene = FarmingScenes.FirstOrDefault(s =>
            string.Equals(s.EnUS, pureSceneName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(s.ZhCN, pureSceneName, StringComparison.OrdinalIgnoreCase)
        );

        if (scene == null)
        {
            return pureSceneName;
        }

        // 判断角色名是否包含中文字符
        bool isChineseCharacter =
            !string.IsNullOrEmpty(characterName) && characterName.Any(c => c >= 0x4e00 && c <= 0x9fff);

        // 根据角色名是否为中文选择对应的短名称
        if (isChineseCharacter)
        {
            return !string.IsNullOrEmpty(scene.ShortZhCN)
                ? scene.ShortZhCN
                : (
                    !string.IsNullOrEmpty(scene.ShortEnName)
                        ? scene.ShortEnName
                        : (!string.IsNullOrEmpty(scene.ShortZhCN) ? scene.ShortZhCN : pureSceneName)
                );
        }
        else
        {
            return !string.IsNullOrEmpty(scene.ShortEnName)
                ? scene.ShortEnName
                : (!string.IsNullOrEmpty(scene.ShortZhCN) ? scene.ShortZhCN : pureSceneName);
        }
    }

    /// <summary>
    /// 根据场景名称获取ACT值
    /// </summary>
    public int GetSceneActValue(string sceneName)
    {
        try
        {
            if (string.IsNullOrEmpty(sceneName))
                return 0;

            // 移除ACT前缀（如果有），提取纯场景名称
            string pureSceneName = GetPureSceneName(sceneName);

            var sceneActMappings = GetSceneActMappings();

            // 尝试在映射中查找纯场景名称对应的ACT值
            if (sceneActMappings.TryGetValue(pureSceneName, out int actValue))
            {
                return actValue;
            }

            // 尝试查找包含场景名称的键
            foreach (var mapping in sceneActMappings)
            {
                if (
                    pureSceneName.Contains(mapping.Key, StringComparison.OrdinalIgnoreCase)
                    || mapping.Key.Contains(pureSceneName, StringComparison.OrdinalIgnoreCase)
                )
                {
                    return mapping.Value;
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("SceneService", $"提取ACT值失败", ex);
        }
        return 0; // 默认返回0
    }

    public string GetLocalizedShortSceneName(string scene, IAppSettings settings)
    {
        string pureEnglishSceneName = GetPureSceneName(scene);

        FarmingScene? farmingScene = FarmingScenes.FirstOrDefault(s =>
            string.Equals(s.EnUS, pureEnglishSceneName, StringComparison.OrdinalIgnoreCase)
        );
        return settings.Language switch
        {
            "Chinese" => farmingScene?.ShortZhCN ?? pureEnglishSceneName,
            "English" => farmingScene?.ShortEnName ?? pureEnglishSceneName,
            _ => pureEnglishSceneName,
        };
    }

    /// <summary>
    /// 根据shortEnName查找场景
    /// </summary>
    /// <param name="shortEnName">场景的英文短名称</param>
    /// <returns>对应的场景对象，如果未找到则返回null</returns>
    public FarmingScene? GetSceneByShortEnName(string shortEnName)
    {
        if (string.IsNullOrWhiteSpace(shortEnName))
            return null;

        return FarmingScenes.FirstOrDefault(s =>
            string.Equals(s.ShortEnName, shortEnName, StringComparison.OrdinalIgnoreCase)
        );
    }
}
