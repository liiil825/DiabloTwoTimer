using System;

namespace DiabloTwoMFTimer.UI;

/// <summary>
/// UI尺寸常量类，集中管理所有UI组件的宽度和高度
/// 便于维护和统一修改
/// </summary>
public static class UISizeConstants
{
    #region MainForm 尺寸常量
    /// <summary>
    /// MainForm 默认宽度
    /// </summary>
    public const int ClientWidth = 480;

    /// <summary>
    /// MainForm 默认高度
    /// </summary>
    public const int ClientHeightWithoutLoot = 650;

    /// <summary>
    /// MainForm 显示掉落记录时的高度
    /// </summary>
    public const int ClientHeightWithLoot = 730;
    #endregion
}
