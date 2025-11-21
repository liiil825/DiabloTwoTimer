using System;
using System.Collections.Generic;
using System.Linq;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.Services {
    public interface IProfileService {
        event Action<CharacterProfile?>? CurrentProfileChangedEvent;
        event Action<string>? CurrentSceneChangedEvent;
        event Action<GameDifficulty>? CurrentDifficultyChangedEvent;
        event Action? ProfileListChangedEvent;

        CharacterProfile? CurrentProfile {
            get; set;
        }
        string CurrentScene {
            get; set;
        }
        GameDifficulty CurrentDifficulty {
            get; set;
        }
        string CurrentDifficultyLocalized {
            get;
        }
        List<FarmingScene> FarmingScenes {
            get;
        }

        void LoadFarmingScenes();
        CharacterProfile? CreateCharacter(string characterName, CharacterClass characterClass);
        bool SwitchCharacter(CharacterProfile profile);
        bool DeleteCharacter(CharacterProfile profile);
        List<CharacterProfile> GetAllProfiles();
        CharacterProfile? FindProfileByName(string name);
        bool HasIncompleteRecord();
        string GetStartButtonText();
        List<string> GetSceneDisplayNames();
        FarmingScene? GetSceneByDisplayName(string displayName);
        List<string> GetLocalizedDifficultyNames();
        GameDifficulty GetDifficultyByIndex(int index);
        int GetDifficultyIndex(GameDifficulty difficulty);
    }

    public class ProfileService : IProfileService {
        public ProfileService() {
            LoadLastUsedProfile();
            LoadLastRunScene();
        }

        #region Events for UI Communication
        public event Action<CharacterProfile?>? CurrentProfileChangedEvent;
        public event Action<string>? CurrentSceneChangedEvent;
        public event Action<GameDifficulty>? CurrentDifficultyChangedEvent;
        public event Action? ProfileListChangedEvent;
        #endregion

        #region Properties
        private CharacterProfile? _currentProfile;
        public CharacterProfile? CurrentProfile {
            get => _currentProfile;
            set {
                if (_currentProfile != value) {
                    _currentProfile = value;
                    CurrentProfileChangedEvent?.Invoke(value);

                    // 保存到设置
                    if (value != null) {
                        var settings = SettingsManager.LoadSettings();
                        settings.LastUsedProfile = value.Name;
                        SettingsManager.SaveSettings(settings);
                    }
                }
            }
        }

        private string _currentScene = string.Empty;
        public string CurrentScene {
            get => LanguageManager.GetString(_currentScene);
            set {
                // 确保保存时使用英文场景名称
                string englishSceneName = SceneService.GetEnglishSceneName(value);

                if (_currentScene != englishSceneName) {
                    _currentScene = englishSceneName;
                    CurrentSceneChangedEvent?.Invoke(englishSceneName);

                    // 保存到设置
                    var settings = SettingsManager.LoadSettings();
                    settings.LastRunScene = englishSceneName;
                    SettingsManager.SaveSettings(settings);
                }
            }
        }

        private GameDifficulty _currentDifficulty = GameDifficulty.Hell;
        public GameDifficulty CurrentDifficulty {
            get => _currentDifficulty;
            set {
                if (_currentDifficulty != value) {
                    _currentDifficulty = value;
                    CurrentDifficultyChangedEvent?.Invoke(value);

                    // 保存到设置
                    var settings = SettingsManager.LoadSettings();
                    settings.LastUsedDifficulty = value.ToString();
                    SettingsManager.SaveSettings(settings);
                }
            }
        }

        public string CurrentDifficultyLocalized =>
             Utils.LanguageManager.GetString($"Difficulty{_currentDifficulty}");

        public List<FarmingScene> FarmingScenes { get; private set; } = [];
        #endregion

        #region Public Methods
        /// <summary>
        /// 加载所有耕作场景
        /// </summary>
        public void LoadFarmingScenes() {
            FarmingScenes = SceneService.LoadFarmingSpots();
            // 加载上次使用的场景
            LoadLastRunScene();
        }

        /// <summary>
        /// 创建新角色
        /// </summary>
        public CharacterProfile? CreateCharacter(string characterName, CharacterClass characterClass) {
            try {
                LogManager.WriteDebugLog("ProfileService", $"开始创建新角色: {characterName}, 职业: {characterClass}");
                var profile = DataService.CreateNewProfile(characterName, characterClass);
                if (profile == null) {
                    LogManager.WriteDebugLog("ProfileService", "创建角色失败，返回的配置文件为null");
                    return null;
                }
                LogManager.WriteDebugLog("ProfileService", $"角色创建成功: {profile.Name}");

                // 设置为当前角色
                CurrentProfile = profile;
                ProfileListChangedEvent?.Invoke();

                return profile;
            }
            catch (Exception ex) {
                LogManager.WriteErrorLog("ProfileService", $"创建角色失败", ex);
                return null;
            }
        }

        /// <summary>
        /// 切换角色
        /// </summary>
        public bool SwitchCharacter(CharacterProfile profile) {
            LogManager.WriteDebugLog("ProfileService", $"开始切换角色到: {profile.Name}");

            // 验证角色数据
            if (profile == null || string.IsNullOrWhiteSpace(profile.Name)) {
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
        public bool DeleteCharacter(CharacterProfile profile) {
            LogManager.WriteDebugLog("ProfileService", $"开始删除角色: {profile.Name}");
            DataService.DeleteProfile(profile);
            // 如果删除的是当前角色，清空当前角色
            if (CurrentProfile?.Name == profile.Name) {
                CurrentProfile = null;
            }

            ProfileListChangedEvent?.Invoke();
            // 触发重置定时器事件
            // _timerService.ResetTimerRequested();
            LogManager.WriteDebugLog("ProfileService", $"成功删除角色: {profile.Name}");

            return true;
        }

        /// <summary>
        /// 获取所有角色档案
        /// </summary>
        public List<CharacterProfile> GetAllProfiles() {
            return DataService.LoadAllProfiles();
        }

        /// <summary>
        /// 根据名称查找角色档案
        /// </summary>
        public CharacterProfile? FindProfileByName(string name) {
            return DataService.FindProfileByName(name);
        }

        /// <summary>
        /// 检查当前场景和难度是否有未完成记录
        /// </summary>
        public bool HasIncompleteRecord() {
            if (CurrentProfile == null || string.IsNullOrEmpty(CurrentScene))
                return false;

            // 获取场景的纯英文名称（与记录存储格式一致）
            string pureEnglishSceneName = LanguageManager.GetPureEnglishSceneName(CurrentScene);
            // 查找同场景、同难度、未完成的记录
            bool hasIncompleteRecord = CurrentProfile.Records.Any(r =>
                r.SceneName == pureEnglishSceneName &&
                r.Difficulty == CurrentDifficulty &&
                !r.IsCompleted);

            LogManager.WriteDebugLog("ProfileService", $"是否存在未完成记录: {hasIncompleteRecord}");
            return hasIncompleteRecord;
        }

        /// <summary>
        /// 获取开始按钮的显示文本
        /// </summary>
        public string GetStartButtonText() {
            bool hasIncompleteRecord = HasIncompleteRecord();
            string key = hasIncompleteRecord ? "ContinueFarm" : "StartTimer";
            return LanguageManager.GetString(key);
        }

        /// <summary>
        /// 获取场景显示名称列表
        /// </summary>
        public List<string> GetSceneDisplayNames() {
            return FarmingScenes.Select(scene => SceneService.GetSceneDisplayName(scene)).ToList();
        }

        /// <summary>
        /// 根据显示名称获取场景对象
        /// </summary>
        public FarmingScene? GetSceneByDisplayName(string displayName) {
            return FarmingScenes.FirstOrDefault(scene =>
                SceneService.GetSceneDisplayName(scene) == displayName);
        }

        /// <summary>
        /// 获取本地化的难度名称列表
        /// </summary>
        public List<string> GetLocalizedDifficultyNames() {
            return [.. Enum.GetValues(typeof(GameDifficulty))
                      .Cast<GameDifficulty>()
                      .Select(d => SceneService.GetLocalizedDifficultyName(d))];
        }

        /// <summary>
        /// 根据索引获取难度
        /// </summary>
        public GameDifficulty GetDifficultyByIndex(int index) {
            var difficulties = Enum.GetValues(typeof(GameDifficulty)).Cast<GameDifficulty>().ToList();
            return index >= 0 && index < difficulties.Count ? difficulties[index] : GameDifficulty.Hell;
        }

        /// <summary>
        /// 根据难度获取索引
        /// </summary>
        public int GetDifficultyIndex(GameDifficulty difficulty) {
            var difficulties = Enum.GetValues(typeof(GameDifficulty)).Cast<GameDifficulty>().ToList();
            return difficulties.IndexOf(difficulty);
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// 加载上次使用的角色档案
        /// </summary>
        private void LoadLastUsedProfile() {
            LogManager.WriteDebugLog("ProfileService", "LoadLastUsedProfile 开始执行");
            var settings = SettingsManager.LoadSettings();
            string lastUsedProfileName = settings.LastUsedProfile;
            LogManager.WriteDebugLog("ProfileService", $"从配置文件加载设置: LastUsedProfile={lastUsedProfileName}");
            if (!string.IsNullOrWhiteSpace(lastUsedProfileName)) {
                LogManager.WriteDebugLog("ProfileService", $"尝试加载上次使用的角色档案: {lastUsedProfileName}");
                var profile = FindProfileByName(lastUsedProfileName);
                if (profile != null) {
                    CurrentProfile = profile;
                    LogManager.WriteDebugLog("ProfileService", $"成功加载上次使用的角色档案: {lastUsedProfileName}");
                }
            }
            else {
                LogManager.WriteDebugLog("ProfileService", "没有保存的上次使用角色档案");
            }
        }

        /// <summary>
        /// 加载上次使用的场景
        /// </summary>
        private void LoadLastRunScene() {
            var settings = SettingsManager.LoadSettings();

            // 优先使用当前角色档案的LastRunScene字段
            if (CurrentProfile != null && !string.IsNullOrEmpty(CurrentProfile.LastRunScene)) {
                CurrentScene = CurrentProfile.LastRunScene;
            }
            // 如果角色档案没有保存场景，则回退到全局设置
            else if (!string.IsNullOrEmpty(settings.LastRunScene)) {
                CurrentScene = settings.LastRunScene;
            }

            // 加载上次使用的难度
            if (!string.IsNullOrEmpty(settings.LastUsedDifficulty) &&
                Enum.TryParse<GameDifficulty>(settings.LastUsedDifficulty, out var difficulty)) {
                CurrentDifficulty = difficulty;
            }
        }
        #endregion
    }
}