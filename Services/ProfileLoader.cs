using System;
using System.Collections.Generic;
using System.IO;
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

        // 加载所有角色档案
        public static List<CharacterProfile> LoadAllProfiles(bool includeHidden = false)
        {
            // 使用LogManager进行日志记录
            
            LogManager.WriteDebugLog("ProfileLoader", $"开始加载所有角色档案，includeHidden={includeHidden}");
            LogManager.WriteDebugLog("ProfileLoader", $"角色档案目录路径: {ProfilesDirectory}");
            LogManager.WriteDebugLog("ProfileLoader", $"目录是否存在: {Directory.Exists(ProfilesDirectory)}");
            
            var profiles = new List<CharacterProfile>();
            
            try
            {
                if (!Directory.Exists(ProfilesDirectory))
                {
                    LogManager.WriteDebugLog("ProfileLoader", "目录不存在");
                    return profiles;
                }
                
                var files = Directory.GetFiles(ProfilesDirectory, "*.yaml");
                LogManager.WriteDebugLog("ProfileLoader", $"找到 {files.Length} 个角色档案文件");
                foreach (var file in files)
                {
                    LogManager.WriteDebugLog("ProfileLoader", $"找到文件: {Path.GetFileName(file)}");
                }
                
                foreach (var file in files)
                {
                    try
                    {
                        LogManager.WriteDebugLog("ProfileLoader", $"正在处理文件: {Path.GetFileName(file)}");
                        
                        // 检查文件是否存在且可读
                        if (!File.Exists(file))
                        {
                            LogManager.WriteDebugLog("ProfileLoader", $"文件不存在: {file}");
                            continue;
                        }

                        // 使用using确保文件流正确关闭
                        using var streamReader = new StreamReader(file, Encoding.UTF8);
                        var yaml = streamReader.ReadToEnd();

                        LogManager.WriteDebugLog("ProfileLoader", $"读取文件成功，内容长度: {yaml.Length} 字符");
                        // 记录文件的前50个字符用于调试
                        LogManager.WriteDebugLog("ProfileLoader", $"文件前50字符: {yaml.Substring(0, Math.Min(50, yaml.Length))}");

                        if (string.IsNullOrEmpty(yaml))
                        {
                            LogManager.WriteDebugLog("ProfileLoader", $"文件内容为空: {file}");
                            continue;
                        }

                        // 使用手动解析方法处理角色档案，确保正确解析YAML属性
                        CharacterProfile? profile;
                        try
                        {
                            LogManager.WriteDebugLog("ProfileLoader", $"使用YamlParser处理文件: {Path.GetFileName(file)}");
                            profile = YamlParser.ParseYamlManually(yaml, file);

                            // 确保profile不为null
                            if (profile == null)
                            {
                                LogManager.WriteDebugLog("ProfileLoader", $"解析文件 {Path.GetFileName(file)} 返回null，创建默认角色");
                                profile = new CharacterProfile() { Name = Path.GetFileNameWithoutExtension(file) };
                            }
                        }
                        catch (Exception ex)
                        {
                            LogManager.WriteDebugLog("ProfileLoader", $"处理文件 {Path.GetFileName(file)} 时出错: {ex.Message}");
                            profile = new CharacterProfile() { Name = Path.GetFileNameWithoutExtension(file) };
                        }

                        // 验证反序列化结果
                        if (profile == null)
                        {
                            LogManager.WriteDebugLog("ProfileLoader", $"反序列化失败，profile为null: {file}");
                            continue;
                        }

                        LogManager.WriteDebugLog("ProfileLoader", $"反序列化成功，角色名称: {profile.Name}, IsHidden: {profile.IsHidden}");

                        // 确保Records集合已初始化
                        if (profile.Records == null)
                        {
                            profile.Records = [];
                            LogManager.WriteDebugLog("ProfileLoader", $"初始化Records集合: {profile.Name}");
                        }

                        // 根据条件添加到列表
                        LogManager.WriteDebugLog("ProfileLoader", $"过滤检查: includeHidden={includeHidden}, IsHidden={profile.IsHidden}");
                        if (includeHidden || !profile.IsHidden)
                        {
                            profiles.Add(profile);
                            LogManager.WriteDebugLog("ProfileLoader", $"成功加载角色: {profile.Name}, 游戏记录数: {profile.Records.Count}");
                        }
                        else
                        {
                            LogManager.WriteDebugLog("ProfileLoader", $"角色已隐藏，跳过: {profile.Name}");
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