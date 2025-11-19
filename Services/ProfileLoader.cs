using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.Models;

namespace DTwoMFTimerHelper.Services
{
    public static class ProfileLoader
    {
        // 动态获取当前用户的AppData\Roaming路径
        private static readonly string ProfilesDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "mf-time-helper",
            "profiles",
            "");

        // 内存缓存，用于存储已加载的角色档案，避免重复加载
        private static readonly Dictionary<string, CharacterProfile> _profileCache = new Dictionary<string, CharacterProfile>();

        /// <summary>
        /// 清除特定档案的缓存
        /// </summary>
        public static void ClearProfileCache(string profileName)
        {
            string safeFileName = GetSafeFileName(profileName);
            string filePath = Path.Combine(ProfilesDirectory, $"{safeFileName}.yaml");
            if (_profileCache.ContainsKey(filePath))
            {
                _profileCache.Remove(filePath);
                LogManager.WriteDebugLog("ProfileLoader", $"清除缓存: {profileName}");
            }
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public static void ClearAllCache()
        {
            _profileCache.Clear();
            LogManager.WriteDebugLog("ProfileLoader", "清除所有缓存");
        }

        /// <summary>
        /// 加载指定名称的单个角色档案
        /// </summary>
        public static CharacterProfile? LoadProfileByName(string profileName)
        {
            LogManager.WriteDebugLog("ProfileLoader", $"开始加载单个角色档案: {profileName}");
            LogManager.WriteDebugLog("ProfileLoader", $"角色档案目录路径: {ProfilesDirectory}");

            try
            {
                if (!Directory.Exists(ProfilesDirectory))
                {
                    LogManager.WriteDebugLog("ProfileLoader", "目录不存在");
                    return null;
                }

                // 安全地清理文件名
                string safeFileName = GetSafeFileName(profileName);
                string filePath = Path.Combine(ProfilesDirectory, $"{safeFileName}.yaml");
                LogManager.WriteDebugLog("ProfileLoader", $"目标文件路径: {filePath}");

                // 检查文件是否存在
                if (!File.Exists(filePath))
                {
                    LogManager.WriteDebugLog("ProfileLoader", $"文件不存在: {filePath}");
                    return null;
                }

                return LoadProfileFromFile(filePath);
            }
            catch (Exception ex)
            {
                LogManager.WriteErrorLog("ProfileLoader", $"加载单个角色档案失败: {profileName}", ex);
                return null;
            }
        }

        /// <summary>
        /// 从文件路径加载角色档案
        /// </summary>
        private static CharacterProfile? LoadProfileFromFile(string filePath)
        {
            try
            {
                // 先检查缓存
                if (_profileCache.TryGetValue(filePath, out var cachedProfile))
                {
                    LogManager.WriteDebugLog("ProfileLoader", $"从缓存获取文件: {Path.GetFileName(filePath)}");
                    return cachedProfile;
                }

                LogManager.WriteDebugLog("ProfileLoader", $"正在加载文件: {Path.GetFileName(filePath)}");

                // 使用using确保文件流正确关闭
                using var streamReader = new StreamReader(filePath, Encoding.UTF8);
                var yaml = streamReader.ReadToEnd();

                LogManager.WriteDebugLog("ProfileLoader", $"读取文件成功，内容长度: {yaml.Length} 字符");

                if (string.IsNullOrEmpty(yaml))
                {
                    LogManager.WriteDebugLog("ProfileLoader", $"文件内容为空: {filePath}");
                    return null;
                }

                // 使用手动解析方法处理角色档案
                CharacterProfile? profile;
                try
                {
                    LogManager.WriteDebugLog("ProfileLoader", $"使用YamlParser处理文件: {Path.GetFileName(filePath)}");
                    profile = YamlParser.ParseYamlManually(yaml, filePath);

                    // 确保profile不为null
                    if (profile == null)
                    {
                        LogManager.WriteDebugLog("ProfileLoader", $"解析文件 {Path.GetFileName(filePath)} 返回null，创建默认角色");
                        profile = new CharacterProfile() { Name = Path.GetFileNameWithoutExtension(filePath) };
                    }
                }
                catch (Exception ex)
                {
                    LogManager.WriteDebugLog("ProfileLoader", $"处理文件 {Path.GetFileName(filePath)} 时出错: {ex.Message}");
                    profile = new CharacterProfile() { Name = Path.GetFileNameWithoutExtension(filePath) };
                }

                // 确保Records集合已初始化
                if (profile != null && profile.Records == null)
                {
                    profile.Records = [];
                    LogManager.WriteDebugLog("ProfileLoader", $"初始化Records集合: {profile.Name}");
                }

                // 将加载的档案存入缓存
                if (profile != null)
                {
                    _profileCache[filePath] = profile;
                    LogManager.WriteDebugLog("ProfileLoader", $"缓存文件: {Path.GetFileName(filePath)}");
                }

                return profile;
            }
            catch (Exception ex)
            {
                LogManager.WriteErrorLog("ProfileLoader", $"加载角色档案文件失败 ({filePath})", ex);
                return null;
            }
        }

        /// <summary>
        /// 获取所有角色档案的文件名列表（不加载文件内容）
        /// </summary>
        public static List<string> GetProfileNames()
        {
            LogManager.WriteDebugLog("ProfileLoader", $"获取所有角色档案文件名");
            LogManager.WriteDebugLog("ProfileLoader", $"角色档案目录路径: {ProfilesDirectory}");

            var profileNames = new List<string>();

            try
            {
                if (!Directory.Exists(ProfilesDirectory))
                {
                    LogManager.WriteDebugLog("ProfileLoader", "目录不存在");
                    return profileNames;
                }

                var files = Directory.GetFiles(ProfilesDirectory, "*.yaml");
                LogManager.WriteDebugLog("ProfileLoader", $"找到 {files.Length} 个角色档案文件");

                foreach (var file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    // 移除IsHidden检查，直接添加所有文件名
                    profileNames.Add(fileName);
                    LogManager.WriteDebugLog("ProfileLoader", $"添加角色档案名称: {fileName}");
                }

                LogManager.WriteDebugLog("ProfileLoader", $"返回 {profileNames.Count} 个角色档案名称");
            }
            catch (Exception ex)
            {
                LogManager.WriteErrorLog("ProfileLoader", $"获取角色档案文件名失败", ex);
            }

            return profileNames;
        }

        /// <summary>
        /// 安全地清理文件名
        /// </summary>
        private static string GetSafeFileName(string name)
        {
            string safeFileName = Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '_'));
            safeFileName = safeFileName.Replace(" ", "_").ToLower();
            return safeFileName;
        }

        // 加载所有角色档案（保留原有方法以兼容）
        public static List<CharacterProfile> LoadAllProfiles()
        {
            LogManager.WriteDebugLog("ProfileLoader", $"开始加载所有角色档案");
            LogManager.WriteDebugLog("ProfileLoader", $"角色档案目录路径: {ProfilesDirectory}");

            var profiles = new List<CharacterProfile>();

            try
            {
                if (!Directory.Exists(ProfilesDirectory))
                {
                    LogManager.WriteDebugLog("ProfileLoader", "目录不存在，创建目录");
                    Directory.CreateDirectory(ProfilesDirectory);
                    return profiles;
                }

                var files = Directory.GetFiles(ProfilesDirectory, "*.yaml");
                LogManager.WriteDebugLog("ProfileLoader", $"找到 {files.Length} 个角色档案文件");

                foreach (var file in files)
                {
                    try
                    {
                        // 加载每个角色档案文件，不再检查IsHidden属性
                        var profile = LoadProfileFromFile(file);
                        if (profile != null)
                        {
                            profiles.Add(profile);
                            LogManager.WriteDebugLog("ProfileLoader", $"成功加载角色档案: {profile.Name}");
                        }
                        else
                        {
                            LogManager.WriteDebugLog("ProfileLoader", $"加载文件失败: {Path.GetFileName(file)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.WriteErrorLog("ProfileLoader", $"加载单个角色档案失败 ({file})", ex);
                    }
                }

                LogManager.WriteDebugLog("ProfileLoader", $"成功加载 {profiles.Count} 个角色档案");
            }
            catch (Exception ex)
            {
                LogManager.WriteErrorLog("ProfileLoader", $"加载角色档案失败", ex);
            }

            return profiles;
        }
    }
}