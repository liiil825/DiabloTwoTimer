using System;
using System.Drawing;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Pomodoro;

partial class BreakForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(1024, 768);
        this.Name = "BreakForm";
        this.Text = "统计信息";

        // 1. 顶部 Header 区域
        pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 110,
            BackColor = Color.Transparent,
        };

        lblTitle = new Label
        {
            AutoSize = false,
            Size = new Size(300, 40),
            Font = Theme.AppTheme.TitleFont,
            ForeColor = Color.Gray,
            TextAlign = ContentAlignment.MiddleLeft,
            Text = _mode == BreakFormMode.PomodoroBreak ? "REST & RECOVER" : "STATISTICS",
            Location = new Point(20, 20),
        };

        // 切换按钮容器
        pnlToggles = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            BackColor = Color.Transparent,
            Location = new Point(20, 60),
        };

        btnToggleSession = CreateToggleButton("本轮战况", StatViewType.Session);
        btnToggleToday = CreateToggleButton("今日累计", StatViewType.Today);
        btnToggleWeek = CreateToggleButton("本周累计", StatViewType.Week);

        pnlToggles.Controls.Add(btnToggleSession);
        pnlToggles.Controls.Add(btnToggleToday);
        pnlToggles.Controls.Add(btnToggleWeek);

        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(pnlToggles);

        // 2. 提示语
        lblMessage = new Label
        {
            AutoSize = false,
            Size = new Size(800, 60), // 高度稍微减小
            Font = Theme.AppTheme.BigTitleFont,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "休息一下",
        };

        // 3. 倒计时 (字体加大，颜色醒目)
        lblTimer = new Label
        {
            AutoSize = true,
            Font = Theme.AppTheme.BigTimeFont,
            ForeColor = Color.LightGreen,
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "00:00",
        };

        // 4. 新增：番茄状态显示
        pomodoroStatusDisplay = new DiabloTwoMFTimer.UI.Components.PomodoroStatusDisplay
        {
            Size = new Size(400, 40),
            IconSize = 24,
            IconSpacing = 8
        };

        // 5. 新增：总时长标签
        lblDuration = new Label
        {
            AutoSize = true,
            Font = Theme.AppTheme.MainFont,
            ForeColor = Color.LightGray,
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "总投入时间: --",
        };

        // 6. 统计内容
        lblStats = new Label
        {
            AutoSize = false,
            Size = new Size(800, 400),
            Font = Theme.AppTheme.ConsoleFont,
            ForeColor = Color.Gold,
            TextAlign = ContentAlignment.TopCenter,
            Text = "Loading...",
        };

        // 7. 底部按钮
        btnClose = CreateFlatButton("关闭", Color.IndianRed);
        btnClose.Click += (s, e) => this.Close();

        btnSkip = CreateFlatButton("跳过休息", Color.SteelBlue);
        btnSkip.Click += (s, e) =>
        {
            _timerService.SkipBreak();
            this.Close();
        };

        // 添加控件 (顺序很重要，但这里主要靠 SizeChanged 手动布局)
        this.Controls.Add(pnlHeader);
        this.Controls.Add(lblMessage);
        this.Controls.Add(lblTimer);           // 调整顺序
        this.Controls.Add(pomodoroStatusDisplay); // 新增
        this.Controls.Add(lblDuration);        // 新增
        this.Controls.Add(lblStats);
        this.Controls.Add(btnSkip);
        this.Controls.Add(btnClose);

        this.FormClosing += BreakForm_FormClosing;
        this.SizeChanged += BreakForm_SizeChanged;
    }

    #endregion

    private Panel pnlHeader;
    private Label lblTitle;
    private FlowLayoutPanel pnlToggles;
    private Button btnToggleSession;
    private Button btnToggleToday;
    private Button btnToggleWeek;

    private Label lblMessage;
    private Label lblTimer;
    private DiabloTwoMFTimer.UI.Components.PomodoroStatusDisplay pomodoroStatusDisplay; // 新增
    private Label lblDuration; // 新增
    private Label lblStats;

    private Button btnClose;
    private Button btnSkip;
}