using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedButton : Button
{
    // 默认圆角半径，8~10 看起来比较现代，如果想要胶囊形状(Pill shape)，可以设为高度的一半
    private int _borderRadius = 8;
    private bool _isHovered = false;
    private bool _isPressed = false;

    public ThemedButton()
    {
        // 1. 关闭系统默认绘制
        this.FlatStyle = FlatStyle.Flat;
        this.FlatAppearance.BorderSize = 0;

        // 2. 基础设置
        this.Size = new Size(120, 40);
        this.Cursor = Cursors.Hand;
        this.Font = AppTheme.MainFont;

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
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias; // 开启抗锯齿，圆角才圆润

        // 1. 确定颜色 (沿用之前的配色逻辑)
        Color backColor = AppTheme.SurfaceColor;
        Color borderColor = Color.FromArgb(80, 80, 80);
        Color textColor = AppTheme.TextColor;

        if (!Enabled)
        {
            backColor = Color.FromArgb(40, 40, 40);
            borderColor = Color.FromArgb(60, 60, 60);
            textColor = Color.Gray;
        }
        else if (_isPressed)
        {
            backColor = Color.FromArgb(30, 30, 30);
            borderColor = AppTheme.AccentColor; // 按下：金边
            textColor = AppTheme.AccentColor; // 按下：金字
        }
        else if (_isHovered)
        {
            backColor = Color.FromArgb(60, 60, 65); // 悬停：稍亮背景
            borderColor = AppTheme.AccentColor; // 悬停：金边
            textColor = AppTheme.AccentColor; // 悬停：金字
        }

        // 2. 准备绘制区域
        // 注意：Rect 需要减去 1，否则边缘会被切掉一点
        var rect = this.ClientRectangle;
        rect.Width -= 1;
        rect.Height -= 1;

        // 3. 绘制背景和边框
        using (var path = GetRoundedPath(rect, _borderRadius))
        using (var brush = new SolidBrush(backColor))
        using (var pen = new Pen(borderColor, 1)) // 边框宽度 1
        {
            g.FillPath(brush, path);
            g.DrawPath(pen, path);
        }

        // 4. 绘制文字 (居中)
        // 使用 MeasureString 确保精准居中
        var textSize = g.MeasureString(this.Text, this.Font);
        var textX = (this.Width - textSize.Width) / 2;
        var textY = (this.Height - textSize.Height) / 2 + 1; // +1 微调视觉重心

        using (var textBrush = new SolidBrush(textColor))
        {
            g.DrawString(this.Text, this.Font, textBrush, textX, textY);
        }
    }

    // 辅助方法：生成圆角路径
    private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int d = radius * 2; // 直径

        // 简单的防崩溃检查，防止圆角大于按钮尺寸
        if (d > rect.Width)
            d = rect.Width;
        if (d > rect.Height)
            d = rect.Height;

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
