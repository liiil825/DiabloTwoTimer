using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedDateTimePicker : DateTimePicker
{
    private const int WM_PAINT = 0xF;

    // 悬停状态
    private bool _isHovered = false;

    public ThemedDateTimePicker()
    {
        // 1. 设置格式
        this.Format = DateTimePickerFormat.Custom;
        this.CustomFormat = "yyyy-MM-dd HH:mm";
        this.Font = new Font("微软雅黑", 10F); // 稍微调大字体

        // 2. 尝试设置下拉日历的颜色 (这部分对下拉框有效)
        this.CalendarMonthBackground = AppTheme.SurfaceColor;
        this.CalendarTitleBackColor = AppTheme.AccentColor;
        this.CalendarTitleForeColor = Color.Black;
        this.CalendarForeColor = AppTheme.TextColor;
        this.CalendarTrailingForeColor = Color.Gray;

        // 3. 启用双缓冲减少闪烁
        this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
    }

    // 鼠标进入/离开用于改变边框颜色
    protected override void OnMouseEnter(EventArgs e)
    {
        _isHovered = true;
        this.Invalidate();
        base.OnMouseEnter(e);
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        _isHovered = false;
        this.Invalidate();
        base.OnMouseLeave(e);
    }

    // 核心：拦截系统消息进行重绘
    protected override void WndProc(ref Message m)
    {
        // 让系统先处理（处理点击、焦点等逻辑）
        base.WndProc(ref m);

        // 处理重绘消息 (WM_PAINT)
        if (m.Msg == WM_PAINT)
        {
            // 获取控件的 Graphics 对象
            // 注意：这里不能使用 e.Graphics，因为这不是 OnPaint 事件
            using (var g = Graphics.FromHwnd(this.Handle))
            {
                PaintCustomControl(g);
            }
        }
    }

    private void PaintCustomControl(Graphics g)
    {
        // 开启抗锯齿
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        // 1. 覆盖背景 (把系统原本白色的背景盖住)
        // 使用 SurfaceColor (深灰)
        using (var bgBrush = new SolidBrush(AppTheme.SurfaceColor))
        {
            g.FillRectangle(bgBrush, 0, 0, this.Width, this.Height);
        }

        // 2. 绘制边框
        Color borderColor = _isHovered ? AppTheme.AccentColor : AppTheme.BorderColor;
        using (var borderPen = new Pen(borderColor))
        {
            // 减1是为了防止画出界
            g.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
        }

        // 3. 绘制下拉图标 (右侧的箭头)
        int iconWidth = 30;
        Rectangle iconRect = new Rectangle(this.Width - iconWidth, 0, iconWidth, this.Height);

        // 绘制图标分割线 (可选)
        // using (var linePen = new Pen(AppTheme.BorderColor))
        // {
        //     g.DrawLine(linePen, iconRect.Left, 0, iconRect.Left, this.Height);
        // }

        // 手绘一个小三角箭头
        int arrowSize = 5;
        Point center = new Point(iconRect.X + iconRect.Width / 2, iconRect.Y + iconRect.Height / 2);
        Point[] arrowPoints = new Point[]
        {
            new Point(center.X - arrowSize, center.Y - 2),
            new Point(center.X + arrowSize, center.Y - 2),
            new Point(center.X, center.Y + 3)
        };

        using (var arrowBrush = new SolidBrush(AppTheme.AccentColor))
        {
            g.FillPolygon(arrowBrush, arrowPoints);
        }

        // 4. 绘制文字 (日期时间)
        // 根据当前的 Value 和 Format 计算要显示的字符串
        string text = this.Value.ToString(this.CustomFormat);

        // 垂直居中
        Rectangle textRect = new Rectangle(5, 0, this.Width - iconWidth - 5, this.Height);

        TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.SingleLine;

        TextRenderer.DrawText(g, text, this.Font, textRect, AppTheme.TextColor, flags);
    }
}