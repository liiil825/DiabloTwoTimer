using System;
using System.Collections.Generic;
using System.IO;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Utils;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace DTwoMFTimerHelper.Services {
    /// <summary>
    /// YAML解析器，使用YamlDotNet库解析角色档案的YAML内容
    /// </summary>
    public static class YamlParser {
        /// <summary>
        /// 使用YamlDotNet解析YAML内容，支持灵活的属性名格式
        /// </summary>
        /// <param name="yamlContent">YAML内容字符串</param>
        /// <param name="filePath">文件路径，用于日志记录</param>
        /// <returns>解析后的角色档案对象，如果解析失败则返回默认对象</returns>
        public static CharacterProfile? ParseYamlManually(string yamlContent, string filePath) {
            try {
                LogManager.WriteDebugLog("YamlParser", $"开始使用YamlDotNet解析文件: {Path.GetFileName(filePath)}");

                // 创建序列化配置，支持不区分大小写的属性匹配
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(YamlDotNet.Serialization.NamingConventions.CamelCaseNamingConvention.Instance)
                    .IgnoreUnmatchedProperties()
                    .Build();

                // 首先尝试直接反序列化
                CharacterProfile? profile;
                try {
                    profile = deserializer.Deserialize<CharacterProfile>(yamlContent);
                    LogManager.WriteDebugLog("YamlParser", "直接反序列化成功");
                }
                catch (Exception) {
                    // 如果直接反序列化失败，使用更灵活的节点解析方式
                    LogManager.WriteDebugLog("YamlParser", "直接反序列化失败，使用节点解析方式");
                    profile = ParseUsingNodeModel(yamlContent);
                }

                // 确保Records集合不为null
                if (profile == null) {
                    profile = new CharacterProfile { Records = new List<MFRecord>() };
                }

                if (profile.Records == null) {
                    profile.Records = new List<MFRecord>();
                }

                // 设置默认名称
                if (string.IsNullOrEmpty(profile.Name)) {
                    profile.Name = "未命名角色";
                    LogManager.WriteDebugLog("YamlParser", $"未找到名称，设置为默认值: {profile.Name}");
                }

                LogManager.WriteDebugLog("YamlParser", $"解析完成，成功加载 {profile.Records.Count} 条记录");
                return profile;
            }
            catch (Exception ex) {
                LogManager.WriteErrorLog("YamlParser", $"解析失败", ex);
                return new CharacterProfile() { Name = "解析失败角色" };
            }
        }

        /// <summary>
        /// 使用YamlDotNet的节点模型进行更灵活的解析，处理不同格式的YAML
        /// </summary>
        private static CharacterProfile ParseUsingNodeModel(string yamlContent) {
            var profile = new CharacterProfile { Records = new List<MFRecord>() };

            var yamlStream = new YamlStream();
            yamlStream.Load(new StringReader(yamlContent));

            if (yamlStream.Documents.Count == 0)
                return profile;

            var rootNode = yamlStream.Documents[0].RootNode as YamlMappingNode;
            if (rootNode == null)
                return profile;

            // 解析基本属性，支持多种属性名格式
            foreach (var node in rootNode) {
                var keyNode = node.Key as YamlScalarNode;
                var key = keyNode?.Value?.ToLower() ?? "";
                var valueNode = node.Value as YamlScalarNode;

                if (valueNode == null || string.IsNullOrEmpty(key))
                    continue;

                LogManager.WriteDebugLog("YamlParser", $"解析属性: {key}, 值: {valueNode.Value}");

                // 调用通用属性解析函数处理基本属性
                ParseProfileProperty(profile, key, valueNode.Value);
            }

            // 解析records数组
            if (rootNode.Children.TryGetValue(new YamlScalarNode("records"), out var recordsNode)) {
                var recordsSequence = recordsNode as YamlSequenceNode;
                if (recordsSequence != null) {
                    LogManager.WriteDebugLog("YamlParser", $"开始解析records数组，共{recordsSequence.Children.Count}条记录");

                    foreach (var recordNode in recordsSequence.Children) {
                        var recordMapping = recordNode as YamlMappingNode;
                        if (recordMapping == null)
                            continue;

                        var record = new MFRecord();

                        foreach (var propNode in recordMapping) {
                            var keyNode = propNode.Key as YamlScalarNode;
                            var propKey = keyNode?.Value?.ToLower() ?? "";
                            var propValueNode = propNode.Value as YamlScalarNode;

                            if (propValueNode == null || string.IsNullOrEmpty(propKey))
                                continue;

                            LogManager.WriteDebugLog("YamlParser", $"解析记录属性: {propKey}, 值: {propValueNode.Value}");

                            // 调用通用属性解析函数处理记录属性
                            ParseRecordProperty(record, propKey, propValueNode.Value);
                        }

                        profile.Records.Add(record);
                        LogManager.WriteDebugLog("YamlParser", $"添加记录，当前记录数: {profile.Records.Count}");
                    }
                }
            }

            return profile;
        }

        /// <summary>
        /// 解析CharacterProfile的基本属性
        /// </summary>
        /// <param name="profile">角色档案对象</param>
        /// <param name="key">属性名（已转换为小写）</param>
        /// <param name="value">属性值</param>
        private static void ParseProfileProperty(CharacterProfile profile, string key, string value) {
            // 添加null检查，防止空引用
            if (profile == null || key == null || value == null)
                return;
                
            switch (key) {
                case "name":
                case "character":
                    profile.Name = value.Trim('"', '\'');
                    LogManager.WriteDebugLog("YamlParser", $"解析到Name: {profile.Name}");
                    break;
                case "class":
                case "characterclass":
                    if (Enum.TryParse<CharacterClass>(value, true, out var charClass)) {
                        profile.Class = charClass;
                        LogManager.WriteDebugLog("YamlParser", $"解析到Class: {profile.Class}");
                    }
                    break;
                case "lastrunscene":
                    profile.LastRunScene = value.Trim('"', '\'');
                    LogManager.WriteDebugLog("YamlParser", $"解析到LastRunScene: {profile.LastRunScene}");
                    break;
                case "lastrundifficulty":
                    if (Enum.TryParse<GameDifficulty>(value, true, out var runDifficulty)) {
                        profile.LastRunDifficulty = runDifficulty;
                        LogManager.WriteDebugLog("YamlParser", $"解析到LastRunDifficulty: {profile.LastRunDifficulty}");
                    }
                    break;
            }
        }

        /// <summary>
        /// 解析MFRecord的记录属性
        /// </summary>
        /// <param name="record">MF记录对象</param>
        /// <param name="key">属性名（已转换为小写）</param>
        /// <param name="value">属性值</param>
        private static void ParseRecordProperty(MFRecord record, string key, string value) {
            // 添加null检查，防止空引用
            if (record == null || key == null || value == null)
                return;
                
            switch (key) {
                case "scenename":
                    record.SceneName = value.Trim('"', '\'');
                    break;
                case "act":
                    if (int.TryParse(value, out var act))
                        record.ACT = act;
                    break;
                case "difficulty":
                    if (Enum.TryParse<GameDifficulty>(value, true, out var difficulty))
                        record.Difficulty = difficulty;
                    break;
                case "starttime":
                    if (DateTime.TryParse(value, out var startTime))
                        record.StartTime = startTime;
                    break;
                case "endtime":
                    if (!string.IsNullOrEmpty(value) && DateTime.TryParse(value, out var endTime))
                        record.EndTime = endTime;
                    break;
                case "latesttime":
                    if (!string.IsNullOrEmpty(value) && DateTime.TryParse(value, out var latestTime))
                        record.LatestTime = latestTime;
                    break;
                case "durationseconds":
                    if (!string.IsNullOrEmpty(value) && double.TryParse(value, out var duration))
                        record.DurationSeconds = duration;
                    break;
            }
        }
    }
}