using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedListBox : ListBox
{
    public ThemedListBox()
    {
        this.BackColor = AppTheme.SurfaceColor;
        this.ForeColor = AppTheme.TextColor;
        this.BorderStyle = BorderStyle.FixedSingle;
        this.Font = AppTheme.MainFont;
        this.DrawMode = DrawMode.OwnerDrawFixed;

        // 移除写死的 24，改为计算
        RecalculateItemHeight();
    }

    // 监听字体变化
    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        RecalculateItemHeight();
    }

    private void RecalculateItemHeight()
    {
        // 字体高度 + 上下各 6px 的 Padding (看起来比较舒适)
        // 也可以使用 ScaleHelper.Scale(24) 但基于字体更准确
        this.ItemHeight = (int)(this.Font.Height * 1.2);
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index < 0)
            return;

        e.DrawBackground();
        bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

        // 背景色
        Color backColor = isSelected ? AppTheme.AccentColor : AppTheme.SurfaceColor;
        using (var brush = new SolidBrush(backColor))
        {
            e.Graphics.FillRectangle(brush, e.Bounds);
        }

        // 文本色
        Color textColor = isSelected ? Color.Black : AppTheme.TextColor;

        // 安全获取文本
        string text = "";
        if (this.Items[e.Index] != null)
        {
            // 优先使用 DisplayMember
            if (!string.IsNullOrEmpty(this.DisplayMember))
            {
                // 这里简单处理，实际复杂对象可能需要反射，但 ToString 通常足够
                // 如果绑定了 DataSource，需更复杂处理，但在本项目中 Items.Add 使用 ToString 即可
                text = this.GetItemText(this.Items[e.Index]) ?? "";
            }
            else
            {
                text = this.Items[e.Index].ToString() ?? "";
            }
        }

        using (var brush = new SolidBrush(textColor))
        {
            // 垂直居中
            float y = e.Bounds.Y + (e.Bounds.Height - e.Font!.Height) / 2;
            e.Graphics.DrawString(text, e.Font, brush, e.Bounds.X + 5, y);
        }
    }
}