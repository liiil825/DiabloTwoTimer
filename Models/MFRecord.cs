using System;
using YamlDotNet.Serialization;

namespace DiabloTwoMFTimer.Models;

// MF记录类
public class MFRecord
{
    [YamlMember(Alias = "sceneName")]
    public string SceneName { get; set; } = string.Empty;

    [YamlMember(Alias = "difficulty")]
    public GameDifficulty Difficulty { get; set; } = GameDifficulty.Normal;

    [YamlMember(Alias = "startTime")]
    public DateTime StartTime { get; set; }

    [YamlMember(Alias = "endTime")]
    public DateTime? EndTime { get; set; }

    [YamlMember(Alias = "latestTime")]
    public DateTime? LatestTime { get; set; }

    [YamlMember(Alias = "durationSeconds")]
    public double DurationSeconds { get; set; } = 0;

    [YamlIgnore]
    public bool IsCompleted => EndTime.HasValue;
}
