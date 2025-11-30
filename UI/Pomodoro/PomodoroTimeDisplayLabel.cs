using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Pomodoro;

public class PomodoroTimeDisplayLabel : Label
{
    private IPomodoroTimerService? _timerService;
    private bool _showMilliseconds = true; // 默认显示毫秒

    public PomodoroTimeDisplayLabel()
    {
        this.AutoSize = true;
        // 使用全局统一的大字体
        this.Font = UI.Theme.AppTheme.BigTimeFont;
        // 默认前景色为白色（适应深色背景），UpdateColor 会根据状态覆盖它
        this.ForeColor = UI.Theme.AppTheme.TextColor;
        this.Text = "00:00:0";
        this.DoubleBuffered = true;
        this.BackColor = System.Drawing.Color.Transparent;
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
            _timerService.PomodoroTimerStateChanged -= OnPomodoroTimerStateChanged;
        }

        _timerService = service;

        // 绑定新服务
        if (_timerService != null)
        {
            _timerService.TimeUpdated += OnTimeUpdated;
            _timerService.PomodoroTimerStateChanged += OnPomodoroTimerStateChanged;
            UpdateDisplay();
        }
    }

    private void OnTimeUpdated(object? sender, EventArgs e)
    {
        this.SafeInvoke(UpdateDisplay);
    }

    private void OnPomodoroTimerStateChanged(object? sender, PomodoroTimerStateChangedEventArgs e)
    {
        this.SafeInvoke(() =>
        {
            UpdateDisplay();
            UpdateColor();
        });
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

        // 之前是 Black，现在深色模式下要改为 TextColor (白色)
        Color targetColor =
            _timerService.CurrentState == PomodoroTimerState.Work
                ? AppTheme.TextColor // 正常工作：白色
                : Color.LightGreen; // 休息：浅绿色

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
            _timerService.PomodoroTimerStateChanged -= OnPomodoroTimerStateChanged;
        }
        base.Dispose(disposing);
    }
}
