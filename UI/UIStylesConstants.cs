using System;
using System.Drawing;
using System.Windows.Forms;

namespace DTwoMFTimerHelper.UI
{
    /// <summary>
    /// UI样式常量类，集中管理所有UI组件的字体、颜色和样式
    /// 便于维护和统一修改
    /// </summary>
    public static class UIStylesConstants
    {
        #region 字体设置
        /// <summary>
        /// 主字体
        /// </summary>
        public static readonly Font MainFont = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);

        /// <summary>
        /// 标题字体
        /// </summary>
        public static readonly Font TitleFont = new Font("Microsoft YaHei UI", 16F, FontStyle.Bold, GraphicsUnit.Point, 134);

        /// <summary>
        /// 时间显示字体
        /// </summary>
        public static readonly Font TimeFont = new Font("Microsoft YaHei UI", 32F, FontStyle.Bold, GraphicsUnit.Point, 134);

        /// <summary>
        /// 番茄钟时间显示字体
        /// </summary>
        public static readonly Font PomodoroTimeFont = new Font("Microsoft YaHei UI", 36F, FontStyle.Bold, GraphicsUnit.Point, 134);

        /// <summary>
        /// 按钮字体
        /// </summary>
        public static readonly Font ButtonFont = new Font("Microsoft YaHei UI", 14F, FontStyle.Regular, GraphicsUnit.Point, 134);

        /// <summary>
        /// 小字体
        /// </summary>
        public static readonly Font SmallFont = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 134);
        #endregion

        #region 颜色设置
        /// <summary>
        /// 主背景色
        /// </summary>
        public static readonly Color MainBackgroundColor = Color.FromArgb(245, 245, 245);

        /// <summary>
        /// 面板背景色
        /// </summary>
        public static readonly Color PanelBackgroundColor = Color.FromArgb(240, 240, 240);

        /// <summary>
        /// 控件背景色
        /// </summary>
        public static readonly Color ControlBackgroundColor = Color.White;

        /// <summary>
        /// 主文本色
        /// </summary>
        public static readonly Color MainTextColor = Color.FromArgb(33, 33, 33);

        /// <summary>
        /// 次要文本色
        /// </summary>
        public static readonly Color SecondaryTextColor = Color.FromArgb(68, 68, 68);

        /// <summary>
        /// 边框颜色
        /// </summary>
        public static readonly Color BorderColor = Color.FromArgb(200, 200, 200);

        /// <summary>
        /// 选中颜色
        /// </summary>
        public static readonly Color SelectedColor = Color.FromArgb(0, 120, 215);

        /// <summary>
        /// 开始按钮颜色
        /// </summary>
        public static readonly Color StartButtonColor = Color.FromArgb(0, 120, 215);

        /// <summary>
        /// 设置按钮颜色
        /// </summary>
        public static readonly Color SettingsButtonColor = Color.FromArgb(51, 153, 51);

        /// <summary>
        /// 重置按钮颜色
        /// </summary>
        public static readonly Color ResetButtonColor = Color.FromArgb(230, 74, 25);

        /// <summary>
        /// 番茄钟颜色
        /// </summary>
        public static readonly Color PomodoroColor = Color.FromArgb(230, 74, 25);

        /// <summary>
        /// 运行状态颜色
        /// </summary>
        public static readonly Color RunningStatusColor = Color.Green;

        /// <summary>
        /// 停止状态颜色
        /// </summary>
        public static readonly Color StoppedStatusColor = Color.Red;
        #endregion

        #region 边框样式
        /// <summary>
        /// 控件边框样式
        /// </summary>
        public static readonly BorderStyle ControlBorderStyle = BorderStyle.FixedSingle;

        /// <summary>
        /// 按钮边框样式
        /// </summary>
        public static readonly FlatStyle ButtonFlatStyle = FlatStyle.Flat;

        /// <summary>
        /// 按钮边框大小
        /// </summary>
        public static readonly int ButtonBorderSize = 0;
        #endregion

        #region 布局设置
        /// <summary>
        /// 控件内边距
        /// </summary>
        public static readonly Padding ControlPadding = new Padding(10);

        /// <summary>
        /// 按钮内边距
        /// </summary>
        public static readonly Padding ButtonPadding = new Padding(6);

        /// <summary>
        /// 标签页大小
        /// </summary>
        public static readonly Size TabSize = new Size(200, 40);

        /// <summary>
        /// 状态指示器大小
        /// </summary>
        public static readonly Size StatusIndicatorSize = new Size(24, 24);
        #endregion
    }
}