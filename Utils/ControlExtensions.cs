using System;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.Utils;

public static class ControlExtensions
{
    /// <summary>
    /// 线程安全的 UI 更新方法。
    /// </summary>
    /// <param name="control">目标控件</param>
    /// <param name="action">要执行的 UI 操作</param>
    public static void SafeInvoke(this Control control, Action action)
    {
        // 1. 如果控件已经销毁，坚决不执行，防止异常
        if (control.IsDisposed)
        {
            return;
        }

        // 2. [关键修复] 如果句柄尚未创建，说明还在初始化阶段（如构造函数中），
        // 或者是为了响应某些不需要句柄的属性设置。
        // 此时通常不需要 Invoke，直接执行即可。
        if (!control.IsHandleCreated)
        {
            action();
            return;
        }

        // 3. 正常的跨线程检查
        if (control.InvokeRequired)
        {
            try
            {
                control.Invoke(action);
            }
            catch (ObjectDisposedException)
            {
                // 再次捕获极其罕见的并发销毁情况
            }
            catch (InvalidOperationException)
            {
                // 捕获 "Invoke or BeginInvoke cannot be called on a control until the window handle has been created"
                // 虽然前面检查了 IsHandleCreated，但在极端的并发窗口关闭时仍可能发生
            }
        }
        else
        {
            // 4. 在 UI 线程直接执行
            action();
        }
    }
}
