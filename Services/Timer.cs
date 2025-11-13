using System;
using System.Linq;
using System.Timers;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.Services
{
    public class TimerService : IDisposable
    {
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
        public event Action<string>? TimeUpdated;
        public event Action<bool>? TimerRunningStateChanged;
        public event Action<bool>? TimerPauseStateChanged;
        public event Action? TimerReset;
        public event Action<TimeSpan>? RunCompleted;
        #endregion

        private readonly Timer _timer;
        private DateTime _startTime = DateTime.MinValue;
        private TimeSpan _pausedDuration = TimeSpan.Zero;
        private DateTime _pauseStartTime = DateTime.MinValue;
        private bool _isRunning = false;
        private bool _isPaused = false;

        // 当前状态属性
        public bool IsRunning => _isRunning;
        public bool IsPaused => _isPaused;
        public DateTime StartTime => _startTime;
        public TimeSpan PausedDuration => _pausedDuration;
        public DateTime PauseStartTime => _pauseStartTime;

        // 直接使用ProfileService中的数据，不再维护重复的变量

        /// <summary>
        /// 开始计时
        /// </summary>
        public void Start()
        {
            if (_isRunning)
                return;

            _isRunning = true;
            _isPaused = false;
            _startTime = DateTime.Now;
            _pausedDuration = TimeSpan.Zero;
            _pauseStartTime = DateTime.MinValue;
            _timer.Start();

            // 创建开始记录
            CreateStartRecord();

            TimerRunningStateChanged?.Invoke(true);
            UpdateTimeDisplay();
        }

        /// <summary>
        /// 停止计时
        /// </summary>
        /// <param name="autoStartNext">是否自动开始下一场</param>
        public void Stop(bool autoStartNext = false)
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _isPaused = false;
            _timer.Stop();

            // 记录本次运行时间
            if (_startTime != DateTime.MinValue)
            {
                TimeSpan runTime = DateTime.Now - _startTime - _pausedDuration;
                RunCompleted?.Invoke(runTime);

                // 保存记录到角色档案
                SaveToProfile();
            }

            // 清除计时状态
            ClearTimerState();

            TimerRunningStateChanged?.Invoke(false);

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
        /// 切换计时状态（开始/停止）
        /// </summary>
        public void Toggle()
        {
            if (!_isRunning)
                Start();
            else
                Stop(true); // 停止并自动开始下一场
        }

        /// <summary>
        /// 切换暂停状态
        /// </summary>
        public void TogglePause()
        {
            if (_isRunning)
            {
                if (_isPaused)
                    Resume();
                else
                    Pause();
            }
        }

        /// <summary>
        /// 暂停计时
        /// </summary>
        public void Pause()
        {
            if (_isRunning && !_isPaused)
            {
                _isPaused = true;
                _pauseStartTime = DateTime.Now;

                // 更新未完成记录
                UpdateIncompleteRecord();

                TimerPauseStateChanged?.Invoke(true);
                UpdateTimeDisplay();
            }
        }

        /// <summary>
        /// 恢复计时
        /// </summary>
        public void Resume()
        {
            if (_isRunning && _isPaused && _pauseStartTime != DateTime.MinValue)
            {
                // 更新记录，使用pauseStartTime作为更新时间点
                UpdateIncompleteRecord(_pauseStartTime);

                _pausedDuration += DateTime.Now - _pauseStartTime;
                _isPaused = false;
                _pauseStartTime = DateTime.MinValue;

                // 只更新latestTime，不影响elapsedTime计算
                UpdateRecordLatestTime();

                TimerPauseStateChanged?.Invoke(false);
                UpdateTimeDisplay();
            }
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

            ClearTimerState();
            TimerReset?.Invoke();
        }

        /// <summary>
        /// 获取当前经过的时间
        /// </summary>
        public TimeSpan GetElapsedTime()
        {
            if (!_isRunning || _startTime == DateTime.MinValue)
                return TimeSpan.Zero;

            if (_isPaused && _pauseStartTime != DateTime.MinValue)
            {
                // 暂停状态，计算到暂停开始时的时间
                return _pauseStartTime - _startTime - _pausedDuration;
            }
            else
            {
                // 运行状态，计算实际经过时间（扣除暂停时间）
                return DateTime.Now - _startTime - _pausedDuration;
            }
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
        public void OnApplicationClosing()
        {
            if (_isRunning)
            {
                // 如果计时器正在运行，更新未完成记录
                if (!_isPaused)
                {
                    UpdateIncompleteRecord();
                }
                
                SaveTimerState();
                LogManager.WriteDebugLog("TimerService", "应用程序关闭时保存了计时状态和未完成记录");
            }
        }

        #region Private Methods

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            UpdateTimeDisplay();
        }

        private void UpdateTimeDisplay()
        {
            string timeString = GetFormattedTime();
            TimeUpdated?.Invoke(timeString);
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

            try
            {                
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
                    LatestTime = _startTime,
                    ElapsedTime = 0.0
                };

                if (profileService.CurrentProfile != null)
                {                    DataService.AddMFRecord(profileService.CurrentProfile, newRecord);
                    LogManager.WriteDebugLog("TimerService", $"已创建开始记录到角色档案: {currentCharacter} - {currentScene}, ACT: {actValue}, 开始时间: {_startTime}");
                }
                else
                {                    LogManager.WriteDebugLog("TimerService", $"已创建临时记录但未保存到档案: {currentCharacter} - {currentScene}, ACT: {actValue}, 开始时间: {_startTime}");
                }            }
            catch (Exception ex)
            {                LogManager.WriteDebugLog("TimerService", $"创建开始记录失败: {ex.Message}, 堆栈: {ex.StackTrace}");
            }        }

        /// <summary>
        /// 更新未完成记录
        /// </summary>
        private void UpdateIncompleteRecord(DateTime? updateTime = null)
        {            
            var profileService = ProfileService.Instance;
            if (profileService.CurrentProfile == null)
                return;

            var record = FindIncompleteRecordForCurrentScene(profileService.CurrentProfile, profileService.CurrentScene, profileService.CurrentDifficulty);
            if (record == null)
                return;

            try
            {
                double previousElapsedTime = record.ElapsedTime ?? 0;
                DateTime? previousLatestTime = record.LatestTime;
                DateTime now = updateTime ?? DateTime.Now;

                if (!record.LatestTime.HasValue)
                {
                    record.LatestTime = record.StartTime;
                    previousLatestTime = record.StartTime;
                    LogManager.WriteDebugLog("TimerService", $"[更新记录] 初始化LatestTime为StartTime: {record.StartTime}");
                }

                double newElapsedTime;
                if (previousLatestTime.HasValue)
                {
                    DateTime effectiveUpdateTime = now > previousLatestTime.Value ? now : previousLatestTime.Value;
                    double additionalSeconds = (effectiveUpdateTime - previousLatestTime.Value).TotalSeconds;
                    newElapsedTime = previousElapsedTime + additionalSeconds;
                    LogManager.WriteDebugLog("TimerService", $"[更新记录] 基于LatestTime计算: 上次时间={previousLatestTime.Value}, 当前时间={effectiveUpdateTime}, 增加时间={additionalSeconds}, 总计={newElapsedTime}");
                }
                else
                {
                    newElapsedTime = (now - record.StartTime).TotalSeconds;
                    LogManager.WriteDebugLog("TimerService", $"[更新记录] 基于StartTime计算: 开始时间={record.StartTime}, 当前时间={now}, 总计={newElapsedTime}");
                }

                if (newElapsedTime > previousElapsedTime || previousElapsedTime == 0)
                {
                    record.ElapsedTime = newElapsedTime;
                }

                record.LatestTime = now;
                DataService.UpdateMFRecord(profileService.CurrentProfile, record);

                LogManager.WriteDebugLog("TimerService", $"已更新未完成记录: 场景={profileService.CurrentScene}, 上次累计时间={previousElapsedTime}秒, 当前累计时间={newElapsedTime}秒, 上次更新时间={previousLatestTime}, 更新时间点={now}");
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("TimerService", $"更新未完成记录失败: {ex.Message}, 堆栈: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 更新记录的LatestTime
        /// </summary>
        private void UpdateRecordLatestTime()
        {            var profileService = ProfileService.Instance;
            if (profileService.CurrentProfile != null)
            {                var record = FindIncompleteRecordForCurrentScene(profileService.CurrentProfile, profileService.CurrentScene, profileService.CurrentDifficulty);
                if (record != null)
                {
                    record.LatestTime = DateTime.Now;
                    DataService.UpdateMFRecord(profileService.CurrentProfile, record);
                    LogManager.WriteDebugLog("TimerService", $"已更新未完成记录的LatestTime: 场景={profileService.CurrentScene}, 更新时间点={DateTime.Now}");
                }
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

            try
            {                int actValue = SceneService.GetSceneActValue(currentScene);
                var difficulty = profileService.CurrentDifficulty;

                // 计算实际持续时间
                TimeSpan actualDuration = DateTime.Now - _startTime - _pausedDuration;
                double durationSeconds = actualDuration.TotalSeconds;

                // 获取英文场景名称
                string sceneEnName = DataService.GetEnglishSceneName(currentScene);
                if (string.IsNullOrEmpty(sceneEnName))
                {                    sceneEnName = "UnknownScene";
                    LogManager.WriteDebugLog("TimerService", $"警告: SaveToProfile中sceneEnName为空，使用默认值 '{sceneEnName}'");
                }

                var newRecord = new MFRecord
                {                    SceneName = sceneEnName,
                    ACT = actValue,
                    Difficulty = difficulty,
                    StartTime = _startTime,
                    EndTime = DateTime.Now,
                    LatestTime = DateTime.Now,
                    ElapsedTime = durationSeconds
                };

                // 查找并更新现有记录或添加新记录
                string pureEnglishSceneName = LanguageManager.GetPureEnglishSceneName(currentScene);
                var existingRecord = profileService.CurrentProfile.Records.FirstOrDefault(r =>
                    r.SceneName == pureEnglishSceneName &&
                    r.Difficulty == difficulty &&
                    !r.IsCompleted);

                if (existingRecord != null)
                {                    TimeSpan existingRecordDuration = DateTime.Now - existingRecord.StartTime - _pausedDuration;
                    double existingRecordSeconds = existingRecordDuration.TotalSeconds;

                    existingRecord.EndTime = DateTime.Now;
                    existingRecord.LatestTime = DateTime.Now;
                    existingRecord.ElapsedTime = existingRecordSeconds;
                    existingRecord.ACT = actValue;
                    existingRecord.Difficulty = difficulty;

                    DataService.UpdateMFRecord(profileService.CurrentProfile, existingRecord);
                    LogManager.WriteDebugLog("TimerService", $"[更新现有记录] {currentCharacter} - {currentScene}, ACT: {actValue}, 难度: {difficulty}, 开始时间: {existingRecord.StartTime}, 结束时间: {DateTime.Now}, ElapsedTime: {existingRecord.ElapsedTime}");
                }                else
                {                    DataService.AddMFRecord(profileService.CurrentProfile, newRecord);
                    LogManager.WriteDebugLog("TimerService", $"[添加新记录] {currentCharacter} - {currentScene}, ACT: {actValue}, 难度: {difficulty}, 开始时间: {_startTime}, 结束时间: {DateTime.Now}, ElapsedTime: {newRecord.ElapsedTime}");
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("TimerService", $"保存计时记录失败: {ex.Message}, 堆栈: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 查找未完成记录
        /// </summary>
        private static MFRecord? FindIncompleteRecordForCurrentScene(CharacterProfile? profile, string scene, GameDifficulty difficulty)
        {            if (profile == null || string.IsNullOrEmpty(scene))
                return null;

            string pureEnglishSceneName = LanguageManager.GetPureEnglishSceneName(scene);
            return profile.Records
                .Where(r => r.SceneName == pureEnglishSceneName && r.Difficulty == difficulty && !r.IsCompleted)
                .OrderByDescending(r => r.StartTime)
                .FirstOrDefault();
        }

        /// <summary>
        /// 保存计时状态
        /// </summary>
        private void SaveTimerState()
        {
            try
            {
                var settings = SettingsManager.LoadSettings();
                settings.IsTimerInProgress = _isRunning;
                settings.TimerStartTime = _startTime;
                settings.TimerPausedDuration = _pausedDuration.TotalMilliseconds;
                settings.IsTimerPaused = _isPaused;
                settings.TimerPauseStartTime = _pauseStartTime;
                settings.InProgressCharacter = ProfileService.Instance.CurrentProfile?.Name ?? "";
                settings.InProgressScene = ProfileService.Instance.CurrentScene;
                SettingsManager.SaveSettings(settings);
                LogManager.WriteDebugLog("TimerService", $"已保存计时状态: isTimerInProgress={_isRunning}, isPaused={_isPaused}, character={ProfileService.Instance.CurrentProfile?.Name}, scene={ProfileService.Instance.CurrentScene}");
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("TimerService", $"保存计时状态失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 清除计时状态
        /// </summary>
        private static void ClearTimerState()
        {
            try
            {
                var settings = SettingsManager.LoadSettings();
                settings.IsTimerInProgress = false;
                settings.TimerStartTime = DateTime.MinValue;
                settings.TimerPausedDuration = 0;
                settings.IsTimerPaused = false;
                settings.TimerPauseStartTime = DateTime.MinValue;
                settings.InProgressCharacter = "";
                settings.InProgressScene = "";
                SettingsManager.SaveSettings(settings);
                LogManager.WriteDebugLog("TimerService", "已清除计时状态");
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("TimerService", $"清除计时状态失败: {ex.Message}");
            }
        }

        #endregion

        /// <summary>
        /// 从未完成记录恢复计时器状态
        /// </summary>
        /// <param name="startTime">记录的开始时间</param>
        /// <param name="elapsedTime">已累计的运行时间</param>
        public void RestoreFromIncompleteRecord(DateTime startTime, double elapsedTime)
        {
            try
            {
                _startTime = startTime;
                _isRunning = true;
                _isPaused = false;
                _pausedDuration = TimeSpan.FromSeconds(elapsedTime);
                _pauseStartTime = DateTime.MinValue;
                
                LogManager.WriteDebugLog("TimerService", $"从记录恢复计时状态: 开始时间={startTime}, 已累计时间={elapsedTime}秒");
                
                // 立即更新显示
                UpdateTimeDisplay();
                
                // 通知UI状态变化
                TimerRunningStateChanged?.Invoke(true);
                TimerPauseStateChanged?.Invoke(false);
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("TimerService", $"恢复计时状态失败: {ex.Message}");
            }
        }
        
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}