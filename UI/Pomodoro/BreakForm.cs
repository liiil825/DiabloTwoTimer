using System;
using System.Windows.Forms;
using DTwoMFTimerHelper.Utils;
using Timer = System.Windows.Forms.Timer;

namespace DTwoMFTimerHelper.UI.Pomodoro
{
    public partial class BreakForm : Form
    {
        // 休息类型枚举
        public enum BreakType
        {
            ShortBreak,
            LongBreak
        }

        // 公共属性
        public int RemainingMilliseconds { get; set; } // 修改为毫秒级
        public BreakType CurrentBreakType { get; private set; }

        // 事件
        public event EventHandler? BreakSkipped;

        private readonly System.Windows.Forms.Timer breakTimer;

        public BreakForm(int breakDurationMinutes, BreakType breakType)
        {
            InitializeComponent();

            // 设置窗口属性 - 全屏显示
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None; // 无边框
            this.TopMost = true;

            // 初始化休息设置
            RemainingMilliseconds = breakDurationMinutes * 60 * 1000; // 转换为毫秒
            CurrentBreakType = breakType;

            // 初始化计时器 - 使用100ms间隔更稳定
            breakTimer = new System.Windows.Forms.Timer
            {
                Interval = 100 // 100毫秒
            };
            breakTimer.Tick += BreakTimer_Tick;
            breakTimer.Start();

            // 添加窗口大小变化事件，确保按钮始终在正确位置
            this.SizeChanged += BreakForm_SizeChanged;

            // 更新界面
            UpdateUI();
        }

        private void InitializeComponent()
        {
            lblBreakMessage = new Label();
            lblBreakTime = new Label();
            btnClose = new Button();
            btnSkipBreak = new Button();
            SuspendLayout();
            // 
            // lblBreakMessage
            // 
            lblBreakMessage.Dock = DockStyle.Top;
            lblBreakMessage.Font = new System.Drawing.Font("微软雅黑", 48F);
            lblBreakMessage.ForeColor = System.Drawing.Color.Green;
            lblBreakMessage.Location = new System.Drawing.Point(0, 0);
            lblBreakMessage.Margin = new Padding(6, 0, 6, 0);
            lblBreakMessage.Name = "lblBreakMessage";
            lblBreakMessage.Size = new System.Drawing.Size(1486, 280);
            lblBreakMessage.TabIndex = 0;
            lblBreakMessage.Text = "休息时间";
            lblBreakMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBreakTime
            // 
            lblBreakTime.Dock = DockStyle.Fill;
            lblBreakTime.Font = new System.Drawing.Font("微软雅黑", 96F);
            lblBreakTime.ForeColor = System.Drawing.Color.Green;
            lblBreakTime.Location = new System.Drawing.Point(0, 280);
            lblBreakTime.Margin = new Padding(6, 0, 6, 0);
            lblBreakTime.Name = "lblBreakTime";
            lblBreakTime.Size = new System.Drawing.Size(1486, 840);
            lblBreakTime.TabIndex = 1;
            lblBreakTime.Text = "05:00.000";
            lblBreakTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblBreakTime.Click += LblBreakTime_Click;
            // 
            // btnClose
            // 
            btnClose.Font = new System.Drawing.Font("微软雅黑", 12F);
            btnClose.Location = new System.Drawing.Point(0, 0);
            btnClose.Margin = new Padding(6, 6, 6, 6);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(223, 93);
            btnClose.TabIndex = 2;
            btnClose.Text = "关闭";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += BtnClose_Click;
            // 
            // btnSkipBreak
            // 
            btnSkipBreak.Font = new System.Drawing.Font("微软雅黑", 12F);
            btnSkipBreak.Location = new System.Drawing.Point(0, 0);
            btnSkipBreak.Margin = new Padding(6, 6, 6, 6);
            btnSkipBreak.Name = "btnSkipBreak";
            btnSkipBreak.Size = new System.Drawing.Size(223, 93);
            btnSkipBreak.TabIndex = 3;
            btnSkipBreak.Text = "跳过休息";
            btnSkipBreak.UseVisualStyleBackColor = true;
            btnSkipBreak.Click += BtnSkipBreak_Click;
            // 
            // BreakForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1486, 1120);
            Controls.Add(btnSkipBreak);
            Controls.Add(btnClose);
            Controls.Add(lblBreakTime);
            Controls.Add(lblBreakMessage);
            Margin = new Padding(6, 6, 6, 6);
            Name = "BreakForm";
            Text = "休息时间";
            FormClosing += BreakForm_FormClosing;
            ResumeLayout(false);
        }

