using System;
using YamlDotNet.Serialization;

namespace DiabloTwoMFTimer.Models;

// 掉落物品类
public class LootRecord
{
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty; // 名称

    [YamlMember(Alias = "sceneName")]
    public string SceneName { get; set; } = string.Empty; // 掉落的场景

    [YamlMember(Alias = "runCount")]
    public int RunCount { get; set; } = 0; // 在多少次同场景中掉落的

    [YamlMember(Alias = "dropTime")]
    public DateTime DropTime { get; set; } = DateTime.Now; // 掉落的时间
}
