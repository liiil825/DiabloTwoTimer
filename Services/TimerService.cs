using System;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.Services
{
    /// <summary>
    /// 计时器状态枚举
    /// </summary>
    public enum TimerStatus
    {
        /// <summary>
        /// 未运行
        /// </summary>
        Stopped,

        /// <summary>
        /// 正在运行
        /// </summary>
        Running,

        /// <summary>
        /// 已暂停
        /// </summary>
        Paused,
    }

    public interface ITimerService : IDisposable
    {
        // 事件
        event Action<string>? TimeUpdatedEvent;
        event Action<bool>? TimerRunningStateChangedEvent;
        event Action<bool>? TimerPauseStateChangedEvent;
        event Action? TimerResetEvent;
        event Action<TimeSpan>? RunCompletedEvent;

        // 属性
        bool IsRunning { get; }
        bool IsPaused { get; }
        bool IsStopped { get; }
        TimerStatus Status { get; }
        DateTime StartTime { get; }
        TimeSpan PausedDuration { get; }
        DateTime PauseStartTime { get; }
        TimerStatus PreviousStatusBeforePause { get; }

        // 方法
        void Start();
        void CompleteRun(bool autoStartNext = false);
        void StartOrNextRun();
        void TogglePause();
        void Pause();
        void Resume();
        void Reset();
        void HandleStartFarm();
        TimeSpan GetElapsedTime();
        string GetFormattedTime();
        void HandleApplicationClosing();
        void RestoreIncompleteRecord();
        void ResetTimerRequested();
    }

    public class TimerService : ITimerService
    {
        private readonly IProfileService _profileService;
        private readonly ITimerHistoryService _historyService;
        private readonly IAppSettings _settings;

        public TimerService(IProfileService profileService, ITimerHistoryService historyService, IAppSettings settings)
        {
            _profileService = profileService;
            _historyService = historyService;
            _settings = settings;
            _timer = new System.Timers.Timer(100); // 100毫秒间隔
            _timer.Elapsed += OnTimerElapsed;
            // 监听 ProfileService 的事件
            _profileService.CurrentProfileChangedEvent += OnProfileChanged;
            _profileService.CurrentSceneChangedEvent += OnSceneChanged;
            _profileService.CurrentDifficultyChangedEvent += OnDifficultyChanged;
        }

        #region Events for UI Communication
        public event Action<string>? TimeUpdatedEvent;
        public event Action<bool>? TimerRunningStateChangedEvent;
        public event Action<bool>? TimerPauseStateChangedEvent;
        public event Action? TimerResetEvent;
        public event Action<TimeSpan>? RunCompletedEvent;
        #endregion

        private readonly System.Timers.Timer _timer;
        private DateTime _startTime = DateTime.MinValue;
        private TimeSpan _pausedDuration = TimeSpan.Zero;
        private DateTime _pauseStartTime = DateTime.MinValue;
        private TimerStatus _status = TimerStatus.Stopped;

        // 记录暂停前的状态，用于番茄钟休息后恢复
        private TimerStatus _previousStatusBeforePause = TimerStatus.Stopped;

        // 当前状态属性
        public bool IsRunning => _status == TimerStatus.Running;
        public bool IsPaused => _status == TimerStatus.Paused;
        public bool IsStopped => _status == TimerStatus.Stopped;
        public TimerStatus Status => _status;
        public DateTime StartTime => _startTime;
        public TimeSpan PausedDuration => _pausedDuration;
        public DateTime PauseStartTime => _pauseStartTime;
        public TimerStatus PreviousStatusBeforePause => _previousStatusBeforePause;

        /// <summary>
        /// 开始计时
        /// </summary>
        public void Start()
        {
            if (_status == TimerStatus.Running)
                return;

            _status = TimerStatus.Running;
            _startTime = DateTime.Now;
            _pauseStartTime = DateTime.Now; // 设置为当前时间，与Resume方法保持一致
            _pausedDuration = TimeSpan.Zero;
            _timer.Start();

            // 创建开始记录
            CreateStartRecord();

            TimerRunningStateChangedEvent?.Invoke(true);
            UpdateTimeDisplay();
        }

        /// <summary>
        /// 完成当前运行并保存记录
        /// </summary>
        /// <param name="autoStartNext">是否自动开始下一场</param>
        public void CompleteRun(bool autoStartNext = false)
        {
            if (IsStopped)
                return;

            if (_pauseStartTime != DateTime.MinValue)
            {
                // 计算暂停期间的时间并累加
                _pausedDuration += DateTime.Now - _pauseStartTime;
                // 重置_pauseStartTime
                _pauseStartTime = DateTime.MinValue;
            }
            _timer.Stop();

            // 记录本次运行时间
            if (_startTime != DateTime.MinValue)
            {
                TimeSpan runTime = GetElapsedTime(); // 使用GetElapsedTime()确保计算一致性
                RunCompletedEvent?.Invoke(runTime);

                // 保存记录到角色档案
                SaveToProfile();
            }

            TimerRunningStateChangedEvent?.Invoke(false);
            _status = TimerStatus.Stopped;

            // 自动开始下一场
            if (autoStartNext)
            {
                System
                    .Threading.Tasks.Task.Delay(100)
                    .ContinueWith(_ =>
                    {
                        Start();
                    });
            }
        }

        /// <summary>
        /// 启动计时器或完成当前运行并开始下一场
        /// </summary>
        public void StartOrNextRun()
        {
            if (IsStopped || IsPaused)
                Start();
            else
                CompleteRun(true); // 完成当前运行并自动开始下一场
        }

        /// <summary>
        /// 切换暂停状态
        /// </summary>
        public void TogglePause()
        {
            if (IsRunning)
                Pause();
            else if (IsPaused)
                Resume();
        }

        /// <summary>
        /// 暂停计时
        /// </summary>
        public void Pause()
        {
            if (!IsRunning)
                return;

            // 保存暂停前的状态
            _previousStatusBeforePause = _status;
            _status = TimerStatus.Paused;
            DateTime now = DateTime.Now;

            // 计算暂停期间的时间并累加
            _pausedDuration += now - _pauseStartTime;

            // 重置_pauseStartTime
            _pauseStartTime = DateTime.MinValue;

            // 停止计时器，避免继续累计
            _timer.Stop();

            // 更新未完成记录 - 使用当前状态计算持续时间
            UpdateIncompleteRecord();
            UpdateTimeDisplay();
            TimerPauseStateChangedEvent?.Invoke(true);
        }

        /// <summary>
        /// 恢复计时
        /// </summary>
        public void Resume()
        {
            if (!IsPaused)
                return;

            _status = TimerStatus.Running;
            // 设置_pauseStartTime = now
            _pauseStartTime = DateTime.Now;
            // 重新启动计时器
            _timer.Start();

            TimerPauseStateChangedEvent?.Invoke(false);
            UpdateTimeDisplay();
        }

        /// <summary>
        /// 重置计时器
        /// </summary>
        public void Reset()
        {
            if (!IsStopped)
            {
                _timer.Stop();
                _status = TimerStatus.Stopped;
                TimerRunningStateChangedEvent?.Invoke(false);
            }

            _startTime = DateTime.MinValue;
            _pausedDuration = TimeSpan.Zero;
            _pauseStartTime = DateTime.MinValue;
            _previousStatusBeforePause = TimerStatus.Stopped;

            TimerResetEvent?.Invoke();
        }

        /// <summary>
        /// 处理开始Farm操作
        /// </summary>
        public void HandleStartFarm()
        {
            // 检查是否有未完成记录
            var hasIncompleteRecord = _profileService.HasIncompleteRecord();
            if (hasIncompleteRecord)
            {
                TogglePause();
            }
            else
            {
                Start();
            }
        }

        /// <summary>
        /// 获取当前经过的时间
        /// </summary>
        public TimeSpan GetElapsedTime()
        {
            if (_startTime == DateTime.MinValue)
                return TimeSpan.Zero;

            // 如果_pauseStartTime被清空重置了，就直接返回_pausedDuration
            if (_pauseStartTime == DateTime.MinValue)
                return _pausedDuration;

            // 否则 = now - _pauseStartTime + _pausedDuration
            DateTime now = DateTime.Now;
            TimeSpan elapsed = now - _pauseStartTime + _pausedDuration;

            // 确保结果不为负
            return elapsed > TimeSpan.Zero ? elapsed : TimeSpan.Zero;
        }

        /// <summary>
        /// 获取格式化的时间字符串
        /// </summary>
        public string GetFormattedTime()
        {
            var elapsed = GetElapsedTime();
            return string.Format(
                "{0:00}:{1:00}:{2:00}:{3}",
                elapsed.Hours,
                elapsed.Minutes,
                elapsed.Seconds,
                (int)(elapsed.Milliseconds / 100)
            );
        }

        /// <summary>
        /// 在应用程序关闭时保存状态
        /// </summary>
        public void HandleApplicationClosing()
        {
            // 如果计时器正在运行，更新未完成记录
            if (IsRunning)
            {
                UpdateIncompleteRecord();
                // SaveTimerState();
                LogManager.WriteDebugLog("TimerService", "应用程序关闭时保存了计时状态和未完成记录");
            }
        }

        /// <summary>
        /// 恢复未完成的计时记录
        /// </summary>
        public void RestoreIncompleteRecord()
        {
            OnRestoreIncompleteRecordRequested();
        }

        /// <summary>
        /// 处理来自ProfileService的重置定时器请求
        /// </summary>
        public void ResetTimerRequested()
        {
            LogManager.WriteDebugLog("TimerService", "接收到重置定时器请求");
            Reset();
            _historyService.ResetHistoryData();
        }

        #region Private Methods
        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            UpdateTimeDisplay();
        }

        private void UpdateTimeDisplay()
        {
            string timeString = GetFormattedTime();
            TimeUpdatedEvent?.Invoke(timeString);
        }

        /// <summary>
        /// 创建开始记录
        /// </summary>
        private void CreateStartRecord()
        {
            string currentCharacter = _profileService.CurrentProfile?.Name ?? "";
            string currentScene = _profileService.CurrentScene;

            if (string.IsNullOrEmpty(currentCharacter) || string.IsNullOrEmpty(currentScene))
                return;

            int actValue = SceneHelper.GetSceneActValue(currentScene);
            string pureEnglishSceneName = SceneHelper.GetEnglishSceneName(currentScene);
            var difficulty = _profileService.CurrentDifficulty;

            if (string.IsNullOrEmpty(pureEnglishSceneName))
            {
                pureEnglishSceneName = "UnknownScene";
                LogManager.WriteDebugLog(
                    "TimerService",
                    $"警告: CreateStartRecord中pureEnglishSceneName为空，使用默认值 '{pureEnglishSceneName}'"
                );
            }

            var newRecord = new MFRecord
            {
                SceneName = pureEnglishSceneName,
                ACT = actValue,
                Difficulty = difficulty,
                StartTime = _startTime,
                EndTime = null,
                LatestTime = _startTime, // 每次启动时，更新latestTime为当前时间
                DurationSeconds = 0.0,
            };

            if (_profileService.CurrentProfile != null)
            {
                _profileService.CurrentProfile.LastRunScene = pureEnglishSceneName;
                _profileService.CurrentProfile.LastRunDifficulty = difficulty;
                DataHelper.AddMFRecord(_profileService.CurrentProfile, newRecord);
                LogManager.WriteDebugLog(
                    "TimerService",
                    $"已创建开始记录到角色档案: {currentCharacter} - {currentScene}, ACT: {actValue}, 开始时间: {_startTime}"
                );
            }
            else
            {
                LogManager.WriteDebugLog(
                    "TimerService",
                    $"已创建临时记录但未保存到档案: {currentCharacter} - {currentScene}, ACT: {actValue}, 开始时间: {_startTime}"
                );
            }
        }

        /// <summary>
        /// 保存记录到角色档案
        /// </summary>
        private void SaveToProfile()
        {
            string currentCharacter = _profileService.CurrentProfile?.Name ?? "";
            string currentScene = _profileService.CurrentScene;

            if (
                string.IsNullOrEmpty(currentCharacter)
                || string.IsNullOrEmpty(currentScene)
                || _profileService.CurrentProfile == null
            )
                return;

            int actValue = SceneHelper.GetSceneActValue(currentScene);
            var difficulty = _profileService.CurrentDifficulty;
            double durationSeconds = GetElapsedTime().TotalSeconds;

            // 统一使用SceneHelper.GetEnglishSceneName获取场景名称
            string pureEnglishSceneName = SceneHelper.GetEnglishSceneName(currentScene);
            if (string.IsNullOrEmpty(pureEnglishSceneName))
            {
                pureEnglishSceneName = "UnknownScene";
                LogManager.WriteDebugLog(
                    "TimerService",
                    $"警告: SaveToProfile中pureEnglishSceneName为空，使用默认值 '{pureEnglishSceneName}'"
                );
            }

            var newRecord = new MFRecord
            {
                SceneName = pureEnglishSceneName,
                ACT = actValue,
                Difficulty = difficulty,
                StartTime = _startTime,
                EndTime = DateTime.Now,
                LatestTime = DateTime.Now,
                DurationSeconds = durationSeconds,
            };
            _profileService.CurrentProfile.LastRunScene = pureEnglishSceneName;
            _profileService.CurrentProfile.LastRunDifficulty = difficulty;
            // 查找并更新现有记录或添加新记录
            var existingRecord = FindIncompleteRecordForCurrentScene();
            if (existingRecord != null)
            {
                existingRecord.EndTime = DateTime.Now;
                existingRecord.LatestTime = DateTime.Now;
                existingRecord.DurationSeconds = durationSeconds;

                DataHelper.UpdateMFRecord(_profileService.CurrentProfile, existingRecord);
                LogManager.WriteDebugLog(
                    "TimerService",
                    $"[更新现有记录] {currentCharacter} - {currentScene}, ACT: {actValue}, 难度: {difficulty}, 开始时间: {existingRecord.StartTime}, 结束时间: {DateTime.Now}, DurationSeconds: {existingRecord.DurationSeconds}"
                );
            }
            else
            {
                DataHelper.AddMFRecord(_profileService.CurrentProfile, newRecord);
                LogManager.WriteDebugLog(
                    "TimerService",
                    $"[添加新记录] {currentCharacter} - {currentScene}, ACT: {actValue}, 难度: {difficulty}, 开始时间: {_startTime}, 结束时间: {DateTime.Now}, DurationSeconds: {newRecord.DurationSeconds}"
                );
            }

            // 保存数据后生成并复制房间名称（如果设置启用）
            if (_settings.GenerateRoomName)
            {
                GenerateAndCopyRoomName();
            }
        }

        /// <summary>
        /// 更新未完成记录 - 简化逻辑
        /// </summary>
        private void UpdateIncompleteRecord(DateTime? updateTime = null)
        {
            var record = FindIncompleteRecordForCurrentScene();
            if (record == null)
                return;

            DateTime now = updateTime ?? DateTime.Now;

            // 直接使用GetElapsedTime()的计算结果，确保一致性
            record.DurationSeconds = GetElapsedTime().TotalSeconds;
            record.LatestTime = now;

            if (_profileService.CurrentProfile != null)
            {
                DataHelper.UpdateMFRecord(_profileService.CurrentProfile, record);
                LogManager.WriteDebugLog(
                    "TimerService",
                    $"更新未完成记录: 场景={_profileService.CurrentScene}, "
                        + $"持续时间={record.DurationSeconds}秒, "
                        + $"开始时间={_startTime}, 当前时间={now}"
                );
            }
            else
            {
                LogManager.WriteDebugLog("TimerService", $"跳过更新记录：当前角色档案为 null");
            }
        }

        /// <summary>
        /// 查找未完成记录
        /// </summary>
        private MFRecord? FindIncompleteRecordForCurrentScene()
        {
            if (_profileService.CurrentProfile == null)
                return null;
            if (_profileService.CurrentScene == null)
                return null;

            string pureEnglishSceneName = SceneHelper.GetEnglishSceneName(_profileService.CurrentScene);
            return _profileService
                .CurrentProfile.Records.Where(r =>
                    r.SceneName == pureEnglishSceneName
                    && r.Difficulty == _profileService.CurrentDifficulty
                    && !r.IsCompleted
                )
                .OrderByDescending(r => r.StartTime)
                .FirstOrDefault();
        }

        /// <summary>
        /// 处理来自ProfileService的恢复未完成记录请求
        /// </summary>
        private void OnRestoreIncompleteRecordRequested()
        {
            Reset();
            LogManager.WriteDebugLog("TimerService", "接收到恢复未完成记录请求");
            var record = FindIncompleteRecordForCurrentScene();
            if (record == null)
                return;

            _status = TimerStatus.Paused;
            _startTime = record.StartTime;
            _pauseStartTime = DateTime.MinValue;
            _pausedDuration = TimeSpan.FromSeconds(record.DurationSeconds);

            // 立即更新显示
            UpdateTimeDisplay();

            LogManager.WriteDebugLog(
                "TimerService",
                $"从记录恢复计时状态: " + $"已累计时间={record.DurationSeconds}秒, " + $"开始时间={_startTime}"
            );
            // 通知UI状态变化
            TimerRunningStateChangedEvent?.Invoke(_status != TimerStatus.Stopped);
            TimerPauseStateChangedEvent?.Invoke(_status == TimerStatus.Paused);
        }

        private void OnProfileChanged(CharacterProfile? profile)
        {
            Reset();
            // 当档案变更时，恢复未完成记录
            if (profile != null)
            {
                RestoreIncompleteRecord();
            }
        }

        private void OnSceneChanged(string? scene)
        {
            Reset();

            // 只有在不是暂停状态下切换场景时才恢复未完成记录
            // 这样可以避免将暂停的时间数据错误地应用到新场景
            if (scene != null)
            {
                RestoreIncompleteRecord();
            }
        }

        private void OnDifficultyChanged(GameDifficulty difficulty)
        {
            Reset();
            RestoreIncompleteRecord();
        }
        #endregion

        /// <summary>
        /// 生成房间名称并复制到剪贴板
        /// 格式: {档案角色命}{场景名}{TimerHistoryService.RunCount+1}
        /// </summary>
        private void GenerateAndCopyRoomName()
        {
            try
            {
                string characterName = _profileService.CurrentProfile?.Name ?? "";
                string sceneName = _profileService.CurrentScene ?? "";
                int runCount = _historyService.RunCount + 1;

                if (string.IsNullOrEmpty(characterName) || string.IsNullOrEmpty(sceneName))
                {
                    LogManager.WriteDebugLog("TimerService", "无法生成房间名称：角色名或场景名为空");
                    return;
                }

                // 使用场景shortName，传递角色名以确定使用中文还是英文短名称
                string sceneShortName = SceneHelper.GetSceneShortName(sceneName, characterName);
                if (string.IsNullOrEmpty(sceneShortName))
                {
                    sceneShortName = "UnknownScene";
                }

                // 生成房间名称
                string roomName = $"{characterName}{sceneShortName}{runCount}";

                // 复制到剪贴板
                Clipboard.SetText(roomName);
                LogManager.WriteDebugLog("TimerService", $"已生成并复制房间名称到剪贴板: {roomName}");
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("TimerService", $"生成房间名称时出错: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _profileService.CurrentProfileChangedEvent -= OnProfileChanged;
            _profileService.CurrentSceneChangedEvent -= OnSceneChanged;
            _profileService.CurrentDifficultyChangedEvent -= OnDifficultyChanged;
            _timer.Dispose();
        }
    }
}
