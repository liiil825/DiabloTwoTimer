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
    private readonly IMainService _mainService;
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

    private Button btnToggleSession = null!;
    private Button btnToggleToday = null!;
    private Button btnToggleWeek = null!;

    // --- 新增：用于存储所有的快捷键提示标签，方便统一显隐 ---
    private readonly List<Label> _shortcutBadges = new();
    private bool _showShortcuts = false; // 默认隐藏

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
        IMainService mainService,
        IPomodoroTimerService timerService,
        IAppSettings appSettings,
        IProfileService? profileService,
        IStatisticsService statsService,
        BreakFormMode mode,
        PomodoroBreakType breakType = PomodoroBreakType.ShortBreak
    )
    {
        _mainService = mainService;
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

        ApplyScaledLayout();

        // 绑定滚动条
        D2ScrollHelper.Attach(this.rtbStats, this.panelStatsContainer);

        // --- 核心修改 1: 开启键盘预览并绑定事件 ---
        this.KeyPreview = true;
        this.KeyDown += BreakForm_KeyDown;

        // --- 核心修改 2: 为底部按钮添加快捷键提示 ---
        // S -> Skip, D -> Close (CancelButton 默认映射 ESC，我们额外增加 D)
        AttachKeyBadge(btnSkip, "Z");
        AttachKeyBadge(btnClose, "X");

        // 设置Esc键关闭窗口 (保留 ESC 功能)
        this.CancelButton = btnClose;

        this.Opacity = 0;
        _fadeInTimer = new System.Windows.Forms.Timer { Interval = 15 };
        _fadeInTimer.Tick += FadeInTimer_Tick;

        UpdateLayoutState();
        UpdateContent();

        if (_mode == BreakFormMode.PomodoroBreak)
        {
            _timerService.TimeUpdated += TimerService_TimeUpdated;
            _timerService.PomodoroTimerStateChanged += TimerService_PomodoroTimerStateChanged;
            UpdateTimerDisplay();
        }

        this.Resize += BreakForm_Resize;
    }

    // --- 核心修改 3: 键盘事件处理逻辑 ---
    private void BreakForm_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.H:
                // 切换提示显示/隐藏
                ToggleShortcutsVisibility();
                e.SuppressKeyPress = true;
                break;

            case Keys.D1:
            case Keys.NumPad1:
                btnToggleSession?.PerformClick();
                break;

            case Keys.D2:
            case Keys.NumPad2:
                btnToggleToday?.PerformClick();
                break;

            case Keys.D3:
            case Keys.NumPad3:
                btnToggleWeek?.PerformClick();
                break;

            case Keys.Z:
                if (btnSkip.Visible && btnSkip.Enabled)
                    btnSkip.PerformClick();
                break;

            case Keys.X:
                if (btnClose.Visible && btnClose.Enabled)
                    btnClose.PerformClick();
                break;
        }
    }

    // --- 核心修改 4: 动态添加快捷键徽标 (Badge) ---
    private void AttachKeyBadge(Button targetBtn, string keyText)
    {
        var lblBadge = new Label
        {
            Text = keyText,
            Font = new Font("Consolas", 8F, FontStyle.Bold),
            ForeColor = Color.Gold,
            BackColor = Color.FromArgb(180, 0, 0, 0),
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor = Cursors.Hand,
            Visible = _showShortcuts,
        };

        // 计算位置：右上角
        lblBadge.Location = new Point(targetBtn.Width - 15, 2);
        lblBadge.Anchor = AnchorStyles.Top | AnchorStyles.Right;

        // 点击徽标也能触发按钮点击
        // 修改后 targetBtn 是 Button 类型，编译器就能找到 PerformClick 了
        lblBadge.Click += (s, e) => targetBtn.PerformClick();

        targetBtn.Controls.Add(lblBadge);
        lblBadge.BringToFront();

        _shortcutBadges.Add(lblBadge);
    }

    private void ToggleShortcutsVisibility()
    {
        _showShortcuts = !_showShortcuts;
        foreach (var badge in _shortcutBadges)
        {
            badge.Visible = _showShortcuts;
        }
    }

    private void ApplyScaledLayout()
    {
        mainLayout.RowStyles[0].Height = ScaleHelper.Scale(110);

        lblMessage.Margin = new Padding(0, ScaleHelper.Scale(40), 0, ScaleHelper.Scale(30));
        lblTimer.Margin = new Padding(0, 0, 0, ScaleHelper.Scale(10));
        pomodoroStatusDisplay.Margin = new Padding(0);
        lblDuration.Margin = new Padding(0, ScaleHelper.Scale(20), 0, ScaleHelper.Scale(20));

        panelStatsContainer.Margin = new Padding(
            ScaleHelper.Scale(100),
            ScaleHelper.Scale(20),
            ScaleHelper.Scale(100),
            0
        );

        mainLayout.Padding = new Padding(0);
    }

    private void FadeInTimer_Tick(object? sender, EventArgs e)
    {
        if (this.Opacity < 1)
        {
            this.Opacity += 0.08;
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
        _fadeInTimer.Start();

        BreakForm_Resize(this, EventArgs.Empty);
    }

    private void BreakForm_Resize(object? sender, EventArgs e)
    {
        if (headerControl != null && headerControl.TogglePanel != null)
        {
            headerControl.TogglePanel.Left = (headerControl.Width - headerControl.TogglePanel.Width) / 2;
        }
    }

    private void InitializeToggleButtons()
    {
        // --- 核心修改 5: 在创建时绑定快捷键 1, 2, 3 ---

        btnToggleSession = CreateToggleButton("本轮战况", StatViewType.Session);
        AttachKeyBadge(btnToggleSession, "1");

        btnToggleToday = CreateToggleButton("今日累计", StatViewType.Today);
        AttachKeyBadge(btnToggleToday, "2");

        btnToggleWeek = CreateToggleButton("本周累计", StatViewType.Week);
        AttachKeyBadge(btnToggleWeek, "3");

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

        bool isBreakMode = (_mode == BreakFormMode.PomodoroBreak);

        lblMessage.Visible = isBreakMode;
        lblTimer.Visible = isBreakMode;
        pomodoroStatusDisplay.Visible = isBreakMode;
        btnSkip.Visible = isBreakMode;
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
                if (rtbStats != null)
                    rtbStats.Text = "暂无角色数据";
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
                        start = _timerService.PomodoroCycleStartTime.AddSeconds(-30);
                    else
                        start = _timerService.FullPomodoroCycleStartTime.AddSeconds(-10);
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

            if (rtbStats != null)
            {
                string content = _statsService.GetDetailedSummary(start, end);
                rtbStats.Text = $"{title}\n\n{content}";

                rtbStats.SelectAll();
                rtbStats.SelectionAlignment = HorizontalAlignment.Center;
                rtbStats.DeselectAll();

                rtbStats.SelectionStart = 0;
                rtbStats.ScrollToCaret();
            }

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
                        );

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
            this.SafeInvoke(() => this.Close());
        }
    }

    private void BreakForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _fadeInTimer.Stop();
        _mainService.SetActiveTabPage(Models.TabPage.Timer);
        _timerService.TimeUpdated -= TimerService_TimeUpdated;
        _timerService.PomodoroTimerStateChanged -= TimerService_PomodoroTimerStateChanged;
    }
}
