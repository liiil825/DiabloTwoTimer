using System;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.Services;

public class TimerService : ITimerService
{
    private readonly IProfileService _profileService;
    private readonly ITimerHistoryService _historyService;
    private readonly IAppSettings _settings;
    private readonly ISceneService _sceneService;
    private readonly IAudioService _audioService;

    private readonly System.Timers.Timer _timer;
    private DateTime _startTime = DateTime.MinValue;
    private TimeSpan _pausedDuration = TimeSpan.Zero;
    private DateTime _pauseStartTime = DateTime.MinValue;
    private TimerStatus _status = TimerStatus.Stopped;

    // 记录暂停前的状态，用于番茄钟休息后恢复
    private TimerStatus _previousStatusBeforePause = TimerStatus.Stopped;

    public TimerService(
        IProfileService profileService,
        ITimerHistoryService historyService,
        IAppSettings settings,
        ISceneService sceneService,
        IAudioService audioService
    )
    {
        _profileService = profileService;
        _historyService = historyService;
        _settings = settings;
        _sceneService = sceneService;
        _audioService = audioService;

        _timer = new System.Timers.Timer(15);
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

    #region Properties
    public bool IsRunning => _status == TimerStatus.Running;
    public bool IsPaused => _status == TimerStatus.Paused;
    public bool IsStopped => _status == TimerStatus.Stopped;
    public TimerStatus Status => _status;
    public DateTime StartTime => _startTime;
    public TimeSpan PausedDuration => _pausedDuration;
    public DateTime PauseStartTime => _pauseStartTime;
    public TimerStatus PreviousStatusBeforePause => _previousStatusBeforePause;
    #endregion

    #region Public Methods

    public void Start()
    {
        if (_profileService.CurrentProfile == null)
        {
            Toast.Warning("当前未选择角色，无法开始计时");
            return;
        }

        if (_status == TimerStatus.Running)
            return;

        _status = TimerStatus.Running;
        _startTime = DateTime.Now;
        _pauseStartTime = DateTime.Now;
        _pausedDuration = TimeSpan.Zero;
        _timer.Start();
        Utils.Toast.Success(Utils.LanguageManager.GetString("TimerStarted"));

        // 播放计时器开始音效
        if (_settings.SoundTimerStartEnabled)
        {
            _audioService.PlaySound(_settings.SoundTimerStart);
        }

        // 创建开始记录
        CreateStartRecord();

        TimerRunningStateChangedEvent?.Invoke(true);
        UpdateTimeDisplay();
    }

    public void CompleteRun(bool autoStartNext = false)
    {
        if (IsStopped)
            return;

        if (_pauseStartTime != DateTime.MinValue)
        {
            _pausedDuration += DateTime.Now - _pauseStartTime;
            _pauseStartTime = DateTime.MinValue;
        }
        _timer.Stop();

        if (_startTime != DateTime.MinValue)
        {
            TimeSpan runTime = GetElapsedTime();
            RunCompletedEvent?.Invoke(runTime);

            // 保存最终记录
            SaveToProfile();
        }

        TimerRunningStateChangedEvent?.Invoke(false);
        _status = TimerStatus.Stopped;

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

    public void StartOrNextRun()
    {
        if (IsStopped || IsPaused)
            Start();
        else
            CompleteRun(true);
    }

    public void TogglePause()
    {
        if (IsRunning)
            Pause();
        else if (IsPaused)
            Resume();
    }

    public void Pause()
    {
        if (!IsRunning)
            return;

        _previousStatusBeforePause = _status;
        _status = TimerStatus.Paused;

        _pausedDuration += DateTime.Now - _pauseStartTime;
        _pauseStartTime = DateTime.MinValue;

        _timer.Stop();
        Utils.Toast.Warning(Utils.LanguageManager.GetString("TimerPaused"));

        // 播放计时器暂停音效
        if (_settings.SoundTimerPauseEnabled)
        {
            _audioService.PlaySound(_settings.SoundTimerPause);
        }

        UpdateIncompleteRecord();
        UpdateTimeDisplay();
        TimerPauseStateChangedEvent?.Invoke(true);
    }

    public void Resume()
    {
        if (!IsPaused)
            return;

        _status = TimerStatus.Running;
        _pauseStartTime = DateTime.Now;
        _timer.Start();
        Utils.Toast.Info(Utils.LanguageManager.GetString("TimerResumed"));
        // 播放计时器继续音效
        if (_settings.SoundTimerStartEnabled)
        {
            _audioService.PlaySound(_settings.SoundTimerStart);
        }

        TimerPauseStateChangedEvent?.Invoke(false);
        UpdateTimeDisplay();
    }

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

    public void HandleStartFarm()
    {
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

    public TimeSpan GetElapsedTime()
    {
        if (_startTime == DateTime.MinValue)
            return TimeSpan.Zero;
        if (_pauseStartTime == DateTime.MinValue)
            return _pausedDuration;

        TimeSpan elapsed = DateTime.Now - _pauseStartTime + _pausedDuration;
        return elapsed > TimeSpan.Zero ? elapsed : TimeSpan.Zero;
    }

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

    public void HandleApplicationClosing()
    {
        if (IsRunning)
        {
            UpdateIncompleteRecord();
            LogManager.WriteDebugLog("TimerService", "应用程序关闭时保存了计时状态");
        }
    }

    public void RestoreIncompleteRecord()
    {
        OnRestoreIncompleteRecordRequested();
    }

    public void ResetTimerRequested()
    {
        Reset();
        _historyService.ResetHistoryData();
    }

    public void Dispose()
    {
        _profileService.CurrentProfileChangedEvent -= OnProfileChanged;
        _profileService.CurrentSceneChangedEvent -= OnSceneChanged;
        _profileService.CurrentDifficultyChangedEvent -= OnDifficultyChanged;
        _timer.Dispose();
    }

    #endregion

    #region Private Logic Methods

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        UpdateTimeDisplay();
    }

    private void UpdateTimeDisplay()
    {
        string timeString = GetFormattedTime();
        TimeUpdatedEvent?.Invoke(timeString);
    }

    private void CreateStartRecord()
    {
        var currentProfile = _profileService.CurrentProfile;
        string currentScene = _profileService.CurrentScene;

        if (currentProfile == null || string.IsNullOrEmpty(currentScene))
            return;

        int actValue = _sceneService.GetSceneActValue(currentScene);
        string pureEnglishSceneName = _sceneService.GetEnglishSceneName(currentScene);
        var difficulty = _profileService.CurrentDifficulty;

        if (string.IsNullOrEmpty(pureEnglishSceneName))
            pureEnglishSceneName = "UnknownScene";

        // 1. 先更新 Profile 的状态属性
        currentProfile.LastRunScene = pureEnglishSceneName;
        currentProfile.LastRunDifficulty = difficulty;

        // 2. 准备记录数据
        var existingRecord = FindIncompleteRecordForCurrentScene();

        if (existingRecord != null)
        {
            // 更新旧记录
            existingRecord.StartTime = _startTime;
            existingRecord.LatestTime = _startTime;
            existingRecord.DurationSeconds = 0.0;

            // 使用 ProfileService 更新并保存（这也将保存 LastRunScene 的变更）
            _profileService.UpdateRecord(existingRecord);
        }
        else
        {
            // 创建新记录
            var newRecord = new MFRecord
            {
                SceneName = pureEnglishSceneName,
                Difficulty = difficulty,
                StartTime = _startTime,
                EndTime = null,
                LatestTime = _startTime,
                DurationSeconds = 0.0,
            };

            // 使用 ProfileService 添加并保存
            _profileService.AddRecord(newRecord);
        }

        LogManager.WriteDebugLog("TimerService", $"开始记录已处理: {currentProfile.Name} - {pureEnglishSceneName}");
    }

    private void SaveToProfile()
    {
        var currentProfile = _profileService.CurrentProfile;
        string currentScene = _profileService.CurrentScene;

        if (currentProfile == null || string.IsNullOrEmpty(currentScene))
            return;

        int actValue = _sceneService.GetSceneActValue(currentScene);
        var difficulty = _profileService.CurrentDifficulty;
        double durationSeconds = GetElapsedTime().TotalSeconds;
        string pureEnglishSceneName = _sceneService.GetEnglishSceneName(currentScene);

        if (string.IsNullOrEmpty(pureEnglishSceneName))
            pureEnglishSceneName = "UnknownScene";

        // 1. 更新 Profile 状态
        currentProfile.LastRunScene = pureEnglishSceneName;
        currentProfile.LastRunDifficulty = difficulty;

        // 2. 查找并完成记录
        var existingRecord = FindIncompleteRecordForCurrentScene();

        if (existingRecord != null)
        {
            existingRecord.EndTime = DateTime.Now;
            existingRecord.LatestTime = DateTime.Now;
            existingRecord.DurationSeconds = durationSeconds;

            _profileService.UpdateRecord(existingRecord);
            var timeString = TimeSpan.FromSeconds(durationSeconds).ToString("mm\\:ss\\.ff");
            string message = LanguageManager.GetString("TimeRecorded", timeString);
            Toast.Info(message);
        }
        else
        {
            // 理论上不应发生，但作为防御性编程，添加一条新记录
            var newRecord = new MFRecord
            {
                SceneName = pureEnglishSceneName,
                Difficulty = difficulty,
                StartTime = _startTime,
                EndTime = DateTime.Now,
                LatestTime = DateTime.Now,
                DurationSeconds = durationSeconds,
            };
            _profileService.AddRecord(newRecord);
            LogManager.WriteDebugLog("TimerService", "新记录已完成并保存");
        }

        if (_settings.GenerateRoomName)
        {
            GenerateAndCopyRoomName();
        }
    }

    private void UpdateIncompleteRecord(DateTime? updateTime = null)
    {
        var record = FindIncompleteRecordForCurrentScene();
        if (record == null)
            return;

        DateTime now = updateTime ?? DateTime.Now;
        record.DurationSeconds = GetElapsedTime().TotalSeconds;
        record.LatestTime = now;

        // 使用 ProfileService 保存更新
        _profileService.UpdateRecord(record);

        LogManager.WriteDebugLog("TimerService", $"中间状态已更新: {record.DurationSeconds:F1}s");
    }

    private MFRecord? FindIncompleteRecordForCurrentScene()
    {
        if (_profileService.CurrentProfile == null || _profileService.CurrentScene == null)
            return null;

        string pureEnglishSceneName = _sceneService.GetEnglishSceneName(_profileService.CurrentScene);

        return _profileService
            .CurrentProfile.Records.Where(r =>
                r.SceneName == pureEnglishSceneName
                && r.Difficulty == _profileService.CurrentDifficulty
                && !r.IsCompleted
            )
            .OrderByDescending(r => r.StartTime)
            .FirstOrDefault();
    }

    private void OnRestoreIncompleteRecordRequested()
    {
        Reset();
        var record = FindIncompleteRecordForCurrentScene();
        if (record == null)
            return;

        _status = TimerStatus.Paused;
        _startTime = record.StartTime;
        _pauseStartTime = DateTime.MinValue;
        _pausedDuration = TimeSpan.FromSeconds(record.DurationSeconds);

        UpdateTimeDisplay();
        TimerRunningStateChangedEvent?.Invoke(true);
        TimerPauseStateChangedEvent?.Invoke(true);

        LogManager.WriteDebugLog("TimerService", $"已恢复未完成记录: {_pausedDuration.TotalSeconds:F1}s");
    }

    private void OnProfileChanged(CharacterProfile? profile)
    {
        Reset();
        if (profile != null)
            RestoreIncompleteRecord();
    }

    private void OnSceneChanged(string? scene)
    {
        Reset();
        if (scene != null)
            RestoreIncompleteRecord();
    }

    private void OnDifficultyChanged(GameDifficulty difficulty)
    {
        Reset();
        RestoreIncompleteRecord();
    }

    private void GenerateAndCopyRoomName()
    {
        try
        {
            string characterName = _profileService.CurrentProfile?.Name ?? "";
            string sceneName = _profileService.CurrentScene ?? "";
            int runCount = _historyService.RunCount + 1;

            if (string.IsNullOrEmpty(characterName) || string.IsNullOrEmpty(sceneName))
                return;

            string sceneShortName = _sceneService.GetSceneShortName(sceneName, characterName);
            if (string.IsNullOrEmpty(sceneShortName))
                sceneShortName = "UnknownScene";

            string roomName = $"{characterName}{sceneShortName}{runCount}";
            Clipboard.SetText(roomName);
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("TimerService", "生成房间名称错误", ex);
        }
    }

    #endregion
}
