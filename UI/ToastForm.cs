using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace DTwoMFTimerHelper.UI {
    public enum ToastType {
        Info,
        Success,
        Warning,
        Error
    }

    public class ToastForm : Form {
        private System.Windows.Forms.Timer _timerLife = null!;
        private System.Windows.Forms.Timer _timerAnim = null!;
        private int _lifeTime = 3000; // 默认显示3秒
        private bool _isClosing = false;
        // private double _targetOpacity = 1.0;

        // UI 组件
        private Label? lblTitle;
        private Label? lblMessage;
        private Panel? pnlColorStrip;

        public ToastForm(string message, ToastType type, string title = "") {
            this.StartPosition = FormStartPosition.Manual;

            // 基础窗体设置
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.Size = new Size(300, 80);
            this.BackColor = Color.White;
            this.Opacity = 0;

            // 绘制边框（可选，简单的灰色边框）
            this.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.LightGray, ButtonBorderStyle.Solid);
            };

            InitializeControls(message, type, title);

            // 2. 动画与生命周期定时器
            _timerAnim = new System.Windows.Forms.Timer { Interval = 20 };
            _timerAnim.Tick += TimerAnim_Tick;

            _timerLife = new System.Windows.Forms.Timer { Interval = _lifeTime };
            _timerLife.Tick += (s, e) => { CloseToast(); };
        }

        // 核心：防止窗体显示时抢占焦点
        protected override bool ShowWithoutActivation => true;

        private void InitializeControls(string message, ToastType type, string title) {
            // 设置颜色
            Color typeColor = Color.Gray;
            switch (type) {
                case ToastType.Success: typeColor = Color.FromArgb(82, 196, 26); break; // AntD Green
                case ToastType.Error: typeColor = Color.FromArgb(255, 77, 79); break;   // AntD Red
                case ToastType.Warning: typeColor = Color.FromArgb(250, 173, 20); break; // AntD Orange
                case ToastType.Info: typeColor = Color.FromArgb(24, 144, 255); break;   // AntD Blue
            }

            // 左侧色条
            pnlColorStrip = new Panel {
                Dock = DockStyle.Left,
                Width = 5,
                BackColor = typeColor
            };

            // 标题
            lblTitle = new Label {
                Text = string.IsNullOrEmpty(title) ? type.ToString() : title,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(15, 10),
                AutoSize = true,
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            // 内容
            lblMessage = new Label {
                Text = message,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                Location = new Point(15, 35),
                Size = new Size(270, 40),
                TextAlign = ContentAlignment.TopLeft,
                ForeColor = Color.FromArgb(100, 100, 100)
            };

            // 点击任意位置关闭
            this.Click += (s, e) => CloseToast();
            lblMessage.Click += (s, e) => CloseToast();
            lblTitle.Click += (s, e) => CloseToast();

            this.Controls.Add(lblMessage);
            this.Controls.Add(lblTitle);
            this.Controls.Add(pnlColorStrip);
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            // 定位到屏幕右下角 (或者右上角)
            // var screen = Screen.PrimaryScreen.WorkingArea;
            // int x = screen.Width - this.Width - 20;
            // int y = screen.Height - this.Height - 20; // 这里可以加入简单的堆叠逻辑

            // this.Location = new Point(x, y);

            _timerAnim.Start(); // 开始淡入
            _timerLife.Start(); // 开始倒计时
        }

        private void TimerAnim_Tick(object? sender, EventArgs e) {
            if (_isClosing) {
                // 淡出
                this.Opacity -= 0.1;
                if (this.Opacity <= 0) {
                    _timerAnim.Stop();
                    this.Close();
                }
            }
            else {
                // 淡入
                if (this.Opacity < 1)
                    this.Opacity += 0.1;
                else
                    _timerAnim.Stop();
            }
        }

        private void CloseToast() {
            _isClosing = true;
            _timerLife.Stop();
            _timerAnim.Start();
        }
    }
}