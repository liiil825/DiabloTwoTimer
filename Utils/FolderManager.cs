using System;
using System.IO;

namespace DiabloTwoMFTimer.Utils;

public static class FolderManager
{
    // 基础应用数据目录: %AppData%/DiabloTwoMFTimer
    public static string AppDataPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "DiabloTwoMFTimer"
    );

    // 档案目录
    public static string ProfilesPath => Path.Combine(AppDataPath, "profiles");

    // 截图目录 (与档案文件夹平级)
    public static string ScreenshotsPath => Path.Combine(AppDataPath, "screenshots");

    // 配置文件路径
    public static string ConfigFilePath => Path.Combine(AppDataPath, "config.yaml");

    /// <summary>
    /// 确保指定目录存在
    /// </summary>
    public static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// 初始化所有核心目录
    /// </summary>
    public static void Initialize()
    {
        EnsureDirectoryExists(AppDataPath);
        EnsureDirectoryExists(ProfilesPath);
        EnsureDirectoryExists(ScreenshotsPath);
    }
}