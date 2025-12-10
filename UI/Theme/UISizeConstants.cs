using System;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Theme;

/// <summary>
/// UI尺寸常量类，集中管理所有UI组件的宽度和高度
/// </summary>
public static class UISizeConstants
{
    private const int _baseClientWidth = 240;
    private const int _baseClientHeightWithoutLoot = 300;
    private const int _baseClientHeightWithLoot = 340;

    private const int _tabItemWidth = 47;
    private const int _tabItemHeight = 25;

    // 【新增】基础弹窗的默认宽度
    private const int _baseFormWidth = 400;

    public static int TabItemWidth => ScaleHelper.Scale(_tabItemWidth);
    public static int TabItemHeight => ScaleHelper.Scale(_tabItemHeight);

    public static int ClientWidth => ScaleHelper.Scale(_baseClientWidth);
    public static int TabPageHeight => ScaleHelper.Scale(_baseClientHeightWithoutLoot - _tabItemHeight);
    public static int SettingTabPageHeight => ScaleHelper.Scale(_baseClientHeightWithoutLoot - _tabItemHeight * 2);
    public static int ClientHeightWithoutLoot => ScaleHelper.Scale(_baseClientHeightWithoutLoot);
    public static int ClientHeightWithLoot => ScaleHelper.Scale(_baseClientHeightWithLoot);

    // 【新增】获取缩放后的弹窗宽度
    public static int BaseFormWidth => ScaleHelper.Scale(_baseFormWidth);
}