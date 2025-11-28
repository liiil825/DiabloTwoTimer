using System;
using System.Collections.Generic;
using System.Linq;

namespace DiabloTwoMFTimer.Models;

// 游戏难度枚举
public enum GameDifficulty
{
    Normal,
    Nightmare,
    Hell,
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
    Necromancer,
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

    [YamlDotNet.Serialization.YamlMember(Alias = "durationSeconds")]
    public double DurationSeconds { get; set; } = 0;

    public bool IsCompleted => EndTime.HasValue;
}

// 角色档案类
public class CharacterProfile
{
    public string Name { get; set; } = string.Empty;
    public CharacterClass Class { get; set; }
    public List<MFRecord> Records { get; set; } = [];
    public List<LootRecord> LootRecords { get; set; } = []; // 存储掉落的历史记录

    public string LastRunScene { get; set; } = string.Empty;
    public GameDifficulty LastRunDifficulty { get; set; } = GameDifficulty.Hell;

    // 计算属性
    public double TotalPlayTimeSeconds
    {
        get
        {
            try
            {
                return Records?.Sum(r => r.DurationSeconds) ?? 0;
            }
            catch
            {
                return 0;
            }
        }
    }
    public double AverageGameTimeSeconds
    {
        get
        {
            try
            {
                var completedRecords = Records.Where(r => r.IsCompleted).ToList();
                return completedRecords.Count > 0 ? completedRecords.Average(r => r.DurationSeconds) : 0;
            }
            catch
            {
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

    [YamlDotNet.Serialization.YamlMember(Alias = "enUS")]
    public string EnUS { get; set; } = string.Empty;

    [YamlDotNet.Serialization.YamlMember(Alias = "zhCN")]
    public string ZhCN { get; set; } = string.Empty;

    [YamlDotNet.Serialization.YamlMember(Alias = "shortName")]
    public string ShortName { get; set; } = string.Empty;

    [YamlDotNet.Serialization.YamlMember(Alias = "shortEnName")]
    public string ShortEnName { get; set; } = string.Empty;

    [YamlDotNet.Serialization.YamlMember(Alias = "shortZhCN")]
    public string ShortZhCN { get; set; } = string.Empty;

    // 根据当前语言获取场景名称
    public string GetSceneName(string language)
    {
        return language == "English" ? EnUS : ZhCN;
    }
}

// 场景数据容器
public class FarmingSpotsData
{
    // 只使用一个属性，并通过YamlMember特性指定别名，避免重复映射
    [YamlDotNet.Serialization.YamlMember(Alias = "farmingSpots", ApplyNamingConventions = false)]
    public List<FarmingScene> FarmingSpots { get; set; } = [];
}

// 掉落物品类
public class LootRecord
{
    public string Name { get; set; } = string.Empty; // 名称
    public string SceneName { get; set; } = string.Empty; // 掉落的场景
    public int RunCount { get; set; } = 0; // 在多少次同场景中掉落的
    public DateTime DropTime { get; set; } = DateTime.Now; // 掉落的时间
}
