using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components
{
    public class ThemedTabControl : TabControl
    {
        public ThemedTabControl()
        {
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.Padding = new Point(24, 8); // 增加一点间距
            this.SizeMode = TabSizeMode.Fixed; // 配合 ItemSize 使用效果更好
            this.ItemSize = new Size(100, 40); // 统一 Tab 大小
        }

        // // 1. 解决鼠标变手型的问题
        // protected override void OnMouseMove(MouseEventArgs e)
        // {
        //     base.OnMouseMove(e);
        //     // 检查鼠标是否在任何一个 Tab 的矩形范围内
        //     bool onTab = false;
        //     for (int i = 0; i < this.TabCount; i++)
        //     {
        //         if (this.GetTabRect(i).Contains(e.Location))
        //         {
        //             onTab = true;
        //             break;
        //         }
        //     }
        //     this.Cursor = onTab ? Cursors.Hand : Cursors.Default;
        // }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            var g = e.Graphics;
            var tabRect = this.GetTabRect(e.Index);
            var page = this.TabPages[e.Index];
            bool isSelected = (this.SelectedIndex == e.Index);

            // 优化：背景绘制
            // 选中时用稍亮的背景，未选中用深色背景，而不是用刺眼的金色填满
            Color backColor = isSelected ? AppTheme.SurfaceColor : AppTheme.BackColor;
            using (var brush = new SolidBrush(backColor))
            {
                g.FillRectangle(brush, tabRect);
            }

            // 优化：选中时底部画一条金线，比全填充更好看
            if (isSelected)
            {
                using (var pen = new Pen(AppTheme.AccentColor, 3))
                {
                    g.DrawLine(pen, tabRect.Left, tabRect.Bottom - 2, tabRect.Right, tabRect.Bottom - 2);
                }
            }

            // 优化：文字绘制
            string text = page.Text;
            var textSize = g.MeasureString(text, this.Font);

            // 选中时文字变金，未选中时文字灰白
            Color textColor = isSelected ? AppTheme.AccentColor : AppTheme.TextSecondaryColor;

            using (var textBrush = new SolidBrush(textColor))
            {
                float x = tabRect.X + (tabRect.Width - textSize.Width) / 2;
                float y = tabRect.Y + (tabRect.Height - textSize.Height) / 2;
                g.DrawString(text, this.Font, textBrush, x, y);
            }
        }
    }
}
