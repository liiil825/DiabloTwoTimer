using System;
using System.Collections.Generic;
using System.Linq;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.Services;

public class ProfileService : IProfileService
{
    private readonly IAppSettings _appSettings;
    private readonly IProfileRepository _repository; // 新增：数据仓库依赖

    private readonly ISceneService _sceneService;

    // 构造函数注入 Repository
    public ProfileService(IAppSettings appSettings, IProfileRepository repository, ISceneService sceneService)
    {
        _appSettings = appSettings;
        _repository = repository;
        _sceneService = sceneService;

        LoadFarmingScenes(); // 确保场景数据已加载
        LoadLastUsedProfile(); // 加载上次使用的角色
    }

    #region Events for UI Communication
    public event Action<CharacterProfile?>? CurrentProfileChangedEvent;
    public event Action<string>? CurrentSceneChangedEvent;
    public event Action<GameDifficulty>? CurrentDifficultyChangedEvent;
    public event Action? ProfileListChangedEvent;
    #endregion

    #region Properties
    private CharacterProfile? _currentProfile;
    public CharacterProfile? CurrentProfile
    {
        get => _currentProfile;
        set
        {
            if (_currentProfile != value)
            {
                _currentProfile = value;
                CurrentProfileChangedEvent?.Invoke(value);

                // 保存到设置
                if (value != null)
                {
                    _appSettings.LastUsedProfile = value.Name;
                    _appSettings.Save();
                }
            }
        }
    }

    private string _currentScene = string.Empty;
    public string CurrentScene
    {
        get => LanguageManager.GetString(_currentScene); // 注意：这里可能需要确认是否还要翻译，建议保持原逻辑
        set
        {
            // 确保保存时使用英文场景名称
            string englishSceneName = _sceneService.GetEnglishSceneName(value);

            if (_currentScene != englishSceneName)
            {
                _currentScene = englishSceneName;
                CurrentSceneChangedEvent?.Invoke(englishSceneName);

                // 保存到设置
                _appSettings.LastRunScene = englishSceneName;
                _appSettings.Save();
            }
        }
    }

    private GameDifficulty _currentDifficulty = GameDifficulty.Hell;
    public GameDifficulty CurrentDifficulty
    {
        get => _currentDifficulty;
        set
        {
            if (_currentDifficulty != value)
            {
                _currentDifficulty = value;
                CurrentDifficultyChangedEvent?.Invoke(value);

                // 保存到设置
                _appSettings.LastUsedDifficulty = value.ToString();
                _appSettings.Save();
            }
        }
    }

    public string CurrentDifficultyLocalized => Utils.LanguageManager.GetString($"Difficulty{_currentDifficulty}");

    public List<FarmingScene> FarmingScenes { get; private set; } = [];
    #endregion

    #region Public Methods
    /// <summary>
    /// 加载所有耕作场景
    /// </summary>
    public void LoadFarmingScenes()
    {
        FarmingScenes = _sceneService.FarmingScenes;
        // 加载上次使用的场景
        LoadLastRunScene();
    }

    /// <summary>
    /// 创建新角色
    /// </summary>
    public CharacterProfile? CreateCharacter(string characterName, CharacterClass characterClass)
    {
        try
        {
            LogManager.WriteDebugLog("ProfileService", $"开始创建新角色: {characterName}, 职业: {characterClass}");

            // 1. 验证输入
            if (string.IsNullOrWhiteSpace(characterName))
            {
                throw new ArgumentException("角色名称不能为空");
            }

            // 2. 检查重名 (使用 Repository)
            if (_repository.GetByName(characterName) != null)
            {
                throw new InvalidOperationException($"角色 '{characterName}' 已存在");
            }

            // 3. 创建对象
            var profile = new CharacterProfile
            {
                Name = characterName,
                Class = characterClass,
                Records = [],
                LootRecords = [],
            };

            // 4. 保存 (使用 Repository)
            _repository.Save(profile);

            LogManager.WriteDebugLog("ProfileService", $"角色创建成功并已保存: {profile.Name}");

            // 5. 设置为当前角色
            CurrentProfile = profile;
            ProfileListChangedEvent?.Invoke();

            return profile;
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("ProfileService", $"创建角色失败", ex);
            // 这里可以选择抛出异常让 UI 层捕获并弹窗
            return null;
        }
    }

