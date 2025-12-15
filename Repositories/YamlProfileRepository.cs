using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils; // 这里引用 Utils 是为了使用 LogManager 和 YamlParser
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DiabloTwoMFTimer.Repositories;

public class YamlProfileRepository : IProfileRepository
{
    // 基础路径
    private readonly string _profilesDirectory;

    // 内存缓存 (原 ProfileLoader 中的逻辑)
    private readonly Dictionary<string, CharacterProfile> _profileCache = [];

    // 序列化器
    private readonly ISerializer _serializer;

    public YamlProfileRepository()
    {
        // 1. 初始化路径
        _profilesDirectory = FolderManager.ProfilesPath;

        // 2. 确保目录存在
        if (!Directory.Exists(_profilesDirectory))
        {
            Directory.CreateDirectory(_profilesDirectory);
        }

        // 3. 初始化序列化器
        _serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
    }

    /// <summary>
    /// 根据名称获取角色档案
    /// </summary>
    public CharacterProfile? GetByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        string safeFileName = GetSafeFileName(name);
        string filePath = Path.Combine(_profilesDirectory, $"{safeFileName}.yaml");

        return LoadProfileFromFile(filePath);
    }

    /// <summary>
    /// 获取所有角色名称
    /// </summary>
    public List<string> GetAllNames()
    {
        var names = new List<string>();
        try
        {
            if (Directory.Exists(_profilesDirectory))
            {
                var files = Directory.GetFiles(_profilesDirectory, "*.yaml");
                foreach (var file in files)
                {
                    names.Add(Path.GetFileNameWithoutExtension(file));
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("YamlProfileRepository", "获取角色列表失败", ex);
        }
        return names;
    }

    /// <summary>
    /// 获取所有角色档案
    /// </summary>
    public List<CharacterProfile> GetAll()
    {
        var profiles = new List<CharacterProfile>();
        try
        {
            if (Directory.Exists(_profilesDirectory))
            {
                var files = Directory.GetFiles(_profilesDirectory, "*.yaml");
                foreach (var file in files)
                {
                    var profile = LoadProfileFromFile(file);
                    if (profile != null)
                    {
                        profiles.Add(profile);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("YamlProfileRepository", "加载所有角色失败", ex);
        }
        return profiles;
    }

    /// <summary>
    /// 保存角色档案
    /// </summary>
    public void Save(CharacterProfile profile)
    {
        if (profile == null)
            throw new ArgumentNullException(nameof(profile));

        try
        {
            string filePath = GetProfileFilePath(profile.Name);
            string yaml = _serializer.Serialize(profile);

            // 写入文件
            using (var streamWriter = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                streamWriter.Write(yaml);
                streamWriter.Flush();
            }

            // 更新缓存 (清除旧缓存，或者直接更新)
            // 这里选择清除，下次读取时重新加载，或者你可以直接 _profileCache[filePath] = profile;
            if (_profileCache.ContainsKey(filePath))
            {
                _profileCache[filePath] = profile;
            }
            else
            {
                _profileCache.Add(filePath, profile);
            }

            LogManager.WriteDebugLog("YamlProfileRepository", $"保存成功: {profile.Name}");
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("YamlProfileRepository", $"保存失败: {profile.Name}", ex);
            throw; // 抛出异常让 Service 层处理或显示错误
        }
    }

    /// <summary>
    /// 删除角色档案
    /// </summary>
    public void Delete(CharacterProfile profile)
    {
        if (profile == null)
            return;

        try
        {
            string filePath = GetProfileFilePath(profile.Name);

            // 1. 清除缓存
            if (_profileCache.ContainsKey(filePath))
            {
                _profileCache.Remove(filePath);
            }

            // 2. 删除文件
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                LogManager.WriteDebugLog("YamlProfileRepository", $"删除成功: {profile.Name}");
            }
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("YamlProfileRepository", $"删除失败: {profile.Name}", ex);
            throw;
        }
    }

    // --- 私有辅助方法 ---

    private CharacterProfile? LoadProfileFromFile(string filePath)
    {
        try
        {
            if (_profileCache.TryGetValue(filePath, out var cachedProfile))
                return cachedProfile;

            if (!File.Exists(filePath))
                return null;

            using var reader = new StreamReader(filePath, Encoding.UTF8);

            // [修改] 直接使用 _serializer 对应的 _deserializer
            // 你可能需要在构造函数里初始化一个 _deserializer
            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties() // 允许 YAML 有多余字段
                .Build();

            var profile = deserializer.Deserialize<CharacterProfile>(reader);

            // 防御性初始化
            if (profile != null)
            {
                profile.Records ??= [];
                LogManager.WriteDebugLog(
                    "YamlProfileRepository",
                    $"加载成功, {profile.Name} 有 {profile.Records.Count} 条记录"
                );
                profile.LootRecords ??= [];
                // 如果 Name 为空，尝试用文件名
                if (string.IsNullOrEmpty(profile.Name))
                    profile.Name = Path.GetFileNameWithoutExtension(filePath);

                _profileCache[filePath] = profile;
            }

            return profile;
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("YamlProfileRepository", $"加载文件失败: {filePath}", ex);
            return null;
        }
    }

    private string GetProfileFilePath(string name)
    {
        string safeName = GetSafeFileName(name);
        return Path.Combine(_profilesDirectory, $"{safeName}.yaml");
    }

    private static string GetSafeFileName(string name)
    {
        string safeFileName = Path.GetInvalidFileNameChars().Aggregate(name, (current, c) => current.Replace(c, '_'));
        return safeFileName.Replace(" ", "_").ToLower();
    }
}
