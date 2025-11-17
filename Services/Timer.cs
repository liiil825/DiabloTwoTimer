using System;
using System.Linq;
using System.Timers;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.Services
{
    public class TimerService : IDisposable
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
            Paused
        }
        #region Singleton Implementation
        private static readonly Lazy<TimerService> _instance =
            new(() => new TimerService());

        public static TimerService Instance => _instance.Value;

        private TimerService()
        {
            _timer = new Timer(100); // 100毫秒间隔
            _timer.Elapsed += OnTimerElapsed;
        }
        #endregion

        #region Events for UI Communication
        public event Action<string>? TimeUpdatedEvent;
        public event Action<bool>? TimerRunningStateChangedEvent;
        public event Action<bool>? TimerPauseStateChangedEvent;
        public event Action? TimerResetEvent;
        public event Action<TimeSpan>? RunCompletedEvent;
        #endregion

        private readonly Timer _timer;
        private DateTime _startTime = DateTime.MinValue;
        private TimeSpan _pausedDuration = TimeSpan.Zero;
        private DateTime _pauseStartTime = DateTime.MinValue;
        private TimerStatus _status = TimerStatus.Stopped;

        // 当前状态属性
        public bool IsRunning => _status == TimerStatus.Running;
        public bool IsPaused => _status == TimerStatus.Paused;
        public bool IsStopped => _status == TimerStatus.Stopped;
        public TimerStatus Status => _status;
        public DateTime StartTime => _startTime;
        public TimeSpan PausedDuration => _pausedDuration;
        public DateTime PauseStartTime => _pauseStartTime;

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
        /// 停止计时
        /// </summary>
        /// <param name="autoStartNext">是否自动开始下一场</param>
        public void Stop(bool autoStartNext = false)
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
                System.Threading.Tasks.Task.Delay(100).ContinueWith(_ =>
                {
                    Start();
                });
            }
        }

        /// <summary>
        /// 启动计时器或停止当前计时并重新开始
        /// </summary>
        public void StartOrRestart()
        {
            if (IsStopped || IsPaused)
                Start();
            else
                Stop(true); // 停止并自动开始下一场
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
            Stop();
            _startTime = DateTime.MinValue;
            _pausedDuration = TimeSpan.Zero;
            _pauseStartTime = DateTime.MinValue;

            TimerResetEvent?.Invoke();
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
            return string.Format("{0:00}:{1:00}:{2:00}:{3}",
                elapsed.Hours, elapsed.Minutes, elapsed.Seconds,
                (int)(elapsed.Milliseconds / 100));
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
            TimerHistoryService.Instance.ResetHistoryData();
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
            var profileService = ProfileService.Instance;
            string currentCharacter = profileService.CurrentProfile?.Name ?? "";
            string currentScene = profileService.CurrentScene;

            if (string.IsNullOrEmpty(currentCharacter) || string.IsNullOrEmpty(currentScene))
                return;

            int actValue = SceneService.GetSceneActValue(currentScene);
            string pureEnglishSceneName = LanguageManager.GetPureEnglishSceneName(currentScene);
            var difficulty = profileService.CurrentDifficulty;

            if (string.IsNullOrEmpty(pureEnglishSceneName))
            {
                pureEnglishSceneName = "UnknownScene";
                LogManager.WriteDebugLog("TimerService", $"警告: CreateStartRecord中pureEnglishSceneName为空，使用默认值 '{pureEnglishSceneName}'");
            }

            var newRecord = new MFRecord
            {
                SceneName = pureEnglishSceneName,
                ACT = actValue,
                Difficulty = difficulty,
                StartTime = _startTime,
                EndTime = null,
                LatestTime = _startTime, // 每次启动时，更新latestTime为当前时间
                DurationSeconds = 0.0
            };

            if (profileService.CurrentProfile != null)
            {
                DataService.AddMFRecord(profileService.CurrentProfile, newRecord);
                LogManager.WriteDebugLog("TimerService", $"已创建开始记录到角色档案: {currentCharacter} - {currentScene}, ACT: {actValue}, 开始时间: {_startTime}");
            }
            else
            {
                LogManager.WriteDebugLog("TimerService", $"已创建临时记录但未保存到档案: {currentCharacter} - {currentScene}, ACT: {actValue}, 开始时间: {_startTime}");
            }
        }

        /// <summary>
        /// 更新未完成记录 - 简化逻辑
        /// </summary>
        private void UpdateIncompleteRecord(DateTime? updateTime = null)
        {
            var profileService = ProfileService.Instance;
            var record = FindIncompleteRecordForCurrentScene();
            if (record == null)
                return;

            DateTime now = updateTime ?? DateTime.Now;

            // 直接使用GetElapsedTime()的计算结果，确保一致性
            record.DurationSeconds = GetElapsedTime().TotalSeconds;
            record.LatestTime = now;

            if (profileService.CurrentProfile != null)
            {
                DataService.UpdateMFRecord(profileService.CurrentProfile, record);
                LogManager.WriteDebugLog("TimerService",
                    $"更新未完成记录: 场景={profileService.CurrentScene}, " +
                    $"持续时间={record.DurationSeconds}秒, " +
                    $"开始时间={_startTime}, 当前时间={now}");
            }
            else
            {
                LogManager.WriteDebugLog("TimerService", $"跳过更新记录：当前角色档案为 null");
            }

        }

        /// <summary>
        /// 保存记录到角色档案
        /// </summary>
        private void SaveToProfile()
        {
            var profileService = ProfileService.Instance;
            string currentCharacter = profileService.CurrentProfile?.Name ?? "";
            string currentScene = profileService.CurrentScene;

            if (profileService.CurrentProfile == null || string.IsNullOrEmpty(currentCharacter) ||
                string.IsNullOrEmpty(currentScene) || _startTime == DateTime.MinValue)
                return;

            int actValue = SceneService.GetSceneActValue(currentScene);
            var difficulty = profileService.CurrentDifficulty;
            double durationSeconds = GetElapsedTime().TotalSeconds;

            // 统一使用LanguageManager.GetPureEnglishSceneName获取场景名称
            string pureEnglishSceneName = LanguageManager.GetPureEnglishSceneName(currentScene);
            if (string.IsNullOrEmpty(pureEnglishSceneName))
            {
                pureEnglishSceneName = "UnknownScene";
                LogManager.WriteDebugLog("TimerService", $"警告: SaveToProfile中pureEnglishSceneName为空，使用默认值 '{pureEnglishSceneName}'");
            }

            var newRecord = new MFRecord
            {
                SceneName = pureEnglishSceneName,
                ACT = actValue,
                Difficulty = difficulty,
                StartTime = _startTime,
                EndTime = DateTime.Now,
                LatestTime = DateTime.Now,
                DurationSeconds = durationSeconds
            };

            // 查找并更新现有记录或添加新记录
            var existingRecord = FindIncompleteRecordForCurrentScene();

            if (existingRecord != null)
            {
                existingRecord.EndTime = DateTime.Now;
                existingRecord.LatestTime = DateTime.Now;
                existingRecord.DurationSeconds = durationSeconds;
                existingRecord.ACT = actValue;
                existingRecord.Difficulty = difficulty;

                DataService.UpdateMFRecord(profileService.CurrentProfile, existingRecord);
                LogManager.WriteDebugLog("TimerService", $"[更新现有记录] {currentCharacter} - {currentScene}, ACT: {actValue}, 难度: {difficulty}, 开始时间: {existingRecord.StartTime}, 结束时间: {DateTime.Now}, DurationSeconds: {existingRecord.DurationSeconds}");
            }
            else
            {
                DataService.AddMFRecord(profileService.CurrentProfile, newRecord);
                LogManager.WriteDebugLog("TimerService", $"[添加新记录] {currentCharacter} - {currentScene}, ACT: {actValue}, 难度: {difficulty}, 开始时间: {_startTime}, 结束时间: {DateTime.Now}, DurationSeconds: {newRecord.DurationSeconds}");
            }
        }

        /// <summary>
        /// 查找未完成记录
        /// </summary>
        private static MFRecord? FindIncompleteRecordForCurrentScene()
        {
            var profileService = ProfileService.Instance;
            if (profileService.CurrentProfile == null)
                return null;
            if (profileService.CurrentScene == null)
                return null;

            string pureEnglishSceneName = LanguageManager.GetPureEnglishSceneName(profileService.CurrentScene);
            return profileService.CurrentProfile.Records
                .Where(r => r.SceneName == pureEnglishSceneName && r.Difficulty == profileService.CurrentDifficulty && !r.IsCompleted)
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

            LogManager.WriteDebugLog("TimerService",
                $"从记录恢复计时状态: " +
                $"已累计时间={record.DurationSeconds}秒, " +
                $"开始时间={_startTime}");
            // 通知UI状态变化
            TimerRunningStateChangedEvent?.Invoke(_status != TimerStatus.Stopped);
            TimerPauseStateChangedEvent?.Invoke(_status == TimerStatus.Paused);
        }
        #endregion

        public void Dispose()
        {

        }
    }
}