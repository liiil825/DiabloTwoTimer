using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Services;

namespace DTwoMFTimerHelper.UI.Pomodoro
{
    // 定义窗口模式
    public enum BreakFormMode
    {
        PomodoroBreak, // 休息模式：有倒计时，有Session统计
        StatisticsView, // 查看模式：无倒计时，仅查看数据
    }

    public enum StatViewType
    {
        Session, // 刚才/本轮
        Today, // 今日
        Week, // 本周
    }

    public partial class BreakForm : Form
    {
        private readonly IPomodoroTimerService _timerService;
        private readonly IAppSettings _appSettings;
        private readonly BreakType _breakType;
        private readonly IProfileService? _profileService;
        private readonly TimeSettings _timeSettings;
        private readonly StatisticsService _statsService;
        private readonly BreakFormMode _mode; // 当前窗口模式

        private StatViewType _currentViewType; // 当前显示的统计类型
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

        // 构造函数
        public BreakForm(
            IPomodoroTimerService timerService,
            IAppSettings appSettings,
            IProfileService? profileService,
            BreakFormMode mode,
            BreakType breakType = BreakType.ShortBreak
        )
        {
            _timerService = timerService;
            _appSettings = appSettings;
            _profileService = profileService;
            _mode = mode;
            _breakType = breakType;
            _timeSettings = timerService.Settings;
            _statsService = new StatisticsService();

            // 确定默认显示的视图
            _currentViewType = (_mode == BreakFormMode.PomodoroBreak) ? StatViewType.Session : StatViewType.Today;

            InitializeComponent();
            SetupForm();
            UpdateContent(); // 设置初始文本
            UpdateLayoutState(); // 根据模式显隐控件

            this.BackColor = Color.FromArgb(28, 28, 28);

            // 初始加载数据
            RefreshStatistics();

            // 仅在休息模式下订阅计时事件
            if (_mode == BreakFormMode.PomodoroBreak)
            {
                _timerService.TimeUpdated += TimerService_TimeUpdated;
                _timerService.TimerStateChanged += TimerService_TimerStateChanged;
            }
        }

        private Button CreateToggleButton(string text, StatViewType type)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(120, 35),
                Font = new Font("微软雅黑", 10F),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Tag = type, // 存储类型
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
                Font = new Font("微软雅黑", 11F),
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

        // 根据模式控制控件的显示/隐藏
        private void UpdateLayoutState()
        {
            if (_mode == BreakFormMode.StatisticsView)
            {
                lblMessage.Visible = false;
                lblTimer.Visible = false;
                btnSkip.Visible = false;

                // 关键点：查看模式下，"本轮战况" 意义不大，隐藏它
                btnToggleSession.Visible = true;
            }
            else
            {
                lblMessage.Visible = true;
                lblTimer.Visible = true;
                btnSkip.Visible = true;
                btnToggleSession.Visible = true;
            }

            // 触发一次布局重算
            this.PerformLayout();
        }

        private void SwitchView(StatViewType type)
        {
            _currentViewType = type;
            RefreshStatistics();
        }

        private void UpdateButtonStyles()
        {
            // 高亮当前选中的按钮
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
            // 只有休息模式需要随机提示语
            if (_mode == BreakFormMode.PomodoroBreak && lblMessage != null)
            {
                var rnd = new Random();
                var list = _breakType == BreakType.ShortBreak ? _shortBreakMessages : _longBreakMessages;
                lblMessage.Text = list[rnd.Next(list.Count)];
            }
        }

        private void RefreshStatistics()
        {
            UpdateButtonStyles();

            if (_profileService == null || _profileService.CurrentProfile == null)
            {
                if (lblStats != null)
                    lblStats.Text = "暂无角色数据";
                return;
            }

            string title = "";
            string content = "";

            switch (_currentViewType)
            {
                case StatViewType.Session:
                    // 计算Session时间
                    DateTime sessionStart;
                    if (_breakType == BreakType.ShortBreak)
                        sessionStart = DateTime.Now.AddMinutes(-_timeSettings.WorkTimeMinutes - 5);
                    else
                    {
                        int cycleMins = (_timeSettings.WorkTimeMinutes * 4) + (_timeSettings.ShortBreakMinutes * 3);
                        sessionStart = DateTime.Now.AddMinutes(-cycleMins - 10);
                    }
                    title = ">>> 本轮战况 <<<";
                    content = _statsService.GetDetailedSummary(
                        _profileService,
                        _appSettings,
                        sessionStart,
                        DateTime.Now
                    );
                    break;

                case StatViewType.Today:
                    title = ">>> 今日战况 <<<";
                    content = _statsService.GetDetailedSummary(
                        _profileService,
                        _appSettings,
                        DateTime.Today,
                        DateTime.Now
                    );
                    break;

                case StatViewType.Week:
                    title = ">>> 本周战况 <<<";
                    content = _statsService.GetDetailedSummary(
                        _profileService,
                        _appSettings,
                        _statsService.GetStartOfWeek(),
                        DateTime.Now
                    );
                    break;
            }

            if (lblStats != null)
                lblStats.Text = $"{title}\n\n{content}";
        }

