using System;
using System.Windows.Forms;
using System.Drawing;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedButton : Button
{
    public ThemedButton()
    {
        this.FlatStyle = FlatStyle.Flat;
        this.FlatAppearance.BorderSize = 1;
        this.FlatAppearance.BorderColor = AppTheme.AccentColor;
        this.BackColor = AppTheme.SurfaceColor;
        this.ForeColor = AppTheme.TextColor;
        this.Font = AppTheme.MainFont;
        this.Cursor = Cursors.Hand;
        this.Size = new Size(120, 40); // 默认大小，布局中会被拉伸
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        this.BackColor = AppTheme.AccentColor;
        this.ForeColor = Color.Black; // 悬停时字变黑
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        this.BackColor = AppTheme.SurfaceColor;
        this.ForeColor = AppTheme.TextColor;
    }
}