        /// <summary>
        /// 公共方法，供外部调用刷新UI
        /// </summary>
        public void RefreshUI()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdateUI));
            }
            else
            {
                UpdateUI();
            }
        }
        
        private void UpdateUI()
        {
            // 更新标题
            this.Text = LanguageManager.GetString("BreakTime") ?? "休息时间";

            // 更新休息消息
            if (CurrentBreakType == BreakType.ShortBreak)
            {
                lblBreakMessage!.Text = LanguageManager.GetString("ShortBreakMessage") ?? "短休息时间";
            }
            else
            {
                lblBreakMessage!.Text = LanguageManager.GetString("LongBreakMessage") ?? "长休息时间";
            }

            // 更新按钮文本
            btnClose!.Text = LanguageManager.GetString("Close") ?? "关闭";
            btnSkipBreak!.Text = LanguageManager.GetString("SkipBreak") ?? "跳过休息";

            // 更新倒计时显示
            UpdateBreakTimeDisplay();
        }

        private void UpdateBreakTimeDisplay()
        {
            // 计算分、秒和毫秒（只显示几百毫秒）
            int totalSeconds = RemainingMilliseconds / 1000;
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            // 只保留百位的毫秒值
            int hundredsOfMilliseconds = (RemainingMilliseconds % 1000) / 100;

            // 格式化为 mm:ss:h 其中h表示几百毫秒
            string timeText = string.Format("{0:00}:{1:00}:{2}", minutes, seconds, hundredsOfMilliseconds);

            // 确保标签不为空再更新
            if (lblBreakTime != null && !string.IsNullOrEmpty(timeText))
            {
                lblBreakTime.Text = timeText;
            }
        }

        private void BreakTimer_Tick(object? sender, EventArgs e)
        {
            if (RemainingMilliseconds > 0)
            {
                RemainingMilliseconds -= 100; // 减去100毫秒，更稳定地更新
                UpdateBreakTimeDisplay();
            }
            else
            {
                // 休息时间结束，关闭窗口
                breakTimer?.Stop();
                this.Close();
            }
        }

        private void BreakForm_SizeChanged(object? sender, EventArgs e)
        {
            // 确保按钮始终在右下角，距离右边和下面各80px，按钮间距离40px
            const int marginRight = 80;
            const int marginBottom = 80;
            const int buttonSpacing = 40;

            // 检查按钮控件是否为null，避免空引用异常
            if (btnClose != null && btnSkipBreak != null)
            {
                int buttonWidth = btnClose.Width;
                int buttonHeight = btnClose.Height;

                // 设置关闭按钮位置
                btnClose.Left = this.ClientSize.Width - marginRight - buttonWidth;
                btnClose.Top = this.ClientSize.Height - marginBottom - buttonHeight;

                // 设置跳过休息按钮位置
                btnSkipBreak.Left = btnClose.Left - buttonWidth - buttonSpacing;
                btnSkipBreak.Top = btnClose.Top;
            }
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            // 暂停计时器
            breakTimer?.Stop();
            // 只关闭窗口，不跳过休息
            this.Close();
        }

        private void BtnSkipBreak_Click(object? sender, EventArgs e)
        {
            // 暂停计时器
            breakTimer?.Stop();
            // 触发跳过休息事件
            BreakSkipped?.Invoke(this, EventArgs.Empty);
            // 关闭窗口
            this.Close();
        }

        private void BreakForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // 确保计时器停止
            breakTimer?.Stop();
        }

        private Label? lblBreakMessage;
        private Label? lblBreakTime;
        private Button? btnClose;
        private Button? btnSkipBreak;

        private void LblBreakTime_Click(object? sender, EventArgs e)
        {

        }
    }
}