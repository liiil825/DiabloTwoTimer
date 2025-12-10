using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq; // 必须引用，用于 Sum
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Pomodoro;

// 定义窗口模式
public enum BreakFormMode
{
    PomodoroBreak,
    StatisticsView,
}

public enum StatViewType
{
    Session,
    Today,
    Week,
}

public partial class BreakForm : System.Windows.Forms.Form
{
    private readonly IPomodoroTimerService _timerService;
    private readonly IAppSettings _appSettings;
    private readonly PomodoroBreakType _breakType;
    private readonly IProfileService? _profileService;
    private readonly PomodoroTimeSettings _timeSettings;
    private readonly IStatisticsService _statsService;
    private readonly BreakFormMode _mode;

    private StatViewType _currentViewType;
    private bool _isAutoClosed = false;

    private readonly List<string> _shortBreakMessages = new()
    {
        "站起来走两步，活动一下筋骨",
        "眺望远方，放松一下眼睛",
        "喝口水，补充水分",
        "深呼吸，放松肩膀",
    };
    private readonly List<string> _longBreakMessages = new()
    {
        "休息时间长一点，去吃点水果吧",
        "这一轮辛苦了，彻底放松一下",
        "即使是奈非天也需要休息",
    };

    public BreakForm(
        IPomodoroTimerService timerService,
        IAppSettings appSettings,
        IProfileService? profileService,
        IStatisticsService statsService,
        BreakFormMode mode,
        PomodoroBreakType breakType = PomodoroBreakType.ShortBreak
    )
    {
        _timerService = timerService;
        _appSettings = appSettings;
        _profileService = profileService;
        _mode = mode;
        _statsService = statsService;
        _breakType = breakType;
        _timeSettings = timerService.Settings;

        _currentViewType = (_mode == BreakFormMode.PomodoroBreak) ? StatViewType.Session : StatViewType.Today;

        InitializeComponent();
        SetupForm();
        UpdateContent();
        UpdateLayoutState();

        this.BackColor = Color.FromArgb(28, 28, 28);

        RefreshStatistics();

        if (_mode == BreakFormMode.PomodoroBreak)
        {
            _timerService.TimeUpdated += TimerService_TimeUpdated;
            _timerService.PomodoroTimerStateChanged += TimerService_PomodoroTimerStateChanged;
        }
    }

    private Button CreateToggleButton(string text, StatViewType type)
    {
        var btn = new Button
        {
            Text = text,
            Size = new Size(120, 43),
            Font = Theme.AppTheme.MainFont,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleCenter, // 显式指定居中
            UseCompatibleTextRendering = true,
            Tag = type,
        };
        btn.FlatAppearance.BorderSize = 1;
        btn.Click += (s, e) => SwitchView(type);
        return btn;
    }

