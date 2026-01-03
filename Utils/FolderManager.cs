using System;
using System.IO;

namespace DiabloTwoMFTimer.Utils;

public static class FolderManager
{
    // --- 基础路径 ---
    public static string AppDataPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DiabloTwoMFTimer");

    // --- 子目录路径 ---
    public static string ProfilesPath => Path.Combine(AppDataPath, "profiles");
    public static string ScreenshotsPath => Path.Combine(AppDataPath, "screenshots");

    // 音频数据目录 (用户存放/修改音频的地方)
    public static string AudioPath => Path.Combine(AppDataPath, "audio");

    // 默认资源目录 (程序自带音频的源目录，通常在运行目录的 Resources/Audio)
    public static string ResourceAudioPath =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Audio");

    // --- 配置文件路径 ---
    public static string KeyMapConfigPath => Path.Combine(AppDataPath, "key_maps.yaml");
    public static string ConfigFilePath => Path.Combine(AppDataPath, "config.yaml");
    public static string SceneFilePath => Path.Combine(AppDataPath, "scene.yaml");

    // --- 辅助方法 ---

    /// <summary>
    /// 获取音频文件的完整路径
    /// </summary>
    public static string GetAudioFilePath(string fileName)
    {
        return Path.Combine(AudioPath, fileName);
    }

    public static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public static void Initialize()
    {
        EnsureDirectoryExists(AppDataPath);
        EnsureDirectoryExists(ProfilesPath);
        EnsureDirectoryExists(ScreenshotsPath);
        EnsureDirectoryExists(AudioPath); // [新增] 初始化音频目录
    }
}