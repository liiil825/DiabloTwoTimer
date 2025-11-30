using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace DiabloTwoMFTimer.Models;

// 角色档案类
public class CharacterProfile
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    [YamlMember(Alias = "class")]
    public CharacterClass Class { get; set; }

    [YamlMember(Alias = "records")]
    public List<MFRecord> Records { get; set; } = [];

    [YamlMember(Alias = "lootRecords")]
    public List<LootRecord> LootRecords { get; set; } = []; // 存储掉落的历史记录

    [YamlMember(Alias = "lastRunScene")]
    public string LastRunScene { get; set; } = string.Empty;

    [YamlMember(Alias = "lastRunDifficulty")]
    public GameDifficulty LastRunDifficulty { get; set; } = GameDifficulty.Hell;

    [YamlIgnore]
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

    [YamlIgnore]
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

    [YamlIgnore]
    public int CompletedGamesCount => Records.Count(r => r.IsCompleted);

    [YamlIgnore]
    public int TotalGamesCount => Records.Count;
}
