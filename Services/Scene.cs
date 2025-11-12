using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Utils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DTwoMFTimerHelper.Services
{
    /// <summary>
    /// 场景数据管理器，负责加载和管理游戏场景数据
    /// </summary>
    public static class SceneManager
    {
        // 场景数据的YAML文件路径
        private static string FarmingSpotsPath => FindFarmingSpotsFile();

        // 智能查找FarmingSpots.yaml文件
        private static string FindFarmingSpotsFile()
        {   
            // 构建可能的路径列表，优先检查Resources文件夹（最佳实践）
            var possiblePaths = new List<string>
            {
                // 相对应用程序基目录的Resources文件夹（推荐的打包位置）
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "FarmingSpots.yaml"),
                // 相对应用程序基目录的data文件夹
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "FarmingSpots.yaml"),
                // 相对当前工作目录
                Path.Combine(Directory.GetCurrentDirectory(), "Resources", "FarmingSpots.yaml"),
                Path.Combine(Directory.GetCurrentDirectory(), "data", "FarmingSpots.yaml")
            };
            
            // 检查哪个路径存在
            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    LogManager.WriteDebugLog("SceneManager", $"找到FarmingSpots.yaml文件：{path}");
                    return path;
                }
            }
            
            // 如果都不存在，返回应用程序基目录下的Resources路径（标准位置）
            string defaultPath = possiblePaths[0];
            LogManager.WriteDebugLog("SceneManager", $"未找到FarmingSpots.yaml文件。尝试使用默认路径：{defaultPath}");
            
            return defaultPath;
        }

        // 初始化YAML反序列化器（保持与原始DataManager一致，不使用命名约定）
        private static readonly IDeserializer sceneDeserializer = new DeserializerBuilder()
            .IgnoreUnmatchedProperties()
            .Build();

        // 缓存加载的场景数据
        private static List<FarmingScene>? _cachedFarmingSpots = null;

        /// <summary>
        /// 加载所有农场场景数据
        /// </summary>
        /// <returns>场景数据列表</returns>
        public static List<FarmingScene> LoadFarmingSpots()
        {
            // 检查缓存是否存在
            if (_cachedFarmingSpots != null)
            {
                LogManager.WriteDebugLog("SceneManager", $"返回缓存的场景数据，共 {_cachedFarmingSpots.Count} 个场景");
                return _cachedFarmingSpots;
            }

            LogManager.WriteDebugLog("SceneManager", "===== 开始加载场景数据 =====");
            LogManager.WriteDebugLog("SceneManager", $"尝试加载的文件路径: {FarmingSpotsPath}");
            LogManager.WriteDebugLog("SceneManager", $"当前应用程序基目录: {AppDomain.CurrentDomain.BaseDirectory}");
            LogManager.WriteDebugLog("SceneManager", $"当前工作目录: {Directory.GetCurrentDirectory()}");

            try
            {
                bool fileExists = File.Exists(FarmingSpotsPath);
                LogManager.WriteDebugLog("SceneManager", $"文件是否存在: {fileExists}");

                if (fileExists)
                {
                    LogManager.WriteDebugLog("SceneManager", "文件存在，开始读取内容...");
                    var yaml = File.ReadAllText(FarmingSpotsPath);
                    LogManager.WriteDebugLog("SceneManager", $"成功读取文件内容，长度: {yaml.Length} 字符");

                    var data = sceneDeserializer.Deserialize<FarmingSpotsData>(yaml);
                     
                    if (data == null)
                    {
                        LogManager.WriteDebugLog("SceneManager", "反序列化失败: 数据为null");
                        return [];
                    }
                     
                    if (data.FarmingSpots == null)
                    {
                        LogManager.WriteDebugLog("SceneManager", "反序列化失败: FarmingSpots为null");
                        return [];
                    }
                     
                    int sceneCount = data.FarmingSpots.Count;
                    LogManager.WriteDebugLog("SceneManager", $"反序列化成功，场景数量: {sceneCount}");

                    // 只输出到控制台，不再显示弹窗
                    int count = data.FarmingSpots != null ? data.FarmingSpots.Count : 0;
                    string keyInfo = $"文件路径: {FarmingSpotsPath}\n文件存在: {fileExists}\n场景数量: {count}";
                    Console.WriteLine(keyInfo);

                    // 更新缓存
                    _cachedFarmingSpots = data.FarmingSpots ?? [];
                    return data.FarmingSpots ?? [];
                }
                else
                {
                    LogManager.WriteDebugLog("SceneManager", "错误: 文件不存在");
                    // 尝试列出当前目录下的文件，帮助诊断问题
                    if (!string.IsNullOrEmpty(FarmingSpotsPath))
                    {
                        string currentDir = Path.GetDirectoryName(FarmingSpotsPath) ?? string.Empty;
                        if (!string.IsNullOrEmpty(currentDir) && Directory.Exists(currentDir))
                        {
                        var files = Directory.GetFiles(currentDir);
                        LogManager.WriteDebugLog("SceneManager", $"目录 {currentDir} 中的文件:");
                        foreach (var file in files)
                        {
                            LogManager.WriteDebugLog("SceneManager", $"  - {Path.GetFileName(file)}");
                        }
                    }
                    }
                    
                    // 尝试多个可能的正确路径，优先检查Resources文件夹
                    var possiblePaths = new List<string>
                    {
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "FarmingSpots.yaml"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "FarmingSpots.yaml"),
                        Path.Combine(Directory.GetCurrentDirectory(), "Resources", "FarmingSpots.yaml"),
                        Path.Combine(Directory.GetCurrentDirectory(), "data", "FarmingSpots.yaml")
                    };

                    foreach (string path in possiblePaths)
                    {
                        bool exists = File.Exists(path);
                        LogManager.WriteDebugLog("SceneManager", $"尝试路径: {path}, 是否存在: {exists}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("SceneManager", $"加载场景数据失败: {ex.Message}");
                LogManager.WriteDebugLog("SceneManager", $"异常类型: {ex.GetType().FullName}");
                LogManager.WriteDebugLog("SceneManager", $"异常堆栈: {ex.StackTrace}");
                
                // 如果有内部异常，也记录下来
                if (ex.InnerException != null)
                {
                    LogManager.WriteDebugLog("SceneManager", $"内部异常: {ex.InnerException.Message ?? "未知异常"}");
                    LogManager.WriteDebugLog("SceneManager", $"内部异常类型: {ex.InnerException.GetType().FullName ?? "未知类型"}");
                }
            }

            return [];
        }

        /// <summary>
        /// 获取场景的中英文映射字典
        /// </summary>
        /// <returns>场景名称映射字典</returns>
        public static Dictionary<string, string> GetSceneMappings()
        {
            var mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var farmingSpots = LoadFarmingSpots();

            foreach (var spot in farmingSpots)
            {
                // 添加英文到中文的映射
                mapping[spot.enUS] = spot.zhCN;
                // 添加中文到英文的映射
                mapping[spot.zhCN] = spot.enUS;
            }

            return mapping;
        }

        /// <summary>
        /// 获取场景的ACT值映射字典
        /// </summary>
        /// <returns>场景ACT值映射字典</returns>
        public static Dictionary<string, int> GetSceneActMappings()
        {
            var mapping = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var farmingSpots = LoadFarmingSpots();

            foreach (var spot in farmingSpots)
            {
                // 添加英文和中文场景名称对应的ACT值
                mapping[spot.enUS] = spot.ACT;
                mapping[spot.zhCN] = spot.ACT;
            }

            return mapping;
        }

        /// <summary>
        /// 根据场景名称获取对应的英文名称
        /// </summary>
        /// <param name="sceneName">场景名称（中文或英文）</param>
        /// <returns>场景的英文名称</returns>
        public static string GetEnglishSceneName(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
                return sceneName;

            // 移除可能的引号（单引号或双引号）
            string cleanSceneName = sceneName.Trim('"', '\'');

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

        /// <summary>
        /// 根据场景名称获取ACT值
        /// </summary>
        public static int GetSceneActValue(string sceneName)
        {
            try
            {
                if (string.IsNullOrEmpty(sceneName))
                    return 0;

                // 移除ACT前缀（如果有），提取纯场景名称
                string pureSceneName = sceneName;
                if (sceneName.StartsWith("ACT ") || sceneName.StartsWith("Act ") || sceneName.StartsWith("act "))
                {
                    int colonIndex = sceneName.IndexOf(':');
                    if (colonIndex > 0)
                    {
                        pureSceneName = sceneName[(colonIndex + 1)..].Trim();
                    }
                }
                
                // 使用SceneManager获取场景到ACT值的映射
                var sceneActMappings = SceneManager.GetSceneActMappings();
                
                // 尝试在映射中查找纯场景名称对应的ACT值
                if (sceneActMappings.TryGetValue(pureSceneName, out int actValue))
                {
                    return actValue;
                }
                
                // 尝试查找包含场景名称的键
                foreach (var mapping in sceneActMappings)
                {
                    if (pureSceneName.Contains(mapping.Key, StringComparison.OrdinalIgnoreCase) || 
                        mapping.Key.Contains(pureSceneName, StringComparison.OrdinalIgnoreCase))
                    {
                        return mapping.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("SceneManager", $"提取ACT值失败: {ex.Message}");
            }
            return 0; // 默认返回0
        }
        
        /// <summary>
        /// 获取场景显示名称（包含ACT前缀）
        /// </summary>
        /// <param name="scene">场景对象</param>
        /// <returns>格式化的场景显示名称</returns>
        public static string GetSceneDisplayName(FarmingScene scene)
        {
            string actText = $"ACT {scene.ACT}";
            // 根据当前语言获取场景名称
            string language = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.StartsWith("zh") ? "Chinese" : "English";
            string name = scene.GetSceneName(language);
            return $"{actText}: {name}";
        }
        
        /// <summary>
        /// 获取本地化的难度名称
        /// </summary>
        /// <param name="difficulty">游戏难度枚举值</param>
        /// <returns>本地化的难度名称</returns>
        public static string GetLocalizedDifficultyName(GameDifficulty difficulty)
        {
            return difficulty switch
            {
                GameDifficulty.Normal => LanguageManager.GetString("DifficultyNormal"),
                GameDifficulty.Nightmare => LanguageManager.GetString("DifficultyNightmare"),
                GameDifficulty.Hell => LanguageManager.GetString("DifficultyHell"),
                _ => LanguageManager.GetString("DifficultyUnknown"),
            };
        }
        
        /// <summary>
        /// 根据索引获取游戏难度
        /// </summary>
        /// <param name="difficultyIndex">难度索引（0=普通，1=噩梦，2=地狱）</param>
        /// <returns>游戏难度枚举值</returns>
        public static GameDifficulty GetDifficultyByIndex(int difficultyIndex)
        {
            return difficultyIndex switch
            {
                0 => GameDifficulty.Normal,
                1 => GameDifficulty.Nightmare,
                2 => GameDifficulty.Hell,
                _ => GameDifficulty.Hell, // 默认返回地狱难度
            };
        }
    }
    
    /// <summary>
    /// YAML数据结构模型
    /// </summary>
    public class FarmingSpotsData
    {
        // 添加YamlMember特性以正确匹配YAML文件中的键名
        [YamlDotNet.Serialization.YamlMember(Alias = "farmingSpots", ApplyNamingConventions = false)]
        public List<FarmingScene> FarmingSpots { get; set; } = new List<FarmingScene>();
    }
}