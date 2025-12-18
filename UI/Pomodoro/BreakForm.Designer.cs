using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;
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
        this.headerControl = new DiabloTwoMFTimer.UI.Components.ThemedWindowHeader();
        this.btnClose = new DiabloTwoMFTimer.UI.Components.ThemedModalButton();
        this.btnSkip = new DiabloTwoMFTimer.UI.Components.ThemedModalButton();

        this.lblMessage = new System.Windows.Forms.Label();
        this.lblTimer = new System.Windows.Forms.Label();
        this.pomodoroStatusDisplay = new DiabloTwoMFTimer.UI.Components.PomodoroStatusDisplay();
        this.lblDuration = new System.Windows.Forms.Label();
        this.lblStats = new System.Windows.Forms.Label();

        this.SuspendLayout();

        // 
        // headerControl
        // 
        this.headerControl.BackColor = System.Drawing.Color.Transparent;
        this.headerControl.Dock = System.Windows.Forms.DockStyle.Top;
        this.headerControl.Location = new System.Drawing.Point(0, 0);
        this.headerControl.Name = "headerControl";
        this.headerControl.Size = new System.Drawing.Size(1024, 100); // 高度由 SizeChanged 调整
        this.headerControl.TabIndex = 0;
        this.headerControl.Title = "TITLE";

        // 
        // btnClose
        // 
        this.btnClose.Name = "btnClose";
        this.btnClose.TabIndex = 6;
        this.btnClose.Text = "关闭";
        this.btnClose.SetThemeDanger();
        this.btnClose.Click += (s, e) => this.Close();

        // 
        // btnSkip
        // 
        this.btnSkip.Name = "btnSkip";
        this.btnSkip.TabIndex = 7;
        this.btnSkip.Text = "结束休息";
        this.btnSkip.SetThemePrimary();
        this.btnSkip.Click += (s, e) =>
        {
            _timerService.SkipBreak();
            this.Close();
        };

        // 
        // lblMessage (提示语)
        // 
        this.lblMessage.AutoSize = false;
        this.lblMessage.BackColor = System.Drawing.Color.Transparent;
        this.lblMessage.Font = AppTheme.BigTitleFont;
        this.lblMessage.ForeColor = System.Drawing.Color.White;
        this.lblMessage.Location = new System.Drawing.Point(112, 120);
        this.lblMessage.Name = "lblMessage";
        this.lblMessage.Size = new System.Drawing.Size(800, 60);
        this.lblMessage.TabIndex = 1;
        this.lblMessage.Text = "休息一下";
        this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        // 
        // lblTimer (倒计时)
        // 
        this.lblTimer.AutoSize = true;
        this.lblTimer.BackColor = System.Drawing.Color.Transparent;
        this.lblTimer.Font = AppTheme.BigTimeFont;
        this.lblTimer.ForeColor = System.Drawing.Color.LightGreen;
        this.lblTimer.Location = new System.Drawing.Point(462, 200);
        this.lblTimer.Name = "lblTimer";
        this.lblTimer.Size = new System.Drawing.Size(100, 50);
        this.lblTimer.TabIndex = 2;
        this.lblTimer.Text = "00:00";
        this.lblTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        // 
        // pomodoroStatusDisplay
        // 
        this.pomodoroStatusDisplay.BackColor = System.Drawing.Color.Transparent;
        this.pomodoroStatusDisplay.IconSize = 24;
        this.pomodoroStatusDisplay.IconSpacing = 8;
        this.pomodoroStatusDisplay.Location = new System.Drawing.Point(312, 270);
        this.pomodoroStatusDisplay.Name = "pomodoroStatusDisplay";
        this.pomodoroStatusDisplay.Size = new System.Drawing.Size(400, 40);
        this.pomodoroStatusDisplay.SmallIconImage = null;
        this.pomodoroStatusDisplay.TabIndex = 3;
        this.pomodoroStatusDisplay.Text = "pomodoroStatusDisplay1";
        this.pomodoroStatusDisplay.TotalCompletedCount = 0;

        // 
        // lblDuration (总时长)
        // 
        this.lblDuration.AutoSize = true;
        this.lblDuration.BackColor = System.Drawing.Color.Transparent;
        this.lblDuration.Font = AppTheme.MainFont;
        this.lblDuration.ForeColor = System.Drawing.Color.LightGray;
        this.lblDuration.Location = new System.Drawing.Point(462, 330);
        this.lblDuration.Name = "lblDuration";
        this.lblDuration.Size = new System.Drawing.Size(100, 20);
        this.lblDuration.TabIndex = 4;
        this.lblDuration.Text = "总投入时间: --";
        this.lblDuration.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        // 
        // lblStats (统计文本)
        // 
        this.lblStats.AutoSize = false;
        this.lblStats.BackColor = System.Drawing.Color.Transparent;
        this.lblStats.Font = AppTheme.ConsoleFont;
        this.lblStats.ForeColor = System.Drawing.Color.Gold;
        this.lblStats.Location = new System.Drawing.Point(112, 370);
        this.lblStats.Name = "lblStats";
        this.lblStats.Size = new System.Drawing.Size(800, 300);
        this.lblStats.TabIndex = 5;
        this.lblStats.Text = "Loading...";
        this.lblStats.TextAlign = System.Drawing.ContentAlignment.TopCenter;

        // 
        // BreakForm
        // 
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
        this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
        this.ClientSize = new System.Drawing.Size(1024, 768);
        this.Controls.Add(this.btnSkip);
        this.Controls.Add(this.btnClose);
        this.Controls.Add(this.lblStats);
        this.Controls.Add(this.lblDuration);
        this.Controls.Add(this.pomodoroStatusDisplay);
        this.Controls.Add(this.lblTimer);
        this.Controls.Add(this.lblMessage);
        this.Controls.Add(this.headerControl);
        this.DoubleBuffered = true;
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        this.Name = "BreakForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "统计信息";
        this.TopMost = true;
        this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private ThemedWindowHeader headerControl;
    private ThemedModalButton btnClose;
    private ThemedModalButton btnSkip;

    private System.Windows.Forms.Label lblMessage;
    private System.Windows.Forms.Label lblTimer;
    private DiabloTwoMFTimer.UI.Components.PomodoroStatusDisplay pomodoroStatusDisplay;
    private System.Windows.Forms.Label lblDuration;
    private System.Windows.Forms.Label lblStats;
}