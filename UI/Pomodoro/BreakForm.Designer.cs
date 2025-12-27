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
        this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
        this.headerControl = new DiabloTwoMFTimer.UI.Components.ThemedWindowHeader();
        this.lblMessage = new System.Windows.Forms.Label();
        this.lblTimer = new System.Windows.Forms.Label();
        this.pomodoroStatusDisplay = new DiabloTwoMFTimer.UI.Components.PomodoroStatusDisplay();
        this.lblDuration = new System.Windows.Forms.Label();

        // 新增容器 Panel
        this.panelStatsContainer = new System.Windows.Forms.Panel();
        this.rtbStats = new System.Windows.Forms.RichTextBox();

        this.panelButtons = new System.Windows.Forms.FlowLayoutPanel();
        this.btnSkip = new DiabloTwoMFTimer.UI.Components.ThemedModalButton();
        this.btnClose = new DiabloTwoMFTimer.UI.Components.ThemedModalButton();

        this.mainLayout.SuspendLayout();
        this.panelStatsContainer.SuspendLayout(); // 挂起 Panel 布局
        this.panelButtons.SuspendLayout();
        this.SuspendLayout();

        // 
        // mainLayout
        // 
        this.mainLayout.ColumnCount = 1;
        this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

        this.mainLayout.Controls.Add(this.headerControl, 0, 0);
        this.mainLayout.Controls.Add(this.lblMessage, 0, 1);
        this.mainLayout.Controls.Add(this.lblTimer, 0, 2);
        this.mainLayout.Controls.Add(this.pomodoroStatusDisplay, 0, 3);
        this.mainLayout.Controls.Add(this.lblDuration, 0, 4);
        // Row 5: 放入容器 Panel，而不是直接放 RTB
        this.mainLayout.Controls.Add(this.panelStatsContainer, 0, 5);
        this.mainLayout.Controls.Add(this.panelButtons, 0, 6);

        this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
        this.mainLayout.Location = new System.Drawing.Point(0, 0);
        this.mainLayout.Name = "mainLayout";
        this.mainLayout.RowCount = 7;

        // RowStyles (初始值，会在代码中 ApplyScaledLayout 覆盖)
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

        this.mainLayout.TabIndex = 0;

        // ... (Header, Labels, StatusDisplay 代码保持不变) ...
        // 为了节省篇幅，省略未变动的控件代码，重点看下面 Panel 和 RTB

        // 
        // headerControl
        // 
        this.headerControl.BackColor = System.Drawing.Color.Transparent;
        this.headerControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.headerControl.Location = new System.Drawing.Point(0, 0);
        this.headerControl.Margin = new System.Windows.Forms.Padding(0);
        this.headerControl.Name = "headerControl";
        this.headerControl.Size = new System.Drawing.Size(1024, 110);
        this.headerControl.TabIndex = 0;
        this.headerControl.Title = "TITLE";

        // lblMessage, lblTimer, pomodoroStatusDisplay, lblDuration ... (保持原样)
        this.lblMessage.AutoSize = true;
        this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblMessage.BackColor = System.Drawing.Color.Transparent;
        this.lblMessage.Font = AppTheme.BigTitleFont;
        this.lblMessage.ForeColor = System.Drawing.Color.White;
        this.lblMessage.Location = new System.Drawing.Point(3, 113);
        this.lblMessage.Name = "lblMessage";
        this.lblMessage.Size = new System.Drawing.Size(1018, 60);
        this.lblMessage.TabIndex = 1;
        this.lblMessage.Text = "休息一下";
        this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        this.lblTimer.AutoSize = true;
        this.lblTimer.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblTimer.BackColor = System.Drawing.Color.Transparent;
        this.lblTimer.Font = AppTheme.BigTimeFont;
        this.lblTimer.ForeColor = System.Drawing.Color.LightGreen;
        this.lblTimer.Location = new System.Drawing.Point(3, 173);
        this.lblTimer.Name = "lblTimer";
        this.lblTimer.Size = new System.Drawing.Size(1018, 50);
        this.lblTimer.TabIndex = 2;
        this.lblTimer.Text = "00:00";
        this.lblTimer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        this.pomodoroStatusDisplay.Anchor = System.Windows.Forms.AnchorStyles.None;
        this.pomodoroStatusDisplay.BackColor = System.Drawing.Color.Transparent;
        this.pomodoroStatusDisplay.IconSize = 24;
        this.pomodoroStatusDisplay.IconSpacing = 8;
        this.pomodoroStatusDisplay.Location = new System.Drawing.Point(312, 233);
        this.pomodoroStatusDisplay.Name = "pomodoroStatusDisplay";
        this.pomodoroStatusDisplay.Size = new System.Drawing.Size(400, 40);
        this.pomodoroStatusDisplay.TabIndex = 3;
        this.pomodoroStatusDisplay.TotalCompletedCount = 0;

        this.lblDuration.AutoSize = true;
        this.lblDuration.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblDuration.BackColor = System.Drawing.Color.Transparent;
        this.lblDuration.Font = AppTheme.MainFont;
        this.lblDuration.ForeColor = System.Drawing.Color.LightGray;
        this.lblDuration.Location = new System.Drawing.Point(3, 293);
        this.lblDuration.Name = "lblDuration";
        this.lblDuration.Size = new System.Drawing.Size(1018, 40);
        this.lblDuration.TabIndex = 4;
        this.lblDuration.Text = "总投入时间: --";
        this.lblDuration.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

        // 
        // panelStatsContainer (新容器)
        // 
        this.panelStatsContainer.BackColor = System.Drawing.Color.Transparent; // 透明，显示窗体背景
        this.panelStatsContainer.Controls.Add(this.rtbStats); // 将 RTB 加入 Panel
        this.panelStatsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panelStatsContainer.Location = new System.Drawing.Point(100, 333);
        this.panelStatsContainer.Name = "panelStatsContainer";
        this.panelStatsContainer.Size = new System.Drawing.Size(824, 332);
        this.panelStatsContainer.TabIndex = 5;
        // 注意：Margin 将在 ApplyScaledLayout 中设置

        // 
        // rtbStats (内容控件)
        // 
        this.rtbStats.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
        this.rtbStats.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.rtbStats.Dock = System.Windows.Forms.DockStyle.Fill; // 填满 Panel
        this.rtbStats.Font = AppTheme.ConsoleFont;
        this.rtbStats.ForeColor = System.Drawing.Color.Gold;
        this.rtbStats.Location = new System.Drawing.Point(0, 0);
        this.rtbStats.Name = "rtbStats";
        this.rtbStats.ReadOnly = true;
        this.rtbStats.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None; // 禁用原生滚动条
        this.rtbStats.Size = new System.Drawing.Size(824, 332);
        this.rtbStats.TabIndex = 0;
        this.rtbStats.Text = "Loading...";

        // 
        // panelButtons
        // 
        this.panelButtons.AutoSize = true;
        this.panelButtons.Anchor = System.Windows.Forms.AnchorStyles.None;
        this.panelButtons.BackColor = System.Drawing.Color.Transparent;
        this.panelButtons.Controls.Add(this.btnSkip);
        this.panelButtons.Controls.Add(this.btnClose);
        this.panelButtons.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        this.panelButtons.Location = new System.Drawing.Point(362, 680);
        this.panelButtons.Name = "panelButtons";
        this.panelButtons.Size = new System.Drawing.Size(300, 61);
        this.panelButtons.TabIndex = 6;
        this.panelButtons.WrapContents = false;

        // 
        // btnSkip
        // 
        this.btnSkip.Name = "btnSkip";
        this.btnSkip.Size = new System.Drawing.Size(120, 40);
        this.btnSkip.Padding = new Padding(ScaleHelper.Scale(5));
        this.btnSkip.Margin = new Padding(ScaleHelper.Scale(10), 0, ScaleHelper.Scale(10), ScaleHelper.Scale(60));
        this.btnSkip.Margin = new System.Windows.Forms.Padding(20);
        this.btnSkip.TabIndex = 0;
        this.btnSkip.Font = Theme.AppTheme.Fonts.SegoeIcon;
        this.btnSkip.Text = "";
        this.btnSkip.SetThemePrimary();
        this.btnSkip.Click += (s, e) =>
        {
            _timerService.SkipBreak();
            this.Close();
        };

        // 
        // btnClose
        // 
        this.btnClose.Name = "btnClose";
        this.btnClose.Size = new System.Drawing.Size(120, 40);
        this.btnClose.Padding = new Padding(ScaleHelper.Scale(5));
        this.btnClose.Margin = new Padding(ScaleHelper.Scale(10), 0, ScaleHelper.Scale(10), ScaleHelper.Scale(60));
        this.btnClose.TabIndex = 1;
        this.btnClose.Font = Theme.AppTheme.Fonts.SegoeIcon;
        this.btnClose.Text = "\uE711";
        this.btnClose.SetThemeDanger();
        this.btnClose.Click += (s, e) => this.Close();

        // 
        // BreakForm
        // 
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
        this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
        this.ClientSize = new System.Drawing.Size(1024, 768);
        this.Controls.Add(this.mainLayout);
        this.DoubleBuffered = true;
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        this.Name = "BreakForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "统计信息";
        this.TopMost = true;
        this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        this.mainLayout.ResumeLayout(false);
        this.mainLayout.PerformLayout();
        this.panelStatsContainer.ResumeLayout(false); // 恢复 Panel 布局
        this.panelButtons.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel mainLayout;
    private ThemedWindowHeader headerControl;
    private System.Windows.Forms.Label lblMessage;
    private System.Windows.Forms.Label lblTimer;
    private DiabloTwoMFTimer.UI.Components.PomodoroStatusDisplay pomodoroStatusDisplay;
    private System.Windows.Forms.Label lblDuration;

    // 这里的变化
    private System.Windows.Forms.Panel panelStatsContainer;
    private System.Windows.Forms.RichTextBox rtbStats;

    private System.Windows.Forms.FlowLayoutPanel panelButtons;
    private ThemedModalButton btnClose;
    private ThemedModalButton btnSkip;
}