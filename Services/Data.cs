using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.Models;

namespace DTwoMFTimerHelper.Services
{
    public static class DataService
    {
        // 动态获取当前用户的AppData\Roaming路径
        private static readonly string ProfilesDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "mf-time-helper",
            "profiles",
            "");

        // 静态构造函数，用于验证目录路径
        static DataService()
        {
            LogManager.WriteDebugLog("DataService", $"[目录验证] 角色档案目录路径: {ProfilesDirectory}");
            LogManager.WriteDebugLog("DataService", $"[目录验证] 目录是否存在: {Directory.Exists(ProfilesDirectory)}");
            if (Directory.Exists(ProfilesDirectory))
            {
                var files = Directory.GetFiles(ProfilesDirectory, "*.yaml");
                LogManager.WriteDebugLog("DataService", $"[目录验证] 目录中找到 {files.Length} 个YAML文件");
                foreach (var file in files)
                {
                    LogManager.WriteDebugLog("DataService", $"[目录验证] - {Path.GetFileName(file)}");
                }
            }
        }

        private static readonly ISerializer serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        // 用于角色档案的反序列化器（使用CamelCase命名约定以匹配YAML中的小写属性名）
        private static readonly IDeserializer characterDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        public static List<CharacterProfile> LoadAllProfiles(bool includeHidden = false)
        {
            LogManager.WriteDebugLog("DataService", $"调用ProfileLoader加载角色档案，includeHidden={includeHidden}");
            return ProfileLoader.LoadAllProfiles(includeHidden);
        }

