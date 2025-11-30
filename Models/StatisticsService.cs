// 统计结果模型：场景数据
using System;

namespace DiabloTwoMFTimer.Models;

public class SceneStatDto
{
    public string SceneName { get; set; } = string.Empty;
    public int RunCount { get; set; }
    public double AverageTimeSeconds { get; set; }
    public double FastestTimeSeconds { get; set; }
    public double TotalTimeSeconds { get; set; }
}

// 统计结果模型：掉落数据
public class LootStatDto
{
    public string ItemName { get; set; } = string.Empty;
    public string SceneName { get; set; } = string.Empty;
    public int RunNumber { get; set; } // 第几轮
    public DateTime DropTime { get; set; }
}
