using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedLabel : Label
{
    public bool IsTitle { get; set; } = false;

    public ThemedLabel()
    {
        this.ForeColor = AppTheme.TextColor;
        this.Font = AppTheme.MainFont;
        this.AutoSize = true;
        this.BackColor = System.Drawing.Color.Transparent;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        // 根据属性动态调整字体
        this.Font = IsTitle ? AppTheme.TitleFont : AppTheme.MainFont;
        base.OnPaint(e);
    }
}