    /// <summary>
    /// 切换角色
    /// </summary>
    public bool SwitchCharacter(CharacterProfile profile)
    {
        LogManager.WriteDebugLog("ProfileService", $"开始切换角色到: {profile.Name}");

        if (profile == null || string.IsNullOrWhiteSpace(profile.Name))
        {
            LogManager.WriteDebugLog("ProfileService", "无效的角色数据");
            return false;
        }

        CurrentProfile = profile;
        LogManager.WriteDebugLog("ProfileService", $"成功切换到角色: {profile.Name}");
        return true;
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    public bool DeleteCharacter(CharacterProfile profile)
    {
        LogManager.WriteDebugLog("ProfileService", $"开始删除角色: {profile.Name}");

        // 使用 Repository 删除
        _repository.Delete(profile);

        // 如果删除的是当前角色，清空当前角色
        if (CurrentProfile?.Name == profile.Name)
        {
            CurrentProfile = null;
        }

        ProfileListChangedEvent?.Invoke();
        LogManager.WriteDebugLog("ProfileService", $"成功删除角色: {profile.Name}");

        return true;
    }

    /// <summary>
    /// 获取所有角色档案
    /// </summary>
    public List<CharacterProfile> GetAllProfiles()
    {
        return _repository.GetAll();
    }

    /// <summary>
    /// 根据名称查找角色档案
    /// </summary>
    public CharacterProfile? FindProfileByName(string name)
    {
        return _repository.GetByName(name);
    }

    /// <summary>
    /// 获取所有角色档案名称
    /// </summary>
    public List<string> GetAllProfileNames()
    {
        return _repository.GetAllNames();
    }

    /// <summary>
    /// 检查当前场景和难度是否有未完成记录
    /// </summary>
    public bool HasIncompleteRecord()
    {
        if (CurrentProfile == null || string.IsNullOrEmpty(CurrentScene))
            return false;

        string pureEnglishSceneName = _sceneService.GetEnglishSceneName(CurrentScene);

        // 注意：这里我们假设 CurrentProfile 已经在内存中是最新的
        // 如果是多开程序，可能需要从 Repository 重新加载
        return CurrentProfile.Records.Any(r =>
            r.SceneName == pureEnglishSceneName && r.Difficulty == CurrentDifficulty && !r.IsCompleted
        );
    }

    /// <summary>
    /// 获取开始按钮的显示文本
    /// </summary>
    public string GetStartButtonText()
    {
        bool hasIncompleteRecord = HasIncompleteRecord();
        string key = hasIncompleteRecord ? "ContinueFarm" : "StartTimer";
        return LanguageManager.GetString(key);
    }

    /// <summary>
    /// 获取场景显示名称列表
    /// </summary>
    public List<string> GetSceneDisplayNames()
    {
        return FarmingScenes.Select(scene => _sceneService.GetSceneDisplayName(scene)).ToList();
    }

    /// <summary>
    /// 根据显示名称获取场景对象
    /// </summary>
    public FarmingScene? GetSceneByDisplayName(string displayName)
    {
        return FarmingScenes.FirstOrDefault(scene => _sceneService.GetSceneDisplayName(scene) == displayName);
    }

    /// <summary>
    /// 获取本地化的难度名称列表
    /// </summary>
    public List<string> GetLocalizedDifficultyNames()
    {
        return Enum.GetValues(typeof(GameDifficulty))
            .Cast<GameDifficulty>()
            .Select(d => _sceneService.GetLocalizedDifficultyName(d))
            .ToList();
    }

    /// <summary>
    /// 根据索引获取难度
    /// </summary>
    public GameDifficulty GetDifficultyByIndex(int index)
    {
        var difficulties = Enum.GetValues(typeof(GameDifficulty)).Cast<GameDifficulty>().ToList();
        return index >= 0 && index < difficulties.Count ? difficulties[index] : GameDifficulty.Hell;
    }

    /// <summary>
    /// 根据难度获取索引
    /// </summary>
    public int GetDifficultyIndex(GameDifficulty difficulty)
    {
        var difficulties = Enum.GetValues(typeof(GameDifficulty)).Cast<GameDifficulty>().ToList();
        return difficulties.IndexOf(difficulty);
    }

    /// <summary>
    /// 根据shortEnName切换场景
    /// </summary>
    /// <param name="shortEnName">场景的英文短名称</param>
    /// <returns>是否成功切换场景</returns>
    public bool SwitchSceneByShortEnName(string shortEnName)
    {
        if (string.IsNullOrWhiteSpace(shortEnName))
            return false;

        // 根据shortEnName查找场景
        var scene = _sceneService.GetSceneByShortEnName(shortEnName);
        if (scene == null)
        {
            Utils.Toast.Error($"未找到场景: {shortEnName}");
            return false;
        }

        // 更新当前场景
        CurrentScene = scene.EnUS;
        Utils.Toast.Success($"已切换到场景: {scene.GetSceneName(scene.EnUS)}");
        return true;
    }

    #endregion

    #region Record Management

    /// <summary>
    /// 添加一条新的 MF 记录并保存
    /// </summary>
    public void AddRecord(MFRecord record)
    {
        if (CurrentProfile == null)
            return;

        // 1. 修改内存数据
        CurrentProfile.Records.Add(record);

        // 2. 持久化保存
        _repository.Save(CurrentProfile);

        // 3. 记录日志 (可选)
        LogManager.WriteDebugLog("ProfileService", $"已添加新记录: 场景={record.SceneName}, 时间={record.StartTime}");
    }

    /// <summary>
    /// 更新现有的 MF 记录并保存
    /// </summary>
    public void UpdateRecord(MFRecord record)
    {
        if (CurrentProfile == null)
            return;

        // 逻辑来自原来的 DataHelper.UpdateMFRecord
        // 查找内存中对应的记录（通常是同一个引用，但在某些情况下为了安全可以重新查找）
        var existingRecord = CurrentProfile.Records.FirstOrDefault(r =>
            r.StartTime == record.StartTime
            && r.SceneName == record.SceneName
            && r.Difficulty == record.Difficulty
            && r.IsCompleted == false
        );

        if (existingRecord != null)
        {
            // 更新属性
            existingRecord.EndTime = record.EndTime;
            existingRecord.LatestTime = record.LatestTime;
            existingRecord.DurationSeconds = record.DurationSeconds;
            // 如果有其他需要更新的字段也在这里赋值

            // 保存更改
            _repository.Save(CurrentProfile);

            LogManager.WriteDebugLog(
                "ProfileService",
                $"已更新记录: 场景={record.SceneName}, 持续时间={record.DurationSeconds}"
            );
        }
        else
        {
            // 如果没找到，可能是逻辑错误，或者应该作为新记录添加？
            // 保持原 DataHelper 逻辑：如果找不到就不保存，或者你可以选择在这里抛出警告
            LogManager.WriteDebugLog("ProfileService", "尝试更新记录但未找到匹配项");
        }
    }

    /// <summary>
    /// 手动保存当前角色状态
    /// </summary>
    public void SaveCurrentProfile()
    {
        if (CurrentProfile != null)
        {
            _repository.Save(CurrentProfile);
        }
    }

    #endregion

    #region Private Methods
    /// <summary>
    /// 加载上次使用的角色档案
    /// </summary>
    private void LoadLastUsedProfile()
    {
        string lastUsedProfileName = _appSettings.LastUsedProfile;
        if (!string.IsNullOrWhiteSpace(lastUsedProfileName))
        {
            // 使用 Repository 加载
            var profile = _repository.GetByName(lastUsedProfileName);
            if (profile != null)
            {
                CurrentProfile = profile;
            }
        }
    }

    /// <summary>
    /// 加载上次使用的场景
    /// </summary>
    private void LoadLastRunScene()
    {
        if (CurrentProfile != null && !string.IsNullOrEmpty(CurrentProfile.LastRunScene))
        {
            CurrentScene = CurrentProfile.LastRunScene;
        }
        else if (!string.IsNullOrEmpty(_appSettings.LastRunScene))
        {
            CurrentScene = _appSettings.LastRunScene;
        }

        if (
            !string.IsNullOrEmpty(_appSettings.LastUsedDifficulty)
            && Enum.TryParse<GameDifficulty>(_appSettings.LastUsedDifficulty, out var difficulty)
        )
        {
            CurrentDifficulty = difficulty;
        }
    }
    #endregion
}
