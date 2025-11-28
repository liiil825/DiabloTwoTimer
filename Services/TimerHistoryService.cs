using System;
using System.Collections.Generic;
using System.Linq;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.Services
{
    // 历史记录变更类型枚举
    public enum HistoryChangeType
    {
        FullRefresh, // 全量刷新
        Add, // 仅添加新记录
    }

    // 历史记录变更事件参数
    public class HistoryChangedEventArgs : EventArgs
    {
        public HistoryChangeType ChangeType { get; set; }
        public TimeSpan? AddedRecord { get; set; } // 当ChangeType为Add时，包含新添加的记录
    }

    public interface ITimerHistoryService
    {
        event EventHandler<HistoryChangedEventArgs>? HistoryDataChanged;
        List<TimeSpan> RunHistory { get; }
        int RunCount { get; }
        TimeSpan FastestTime { get; }
        TimeSpan AverageTime { get; }
        void ResetHistoryData();
        bool LoadProfileHistoryData(
            CharacterProfile? profile,
            string scene,
            string characterName,
            GameDifficulty difficulty
        );

        void AddRunRecord(TimeSpan runTime);
        bool DeleteHistoryRecordByIndex(CharacterProfile? profile, string scene, GameDifficulty difficulty, int index);
        void UpdateHistory(List<TimeSpan> runHistory);
    }

    public class TimerHistoryService : ITimerHistoryService
    {
        public TimerHistoryService()
        {
            RunHistory = [];
        }

        // 使用更具体的事件参数
        public event EventHandler<HistoryChangedEventArgs>? HistoryDataChanged;

        // 触发历史数据变更事件
        private void OnHistoryDataChanged(HistoryChangeType changeType, TimeSpan? addedRecord = null)
        {
            HistoryDataChanged?.Invoke(
                this,
                new HistoryChangedEventArgs { ChangeType = changeType, AddedRecord = addedRecord }
            );
        }

        // 历史记录数据
        public List<TimeSpan> RunHistory { get; private set; }

        // 历史记录统计信息
        public int RunCount { get; private set; } = 0;
        public TimeSpan FastestTime { get; private set; } = TimeSpan.MaxValue;
        public TimeSpan AverageTime { get; private set; } = TimeSpan.Zero;

        /// <summary>
        /// 获取符合条件的场景记录
        /// </summary>
        /// <param name="profile">角色档案</param>
        /// <param name="scene">场景名称</param>
        /// <param name="difficulty">游戏难度</param>
        /// <returns>符合条件的记录列表</returns>
        private List<MFRecord> GetSceneRecords(CharacterProfile? profile, string scene, GameDifficulty difficulty)
        {
            if (profile == null || string.IsNullOrEmpty(scene))
                return [];

            // 使用SceneHelper获取英文场景名称进行匹配
            string pureEnglishSceneName = SceneHelper.GetEnglishSceneName(scene);

            // 过滤条件：匹配场景名称、已完成、指定难度
            return profile
                .Records.Where(r =>
                    r.SceneName.Equals(pureEnglishSceneName, StringComparison.OrdinalIgnoreCase)
                    && r.IsCompleted
                    && r.Difficulty == difficulty
                )
                .ToList();
        }

        /// <summary>
        /// 根据索引删除历史记录
        /// </summary>
        /// <param name="profile">角色档案</param>
        /// <param name="scene">场景名称</param>
        /// <param name="difficulty">游戏难度</param>
        /// <param name="index">要删除的记录索引</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteHistoryRecordByIndex(
            CharacterProfile? profile,
            string scene,
            GameDifficulty difficulty,
            int index
        )
        {
            if (profile == null || string.IsNullOrEmpty(scene) || index < 0)
                return false;

            try
            { // 获取符合条件的场景记录
                var sceneRecords = GetSceneRecords(profile, scene, difficulty);

                // 按StartTime升序排序，与RunHistory的构建方式保持一致
                var sortedRecords = sceneRecords.OrderBy(r => r.StartTime).ToList();

                // 检查索引是否有效
                if (index >= sortedRecords.Count)
                    return false;

                // 获取要删除的记录
                var recordToDelete = sortedRecords[index];

                // 从CharacterProfile的Records列表中删除
                bool removedFromProfile = profile.Records.Remove(recordToDelete);

                // 从当前加载的RunHistory列表中删除对应的时间记录
                var timeSpan = TimeSpan.FromSeconds(recordToDelete.DurationSeconds);
                bool removedFromRunHistory = RunHistory.Remove(timeSpan);

                // 如果有一个删除成功，则更新统计信息并触发历史数据变更事件
                if (removedFromProfile || removedFromRunHistory)
                {
                    // 更新统计信息
                    RunCount = RunHistory.Count;

                    // 重新计算最快时间
                    FastestTime = RunHistory.Count > 0 ? RunHistory.Min() : TimeSpan.MaxValue;

                    // 重新计算平均时间
                    if (RunCount > 0)
                    {
                        double totalSeconds = RunHistory.Sum(t => t.TotalSeconds);
                        AverageTime = TimeSpan.FromSeconds(totalSeconds / RunCount);
                    }
                    else
                    {
                        AverageTime = TimeSpan.Zero;
                    }

                    OnHistoryDataChanged(HistoryChangeType.FullRefresh, null);
                    LogManager.WriteDebugLog(
                        "TimerHistoryService",
                        $"根据索引删除记录成功: 索引={index}, 场景 '{recordToDelete.SceneName}', 耗时 '{timeSpan}'"
                    );
                    LogManager.WriteDebugLog(
                        "TimerHistoryService",
                        $"删除后统计信息: 运行次数={RunCount}, 最快时间={FastestTime}, 平均时间={AverageTime}"
                    );
                }

                return removedFromProfile;
            }
            catch (Exception ex)
            {
                // 记录异常信息
                LogManager.WriteErrorLog("TimerHistoryService", $"根据索引删除记录时出错", ex);
                return false;
            }
        }

        /// <summary>
        /// 从角色档案加载特定场景的历史数据
        /// </summary>
        /// <param name="profile">角色档案</param>
        /// <param name="scene">场景名称</param>
        /// <param name="characterName">角色名称</param>
        /// <param name="difficulty">游戏难度</param>
        /// <returns>是否成功加载历史数据</returns>
        public bool LoadProfileHistoryData(
            CharacterProfile? profile,
            string scene,
            string characterName,
            GameDifficulty difficulty
        )
        {
            // 添加调试日志
            LogManager.WriteDebugLog(
                "TimerHistoryService",
                $"开始加载历史数据 - 场景: {scene}, 角色: {characterName}, 难度: {difficulty}"
            );

            // 重置当前的统计数据
            ResetHistoryData();

            // 如果有当前角色档案和场景，加载历史数据
            if (profile != null && !string.IsNullOrEmpty(scene))
            {
                // 使用封装的方法获取符合条件的记录
                var sceneRecords = GetSceneRecords(profile, scene, difficulty);

                LogManager.WriteDebugLog("TimerHistoryService", $"获取到的场景记录数: {sceneRecords.Count}");

                // 如果有记录，更新统计数据
                if (sceneRecords.Count > 0)
                {
                    // 先按StartTime升序排序原始记录，确保按时间从旧到新排列
                    var sortedRecords = sceneRecords.OrderBy(r => r.StartTime).ToList();

                    // 清空现有历史记录，避免重复
                    RunHistory.Clear();
                    LogManager.WriteDebugLog("TimerHistoryService", $"清空后的RunHistory计数: {RunHistory.Count}");

                    // 用于计算平均时间的总和
                    double totalSeconds = 0;

                    // 转换为TimeSpan并添加到历史记录
                    for (int i = 0; i < sortedRecords.Count; i++)
                    {
                        var record = sortedRecords[i];
                        double correctDuration = record.DurationSeconds;

                        TimeSpan duration = TimeSpan.FromSeconds(correctDuration);
                        RunHistory.Add(duration);
                        totalSeconds += correctDuration;

                        LogManager.WriteDebugLog(
                            "TimerHistoryService",
                            $"[加载记录 #{i + 1}] 添加到RunHistory: {duration}"
                        );

                        // 更新最快时间
                        if (duration < FastestTime)
                        {
                            FastestTime = duration;
                        }
                    }

                    RunCount = RunHistory.Count;
                    LogManager.WriteDebugLog("TimerHistoryService", $"最终RunHistory计数: {RunHistory.Count}");

                    // 计算平均时间
                    if (RunCount > 0)
                    {
                        AverageTime = TimeSpan.FromSeconds(totalSeconds / RunCount);
                    }

                    LogManager.WriteDebugLog(
                        "TimerHistoryService",
                        $"[加载完成] 运行次数: {RunCount}, 最快时间: {FastestTime}, 平均时间: {AverageTime}"
                    );

                    // 记录RunHistory的所有内容
                    LogManager.WriteDebugLog("TimerHistoryService", "RunHistory内容:");
                    for (int i = 0; i < RunHistory.Count; i++)
                    {
                        LogManager.WriteDebugLog("TimerHistoryService", $"[{i}] {RunHistory[i]}");
                    }

                    OnHistoryDataChanged(HistoryChangeType.FullRefresh);
                    return true;
                }
            }
            else
            {
                LogManager.WriteDebugLog("TimerHistoryService", "profile为空或scene为空，无法加载历史数据");
            }
            return false;
        }

        /// <summary>
        /// 更新历史记录数据
        /// </summary>
        /// <param name="runHistory">新的历史记录列表</param>
        public void UpdateHistory(List<TimeSpan> runHistory)
        { // 更新历史记录数据
            RunHistory = runHistory;
            RunCount = runHistory.Count;

            // 重新计算统计数据
            if (RunCount > 0)
            {
                FastestTime = TimeSpan.MaxValue;
                double totalSeconds = 0;

                foreach (var time in runHistory)
                {
                    if (time < FastestTime)
                    {
                        FastestTime = time;
                    }
                    totalSeconds += time.TotalSeconds;
                }

                AverageTime = TimeSpan.FromSeconds(totalSeconds / RunCount);
            }
            else
            {
                FastestTime = TimeSpan.MaxValue;
                AverageTime = TimeSpan.Zero;
            }

            OnHistoryDataChanged(HistoryChangeType.FullRefresh);
        }

        /// <summary>
        /// 添加新的运行记录
        /// </summary>
        /// <param name="runTime">运行时间</param>
        public void AddRunRecord(TimeSpan runTime)
        {
            // 添加到历史记录末尾，保持按StartTime升序排序
            RunHistory.Add(runTime);
            RunCount++;

            // 更新最快时间
            if (runTime < FastestTime)
            {
                FastestTime = runTime;
            }

            // 更新平均时间
            double totalSeconds = 0;
            foreach (var time in RunHistory)
            {
                totalSeconds += time.TotalSeconds;
            }
            AverageTime = TimeSpan.FromSeconds(totalSeconds / RunCount);

            // 触发单项添加事件，而不是全量刷新
            OnHistoryDataChanged(HistoryChangeType.Add, runTime);
        }

        /// <summary>
        /// 重置历史数据
        /// </summary>
        public void ResetHistoryData()
        {
            RunHistory.Clear();
            RunCount = 0;
            FastestTime = TimeSpan.MaxValue;
            AverageTime = TimeSpan.Zero;

            // 触发全量刷新事件
            OnHistoryDataChanged(HistoryChangeType.FullRefresh);
        }
    }
}
