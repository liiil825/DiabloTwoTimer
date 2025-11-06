using System;
using System.Windows.Forms;
using DTwoMFTimerHelper.Data;

namespace DTwoMFTimerHelper
{
    public partial class TimerControl : UserControl
    {
        // 计时器相关字段
        private bool isTimerRunning = false;
        private DateTime startTime = DateTime.MinValue;
        private System.Windows.Forms.Timer? timer;
        private Data.CharacterProfile? currentProfile = null;

        // 事件
        public event EventHandler? TimerStateChanged;

        // 公共属性
        public bool IsTimerRunning => isTimerRunning;
        public Data.CharacterProfile? CurrentProfile => currentProfile;
        // Removed reference to non-existent currentRecord field

        public TimerControl()
        {
            InitializeComponent();
            InitializeTimer();
            UpdateUI();
        }

        private void InitializeComponent()
        {
            // 主要计时显示标签
            lblTimeDisplay = new Label();
            
            // 信息显示标签
            lblCurrentProfile = new Label();
            
            // 提示标签
            lblHint = new Label();
            
            SuspendLayout();
            // 
            // lblTimeDisplay - 计时显示
            // 
            lblTimeDisplay.AutoSize = true;
            lblTimeDisplay.Font = new System.Drawing.Font("Microsoft YaHei UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            lblTimeDisplay.Location = new System.Drawing.Point(20, 30);
            lblTimeDisplay.Name = "lblTimeDisplay";
            lblTimeDisplay.Size = new System.Drawing.Size(288, 64);
            lblTimeDisplay.TabIndex = 0;
            lblTimeDisplay.Text = "00:00:00";
            
            // 
            // lblCurrentProfile - 当前角色显示
            // 
            lblCurrentProfile.AutoSize = true;
            lblCurrentProfile.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            lblCurrentProfile.Location = new System.Drawing.Point(20, 110);
            lblCurrentProfile.Name = "lblCurrentProfile";
            lblCurrentProfile.Size = new System.Drawing.Size(120, 20);
            lblCurrentProfile.TabIndex = 1;
            
            // 
            // lblHint - 使用提示
            // 
            lblHint.AutoSize = true;
            lblHint.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            lblHint.ForeColor = System.Drawing.Color.Gray;
            lblHint.Location = new System.Drawing.Point(20, 150);
            lblHint.Name = "lblHint";
            lblHint.Size = new System.Drawing.Size(220, 17);
            lblHint.TabIndex = 2;
            lblHint.Text = "使用快捷键开始/结束计时（默认Alt+Q）";
            
            // 
            // TimerControl - 主控件设置
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblTimeDisplay);
            Controls.Add(lblCurrentProfile);
            Controls.Add(lblHint);
            Name = "TimerControl";
            Size = new System.Drawing.Size(340, 200);
            ResumeLayout(false);
            PerformLayout();
        }

        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 1000; // 1秒
            timer.Tick += Timer_Tick;
        }

        public void UpdateUI()
        {
            // 更新当前角色显示
            if (currentProfile != null)
            {
                if (lblCurrentProfile != null) lblCurrentProfile.Text = $"角色: {currentProfile.Name}";
            }
            else
            {
                if (lblCurrentProfile != null) lblCurrentProfile.Text = "角色: 未选择";
            }
            
            // 更新时间显示
            if (isTimerRunning && startTime != DateTime.MinValue)
            {
                TimeSpan elapsed;
                
                if (isPaused && pauseStartTime != DateTime.MinValue)
                {
                    // 暂停状态，计算到暂停开始时的时间
                    elapsed = pauseStartTime - startTime - pausedDuration;
                }
                else
                {
                    // 运行状态，计算实际经过时间（扣除暂停时间）
                    elapsed = DateTime.Now - startTime - pausedDuration;
                }
                
                string formattedTime = string.Format("{0:00}:{1:00}:{2:00}", elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
                if (lblTimeDisplay != null) 
                {
                    // 根据时间长度调整字体大小确保显示完整
                    if (elapsed.Hours > 9)
                    {
                        lblTimeDisplay.Font = new System.Drawing.Font("Microsoft YaHei UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    }
                    else
                    {
                        lblTimeDisplay.Font = new System.Drawing.Font("Microsoft YaHei UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    }
                    
                    // 暂停时显示不同的样式
                    if (isPaused)
                    {
                        lblTimeDisplay.Text = $"暂停 - {formattedTime}";
                    }
                    else
                    {
                        lblTimeDisplay.Text = formattedTime;
                    }
                }
            }
            else
            {
                if (lblTimeDisplay != null) 
                {
                    lblTimeDisplay.Font = new System.Drawing.Font("Microsoft YaHei UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    lblTimeDisplay.Text = "00:00:00";
                }
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        // 提供给外部调用的开始/停止方法，用于快捷键触发
        public void ToggleTimer()
        {
            if (!isTimerRunning)
            {
                StartTimer();
            }
            else
            {
                StopTimer();
            }
        }
        
        // 提供给外部调用的暂停方法，用于快捷键触发
        public void TogglePause()
        {
            if (isTimerRunning)
            {
                if (isPaused)
                {
                    ResumeTimer();
                }
                else
                {
                    PauseTimer();
                }
            }
        }
        
        private void PauseTimer()
        {
            if (isTimerRunning && !isPaused)
            {
                isPaused = true;
                pauseStartTime = DateTime.Now;
                UpdateUI();
            }
        }
        
        private void ResumeTimer()
        {
            if (isTimerRunning && isPaused && pauseStartTime != DateTime.MinValue)
            {
                pausedDuration += DateTime.Now - pauseStartTime;
                isPaused = false;
                pauseStartTime = DateTime.MinValue;
                UpdateUI();
            }
        }
        
        // 提供给外部调用的重置方法
        public void ResetTimerExternally()
        {
            ResetTimer();
        }

        private void StartTimer()
        {
            isTimerRunning = true;
            isPaused = false;
            startTime = DateTime.Now;
            pausedDuration = TimeSpan.Zero;
            pauseStartTime = DateTime.MinValue;
            timer?.Start();
            
            UpdateUI();
            TimerStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void StopTimer()
        {
            isTimerRunning = false;
            isPaused = false;
            timer?.Stop();
            UpdateUI();
            TimerStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ResetTimer()
        {
            StopTimer();
            ResetTimerDisplay();
        }

        private void ResetTimerDisplay()
        {
            startTime = DateTime.MinValue;
            pausedDuration = TimeSpan.Zero;
            pauseStartTime = DateTime.MinValue;
            UpdateUI();
        }

        public void SetCurrentProfile(Data.CharacterProfile? profile)
        {
            currentProfile = profile;
            UpdateUI();
        }

        // 私有字段定义
        // 控件字段定义
        private Label? lblTimeDisplay;
        private Label? lblCurrentProfile;
        private Label? lblHint;
        // 已移除按钮，改为使用快捷键控制
        
        // 计时器状态字段
        private bool isPaused = false;
        private TimeSpan pausedDuration = TimeSpan.Zero;
        private DateTime pauseStartTime = DateTime.MinValue;
    }
}