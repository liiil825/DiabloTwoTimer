using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.Services;

public class StatisticsService(IProfileService profileService, IAppSettings appSettings, ISceneService sceneService)
    : IStatisticsService
{
    private readonly IProfileService _profileService = profileService;
    private readonly IAppSettings _appSettings = appSettings;
    private readonly ISceneService _sceneService = sceneService;

    /// <summary>
    /// 获取指定时间段内的场景统计数据
    /// </summary>
    public List<SceneStatDto> GetSceneStatistics(DateTime startTime, DateTime endTime, bool sortByCount = true)
    {
        var profile = _profileService.CurrentProfile;
        if (profile == null || profile.Records == null)
            return [];

        // 筛选时间段内且已完成的记录
        var validRecords = profile
            .Records.Where(r => r.IsCompleted && r.StartTime >= startTime && r.StartTime <= endTime)
            .ToList();

        var stats = validRecords
            .GroupBy(r => r.SceneName)
            .Select(g => new SceneStatDto
            {
                SceneName = g.Key,
                RunCount = g.Count(),
                TotalTimeSeconds = g.Sum(r => r.DurationSeconds),
                AverageTimeSeconds = Math.Round(g.Average(r => r.DurationSeconds), 1),
                FastestTimeSeconds = Math.Round(g.Min(r => r.DurationSeconds), 1),
            })
            .ToList();

        // 排序
        if (sortByCount)
        {
            return stats.OrderByDescending(s => s.RunCount).ThenBy(s => s.SceneName).ToList();
        }
        else
        {
            // 按平均时间排序
            return stats.OrderBy(s => s.AverageTimeSeconds).ToList();
        }
    }

    /// <summary>
    /// 获取本周一开始的时间
    /// </summary>
    public DateTime GetStartOfWeek()
    {
        DateTime now = DateTime.Now;
        // DayOfWeek: Sunday=0, Monday=1...
        int diff = (7 + (now.DayOfWeek - DayOfWeek.Monday)) % 7;
        return now.AddDays(-1 * diff).Date;
    }

    /// <summary>
    /// 获取指定时间段内的掉落记录
    /// </summary>
    public List<LootStatDto> GetLootStatistics(DateTime startTime, DateTime endTime)
    {
        var profile = _profileService.CurrentProfile;
        if (profile == null || profile.LootRecords == null)
            return [];

        return profile
            .LootRecords.Where(l => l.DropTime >= startTime && l.DropTime <= endTime)
            .Select(l => new LootStatDto
            {
                ItemName = l.Name,
                SceneName = l.SceneName,
                RunNumber = l.RunCount,
                DropTime = l.DropTime,
            })
            .OrderByDescending(l => l.DropTime)
            .ToList();
    }

    /// <summary>
    /// 获取今日概览简单文本（用于BreakForm）
    /// </summary>
    public string GetSimpleSummary(DateTime start, DateTime end)
    {
        var profile = _profileService.CurrentProfile;
        if (profile == null)
            return LanguageManager.GetString("NoData");

        var records = profile.Records.Where(r => r.IsCompleted && r.StartTime >= start && r.StartTime <= end).ToList();
        if (records.Count == 0)
            return LanguageManager.GetString("NoData");

        int totalRuns = records.Count;
        double avgTime = records.Average(r => r.DurationSeconds);
        int loots = profile.LootRecords.Count(l => l.DropTime >= start && l.DropTime <= end);

        return $"{LanguageManager.GetString("TotalRuns")}: {totalRuns} | {LanguageManager.GetString("Average")}: {avgTime:F1}s | {LanguageManager.GetString("Loots")}: {loots}";
    }

    /// <summary>
    /// 获取详细的统计摘要（多行文本）
    /// </summary>
    public string GetDetailedSummary(DateTime start, DateTime end)
    {
        if (_profileService.CurrentProfile == null)
            return LanguageManager.GetString("NoData");

        var sb = new StringBuilder();
        // 1. 获取场景统计
        var validRecords = _profileService
            .CurrentProfile.Records.Where(r => r.IsCompleted && r.StartTime >= start && r.StartTime <= end)
            .ToList();

        if (validRecords.Count > 0)
        {
            var sceneStats = validRecords
                .GroupBy(r => r.SceneName)
                .Select(g => new
                {
                    Name = g.Key,
                    Count = g.Count(),
                    Avg = g.Average(r => r.DurationSeconds),
                    Fastest = g.Min(r => r.DurationSeconds),
                })
                .OrderByDescending(x => x.Count) // 按次数排序
                .ToList();

            sb.AppendLine($"【{LanguageManager.GetString("SceneData")}】");
            foreach (var s in sceneStats)
            {
                // 格式：崔凡客: 25次 | Avg: 45s | Best: 40s
                string localizedSceneName = _sceneService.GetLocalizedShortSceneName(s.Name);
                sb.AppendLine(
                    $"{localizedSceneName}: {s.Count}{LanguageManager.GetString("Times")} | {LanguageManager.GetString("Avg")}: {s.Avg:F1}s | {LanguageManager.GetString("Fastest")}: {s.Fastest:F1}s"
                );
            }
        }
        else
        {
            sb.AppendLine($"【{LanguageManager.GetString("SceneData")}】");
            sb.AppendLine(LanguageManager.GetString("NoRunRecords"));
        }

        sb.AppendLine(); // 空一行

        // 2. 获取掉落统计
        var loots = _profileService
            .CurrentProfile.LootRecords.Where(l => l.DropTime >= start && l.DropTime <= end)
            .OrderByDescending(l => l.DropTime) // 最近的在上面
            .ToList();

        if (loots.Count > 0)
        {
            // 只显示最近的
            sb.AppendLine($"【{LanguageManager.GetString("LootItems")}】");
            int maxDisplayCount = 10;
            var displayLoots = loots.Take(maxDisplayCount).ToList();
            foreach (var l in displayLoots)
            {
                // 格式：崔凡客(25): 28号符文
                string localizedSceneName = _sceneService.GetLocalizedShortSceneName(l.SceneName);
                sb.AppendLine($"{localizedSceneName} ({LanguageManager.GetString("Round")} {l.RunCount}): {l.Name}");
            }

            // 如果超过10条，给出提示
            if (loots.Count > maxDisplayCount)
            {
                int hiddenCount = loots.Count - maxDisplayCount;
                sb.AppendLine($"... {LanguageManager.GetString("And")} {hiddenCount} {LanguageManager.GetString("MoreRecords")}");
            }
        }
        // 如果没有掉落，就不显示掉落栏位，或者显示"无"
        else if (validRecords.Count > 0)
        {
            sb.AppendLine($"【{LanguageManager.GetString("LootItems")}】");
            sb.AppendLine(LanguageManager.GetString("NoHighValueLoots"));
        }

        return sb.ToString();
    }
}