    private Button CreateFlatButton(string text, Color hoverColor)
    {
        var btn = new Button
        {
            Text = text,
            Size = new Size(160, 50),
            Font = Theme.AppTheme.MainFont,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.White,
            Cursor = Cursors.Hand,
            BackColor = Color.FromArgb(60, 60, 60),
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = hoverColor;
        return btn;
    }

    private void SetupForm()
    {
        this.StartPosition = FormStartPosition.CenterScreen;
        this.WindowState = FormWindowState.Maximized;
        this.FormBorderStyle = FormBorderStyle.None;
        this.TopMost = true;
        this.DoubleBuffered = true;
    }

    private void UpdateLayoutState()
    {
        if (_mode == BreakFormMode.StatisticsView)
        {
            lblMessage.Visible = false;
            lblTimer.Visible = false;
            pomodoroStatusDisplay.Visible = false; // 统计模式隐藏番茄
            btnSkip.Visible = false;
            btnToggleSession.Visible = true;
        }
        else
        {
            lblMessage.Visible = true;
            lblTimer.Visible = true;
            pomodoroStatusDisplay.Visible = true;
            btnSkip.Visible = true;
            btnToggleSession.Visible = true;
        }
        this.PerformLayout();
    }

    private void SwitchView(StatViewType type)
    {
        _currentViewType = type;
        RefreshStatistics();
    }

    private void UpdateButtonStyles()
    {
        HighlightButton(btnToggleSession, _currentViewType == StatViewType.Session);
        HighlightButton(btnToggleToday, _currentViewType == StatViewType.Today);
        HighlightButton(btnToggleWeek, _currentViewType == StatViewType.Week);
    }

    private void HighlightButton(Button? btn, bool isActive)
    {
        if (btn == null)
            return;
        if (isActive)
        {
            btn.BackColor = Color.Gray;
            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderColor = Color.White;
        }
        else
        {
            btn.BackColor = Color.Transparent;
            btn.ForeColor = Color.Gray;
            btn.FlatAppearance.BorderColor = Color.Gray;
        }
    }

    private void UpdateContent()
    {
        if (_mode == BreakFormMode.PomodoroBreak && lblMessage != null)
        {
            var rnd = new Random();
            var list = _breakType == PomodoroBreakType.ShortBreak ? _shortBreakMessages : _longBreakMessages;
            lblMessage.Text = list[rnd.Next(list.Count)];
        }
    }

    private void RefreshStatistics()
    {
        UpdateButtonStyles();

        // 1. 更新番茄钟状态
        if (pomodoroStatusDisplay != null)
        {
            pomodoroStatusDisplay.TotalCompletedCount = _timerService.CompletedPomodoros;
        }

        if (_profileService == null || _profileService.CurrentProfile == null)
        {
            if (lblStats != null)
                lblStats.Text = "暂无角色数据";
            if (lblDuration != null)
                lblDuration.Text = "";
            return;
        }

        DateTime start = DateTime.MinValue;
        DateTime end = DateTime.Now;
        string title = "";

        // 确定时间范围
        switch (_currentViewType)
        {
            case StatViewType.Session:
                if (_breakType == PomodoroBreakType.ShortBreak)
                    start = DateTime.Now.AddMinutes(-_timeSettings.WorkTimeMinutes - 5);
                else
                {
                    int cycleMins = (_timeSettings.WorkTimeMinutes * 4) + (_timeSettings.ShortBreakMinutes * 3);
                    start = DateTime.Now.AddMinutes(-cycleMins - 10);
                }
                title = ">>> 本轮战况 <<<";
                break;

            case StatViewType.Today:
                start = DateTime.Today;
                title = ">>> 今日战况 <<<";
                break;

            case StatViewType.Week:
                start = _statsService.GetStartOfWeek();
                title = ">>> 本周战况 <<<";
                break;
        }

        // 2. 获取统计文本
        string content = _statsService.GetDetailedSummary(start, end);
        if (lblStats != null)
            lblStats.Text = $"{title}\n\n{content}";

        // 3. 计算并显示总时长 (新增功能)
        if (lblDuration != null)
        {
            var validRecords = _profileService.CurrentProfile.Records.Where(r =>
                r.IsCompleted && r.StartTime >= start && r.StartTime <= end
            );

            double totalSeconds = validRecords.Sum(r => r.DurationSeconds);
            TimeSpan ts = TimeSpan.FromSeconds(totalSeconds);

            // 格式化时长显示
            string durationText =
                totalSeconds < 60
                    ? $"{ts.Seconds}秒"
                    : (totalSeconds < 3600 ? $"{ts.Minutes}分 {ts.Seconds}秒" : $"{ts.Hours}小时 {ts.Minutes}分");

            lblDuration.Text = $"累计游戏时长: {durationText}";
        }
    }

    private void BreakForm_SizeChanged(object? sender, EventArgs e)
    {
        if (pnlHeader == null)
            return;

        int cx = this.ClientSize.Width / 2;
        int totalH = this.ClientSize.Height;
        int totalW = this.ClientSize.Width;

        // 1. Header
        pnlToggles.PerformLayout();
        pnlToggles.Left = (totalW - pnlToggles.Width) / 2;

        int currentY = 150; // 起始高度

        // 2. 提示语 (仅休息模式)
        if (_mode == BreakFormMode.PomodoroBreak)
        {
            lblMessage.Width = totalW - 100;
            lblMessage.Location = new Point(50, currentY);
            currentY = lblMessage.Bottom + 50;

            // 3. 倒计时 (移到这里)
            // 确保 Label AutoSize = true, 居中计算
            lblTimer.Location = new Point(cx - (lblTimer.Width / 2), currentY);
            currentY = lblTimer.Bottom + 10;

            // 4. 番茄状态 (新增)
            pomodoroStatusDisplay.Location = new Point(cx - (pomodoroStatusDisplay.Width / 2), currentY);
            currentY = pomodoroStatusDisplay.Bottom + 20;
        }
        else
        {
            currentY += 50;
        }

        // 5. 总时长 (新增)
        lblDuration.Location = new Point(cx - (lblDuration.Width / 2), currentY);
        currentY = lblDuration.Bottom + 40;

        // --- 底部按钮 ---
        int btnY = totalH - 200;
        if (_mode == BreakFormMode.PomodoroBreak)
        {
            int spacing = 40;
            int totalBtnW = btnClose.Width + btnSkip.Width + spacing;
            btnSkip.Location = new Point(cx - (totalBtnW / 2), btnY);
            btnClose.Location = new Point(btnSkip.Right + spacing, btnY);
        }
        else
        {
            btnClose.Location = new Point(cx - (btnClose.Width / 2), btnY);
        }

        // 6. 统计内容 (填充剩余空间)
        int statsBottomLimit = btnY - 20;
        int statsHeight = statsBottomLimit - currentY;
        if (statsHeight < 100)
            statsHeight = 100;

        lblStats.Width = totalW - 100;
        lblStats.Height = statsHeight;
        lblStats.Location = new Point(50, currentY);
    }

    private void TimerService_PomodoroTimerStateChanged(object? sender, PomodoroTimerStateChangedEventArgs e)
    {
        if (_mode == BreakFormMode.PomodoroBreak)
        {
            if (
                e.State == PomodoroTimerState.Work
                && (
                    (_breakType == PomodoroBreakType.ShortBreak && e.PreviousState == PomodoroTimerState.ShortBreak)
                    || (_breakType == PomodoroBreakType.LongBreak && e.PreviousState == PomodoroTimerState.LongBreak)
                )
            )
            {
                AutoCloseForm();
            }
        }
    }

    private void CheckBreakTimeEnded()
    {
        if (_mode == BreakFormMode.PomodoroBreak)
        {
            if (_timerService.TimeLeft <= TimeSpan.Zero && _timerService.CurrentState == PomodoroTimerState.Work)
            {
                AutoCloseForm();
            }
        }
    }

    private void TimerService_TimeUpdated(object? sender, EventArgs e)
    {
        this.SafeInvoke(() =>
        {
            var t = _timerService.TimeLeft;
            if (lblTimer != null)
                lblTimer.Text = $"{(int)t.TotalMinutes:00}:{t.Seconds:00}";
            CheckBreakTimeEnded();
        });
    }

    private void AutoCloseForm()
    {
        if (!_isAutoClosed && !this.IsDisposed)
        {
            _isAutoClosed = true;
            this.SafeInvoke(() =>
            {
                this.Close();
            });
        }
    }

    private void BreakForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _timerService.TimeUpdated -= TimerService_TimeUpdated;
        _timerService.PomodoroTimerStateChanged -= TimerService_PomodoroTimerStateChanged;
    }
}
