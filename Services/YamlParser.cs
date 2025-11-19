using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.Services
{
    /// <summary>
    /// YAML解析器，用于手动解析角色档案的YAML内容
    /// </summary>
    public static class YamlParser
    {
        /// <summary>
        /// 手动解析YAML内容，处理不同的属性名格式
        /// </summary>
        /// <param name="yamlContent">YAML内容字符串</param>
        /// <param name="filePath">文件路径，用于日志记录</param>
        /// <returns>解析后的角色档案对象，如果解析失败则返回默认对象</returns>
        public static CharacterProfile? ParseYamlManually(string yamlContent, string filePath)
        {
            var profile = new CharacterProfile { Records = new List<MFRecord>() };

            try
            {
                LogManager.WriteDebugLog("YamlParser", $"开始手动解析文件: {Path.GetFileName(filePath)}");

                bool isInRecordsSection = false;
                MFRecord? currentRecord = null;

                // 辅助方法：处理记录属性
                static void ProcessRecordProperty(MFRecord record, string key, string value, string logPrefix = "")
                {
                    LogManager.WriteDebugLog("YamlParser", $"{logPrefix}key: {key}, value: {value}");

                    switch (key)
                    {
                        case "scenename":
                            record.SceneName = value.Trim('"', '\'');
                            LogManager.WriteDebugLog("YamlParser", $"解析到SceneName: {record.SceneName}");
                            break;
                        case "act":
                            if (int.TryParse(value, out var act))
                            {
                                record.ACT = act;
                                LogManager.WriteDebugLog("YamlParser", $"解析到ACT: {record.ACT}");
                            }
                            break;
                        case "difficulty":
                            if (Enum.TryParse<GameDifficulty>(value, true, out var difficulty))
                            {
                                record.Difficulty = difficulty;
                                LogManager.WriteDebugLog("YamlParser", $"解析到Difficulty: {record.Difficulty}");
                            }
                            break;
                        case "starttime":
                            if (DateTime.TryParse(value, out var startTime))
                            {
                                record.StartTime = startTime;
                                LogManager.WriteDebugLog("YamlParser", $"解析到StartTime: {record.StartTime}");
                            }
                            break;
                        case "endtime":
                            if (!string.IsNullOrEmpty(value) && DateTime.TryParse(value, out var endTime))
                            {
                                record.EndTime = endTime;
                                LogManager.WriteDebugLog("YamlParser", $"解析到EndTime: {record.EndTime}");
                            }
                            break;
                        case "latesttime":
                            if (!string.IsNullOrEmpty(value) && DateTime.TryParse(value, out var latestTime))
                            {
                                record.LatestTime = latestTime;
                                LogManager.WriteDebugLog("YamlParser", $"解析到LatestTime: {record.LatestTime}");
                            }
                            break;
                        case "durationseconds":
                            if (!string.IsNullOrEmpty(value) && double.TryParse(value, out var elapsedTime))
                            {
                                record.DurationSeconds = elapsedTime;
                                LogManager.WriteDebugLog("YamlParser", $"解析到{key}，设置为DurationSeconds: {record.DurationSeconds}");
                            }
                            break;
                    }
                }

                foreach (var line in yamlContent.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
                {
                    // 检查是否进入records部分
                    if (line.Trim().Equals("records:", StringComparison.CurrentCultureIgnoreCase))
                    {
                        isInRecordsSection = true;
                        LogManager.WriteDebugLog("YamlParser", "进入records部分");
                        continue;
                    }

                    if (!isInRecordsSection)
                    {
                        // 处理基本属性
                        var parts = line.Split([':'], 2);
                        if (parts.Length < 2)
                            continue;

                        var key = parts[0].Trim().ToLower();
                        var value = parts[1].Trim();

                        if (key == "name" || key == "character")
                        {
                            profile.Name = value.Trim('"', '\'');
                            LogManager.WriteDebugLog("YamlParser", $"解析到Name: {profile.Name}");
                        }
                        else if (key == "class" || key == "characterclass")
                        {
                            if (Enum.TryParse<CharacterClass>(value, true, out var charClass))
                            {
                                profile.Class = charClass;
                                LogManager.WriteDebugLog("YamlParser", $"解析到Class: {profile.Class}");
                            }
                        }
                    }
                    else
                    {
                        // 处理记录数据
                        if (line.Trim().StartsWith('-'))
                        {
                            // 添加当前记录
                            if (currentRecord != null)
                            {
                                profile.Records.Add(currentRecord);
                                LogManager.WriteDebugLog("YamlParser", $"添加记录完成，当前记录数: {profile.Records.Count}");
                            }

                            // 开始新记录
                            currentRecord = new MFRecord();
                            LogManager.WriteDebugLog("YamlParser", "开始新记录");

                            // 检查同一行是否有属性
                            string trimmedLine = line.TrimStart('-').Trim();
                            if (!string.IsNullOrEmpty(trimmedLine) && trimmedLine.Contains(":"))
                            {
                                LogManager.WriteDebugLog("YamlParser", $"处理行中属性: {trimmedLine}");
                                var parts = trimmedLine.Split([':'], 2);
                                if (parts.Length >= 2)
                                {
                                    ProcessRecordProperty(currentRecord, parts[0].Trim().ToLower(), parts[1].Trim(), "解析行中属性 - ");
                                }
                            }
                        }
                        // 处理记录的常规属性
                        else if (currentRecord != null && line.Trim().Length > 0)
                        {
                            var parts = line.Split([':'], 2);
                            if (parts.Length >= 2)
                            {
                                ProcessRecordProperty(currentRecord, parts[0].Trim().ToLower(), parts[1].Trim());
                            }
                        }
                    }
                }

                // 添加最后一条记录
                if (currentRecord != null)
                {
                    profile.Records.Add(currentRecord);
                    LogManager.WriteDebugLog("YamlParser", $"添加最后一条记录，最终记录数: {profile.Records.Count}");
                }

                // 设置默认名称
                if (string.IsNullOrEmpty(profile.Name))
                {
                    profile.Name = "未命名角色";
                    LogManager.WriteDebugLog("YamlParser", $"未找到名称，设置为默认值: {profile.Name}");
                }

                LogManager.WriteDebugLog("YamlParser", $"手动解析完成，成功加载 {profile.Records.Count} 条记录");
                return profile;
            }
            catch (Exception ex)
            {
                LogManager.WriteErrorLog("YamlParser", $"手动解析失败", ex);
                return new CharacterProfile() { Name = "解析失败角色" };
            }
        }
    }
}