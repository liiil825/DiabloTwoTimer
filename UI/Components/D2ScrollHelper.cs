using System;
using System.Drawing;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Components;

public static class D2ScrollHelper
{
    /// <summary>
    /// 将 D2 风格滚动条附加到指定的 DataGridView 上
    /// </summary>
    /// <param name="grid">目标表格</param>
    /// <param name="parent">表格所在的父容器（通常是 this）</param>
    public static void Attach(DataGridView grid, Control parent)
    {
        // 防止重复附加
        if (grid.Tag is D2ScrollBar) return;

        // 1. 创建并配置滚动条
        var scrollBar = new D2ScrollBar
        {
            Dock = DockStyle.Right,
            Width = 10,
            // 默认初始 Padding，后续会动态调整
            Padding = new Padding(0, grid.ColumnHeadersHeight, 0, 0)
        };

        // 2. 调整布局顺序 (Z-Order)
        parent.Controls.Add(scrollBar);
        scrollBar.SendToBack(); // 滚动条沉底，占据右侧空间
        grid.BringToFront();    // 表格浮起，填充剩余空间

        // 3. 禁用原生滚动条
        grid.ScrollBars = ScrollBars.None;

        // 4. 存储引用，防止重复附加
        grid.Tag = scrollBar;

        // ==========================================
        // 事件绑定区域
        // ==========================================

        // A. 滚动条 -> 表格
        scrollBar.Scroll += (s, e) =>
        {
            if (grid.RowCount > 0)
            {
                try
                {
                    // 【优化 1】防止越界
                    int targetRow = Math.Min(scrollBar.Value, grid.RowCount - 1);
                    grid.FirstDisplayedScrollingRowIndex = targetRow;
                }
                catch { }
            }
        };

        // B. 表格 -> 滚动条 (反向同步)
        grid.Scroll += (s, e) =>
        {
            if (grid.FirstDisplayedScrollingRowIndex >= 0)
            {
                // 只有当偏差较大时才更新，防止循环触发
                if (Math.Abs(scrollBar.Value - grid.FirstDisplayedScrollingRowIndex) > 0)
                {
                    scrollBar.Value = grid.FirstDisplayedScrollingRowIndex;
                }
            }
        };

        // C. 鼠标滚轮支持
        grid.MouseWheel += (s, e) =>
        {
            if (scrollBar.Maximum <= 0) return;
            int step = (e.Delta / 120) * -1;
            scrollBar.Value += step; // 这里会自动触发 Scroll 事件更新表格
        };

        // D. 数据变动或大小变动 -> 更新滚动条参数
        EventHandler updateParams = (s, e) => UpdateScrollBarParams(grid, scrollBar);
        grid.RowsAdded += (s, e) => UpdateScrollBarParams(grid, scrollBar);
        grid.RowsRemoved += (s, e) => UpdateScrollBarParams(grid, scrollBar);
        grid.Resize += (s, e) => UpdateScrollBarParams(grid, scrollBar);

        // 初始化一次
        UpdateScrollBarParams(grid, scrollBar);
    }

    private static void UpdateScrollBarParams(DataGridView grid, D2ScrollBar scrollBar)
    {
        // 1. 动态调整 Padding (保持不变)
        int topOffset = grid.ColumnHeadersHeight;
        scrollBar.Padding = new Padding(0, topOffset, 0, 0);

        // 2. 计算可见行数
        int visibleRows = grid.DisplayedRowCount(false); // 只计算完整显示的行

        // ================================================================
        // 【修复核心逻辑】
        // 既然 D2ScrollBar 内部采用了 (Max - Large + 1) 的标准逻辑
        // 这里 Maximum 应该设置为 (总行数 - 1)
        // ================================================================

        if (grid.RowCount > 0)
        {
            // 这里的逻辑是：
            // 假设总行数 100，Visible 10。
            // Maximum 设为 99。
            // ScrollBar 内部上限 = 99 - 10 + 1 = 90。
            // 此时 Value 最大为 90，表格正好显示第 90 行到 99 行。完美。
            scrollBar.Maximum = grid.RowCount - 1;
        }
        else
        {
            scrollBar.Maximum = 0;
        }

        // 设置滑块大小（代表一页能显示多少数据）
        scrollBar.LargeChange = visibleRows > 0 ? visibleRows : 1;

        // 3. 只有当内容超出显示范围时才显示
        // (TotalRows > VisibleRows)
        scrollBar.Visible = grid.RowCount > visibleRows;
    }


