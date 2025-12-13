using System.Drawing;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Theme;

public static class AppTheme
{
    // 背景色 (深灰)
    public static Color BackColor = Color.FromArgb(32, 32, 32);

    public static Color ButtonBackColor { get; } = Color.FromArgb(50, 45, 40);

    // 容器背景色 (稍亮)
    public static Color SurfaceColor = Color.FromArgb(45, 45, 48);

    // 强调色 (暗黑金)
    public static Color AccentColor = Color.FromArgb(199, 179, 119);

    // 套装绿色 (Set Items) -> 用于 Success
    public static Color SetColor = Color.FromArgb(0, 255, 0);

    // 稀有黄色 (Rare Items) -> 用于 Warning
    public static Color RareColor = Color.FromArgb(255, 255, 0);

    // 魔法蓝色 (Magic Items) -> 用于 Info
    // 稍微调亮一点以适应深色背景，原版偏深蓝 (69, 69, 255)
    public static Color MagicColor = Color.FromArgb(100, 100, 255);

    // 错误/生命红色 (Error) -> 用于 Error
    public static Color ErrorColor = Color.FromArgb(255, 80, 80);

    // 文本颜色
    public static Color TextColor = Color.FromArgb(240, 240, 240);

    // 次要文本
    public static Color TextSecondaryColor = Color.FromArgb(160, 160, 160);

    // 边框颜色
    public static Color BorderColor = Color.FromArgb(60, 60, 60);

    // 字体
    public static Font MainFont { get; private set; } = null!;
    public static Font BigFont { get; private set; } = null!;
    public static Font ArialFont { get; private set; } = null!;
    public static Font ConsoleFont { get; private set; } = null!;
    // 小标题字体 (1080p下 10pt)
    public static Font SmallTitleFont { get; private set; } = null!;
    // 大标题字体 (1080p下 20pt)
    public static Font BigTitleFont { get; private set; } = null!;
    public static Font TitleFont { get; private set; } = null!;
    public static Font BigTimeFont { get; private set; } = null!;
    public static Font FullTimeFont { get; private set; } = null!;
    // 普通文本 (1080p下 10pt)
    private const float BaseMainFontSize = 10F;
    // 标题文本 (1080p下 12pt)
    private const float BaseTitleFontSize = 12F;
    private const float BaseBigTitleFontSize = 20F;
    // 计时器大字 (1080p下 36pt，配合 70px 行高)
    private const float BaseBigTimeFontSize = 18F;
    // 完整时间字体 (1080p下 14pt)
    private const float BaseFullTimeFontSize = 36F;
    public static void InitializeFonts()
    {
        MainFont = new Font("微软雅黑", ScaleHelper.ScaleFont(BaseMainFontSize), FontStyle.Regular);
        BigFont = new Font("微软雅黑", ScaleHelper.ScaleFont(BaseBigTitleFontSize), FontStyle.Regular);
        ArialFont = new Font("Arial", ScaleHelper.ScaleFont(BaseMainFontSize), FontStyle.Regular);
        ConsoleFont = new Font("Consolas", ScaleHelper.ScaleFont(BaseMainFontSize), FontStyle.Regular);
        SmallTitleFont = new Font("微软雅黑", ScaleHelper.ScaleFont(BaseMainFontSize), FontStyle.Bold);
        BigTitleFont = new Font("微软雅黑", ScaleHelper.ScaleFont(BaseBigTitleFontSize), FontStyle.Bold);
        TitleFont = new Font("微软雅黑", ScaleHelper.ScaleFont(BaseTitleFontSize), FontStyle.Bold);
        BigTimeFont = new Font("Verdana", ScaleHelper.ScaleFont(BaseBigTimeFontSize), FontStyle.Bold);
        FullTimeFont = new Font("Consolas", BaseFullTimeFontSize, FontStyle.Bold);
    }
}
