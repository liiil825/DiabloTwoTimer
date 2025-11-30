using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedComboBox : ComboBox
{
    public ThemedComboBox()
    {
        this.DrawMode = DrawMode.OwnerDrawFixed;
        this.DropDownStyle = ComboBoxStyle.DropDownList; // 建议只读，编辑模式很难完美深色化
        this.FlatStyle = FlatStyle.Flat;
        this.BackColor = AppTheme.SurfaceColor;
        this.ForeColor = AppTheme.TextColor;
        this.Font = AppTheme.MainFont;
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index < 0)
            return;

        e.DrawBackground();

        //哪怕是系统绘制背景，我们也要覆盖它以保证深色
        bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

        // 背景色
        var backColor = isSelected ? AppTheme.AccentColor : AppTheme.SurfaceColor;
        using (var brush = new SolidBrush(backColor))
        {
            e.Graphics.FillRectangle(brush, e.Bounds);
        }

        // 文本色
        var textColor = isSelected ? Color.Black : AppTheme.TextColor;
        string text = this.Items[e.Index]?.ToString() ?? "";

        using (var brush = new SolidBrush(textColor))
        {
            // 垂直居中绘制
            float y = e.Bounds.Y + (e.Bounds.Height - e.Font!.Height) / 2;
            e.Graphics.DrawString(text, e.Font, brush, e.Bounds.X + 2, y);
        }

        // 不画虚线框
        // e.DrawFocusRectangle();
    }
}
