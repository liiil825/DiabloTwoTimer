using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedLabel : Label
{
    public bool IsTitle { get; set; } = false;

    public ThemedLabel()
    {
        this.ForeColor = AppTheme.TextColor;
        // 仅在构造函数设为默认值，允许外部覆盖
        this.Font = AppTheme.MainFont;
        this.AutoSize = true;
        this.BackColor = Color.Transparent;
        this.DoubleBuffered = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        // -----------------------------------------------------
        // 【关键修复】删除下面这行强制设置 Font 的代码
        // this.Font = IsTitle ? AppTheme.TitleFont : AppTheme.MainFont;
        // -----------------------------------------------------

        // 如果你希望 IsTitle 属性仍然生效，应该在属性 setter 里改 Font，而不是在 OnPaint 里

        // 提升文字渲染质量
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        base.OnPaint(e);
    }

    // 如果想保留 IsTitle 功能，改写属性：
    /*
    public bool IsTitle
    {
        get => _isTitle;
        set
        {
            _isTitle = value;
            this.Font = value ? AppTheme.TitleFont : AppTheme.MainFont;
        }
    }
    private bool _isTitle;
    */
}
