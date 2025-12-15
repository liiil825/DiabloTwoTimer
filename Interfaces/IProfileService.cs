using System;
using System.Collections.Generic;
using DiabloTwoMFTimer.Models;

namespace DiabloTwoMFTimer.Interfaces;

public interface IProfileService
{
    event Action<CharacterProfile?>? CurrentProfileChangedEvent;
    event Action<string>? CurrentSceneChangedEvent;
    event Action<GameDifficulty>? CurrentDifficultyChangedEvent;
    event Action? ProfileListChangedEvent;

    CharacterProfile? CurrentProfile { get; set; }
    string CurrentScene { get; set; }
    GameDifficulty CurrentDifficulty { get; set; }
    string CurrentDifficultyLocalized { get; }
    List<FarmingScene> FarmingScenes { get; }

    void LoadFarmingScenes();
    CharacterProfile? CreateCharacter(string characterName, CharacterClass characterClass);
    bool SwitchCharacter(CharacterProfile profile);
    List<string> GetAllProfileNames();
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

    /// <summary>
    /// 根据shortEnName切换场景
    /// </summary>
    /// <param name="shortEnName">场景的英文短名称</param>
    /// <returns>是否成功切换场景</returns>
    bool SwitchSceneByShortEnName(string shortEnName);

    void AddRecord(MFRecord record);

    void UpdateRecord(MFRecord record);

    void SaveCurrentProfile();
}
