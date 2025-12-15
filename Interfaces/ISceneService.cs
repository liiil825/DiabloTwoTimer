using System.Collections.Generic;
using DiabloTwoMFTimer.Models;

namespace DiabloTwoMFTimer.Interfaces;

public interface ISceneService
{
    List<FarmingScene> FarmingScenes { get; }

    // 加载场景数据
    void LoadFarmingSpots();

    // 各种查找和转换方法
    string GetSceneDisplayName(FarmingScene scene);
    string GetSceneShortName(string sceneName, string characterName = "");
    string GetSceneName(string sceneName, SceneNameType type);
    string GetLocalizedShortSceneName(string sceneName);
    string GetEnglishSceneName(string sceneName);
    int GetSceneActValue(string sceneName);

    // 难度相关
    string GetLocalizedDifficultyName(GameDifficulty difficulty);
    GameDifficulty GetDifficultyByIndex(int index);
    
    // 根据shortEnName查找场景
    FarmingScene? GetSceneByShortEnName(string shortEnName);
}
