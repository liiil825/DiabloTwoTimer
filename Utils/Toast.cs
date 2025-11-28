using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI;

namespace DiabloTwoMFTimer.Utils
{
    public static class Toast
    {
        // 简单的堆叠管理，防止重叠
        private static List<ToastForm> _openToasts = new List<ToastForm>();

        public static void Show(string message, ToastType type = ToastType.Info, string title = "")
        {
            var toast = new ToastForm(message, type, title);

            // 获取主屏幕的工作区（去除任务栏的区域）
            var screen = Screen.PrimaryScreen;
            if (screen == null)
            {
                // 如果无法获取主屏幕，使用默认位置
                toast.Location = new Point(100, 100);
            }
            else
            {
                var workingArea = screen.WorkingArea;

                // 1. 计算水平居中 X 坐标
                // 公式：屏幕左边距 + (屏幕宽 - 窗体宽) / 2
                // 注意：加上 screen.X 是为了兼容多显示器，防止计算到负坐标去
                int x = workingArea.X + (workingArea.Width - toast.Width) / 2;

                // 2. 计算 Y 坐标 (离顶部 10%)
                int startY = workingArea.Y + (int)(workingArea.Height * 0.2);

                // 3. 堆叠偏移
                int offset = _openToasts.Count * (toast.Height + 10);

                // 设置坐标
                toast.Location = new Point(x, startY + offset);
            }

            toast.FormClosed += (s, e) =>
            {
                _openToasts.Remove(toast);
            };
            _openToasts.Add(toast);

            // 显示
            toast.Show();
        }

        public static void Success(string msg) => Show(msg, ToastType.Success);

        public static void Error(string msg) => Show(msg, ToastType.Error);

        public static void Info(string msg) => Show(msg, ToastType.Info);

        public static void Warning(string msg) => Show(msg, ToastType.Warning);
    }
}
