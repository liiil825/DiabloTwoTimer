using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

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

// 场景类
public class FarmingScene
{
    [YamlMember(Alias = "act")]
    public int ACT { get; set; } = 0;

    [YamlMember(Alias = "enUS")]
    public string EnUS { get; set; } = string.Empty;

    [YamlMember(Alias = "zhCN")]
    public string ZhCN { get; set; } = string.Empty;

    [YamlMember(Alias = "shortEnName")]
    public string ShortEnName { get; set; } = string.Empty;

    [YamlMember(Alias = "shortZhCN")]
    public string ShortZhCN { get; set; } = string.Empty;

    // 根据当前语言获取场景名称
    public string GetSceneName(string language)
    {
        return language == "English" ? EnUS : ZhCN;
    }

    // 重写ToString方法，返回所有字段的字符串表示
    public override string ToString()
    {
        return $"ACT={ACT}, EnUS={EnUS}, ZhCN={ZhCN}, ShortEnName={ShortEnName}, ShortZhCN={ShortZhCN}";
    }
}

// 场景数据容器
public class FarmingSpotsData
{
    // 只使用一个属性，并通过YamlMember特性指定别名，避免重复映射
    [YamlMember(Alias = "farmingSpots", ApplyNamingConventions = false)]
    public List<FarmingScene> FarmingSpots { get; set; } = [];
}
