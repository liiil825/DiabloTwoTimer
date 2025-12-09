using System;
using System.Drawing;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedTabControl : TabControl
{
    public ThemedTabControl()
    {
        this.DoubleBuffered = true;

        // 核心修改：移除复杂的 WndProc RECT 计算逻辑
        // 设置极小的 Header 尺寸，从物理上减小冲突可能性
        this.Appearance = TabAppearance.FlatButtons; // 关键：按钮模式比普通模式更稳定
        this.ItemSize = new System.Drawing.Size(0, 1);
        this.SizeMode = TabSizeMode.Fixed;
        this.Multiline = true;
    }

    // 只保留最基础的消息屏蔽，防止绘制原生边框残留
    protected override void WndProc(ref Message m)
    {
        // 0x1328 = TCM_ADJUSTRECT。这里我们直接返回，不做坐标加减。
        // 这表示我们完全不让 TabControl 留出任何 Header 空间，内容区填满父级。
        if (m.Msg == 0x1328 && !DesignMode)
        {
            m.Result = (IntPtr)1;
            return;
        }
        base.WndProc(ref m);
    }
}