using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Timer
{
    public partial class StatisticsControl : UserControl
    {
        // 统计数据属性
        public int RunCount { get; set; }
        public TimeSpan FastestTime { get; set; }
        public TimeSpan AverageTime { get; set; }

        public StatisticsControl()
        {
            InitializeComponent();
            // 注册语言变更事件
            Utils.LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
        }

        private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        public void UpdateStatistics(int runCount, TimeSpan fastestTime, List<TimeSpan> runHistory)
        {
            // 更新属性
            this.RunCount = runCount;
            this.FastestTime = fastestTime;

            // 计算平均时间
            if (runCount > 0 && runHistory.Count > 0)
            {
                TimeSpan totalTime = TimeSpan.Zero;
                foreach (var time in runHistory)
                {
                    totalTime += time;
                }
                this.AverageTime = new TimeSpan(totalTime.Ticks / runHistory.Count);
            }
            else
            {
                this.AverageTime = TimeSpan.Zero;
            }

            // 更新UI
            UpdateUI();
        }

        public void UpdateUI()
        {
            // 更新统计信息
            if (lblRunCount != null)
            {
                // 使用多语言显示运行次数
                string runCountText = Utils.LanguageManager.GetString("RunCount", RunCount);
                if (string.IsNullOrEmpty(runCountText) || runCountText == "RunCount")
                {
                    // 如果未找到翻译，使用默认格式
                    runCountText = $"--- 运行次数 {RunCount} ---";
                }
                else
                {
                    runCountText = $"--- {runCountText} ---";
                }
                lblRunCount.Text = runCountText;
            }

            if (lblFastestTime != null)
            {
                if (RunCount > 0 && FastestTime != TimeSpan.MaxValue && FastestTime > TimeSpan.Zero)
                {
                    string fastestFormatted = string.Format(
                        "{0:00}:{1:00}:{2:00}.{3}",
                        FastestTime.Hours,
                        FastestTime.Minutes,
                        FastestTime.Seconds,
                        (int)(FastestTime.Milliseconds / 100)
                    );

                    string fastestTimeText = Utils.LanguageManager.GetString("FastestTime", fastestFormatted);
                    if (string.IsNullOrEmpty(fastestTimeText) || fastestTimeText == "FastestTime")
                    {
                        fastestTimeText = $"最快时间: {fastestFormatted}";
                    }

                    lblFastestTime.Text = fastestTimeText;
                }
                else
                {
                    string fastestTimeText = Utils.LanguageManager.GetString("FastestTimePlaceholder");
                    if (string.IsNullOrEmpty(fastestTimeText) || fastestTimeText == "FastestTimePlaceholder")
                    {
                        fastestTimeText = "最快时间: --:--:--.-";
                    }

                    lblFastestTime.Text = fastestTimeText;
                }
            }

            if (lblAverageTime != null)
            {
                if (RunCount > 0)
                {
                    string averageFormatted = string.Format(
                        "{0:00}:{1:00}:{2:00}.{3}",
                        AverageTime.Hours,
                        AverageTime.Minutes,
                        AverageTime.Seconds,
                        (int)(AverageTime.Milliseconds / 100)
                    );

                    string averageTimeText = Utils.LanguageManager.GetString("AverageTime", averageFormatted);
                    if (string.IsNullOrEmpty(averageTimeText) || averageTimeText == "AverageTime")
                    {
                        averageTimeText = $"平均时间: {averageFormatted}";
                    }

                    lblAverageTime.Text = averageTimeText;
                }
                else
                {
                    string averageTimeText = Utils.LanguageManager.GetString("AverageTimePlaceholder");
                    if (string.IsNullOrEmpty(averageTimeText) || averageTimeText == "AverageTimePlaceholder")
                    {
                        averageTimeText = "平均时间: --:--:--.-";
                    }

                    lblAverageTime.Text = averageTimeText;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 取消注册语言变更事件
                Utils.LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
            }
            base.Dispose(disposing);
        }
    }
}