        // 布局逻辑 (根据模式动态调整)
        private void BreakForm_SizeChanged(object? sender, EventArgs e)
        {
            if (pnlHeader == null)
                return; // 防止初始化前的空引用

            int cx = this.ClientSize.Width / 2;
            int totalH = this.ClientSize.Height;
            int totalW = this.ClientSize.Width;

            // 1. 顶部 Header 区域
            // 强制刷新 FlowLayoutPanel 的布局，确保隐藏按钮后 Width 是准确的
            pnlToggles.PerformLayout();
            pnlToggles.Left = (totalW - pnlToggles.Width) / 2; // 重新计算居中

            int currentY = 120; // 内容起始 Y 坐标

            // 2. 提示语 (仅休息模式)
            if (_mode == BreakFormMode.PomodoroBreak)
            {
                lblMessage.Width = totalW - 100;
                lblMessage.Location = new Point(50, currentY);
                currentY = lblMessage.Bottom + 10;
            }
            else
            {
                currentY += 20;
            }

            // --- 从底部向上布局，防止重叠 ---

            // 5. 底部按钮位置 (固定在底部 100px 处)
            int btnY = totalH - 100;

            // 按钮布局
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

            // 4. 倒计时 (仅休息模式)
            // 放在按钮上方 60px 处
            int statsBottomLimit;

            if (_mode == BreakFormMode.PomodoroBreak)
            {
                int timerY = btnY - 60;
                lblTimer.Location = new Point(cx - (lblTimer.Width / 2), timerY);
                // 统计区域的底部界限 = 倒计时上方再留 20px
                statsBottomLimit = timerY - 20;
            }
            else
            {
                // 查看模式下，统计区域底部界限 = 按钮上方再留 40px
                statsBottomLimit = btnY - 40;
            }

            // 3. 统计内容 (填充中间剩余空间)
            // 高度 = 底部界限 - 当前Y坐标
            int statsHeight = statsBottomLimit - currentY;
            if (statsHeight < 100)
                statsHeight = 100; // 最小高度保护

            lblStats.Width = totalW - 100;
            lblStats.Height = statsHeight;
            lblStats.Location = new Point(50, currentY);
        }

        private void TimerService_TimerStateChanged(object? sender, TimerStateChangedEventArgs e)
        {
            // 只有在休息模式下才处理自动关闭逻辑
            if (_mode == BreakFormMode.PomodoroBreak)
            {
                // 如果状态从休息切换到工作，自动关闭窗口
                // 逻辑：当前是 Work 状态，且上一个状态是对应的休息状态
                if (
                    e.State == TimerState.Work
                    && (
                        (_breakType == BreakType.ShortBreak && e.PreviousState == TimerState.ShortBreak)
                        || (_breakType == BreakType.LongBreak && e.PreviousState == TimerState.LongBreak)
                    )
                )
                {
                    AutoCloseForm();
                }
            }
        }

        private void CheckBreakTimeEnded()
        {
            // 只有在休息模式下才检查
            if (_mode == BreakFormMode.PomodoroBreak)
            {
                // 双重保险：如果倒计时归零，且服务状态已经是 Work（通常 TimerStateChanged 会先触发，这里是兜底）
                if (_timerService.TimeLeft <= TimeSpan.Zero && _timerService.CurrentState == TimerState.Work)
                {
                    AutoCloseForm();
                }
            }
        }

        // ... Timer 事件处理保持不变 ...
        private void TimerService_TimeUpdated(object? sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<object?, EventArgs>(TimerService_TimeUpdated), sender, e);
                return;
            }
            var t = _timerService.TimeLeft;
            if (lblTimer != null)
                lblTimer.Text = $"{(int)t.TotalMinutes:00}:{t.Seconds:00}";
            CheckBreakTimeEnded();
        }

        private void AutoCloseForm()
        {
            if (!_isAutoClosed && !this.IsDisposed)
            {
                _isAutoClosed = true;
                if (this.InvokeRequired)
                    this.Invoke(new Action(() => this.Close()));
                else
                    this.Close();
            }
        }

        private void BreakForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            _timerService.TimeUpdated -= TimerService_TimeUpdated;
            _timerService.TimerStateChanged -= TimerService_TimerStateChanged;
        }
    }
}