    /// <summary>
    /// 【重载】将 D2 风格滚动条附加到普通 Panel/UserControl 上
    /// </summary>
    /// <param name="viewport">外层容器（固定高度，AutoScroll=false）</param>
    /// <param name="content">内层长内容（高度超过容器，Top会被修改）</param>
    public static void Attach(ScrollableControl viewport, Control content)
    {
        if (viewport.Tag is D2ScrollBar) return;

        // 1. 基础配置
        // 强制关闭原生滚动条，否则会打架
        viewport.AutoScroll = false;

        // 配置 D2 滚动条
        var scrollBar = new D2ScrollBar
        {
            Dock = DockStyle.Right,
            Width = 10,
            // 普通面板通常不需要避开表头，Padding 设为 0 或按需微调
            Padding = new Padding(0, 0, 0, 0)
        };

        viewport.Controls.Add(scrollBar);
        scrollBar.SendToBack();
        scrollBar.BringToFront(); // 注意：普通Panel里，滚动条通常要浮在最上面，或者Dock Right挤占空间
                                  // 如果你希望滚动条挤占空间（内容变窄），用 SendToBack() + Dock.Right
                                  // 如果你希望滚动条悬浮在内容上（覆盖），用 BringToFront() + Dock.Right (由于内容是非Dock的，这通常没问题)

        // 这里我们采用 "挤占空间" 模式，比较稳妥
        scrollBar.SendToBack();

        viewport.Tag = scrollBar;

        // ==========================================
        // 事件绑定
        // ==========================================

        // A. 滚动条 -> 移动内容 (修改 Top)
        scrollBar.Scroll += (s, e) =>
        {
            // 只有当内容比视口高时才移动
            if (content.Height > viewport.Height)
            {
                // Value = 0, Top = 0
                // Value = Max, Top = -(ContentHeight - ViewportHeight)
                content.Top = -scrollBar.Value;
            }
        };

        // B. 鼠标滚轮 (同时监听视口和内容，体验更好)
        MouseEventHandler wheelHandler = (s, e) =>
        {
            if (!scrollBar.Visible) return;
            int step = (e.Delta / 120) * -1 * 40; // *40 是滚动速度，可调整
            scrollBar.Value += step;
        };
        viewport.MouseWheel += wheelHandler;
        content.MouseWheel += wheelHandler; // 鼠标指着内容时也能滚

        // C. 大小/内容变化 -> 更新参数
        EventHandler updateParams = (s, e) => UpdatePanelParams(viewport, content, scrollBar);

        viewport.Resize += updateParams;
        content.Resize += updateParams; // 如果内容高度变了（比如动态加载了控件）

        // 初始化
        UpdatePanelParams(viewport, content, scrollBar);
    }

    private static void UpdatePanelParams(ScrollableControl viewport, Control content, D2ScrollBar scrollBar)
    {
        // 计算可滚动范围 = 内容高度 - 视口高度
        // 注意：视口高度要减去滚动条可能的 Padding（虽然这里设了0）
        int availableHeight = viewport.Height;
        int maxScroll = content.Height - availableHeight;

        if (maxScroll < 0) maxScroll = 0;

        scrollBar.Maximum = maxScroll;

        // LargeChange 设为视口高度（PageDown的效果）
        scrollBar.LargeChange = availableHeight;

        scrollBar.Visible = maxScroll > 0;

        // 如果窗口变大导致不需要滚动了，重置位置
        if (!scrollBar.Visible)
        {
            content.Top = 0;
            scrollBar.Value = 0;
        }
        else
        {
            // 窗口缩放时，确保内容位置合法
            // 比如原来滚到了底，现在窗口拉长了，要自动回弹
            if (-content.Top > maxScroll)
            {
                scrollBar.Value = maxScroll; // 这会触发 Scroll 事件修正 Top
            }
        }
    }
}