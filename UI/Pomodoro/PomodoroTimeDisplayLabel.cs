using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Services;

namespace DiabloTwoMFTimer.UI.Pomodoro
{
    public class PomodoroTimeDisplayLabel : Label
    {
        private IPomodoroTimerService? _timerService;
        private bool _showMilliseconds = true; // 默认显示毫秒

        public PomodoroTimeDisplayLabel()
        {
            this.AutoSize = true;
            this.Font = new Font("微软雅黑", 16F, FontStyle.Bold);
            this.Text = "00:00:0";
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// 是否显示毫秒。设置为 false 则只显示 分:秒
        /// </summary>
        public bool ShowMilliseconds
        {
            get => _showMilliseconds;
            set
            {
                _showMilliseconds = value;
                // 属性改变时立即刷新显示
                if (!IsDisposed && IsHandleCreated)
                {
                    UpdateDisplay();
                }
            }
        }

        public void BindService(IPomodoroTimerService service)
        {
            // 解绑旧服务
            if (_timerService != null)
            {
                _timerService.TimeUpdated -= OnTimeUpdated;
                _timerService.TimerStateChanged -= OnStateChanged;
            }

            _timerService = service;

            // 绑定新服务
            if (_timerService != null)
            {
                _timerService.TimeUpdated += OnTimeUpdated;
                _timerService.TimerStateChanged += OnStateChanged;
                UpdateDisplay();
            }
        }

        private void OnTimeUpdated(object? sender, EventArgs e)
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<object?, EventArgs>(OnTimeUpdated), sender, e);
                return;
            }
            UpdateDisplay();
        }

        private void OnStateChanged(object? sender, TimerStateChangedEventArgs e)
        {
            if (IsDisposed || !IsHandleCreated)
                return;

            if (InvokeRequired)
            {
                BeginInvoke(new Action<object?, TimerStateChangedEventArgs>(OnStateChanged), sender, e);
                return;
            }
            UpdateDisplay();
            UpdateColor();
        }

        private void UpdateDisplay()
        {
            if (_timerService == null)
                return;

            var timeLeft = _timerService.TimeLeft;
            string formattedTime;

            if (_showMilliseconds)
            {
                // 格式：25:00:0 (原有格式)
                formattedTime = string.Format(
                    "{0:00}:{1:00}:{2}",
                    timeLeft.Minutes,
                    timeLeft.Seconds,
                    timeLeft.Milliseconds / 100
                );
            }
            else
            {
                // 格式：25:00 (隐藏毫秒)
                formattedTime = string.Format("{0:00}:{1:00}", timeLeft.Minutes, timeLeft.Seconds);
            }

            if (this.Text != formattedTime)
            {
                this.Text = formattedTime;
            }

            UpdateColor();
        }

        private void UpdateColor()
        {
            if (_timerService == null)
                return;
            Color targetColor = _timerService.CurrentState == TimerState.Work ? Color.Black : Color.Green;

            if (this.ForeColor != targetColor)
            {
                this.ForeColor = targetColor;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _timerService != null)
            {
                _timerService.TimeUpdated -= OnTimeUpdated;
                _timerService.TimerStateChanged -= OnStateChanged;
            }
            base.Dispose(disposing);
        }
    }
}
