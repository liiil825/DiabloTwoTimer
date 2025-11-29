using System.Drawing;

namespace DiabloTwoMFTimer.UI.Theme;

public static class AppTheme
{
    // 背景色 (深灰)
    public static Color BackColor = Color.FromArgb(32, 32, 32);
    // 容器背景色 (稍亮)
    public static Color SurfaceColor = Color.FromArgb(45, 45, 48);
    // 强调色 (暗黑金)
    public static Color AccentColor = Color.FromArgb(199, 179, 119);
        // 文本颜色
    public static Color TextColor = Color.FromArgb(240, 240, 240);
    // 次要文本
    public static Color TextSecondaryColor = Color.FromArgb(160, 160, 160);
    // 边框颜色
    public static Color BorderColor = Color.FromArgb(60, 60, 60);

        // 字体
    public static Font MainFont = new Font("微软雅黑", 10F, FontStyle.Regular);
    public static Font TitleFont = new Font("微软雅黑", 14F, FontStyle.Bold);
}
