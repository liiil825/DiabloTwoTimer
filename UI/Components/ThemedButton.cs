using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedButton : Button
{
    // 默认圆角半径，8~10 看起来比较现代，如果想要胶囊形状(Pill shape)，可以设为高度的一半
    private int _borderRadius = 8;
    private bool _isHovered = false;
    private bool _isPressed = false;
    private bool _isSelected = false; // 新增字段

    [Category("Appearance")]
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                Invalidate(); // 状态改变时重绘
            }
        }
    }

    public ThemedButton()
    {
        // 1. 关闭系统默认绘制
        this.FlatStyle = FlatStyle.Flat;
        this.FlatAppearance.BorderSize = 0;

        // 2. 基础设置
        this.Size = new Size(120, 40);
        this.Cursor = Cursors.Hand;
        this.Font = AppTheme.MainFont;
        this.AutoSize = true;
        this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

        // 其他原本的初始化代码...
        this.FlatStyle = FlatStyle.Flat;
        this.FlatAppearance.BorderSize = 0;

        // 3. 关键：支持透明背景，这样圆角外面的四个角才能透出父容器的颜色
        this.BackColor = Color.Transparent;

        // 4. 开启双缓冲和自绘样式
        this.SetStyle(ControlStyles.UserPaint, true);
        this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        this.SetStyle(ControlStyles.ResizeRedraw, true);
        this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
    }

    // 暴露属性，方便在设计器里调整圆角大小
    [Category("Appearance")]
    [Description("圆角半径大小")]
    public int BorderRadius
    {
        get => _borderRadius;
        set
        {
            _borderRadius = value;
            Invalidate();
        }
    }

    // 核心逻辑：告诉 WinForms 这个控件“想要”多大
    public override Size GetPreferredSize(Size proposedSize)
    {
        // 1. 获取基础文字的大小 (比 MeasureString 更准确)
        Size textSize = TextRenderer.MeasureText(this.Text, this.Font);

        // 2. 定义留白 (Padding)
        // 这里直接调用 ScaleHelper，确保 4K 下也是按比例的
        // 比如：上下各加 8px，左右各加 15px
        int verticalPadding = ScaleHelper.Scale(8);
        int horizontalPadding = ScaleHelper.Scale(16);

        // 3. 计算总大小
        int w = textSize.Width + horizontalPadding;
        int h = textSize.Height + verticalPadding;

        // 确保不小于最小尺寸（可选，防止按钮太小点不到）
        // w = Math.Max(w, ScaleHelper.Scale(80)); 

        return new Size(w, h);
    }

    // 可选：为了确保修改文字或字体时，尺寸能立即更新
    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);
        // 通知布局引擎重新计算尺寸
        this.Invalidate();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        this.Invalidate();
    }

    // --- 鼠标事件处理 (用于切换状态) ---
    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _isHovered = true;
        Invalidate(); // 触发重绘
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _isHovered = false;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        _isPressed = true;
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        _isPressed = false;
        Invalidate();
    }

    // --- 核心绘制逻辑 ---
    protected override void OnPaint(PaintEventArgs e)
    {
        if (this.Width <= 1 || this.Height <= 1) return;

        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        // --- 1. 确定颜色逻辑 (修改部分) ---
        Color backColor = AppTheme.SurfaceColor;
        Color borderColor = Color.FromArgb(80, 80, 80);

        // 默认文字颜色
        Color textColor = AppTheme.TextSecondaryColor; // 默认改为灰色，突显选中状态

        if (!Enabled)
        {
            backColor = Color.FromArgb(40, 40, 40);
            textColor = Color.Gray;
        }
        else if (IsSelected) // 【新增】选中状态优先级最高
        {
            // 选中时：背景稍微亮一点，文字变金
            backColor = AppTheme.SurfaceColor;
            textColor = AppTheme.AccentColor;
            borderColor = AppTheme.AccentColor;
        }
        else if (_isPressed)
        {
            backColor = Color.FromArgb(30, 30, 30);
            textColor = AppTheme.AccentColor;
        }
        else if (_isHovered)
        {
            backColor = Color.FromArgb(60, 60, 65); // 悬停：稍亮背景
            borderColor = AppTheme.AccentColor; // 悬停：金边
            textColor = AppTheme.AccentColor; // 悬停：金字
        }

        // --- 2. 绘制背景和边框 ---
        var rect = this.ClientRectangle;
        rect.Width -= 1;
        rect.Height -= 1;

        using (var path = GetRoundedPath(rect, _borderRadius))
        using (var brush = new SolidBrush(backColor))
        using (var pen = new Pen(borderColor, 1))
        {
            g.FillPath(brush, path);
            // 如果选中，就不画四周的框了，只画底部的线，显得更现代
            // 或者保留边框也可以，看你喜好。这里保留边框逻辑。
            if (!IsSelected)
            {
                g.DrawPath(pen, path);
            }
        }

        // --- 3. 【新增】绘制选中状态的金边 (底部线条) ---
        if (IsSelected)
        {
            using (var pen = new Pen(AppTheme.AccentColor, 3)) // 线宽 3
            {
                // 在底部画一条线
                int y = this.Height - 2;
                g.DrawLine(pen, 5, y, this.Width - 5, y); // 稍微留点左右边距
            }
        }

        // --- 4. 绘制文字 ---
        var textSize = g.MeasureString(this.Text, this.Font);
        var textX = (this.Width - textSize.Width) / 2;
        var textY = (this.Height - textSize.Height) / 2 + 1;

        using (var textBrush = new SolidBrush(textColor))
        {
            g.DrawString(this.Text, this.Font, textBrush, textX, textY);
        }
    }

    // 辅助方法：生成圆角路径
    private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();

        // 安全检查：如果 rect 无效，直接返回空路径
        if (rect.Width <= 0 || rect.Height <= 0)
            return path;

        int d = radius * 2; // 直径

        // 简单的防崩溃检查，防止圆角大于按钮尺寸
        if (d > rect.Width)
            d = rect.Width;
        if (d > rect.Height)
            d = rect.Height;

        // 如果 d 计算出来仍然 <= 0 (理论上前面 rect 检查已过滤，双重保险)，设为 1
        if (d <= 0)
            d = 1;

        // 顺时针添加四段圆弧
        path.AddArc(rect.X, rect.Y, d, d, 180, 90); // 左上
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90); // 右上
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90); // 右下
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90); // 左下
        path.CloseFigure();
        return path;
    }

    // 禁用焦点虚线框
    protected override bool ShowFocusCues => false;
}
