using System;
using System.Collections.Generic;
using System.Linq;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.Services
{
    public class ProfileService
    {
        #region Singleton Implementation
        private static readonly Lazy<ProfileService> _instance = 
            new Lazy<ProfileService>(() => new ProfileService());
        
        public static ProfileService Instance => _instance.Value;
        
        private ProfileService()
        {
            LoadLastUsedProfile();
        }
        #endregion

        #region Events for UI Communication
        public event Action<CharacterProfile?>? CurrentProfileChanged;
        public event Action<string>? CurrentSceneChanged;
        public event Action<GameDifficulty>? CurrentDifficultyChanged;
        public event Action<bool>? HasIncompleteRecordChanged;
        public event Action? ProfileListChanged;
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
                    CurrentProfileChanged?.Invoke(value);
                    
                    // 保存到设置
                    if (value != null)
                    {
                        var settings = SettingsManager.LoadSettings();
                        settings.LastUsedProfile = value.Name;
                        SettingsManager.SaveSettings(settings);
                    }
                }
            }
        }

        private string _currentScene = string.Empty;
        public string CurrentScene
        {
            get => _currentScene;
            set
            {
                if (_currentScene != value)
                {
                    _currentScene = value;
                    CurrentSceneChanged?.Invoke(value);
                    
                    // 保存到设置
                    var settings = SettingsManager.LoadSettings();
                    settings.LastUsedScene = value;
                    SettingsManager.SaveSettings(settings);
                    
                    // 检查是否有未完成记录
                    CheckIncompleteRecord();
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
                    CurrentDifficultyChanged?.Invoke(value);
                    
                    // 保存到设置
                    var settings = SettingsManager.LoadSettings();
                    settings.LastUsedDifficulty = value.ToString();
                    SettingsManager.SaveSettings(settings);
                    
                    // 检查是否有未完成记录
                    CheckIncompleteRecord();
                }
            }
        }

        public List<FarmingScene> FarmingScenes { get; private set; } = new List<FarmingScene>();
        #endregion

        #region Public Methods
        /// <summary>
        /// 加载所有耕作场景
        /// </summary>
        public void LoadFarmingScenes()
        {
            try
            {
                FarmingScenes = SceneManager.LoadFarmingSpots();
                
                if (FarmingScenes.Count == 0)
                {
                    LogManager.WriteDebugLog("ProfileService", "场景列表为空");
                }
                else
                {
                    LogManager.WriteDebugLog("ProfileService", $"场景列表加载完成，总数: {FarmingScenes.Count}");
                }
                
                // 加载上次使用的场景
                LoadLastUsedScene();
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("ProfileService", $"加载耕作场景失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 创建新角色
        /// </summary>
        public CharacterProfile? CreateCharacter(string characterName, CharacterClass characterClass)
        {
            try
            {
                LogManager.WriteDebugLog("ProfileService", $"开始创建新角色: {characterName}, 职业: {characterClass}");
                
                var profile = DataManager.CreateNewProfile(characterName, characterClass);
                
                if (profile == null)
                {
                    LogManager.WriteDebugLog("ProfileService", "创建角色失败，返回的配置文件为null");
                    return null;
                }

                LogManager.WriteDebugLog("ProfileService", $"角色创建成功: {profile.Name}");
                
                // 设置为当前角色
                CurrentProfile = profile;
                ProfileListChanged?.Invoke();
                
                return profile;
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("ProfileService", $"创建角色失败: {ex.Message}");
                LogManager.WriteDebugLog("ProfileService", $"异常堆栈: {ex.StackTrace}");
                return null;
            }
        }

        /// <summary>
        /// 切换角色
        /// </summary>
        public bool SwitchCharacter(CharacterProfile profile)
        {
            try
            {
                LogManager.WriteDebugLog("ProfileService", $"开始切换角色到: {profile.Name}");
                
                // 验证角色数据
                if (profile == null || string.IsNullOrWhiteSpace(profile.Name))
                {
                    LogManager.WriteDebugLog("ProfileService", "无效的角色数据");
                    return false;
                }

                CurrentProfile = profile;
                LogManager.WriteDebugLog("ProfileService", $"成功切换到角色: {profile.Name}");
                
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("ProfileService", $"切换角色失败: {ex.Message}");
                LogManager.WriteDebugLog("ProfileService", $"异常堆栈: {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        public bool DeleteCharacter(CharacterProfile profile)
        {
            try
            {
                LogManager.WriteDebugLog("ProfileService", $"开始删除角色: {profile.Name}");
                
                DataManager.DeleteProfile(profile);
                
                // 如果删除的是当前角色，清空当前角色
                if (CurrentProfile?.Name == profile.Name)
                {
                    CurrentProfile = null;
                }
                
                ProfileListChanged?.Invoke();
                LogManager.WriteDebugLog("ProfileService", $"成功删除角色: {profile.Name}");
                
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("ProfileService", $"删除角色失败: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 获取所有角色档案
        /// </summary>
        public List<CharacterProfile> GetAllProfiles()
        {
            return DataManager.LoadAllProfiles(false);
        }

        /// <summary>
        /// 根据名称查找角色档案
        /// </summary>
        public CharacterProfile? FindProfileByName(string name)
        {
            return DataManager.FindProfileByName(name);
        }

        /// <summary>
        /// 检查当前场景和难度是否有未完成记录
        /// </summary>
        public bool HasIncompleteRecord()
        {
            if (CurrentProfile == null || string.IsNullOrEmpty(CurrentScene))
                return false;

            try
            {
                LogManager.WriteDebugLog("ProfileService", "开始检查未完成记录");
                LogManager.WriteDebugLog("ProfileService", $"当前角色: {CurrentProfile.Name}");
                LogManager.WriteDebugLog("ProfileService", $"当前场景: {CurrentScene}");
                LogManager.WriteDebugLog("ProfileService", $"当前难度: {CurrentDifficulty}");

                // 获取场景的纯英文名称（与记录存储格式一致）
                string pureEnglishSceneName = LanguageManager.GetPureEnglishSceneName(CurrentScene);
                LogManager.WriteDebugLog("ProfileService", $"纯英文场景名称: {pureEnglishSceneName}");

                // 记录当前配置文件中的记录数量
                LogManager.WriteDebugLog("ProfileService", $"当前角色记录数量: {CurrentProfile.Records.Count}");

                // 查找同场景、同难度、未完成的记录
                bool hasIncompleteRecord = CurrentProfile.Records.Any(r =>
                    r.SceneName == pureEnglishSceneName &&
                    r.Difficulty == CurrentDifficulty &&
                    !r.IsCompleted);

                LogManager.WriteDebugLog("ProfileService", $"是否存在未完成记录: {hasIncompleteRecord}");

                // 记录第一条匹配场景和难度的记录详细信息（用于调试）
                var matchingRecord = CurrentProfile.Records.FirstOrDefault(r =>
                    r.SceneName == pureEnglishSceneName &&
                    r.Difficulty == CurrentDifficulty);
                
                if (matchingRecord != null)
                {
                    LogManager.WriteDebugLog("ProfileService", 
                        $"找到匹配记录 - 场景: {matchingRecord.SceneName}, 难度: {matchingRecord.Difficulty}, 完成状态: {matchingRecord.IsCompleted}");
                }

                return hasIncompleteRecord;
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("ProfileService", $"检查未完成记录时出错: {ex.Message}");
                return false;
            }
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
            return FarmingScenes.Select(scene => SceneManager.GetSceneDisplayName(scene)).ToList();
        }

        /// <summary>
        /// 根据显示名称获取场景对象
        /// </summary>
        public FarmingScene? GetSceneByDisplayName(string displayName)
        {
            return FarmingScenes.FirstOrDefault(scene => 
                SceneManager.GetSceneDisplayName(scene) == displayName);
        }

        /// <summary>
        /// 获取本地化的难度名称列表
        /// </summary>
        public List<string> GetLocalizedDifficultyNames()
        {
            return Enum.GetValues(typeof(GameDifficulty))
                      .Cast<GameDifficulty>()
                      .Select(d => SceneManager.GetLocalizedDifficultyName(d))
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
        #endregion

        #region Private Methods
        /// <summary>
        /// 加载上次使用的角色档案
        /// </summary>
        private void LoadLastUsedProfile()
        {
            LogManager.WriteDebugLog("ProfileService", "LoadLastUsedProfile 开始执行");
            try
            {
                var settings = SettingsManager.LoadSettings();
                string lastUsedProfileName = settings.LastUsedProfile;
                LogManager.WriteDebugLog("ProfileService", $"从配置文件加载设置: LastUsedProfile={lastUsedProfileName}");
                
                if (!string.IsNullOrWhiteSpace(lastUsedProfileName))
                {
                    LogManager.WriteDebugLog("ProfileService", $"尝试加载上次使用的角色档案: {lastUsedProfileName}");
                    
                    var profile = FindProfileByName(lastUsedProfileName);
                    if (profile != null)
                    {
                        CurrentProfile = profile;
                        LogManager.WriteDebugLog("ProfileService", $"成功加载上次使用的角色档案: {lastUsedProfileName}");
                    }
                    else
                    {
                        LogManager.WriteDebugLog("ProfileService", $"未找到上次使用的角色档案: {lastUsedProfileName}");
                    }
                }
                else
                {
                    LogManager.WriteDebugLog("ProfileService", "没有保存的上次使用角色档案");
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("ProfileService", $"加载上次使用角色档案失败: {ex.Message}, 堆栈: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 加载上次使用的场景
        /// </summary>
        private void LoadLastUsedScene()
        {
            try
            {
                var settings = SettingsManager.LoadSettings();
                if (!string.IsNullOrEmpty(settings.LastUsedScene))
                {
                    CurrentScene = settings.LastUsedScene;
                }
                
                // 加载上次使用的难度
                if (!string.IsNullOrEmpty(settings.LastUsedDifficulty) && 
                    Enum.TryParse<GameDifficulty>(settings.LastUsedDifficulty, out var difficulty))
                {
                    CurrentDifficulty = difficulty;
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteDebugLog("ProfileService", $"加载上次使用场景失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 检查未完成记录并触发事件
        /// </summary>
        private void CheckIncompleteRecord()
        {
            bool hasIncompleteRecord = HasIncompleteRecord();
            HasIncompleteRecordChanged?.Invoke(hasIncompleteRecord);
        }
        #endregion
    }
}