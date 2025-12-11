using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.Utils;

public static class ScreenshotHelper
{
    /// <summary>
    /// 截取主屏幕并保存
    /// </summary>
    /// <param name="lootName">掉落物品名称</param>
    /// <returns>保存的文件完整路径，失败返回 null</returns>
    public static string? CaptureAndSave(string lootName)
    {
        try
        {
            // 1. 使用 FolderManager 获取路径并确保存在
            string folderPath = FolderManager.ScreenshotsPath;
            FolderManager.EnsureDirectoryExists(folderPath);

            // 2. 处理文件名非法字符
            string safeLootName = string.Join("_", lootName.Split(Path.GetInvalidFileNameChars()));
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"{safeLootName}_{timestamp}.png";
            string fullPath = Path.Combine(folderPath, fileName);

            // 3. 执行截屏 (主屏幕)
            Rectangle bounds = Screen.PrimaryScreen!.Bounds;

            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                bitmap.Save(fullPath, ImageFormat.Png);
            }

            LogManager.WriteDebugLog("ScreenshotHelper", $"截图已保存: {fullPath}");
            return fullPath;
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("ScreenshotHelper", "截图失败", ex);
            return null;
        }
    }
}