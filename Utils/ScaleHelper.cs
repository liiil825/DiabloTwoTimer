using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;

namespace DiabloTwoMFTimer.Utils;

public static class ScaleHelper
{
    public static float ScaleFactor { get; private set; } = 1.0f;

    public static void Initialize(IAppSettings settings)
    {
        // 1. 优先使用用户手动设置
        if (settings.UiScale > 0.1f)
        {
            ScaleFactor = settings.UiScale;
        }
        else
        {
            int currentScreenHeight = Screen.PrimaryScreen?.Bounds.Height ?? 1080;
            float baseHeight = 1080f;

            // --- 核心修改：非线性计算逻辑 ---
            float rawRatio = currentScreenHeight / baseHeight; // 比如 4K 算出来是 2.0
            if (rawRatio >= 1.9f) // 针对 4K及以上
            {
                // 4K 屏幕不要真的放大 2 倍，而是 1.6 倍，这样看起来更精致，不臃肿
                ScaleFactor = 1.6f;
            }
            else if (rawRatio >= 1.4f) // 针对 2K
            {
                // 2K 屏幕放大 1.25 倍或者 1.3 倍
                ScaleFactor = 1.3f;
            }
            else
            {
                // 1080p 及以下保持 1.0
                ScaleFactor = 1.0f;
            }
        }

        // 安全限制
        if (ScaleFactor < 0.8f) ScaleFactor = 0.8f;
        if (ScaleFactor > 3.0f) ScaleFactor = 3.0f;

        LogManager.WriteDebugLog("ScaleHelper", $"UI Scale: {ScaleFactor}");
    }

    // 控件尺寸依然保持线性缩放，保证布局不乱
    public static int Scale(int value) => (int)(value * ScaleFactor);

    // 【核心修改】字体采用阶梯式缩放 (Stepped Scaling)
    // 目的：在 200% 尺寸下，字体只放大到 160% 左右，避免傻大黑粗
    public static float ScaleFont(float baseSize)
    {
        float fontFactor = 1.0f;

        // 使用 switch (或者 if-else 阶梯) 来决定字体的倍率
        if (ScaleFactor >= 2.1f)
        {
            // 界面放大 2 倍，字体只放大 1.6 倍，留出更多留白，显得精致
            fontFactor = 1.6f;
        }
        else if (ScaleFactor >= 2f)
        {
            fontFactor = 1.4f;
        }
        else if (ScaleFactor >= 1.4f)
        {
            fontFactor = 1.2f;
        }
        else if (ScaleFactor <= 0.9f) // 小屏幕
        {
            fontFactor = 0.85f;
        }
        else // 标准 1080p (1.0 ~ 1.3)
        {
            fontFactor = 1.0f;
        }

        return baseSize * fontFactor;
    }
}