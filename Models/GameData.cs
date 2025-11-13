using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using YamlDotNet.Serialization;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.Models
{
    // 游戏难度枚举
    public enum GameDifficulty
    {
        Normal,
        Nightmare,
        Hell
    }

    // 角色职业枚举
    public enum CharacterClass
    {
        Barbarian,
        Sorceress,
        Assassin,
        Druid,
        Paladin,
        Amazon,
        Necromancer
    }

    // MF记录类
    public class MFRecord
    {
        public string SceneName { get; set; } = string.Empty;
        public int ACT { get; set; } = 0;
        public GameDifficulty Difficulty { get; set; } = GameDifficulty.Normal;
        [YamlDotNet.Serialization.YamlMember(Alias = "startTime")]
        public DateTime StartTime { get; set; }
        [YamlDotNet.Serialization.YamlMember(Alias = "endTime")]
        public DateTime? EndTime { get; set; }
        [YamlDotNet.Serialization.YamlMember(Alias = "latestTime")]
        public DateTime? LatestTime { get; set; }
        [YamlDotNet.Serialization.YamlMember(Alias = "elapsedTime")]
        public double? ElapsedTime { get; set; } = 0;
        /// <summary>
        /// 游戏持续时间（秒）
        /// </summary>
        [YamlMember(Alias = "durationSeconds")]
        public double DurationSeconds
        {
            get
            {
                try
                {
                    // 使用LogManager添加详细的时间计算日志
                    string recordId = this.GetHashCode().ToString();
                    LogManager.WriteDebugLog("GameData", "==============================================");
                    LogManager.WriteDebugLog("GameData", $"[DurationSeconds] 计算开始，记录ID: {recordId}");
                    LogManager.WriteDebugLog("GameData", $"[DurationSeconds] StartTime: {StartTime}");
                    LogManager.WriteDebugLog("GameData", $"[DurationSeconds] EndTime: {(EndTime.HasValue ? EndTime.Value.ToString() : "null")}");
                    LogManager.WriteDebugLog("GameData", $"[DurationSeconds] LatestTime: {(LatestTime.HasValue ? LatestTime.Value.ToString() : "null")}");
                    LogManager.WriteDebugLog("GameData", $"[DurationSeconds] ElapsedTime: {(ElapsedTime.HasValue ? ElapsedTime.Value.ToString() : "null")}");
                    LogManager.WriteDebugLog("GameData", $"[DurationSeconds] isCompleted: {IsCompleted}");
                    LogManager.WriteDebugLog("GameData", "[DurationSeconds] 条件检查:");
                    LogManager.WriteDebugLog("GameData", $"[DurationSeconds] - EndTime.HasValue: {EndTime.HasValue}");
                    LogManager.WriteDebugLog("GameData", $"[DurationSeconds] - ElapsedTime.HasValue: {ElapsedTime.HasValue}");
                    LogManager.WriteDebugLog("GameData", $"[DurationSeconds] - ElapsedTime.Value > 0: {(ElapsedTime.HasValue ? (ElapsedTime.Value > 0).ToString() : "N/A")}");
                    LogManager.WriteDebugLog("GameData", $"[DurationSeconds] - LatestTime.HasValue: {LatestTime.HasValue}");
                    
                    double result = 0;
                    string calculationSource = "未知";
                    
                    if (!EndTime.HasValue)
                    {
                        LogManager.WriteDebugLog("GameData", "[DurationSeconds] 计算路径: EndTime为空，返回0");
                        calculationSource = "EndTime为空";
                    }
                    else if (IsCompleted) // 已完成的记录优先使用更可靠的计算方式
                    {
                        // 对于已完成的记录，直接使用EndTime - StartTime作为主要计算方式
                        result = (EndTime.Value - StartTime).TotalSeconds;
                        LogManager.WriteDebugLog("GameData", "[DurationSeconds] 计算路径: 已完成记录，使用EndTime - StartTime");
                        calculationSource = "EndTime - StartTime (已完成记录)";
                        
                        // 如果ElapsedTime有有效值，记录对比信息用于调试
                        if (ElapsedTime.HasValue && ElapsedTime.Value > 0)
                        {
                            LogManager.WriteDebugLog("GameData", $"[DurationSeconds] - ElapsedTime: {ElapsedTime.Value} (已完成记录优先使用直接计算)");
                        }
                    }
                    else if (ElapsedTime.HasValue && ElapsedTime.Value > 0)
                    {   
                        // 未完成记录使用ElapsedTime相关计算
                        calculationSource = "ElapsedTime (未完成记录)";
                        if (LatestTime.HasValue)
                        {
                            double latestToNow = (DateTime.Now - LatestTime.Value).TotalSeconds;
                            result = ElapsedTime.Value + latestToNow;
                            LogManager.WriteDebugLog("GameData", "[DurationSeconds] 计算路径: 未完成记录，ElapsedTime + (Now - LatestTime)");
                            LogManager.WriteDebugLog("GameData", $"[DurationSeconds] - ElapsedTime: {ElapsedTime.Value}");
                            LogManager.WriteDebugLog("GameData", $"[DurationSeconds] - Now - LatestTime: {latestToNow}");
                            calculationSource += " + NowToLatest";
                        }
                        else
                        {
                            result = ElapsedTime.Value;
                            LogManager.WriteDebugLog("GameData", "[DurationSeconds] 计算路径: 未完成记录，仅使用ElapsedTime");
                        }
                    }
                    else
                    {   
                        // 兜底逻辑
                        result = (EndTime.Value - StartTime).TotalSeconds;
                        LogManager.WriteDebugLog("GameData", "[DurationSeconds] 计算路径: 兜底逻辑，使用EndTime - StartTime");
                        calculationSource = "EndTime - StartTime (兜底)";
                    }
                    
                    LogManager.WriteDebugLog("GameData", $"[DurationSeconds] 最终结果: {result} 秒 (来源: {calculationSource})");
                    LogManager.WriteDebugLog("GameData", "==============================================");
                    
                    return result;
                }
                catch (Exception ex)
                {
                   
                    LogManager.WriteErrorLog("GameData", "[DurationSeconds] 计算异常", ex);

                    // 原始计算逻辑作为备用
                    if (!EndTime.HasValue)
                        return 0;
                    
                    if (ElapsedTime.HasValue && ElapsedTime.Value > 0)
                    {
                        if (LatestTime.HasValue)
                            return ElapsedTime.Value + (EndTime.Value - LatestTime.Value).TotalSeconds;
                        else
                            return ElapsedTime.Value;
                    }
                    else
                        return (EndTime.Value - StartTime).TotalSeconds;
                }
            }
        }
        public bool IsCompleted => EndTime.HasValue;
    }

    // 角色档案类
    public class CharacterProfile
    {
        public string Name { get; set; } = string.Empty;
        public CharacterClass Class { get; set; }
        public bool IsHidden { get; set; } = false;
        public List<MFRecord> Records { get; set; } = new List<MFRecord>();
        
        // 计算属性
        public double TotalPlayTimeSeconds {
            get {
                try {
                    return Records?.Sum(r => r.DurationSeconds) ?? 0;
                } catch {
                    return 0;
                }
            }
        }
        public double AverageGameTimeSeconds {
            get {
                try {
                    var completedRecords = Records.Where(r => r.IsCompleted).ToList();
                    return completedRecords.Count > 0 ? completedRecords.Average(r => r.DurationSeconds) : 0;
                } catch {
                    return 0;
                }
            }
        }
        public int CompletedGamesCount => Records.Count(r => r.IsCompleted);
        public int TotalGamesCount => Records.Count;
    }

    // 场景类
    public class FarmingScene
    {
        public int ACT { get; set; } = 0;
        public string enUS { get; set; } = string.Empty;
        public string zhCN { get; set; } = string.Empty;
        
        // 根据当前语言获取场景名称
        public string GetSceneName(string language)
        {
            return language == "English" ? enUS : zhCN;
        }
    }

    // 场景数据容器
    public class FarmingSpotsData
    {
        // 只使用一个属性，并通过YamlMember特性指定别名，避免重复映射
        [YamlDotNet.Serialization.YamlMember(Alias = "farmingSpots", ApplyNamingConventions = false)]
        public List<FarmingScene> FarmingSpots { get; set; } = new List<FarmingScene>();
    }
}