        private static string GetSafeFileName(string name)
        {
            // 清理文件名，确保安全
            string safeFileName = Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '_'));
            safeFileName = safeFileName.Replace(" ", "_").ToLower();
            return safeFileName;
        }

        // 获取角色档案文件的完整路径
        private static string GetProfileFilePath(string name)
        {
            string safeFileName = GetSafeFileName(name);
            return Path.Combine(ProfilesDirectory, $"{safeFileName}.yaml");
        }

        // 保存角色档案
        public static void SaveProfile(CharacterProfile profile)
        {
            try
            {
                // 验证profile对象不为null
                if (profile == null)
                {
                    LogManager.WriteDebugLog("DataService", "保存失败: profile对象为null");
                    throw new ArgumentNullException(nameof(profile), "保存失败: profile对象为null");
                }

                LogManager.WriteDebugLog("DataService", $"开始保存角色: {profile.Name}");

                // 确保目录存在
                try
                {
                    if (!Directory.Exists(ProfilesDirectory))
                    {
                        LogManager.WriteDebugLog("DataService", $"创建目录: {ProfilesDirectory}");
                        Directory.CreateDirectory(ProfilesDirectory);
                        // 验证目录是否创建成功
                        if (!Directory.Exists(ProfilesDirectory))
                        {
                            throw new IOException($"无法创建目录: {ProfilesDirectory}");
                        }
                        LogManager.WriteDebugLog("DataService", $"目录创建成功: {ProfilesDirectory}");
                    }
                }
                catch (Exception dirEx)
                {
                    LogManager.WriteErrorLog("DataService", $"创建目录失败: {dirEx.Message}", dirEx);
                    throw new IOException($"创建配置文件目录失败: {dirEx.Message}", dirEx);
                }

                // 使用统一的方法获取文件路径
                var filePath = GetProfileFilePath(profile.Name);
                LogManager.WriteDebugLog("DataService", $"准备保存到文件: {filePath}");

                // 序列化数据
                var yaml = serializer.Serialize(profile);
                if (string.IsNullOrEmpty(yaml))
                {
                    throw new InvalidOperationException("序列化失败，生成了空的YAML数据");
                }
                LogManager.WriteDebugLog($"序列化成功，数据长度: {yaml.Length} 字符");

                // 使用try-finally确保文件流正确关闭
                bool saveSuccess = false;
                int retryCount = 0;
                const int maxRetries = 3;

                while (!saveSuccess && retryCount < maxRetries)
                {
                    try
                    {
                        // 使用更安全的文件写入方式
                        using (var streamWriter = new StreamWriter(filePath, false, Encoding.UTF8))
                        {
                            streamWriter.Write(yaml);
                            streamWriter.Flush();
                            FileInfo fileInfo = new FileInfo(filePath);
                            LogManager.WriteDebugLog($"文件保存成功，大小: {fileInfo.Length} 字节");
                            saveSuccess = true;
                        }

                        // 验证文件是否真的被写入并包含内容
                        if (File.Exists(filePath))
                        {
                            FileInfo fileInfo = new FileInfo(filePath);
                            if (fileInfo.Length == 0)
                            {
                                throw new IOException("文件已创建但内容为空");
                            }
                            LogManager.WriteDebugLog("DataService", $"文件验证成功，实际大小: {fileInfo.Length} 字节");
                        }
                        else
                        {
                            throw new IOException("文件保存后不存在");
                        }
                    }
                    catch (IOException ex) when (retryCount < maxRetries - 1)
                    {
                        retryCount++;
                        LogManager.WriteErrorLog("DataService", $"文件保存失败，正在重试 ({retryCount}/{maxRetries}): {ex.Message}", ex);
                        System.Threading.Thread.Sleep(100); // 短暂延迟后重试
                    }
                }

                if (!saveSuccess)
                {
                    throw new IOException($"在{maxRetries}次尝试后仍无法保存文件");
                }

                LogManager.WriteDebugLog($"角色 {profile.Name} 保存完成");
            }
            catch (Exception ex)
            {
                LogManager.WriteErrorLog("DataService", $"保存角色档案失败", ex);

                // 在所有模式下都显示错误信息，确保用户知道保存失败
                string errorMsg = $"保存角色档案失败: {ex.Message}\n文件路径: {ProfilesDirectory}";
                MessageBox.Show(errorMsg, "保存错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // 重新抛出异常，让调用者知道发生了错误
                throw;
            }
        }

        // 删除角色档案
        public static void DeleteProfile(CharacterProfile profile)
        {
            try
            {
                // 使用统一的方法获取文件路径
                var filePath = GetProfileFilePath(profile.Name);
                if (File.Exists(filePath))
                {
                    LogManager.WriteDebugLog("DataService", $"删除角色档案: {filePath}");
                    File.Delete(filePath);
                    LogManager.WriteDebugLog("DataService", $"角色档案删除成功: {profile.Name}");
                }
                else
                {
                    LogManager.WriteDebugLog("DataService", $"文件不存在，无法删除: {filePath}");
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteErrorLog("DataService", $"删除角色档案失败", ex);

                // 在所有模式下都显示错误信息
                string errorMsg = $"删除角色档案失败: {ex.Message}";
                MessageBox.Show(errorMsg, "删除错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 隐藏/显示角色档案
        public static void ToggleProfileVisibility(CharacterProfile profile, bool isHidden)
        {
            profile.IsHidden = isHidden;
            SaveProfile(profile);
        }

        /// <summary>
        /// 加载场景数据
        /// </summary>
        public static List<FarmingScene> LoadFarmingSpots()
        {
            // 调用SceneService加载场景数据
            return SceneService.LoadFarmingSpots();
        }

        /// <summary>
        /// 获取场景的中英文映射字典
        /// </summary>
        public static Dictionary<string, string> GetSceneMappings()
        {
            // 调用SceneService获取场景映射
            return SceneService.GetSceneMappings();
        }

        /// <summary>
        /// 获取场景的ACT值映射字典
        /// </summary>
        public static Dictionary<string, int> GetSceneActMappings()
        {
            // 调用SceneService获取场景ACT映射
            return SceneService.GetSceneActMappings();
        }

        /// <summary>
        /// 根据场景名称获取对应的英文名称
        /// </summary>
        public static string GetEnglishSceneName(string sceneName)
        {
            // 调用SceneService获取英文场景名称
            return SceneService.GetEnglishSceneName(sceneName);
        }

        // 根据名称查找角色档案
        public static CharacterProfile? FindProfileByName(string name, bool includeHidden = false)
        {
            var profiles = LoadAllProfiles(includeHidden);
            return profiles.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        // 创建新的角色档案
        public static CharacterProfile CreateNewProfile(string name, DTwoMFTimerHelper.Models.CharacterClass characterClass)
        {
            try
            {
                LogManager.WriteDebugLog("DataService", $"开始创建新角色档案: {name}, 职业: {characterClass}");

                // 验证输入参数
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("角色名称不能为空", nameof(name));
                }

                // 检查角色是否已存在
                var existingProfile = FindProfileByName(name);
                if (existingProfile != null)
                {
                    LogManager.WriteDebugLog("DataService", $"角色 '{name}' 已存在");
                    throw new InvalidOperationException($"角色 '{name}' 已存在");
                }

                // 创建新角色
                var profile = new CharacterProfile
                {
                    Name = name,
                    Class = characterClass,
                    IsHidden = false,
                    Records = []
                };

                LogManager.WriteDebugLog("DataService", $"角色对象创建成功，现在准备保存");

                // 目录检查和创建将在SaveProfile方法中完成，避免重复逻辑

                // 保存配置文件
                SaveProfile(profile);

                LogManager.WriteDebugLog("DataService", $"成功创建并保存角色档案: {name}");

                // 直接返回创建的profile对象，避免因文件系统缓存导致的验证失败
                // 后续操作会通过正常加载流程确保数据一致性
                return profile;
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("DataService", $"创建角色档案失败: {ex.Message}");
                LogManager.WriteDebugLog("DataService", $"异常堆栈: {ex.StackTrace}");
                // 在所有模式下都显示详细错误信息，确保用户知道具体失败原因
                string errorMsg = ex.InnerException != null
                    ? $"创建角色失败: {ex.Message}\n内部错误: {ex.InnerException.Message}"
                    : $"创建角色失败: {ex.Message}";
                MessageBox.Show(errorMsg, "创建错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw; // 重新抛出异常，让调用者知道发生了错误
            }
        }

        // 添加MF记录
        public static void AddMFRecord(CharacterProfile profile, MFRecord record)
        {
            profile.Records.Add(record);
            SaveProfile(profile);
        }

        // 更新MF记录
        public static void UpdateMFRecord(CharacterProfile profile, MFRecord record)
        {
            var existingRecord = profile.Records.FirstOrDefault(r =>
                r.StartTime == record.StartTime &&
                r.SceneName == record.SceneName);

            if (existingRecord != null)
            {
                existingRecord.EndTime = record.EndTime;
                existingRecord.Difficulty = record.Difficulty;
                existingRecord.LatestTime = record.LatestTime;
                existingRecord.ElapsedTime = record.ElapsedTime;
                // 更新其他字段
            }

            SaveProfile(profile);
        }
    }
}