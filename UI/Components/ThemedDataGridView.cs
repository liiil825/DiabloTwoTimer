using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedDataGridView : DataGridView
{
    public ThemedDataGridView()
    {
        // 1. 基础外观设置
        this.Dock = DockStyle.Fill;
        this.BackgroundColor = AppTheme.BackColor; // 表格背景色
        this.BorderStyle = BorderStyle.None;
        this.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        this.GridColor = Color.FromArgb(60, 60, 60); // 网格线颜色

        // 2. 行为设置
        this.ColumnHeadersVisible = true;
        this.RowHeadersVisible = false;
        this.AllowUserToAddRows = false;
        this.AllowUserToDeleteRows = false;
        this.AllowUserToResizeRows = false;
        this.ReadOnly = true;
        this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.MultiSelect = false;
        this.VirtualMode = true; // 保持虚拟模式

        // 双缓冲，防止闪烁
        this.DoubleBuffered = true;

        // --- 关键：深色主题样式 ---

        // 3. 表头样式 (必须禁用系统样式才能生效)
        this.EnableHeadersVisualStyles = false;

        DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
        headerStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        headerStyle.BackColor = AppTheme.SurfaceColor; // 表头深灰
        headerStyle.ForeColor = AppTheme.TextSecondaryColor; // 表头文字灰白
        headerStyle.SelectionBackColor = AppTheme.SurfaceColor; // 表头选中不变色
        headerStyle.SelectionForeColor = AppTheme.TextSecondaryColor;
        headerStyle.Font = new Font("微软雅黑", 9F, FontStyle.Bold);
        headerStyle.WrapMode = DataGridViewTriState.True;
        this.ColumnHeadersDefaultCellStyle = headerStyle;
        this.ColumnHeadersHeight = 35;
        this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

        // 4. 单元格（数据行）样式
        DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
        cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        cellStyle.BackColor = AppTheme.BackColor; // 数据行深黑
        cellStyle.ForeColor = AppTheme.TextColor; // 数据文字亮白
        cellStyle.SelectionBackColor = AppTheme.AccentColor; // 选中背景（暗金）
        cellStyle.SelectionForeColor = Color.Black; // 选中文字（黑色）
        cellStyle.Padding = new Padding(5, 0, 0, 0);
        this.DefaultCellStyle = cellStyle;

        // 行高
        this.RowTemplate.Height = 30;
    }

    // 可以在这里重写 OnPaint 做更高级的绘制，但在 DataGridView 中通常不需要
}
