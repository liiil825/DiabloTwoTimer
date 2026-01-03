using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DiabloTwoMFTimer.Repositories;

public class KeyMapRepository : IKeyMapRepository
{
    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;

    public KeyMapRepository()
    {
        // 配置 YamlDotNet 使用驼峰命名 (camelCase)，符合 YAML 惯例
        _serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    public List<KeyMapNode> LoadKeyMap()
    {
        try
        {
            // 1. 如果文件不存在，创建默认配置
            if (!File.Exists(FolderManager.KeyMapConfigPath))
            {
                var defaults = DefaultKeyMapGenerator.GenerateDefaultKeyMap();
                SaveKeyMap(defaults);
                return defaults;
            }

            // 2. 读取文件
            using var reader = new StreamReader(FolderManager.KeyMapConfigPath, Encoding.UTF8);
            var nodes = _deserializer.Deserialize<List<KeyMapNode>>(reader);

            return nodes ?? new List<KeyMapNode>();
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("KeyMapRepository", "加载按键映射失败，将使用空配置", ex);
            // 这里为了不让程序崩溃，返回空列表，或者也可以返回默认配置
            return DefaultKeyMapGenerator.GenerateDefaultKeyMap();
        }
    }

    public void SaveKeyMap(List<KeyMapNode> nodes)
    {
        try
        {
            FolderManager.EnsureDirectoryExists(FolderManager.AppDataPath);
            using var writer = new StreamWriter(FolderManager.KeyMapConfigPath, false, Encoding.UTF8);
            _serializer.Serialize(writer, nodes);
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("KeyMapRepository", "保存按键映射失败", ex);
        }
    }
}
