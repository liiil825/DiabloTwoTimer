using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Pomodoro;

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

    // 动画定时器
    private System.Windows.Forms.Timer _fadeInTimer;

    // 动态生成的切换按钮
    private Button btnToggleSession = null!;
    private Button btnToggleToday = null!;
    private Button btnToggleWeek = null!;

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
        InitializeToggleButtons();

        // --- 1. 初始透明，准备动画 ---
        this.Opacity = 0;
        _fadeInTimer = new System.Windows.Forms.Timer { Interval = 15 };
        _fadeInTimer.Tick += FadeInTimer_Tick;

        // 强制设置所有标签为非自动大小，以便统一宽度对齐
        ConfigureLabelStyles();

        UpdateLayoutState();
        UpdateContent();

        if (_mode == BreakFormMode.PomodoroBreak)
        {
            _timerService.TimeUpdated += TimerService_TimeUpdated;
            _timerService.PomodoroTimerStateChanged += TimerService_PomodoroTimerStateChanged;
            UpdateTimerDisplay();
        }

        this.SizeChanged += BreakForm_SizeChanged;
    }

    // --- 2. 动画逻辑 ---
    private void FadeInTimer_Tick(object? sender, EventArgs e)
    {
        if (this.Opacity < 1)
        {
            this.Opacity += 0.08; // 调整这个数值控制速度
        }
        else
        {
            this.Opacity = 1;
            _fadeInTimer.Stop();
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        RefreshStatistics();

        // 触发一次布局计算，确保初始位置正确
        BreakForm_SizeChanged(this, EventArgs.Empty);

        // --- 3. 开始淡入 ---
        _fadeInTimer.Start();
    }

    private void ConfigureLabelStyles()
    {
        // 统一设置标签属性：关闭 AutoSize，启用居中对齐
        void SetStyle(Label lbl)
        {
            if (lbl == null)
                return;
            lbl.AutoSize = false;
            lbl.TextAlign = ContentAlignment.MiddleCenter;
        }

        SetStyle(lblMessage);
        SetStyle(lblTimer);
        SetStyle(lblDuration);

        // lblStats 是多行文本，通常 TopCenter 更自然
        if (lblStats != null)
        {
            lblStats.AutoSize = false;
            lblStats.TextAlign = ContentAlignment.TopCenter;
        }
    }

    private void InitializeToggleButtons()
    {
        btnToggleSession = CreateToggleButton("本轮战况", StatViewType.Session);
        btnToggleToday = CreateToggleButton("今日累计", StatViewType.Today);
        btnToggleWeek = CreateToggleButton("本周累计", StatViewType.Week);

        headerControl.AddToggleButton(btnToggleSession);
        headerControl.AddToggleButton(btnToggleToday);
        headerControl.AddToggleButton(btnToggleWeek);
    }

    private Button CreateToggleButton(string text, StatViewType type)
    {
        var btn = new Button
        {
            Text = text,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(
                ScaleHelper.Scale(25),
                ScaleHelper.Scale(5),
                ScaleHelper.Scale(25),
                ScaleHelper.Scale(5)
            ),
            MinimumSize = new Size(0, ScaleHelper.Scale(43)),
            Font = AppTheme.MainFont,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleCenter,
            UseCompatibleTextRendering = true,
            Tag = type,
        };
        btn.FlatAppearance.BorderSize = 1;
        btn.Click += (s, e) => SwitchView(type);
        return btn;
    }

    private void UpdateLayoutState()
    {
        headerControl.Title = _mode == BreakFormMode.PomodoroBreak ? "REST & RECOVER" : "STATISTICS";

        if (_mode == BreakFormMode.StatisticsView)
        {
            lblMessage.Visible = false;
            lblTimer.Visible = false;
            pomodoroStatusDisplay.Visible = false;
            btnSkip.Visible = false;
        }
        else
        {
            lblMessage.Visible = true;
            lblTimer.Visible = true;
            pomodoroStatusDisplay.Visible = true;
            btnSkip.Visible = true;
        }
    }

    // --- 核心布局逻辑 ---
    private void BreakForm_SizeChanged(object? sender, EventArgs e)
    {
        // --- 4. 挂起布局，防止计算过程中控件乱跳 ---
        this.SuspendLayout();

        try
        {
            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;
            int cx = w / 2;

            // 1. Header
            int headerHeight = ScaleHelper.Scale(110);
            headerControl.Height = headerHeight;

            // Header 内部按钮居中
            var togglePanel = headerControl.TogglePanel;
            if (togglePanel != null)
            {
                togglePanel.PerformLayout();
                togglePanel.Left = (w - togglePanel.Width) / 2;
            }

            // --- 核心修改：统一内容宽度 ---
            // 所有中间的控件都使用这个宽度，并且 X 坐标统一
            int contentWidth = w - ScaleHelper.Scale(100);
            int contentLeft = (w - contentWidth) / 2;

            // 辅助方法：统一设置控件位置和宽度
            void LayoutCenterControl(Control ctrl, int y, int height)
            {
                ctrl.Location = new Point(contentLeft, y);
                ctrl.Size = new Size(contentWidth, height);
            }

            int currentY = headerHeight + ScaleHelper.Scale(40);

            // 2. 提示语
            if (_mode == BreakFormMode.PomodoroBreak)
            {
                // 提示语
                LayoutCenterControl(lblMessage, currentY, ScaleHelper.Scale(60));
                currentY = lblMessage.Bottom + ScaleHelper.Scale(30);

                // 倒计时
                LayoutCenterControl(lblTimer, currentY, ScaleHelper.Scale(60)); // 高度给足，防止大字体切边
                currentY = lblTimer.Bottom + ScaleHelper.Scale(10);

                // 番茄状态 (PomodoroStatusDisplay 内部自绘逻辑已支持基于 Width 居中)
                LayoutCenterControl(pomodoroStatusDisplay, currentY, ScaleHelper.Scale(40));
                currentY = pomodoroStatusDisplay.Bottom + ScaleHelper.Scale(20);
            }
            else
            {
                currentY = headerHeight + ScaleHelper.Scale(20);
            }

            // 3. 总时长 (现在和上面的控件完全对齐)
            LayoutCenterControl(lblDuration, currentY, ScaleHelper.Scale(30));
            currentY = lblDuration.Bottom + ScaleHelper.Scale(20);

            // 4. 底部按钮
            int btnY = h - ScaleHelper.Scale(100);
            if (_mode == BreakFormMode.PomodoroBreak)
            {
                int spacing = ScaleHelper.Scale(40);
                int totalBtnW = btnSkip.Width + btnClose.Width + spacing;
                int startX = (w - totalBtnW) / 2;

                btnSkip.Location = new Point(startX, btnY);
                btnClose.Location = new Point(btnSkip.Right + spacing, btnY);
            }
            else
            {
                btnClose.Location = new Point(cx - (btnClose.Width / 2), btnY);
            }

            // 5. 统计内容 (填充剩余空间)
            int statsBottomLimit = btnY - ScaleHelper.Scale(20);
            int statsHeight = statsBottomLimit - currentY;
            if (statsHeight < 100)
                statsHeight = 100;

            LayoutCenterControl(lblStats, currentY, statsHeight);
        }
        finally
        {
            // --- 5. 恢复布局 ---
            this.ResumeLayout();
        }
    }

    private void SwitchView(StatViewType type)
    {
        // 这里也可以加一点简单的 Opacity 动画让切换更柔和，不过暂不复杂化
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
        // 为了防止数据计算时 UI 卡顿，可以使用 SuspendLayout
        this.SuspendLayout();
        try
        {
            UpdateButtonStyles();

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

            switch (_currentViewType)
            {
                case StatViewType.Session:
                    if (_breakType == PomodoroBreakType.ShortBreak)
                        start = _timerService.CurrentSessionStartTime.AddSeconds(-30);
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

            string content = _statsService.GetDetailedSummary(start, end);
            if (lblStats != null)
                lblStats.Text = $"{title}\n\n{content}";

            if (lblDuration != null)
            {
                var validRecords = _profileService
                    .CurrentProfile.Records.Where(r =>
                        r.IsCompleted && r.EndTime >= start && r.EndTime <= end && r.DurationSeconds > 0
                    )
                    .ToList();

                double totalSeconds = validRecords.Sum(r => r.DurationSeconds);
                TimeSpan ts = TimeSpan.FromSeconds(totalSeconds);

                string durationText =
                    totalSeconds < 60
                        ? $"{ts.Seconds}秒"
                        : (
                            totalSeconds < 3600
                                ? $"{ts.Minutes}分 {ts.Seconds}秒"
                                : $"{(int)ts.TotalHours}小时 {ts.Minutes}分"
                        ); // 修改了这里

                lblDuration.Text = $"累计游戏时长: {durationText}";
            }
        }
        finally
        {
            this.ResumeLayout();
        }
    }

    private void TimerService_PomodoroTimerStateChanged(object? sender, PomodoroTimerStateChangedEventArgs e)
    {
        if (_mode == BreakFormMode.PomodoroBreak)
        {
            if (
                e.State == PomodoroTimerState.Work
                && (e.PreviousState == PomodoroTimerState.ShortBreak || e.PreviousState == PomodoroTimerState.LongBreak)
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
            UpdateTimerDisplay();
            CheckBreakTimeEnded();
        });
    }

    private void UpdateTimerDisplay()
    {
        var t = _timerService.TimeLeft;
        if (lblTimer != null)
            lblTimer.Text = $"{(int)t.TotalMinutes:00}:{t.Seconds:00}";
    }

    private void AutoCloseForm()
    {
        if (!_isAutoClosed && !this.IsDisposed)
        {
            _isAutoClosed = true;
            // 可以在这里做 FadeOut 动画，但这里直接关闭响应更快
            this.SafeInvoke(() => this.Close());
        }
    }

    private void BreakForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _fadeInTimer.Stop(); // 清理 Timer
        _timerService.TimeUpdated -= TimerService_TimeUpdated;
        _timerService.PomodoroTimerStateChanged -= TimerService_PomodoroTimerStateChanged;
    }
}
