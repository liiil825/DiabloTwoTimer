#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DTwoMFTimerHelper.UI.Pomodoro;
partial class BreakForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
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
            Height = 100,
            BackColor = Color.Transparent,
        };

        lblTitle = new Label
        {
            AutoSize = false,
            Size = new Size(300, 40),
            Font = new Font("微软雅黑", 14F, FontStyle.Bold),
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
            Location = new Point(20, 60), // 在标题下方
        };

        btnToggleSession = CreateToggleButton("本轮战况", StatViewType.Session);
        btnToggleToday = CreateToggleButton("今日累计", StatViewType.Today);
        btnToggleWeek = CreateToggleButton("本周累计", StatViewType.Week);

        pnlToggles.Controls.Add(btnToggleSession);
        pnlToggles.Controls.Add(btnToggleToday);
        pnlToggles.Controls.Add(btnToggleWeek);

        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(pnlToggles);

        // 2. 提示语 (仅休息模式)
        lblMessage = new Label
        {
            AutoSize = false,
            Size = new Size(800, 80),
            Font = new Font("微软雅黑", 24F, FontStyle.Bold),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "休息一下",
        };

        // 3. 统计内容 (核心区域)
        lblStats = new Label
        {
            AutoSize = false,
            Size = new Size(800, 400),
            Font = new Font("Consolas", 12F),
            ForeColor = Color.Gold,
            TextAlign = ContentAlignment.TopCenter,
            Text = "Loading...",
        };

        // 4. 倒计时
        lblTimer = new Label
        {
            AutoSize = true,
            Font = new Font("Consolas", 20F, FontStyle.Bold),
            ForeColor = Color.LightGreen,
            TextAlign = ContentAlignment.MiddleCenter,
            Text = "00:00",
        };

        // 5. 底部按钮
        btnClose = CreateFlatButton("关闭", Color.IndianRed);
        btnClose.Click += (s, e) => this.Close();

        btnSkip = CreateFlatButton("跳过休息", Color.SteelBlue);
        btnSkip.Click += (s, e) =>
        {
            _timerService.SkipBreak();
            this.Close();
        };

        // 添加控件
        this.Controls.Add(pnlHeader);
        this.Controls.Add(lblMessage);
        this.Controls.Add(lblStats);
        this.Controls.Add(lblTimer);
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
    private Label lblStats;
    private Label lblTimer;
    private Button btnClose;
    private Button btnSkip;
}