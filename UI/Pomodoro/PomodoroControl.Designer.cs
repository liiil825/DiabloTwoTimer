using System.Drawing;
using DiabloTwoMFTimer.UI.Components;

namespace DiabloTwoMFTimer.UI.Pomodoro;
partial class PomodoroControl
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

    #region 组件设计器生成的代码

    private void InitializeComponent()
    {
        this.mainLayout = new System.Windows.Forms.TableLayoutPanel();

        // 按钮容器
        this.tlpButtons = new System.Windows.Forms.TableLayoutPanel();

        // 原有按钮
        this.btnStartPomodoro = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnPomodoroSettings = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnPomodoroReset = new DiabloTwoMFTimer.UI.Components.ThemedButton();

        // 新增按钮
        this.btnNextState = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnAddMinute = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnShowStats = new DiabloTwoMFTimer.UI.Components.ThemedButton();

        // 标签
        this.lblPomodoroTime = new DiabloTwoMFTimer.UI.Pomodoro.PomodoroTimeDisplayLabel();
        this.pomodoroStatusDisplay1 = new DiabloTwoMFTimer.UI.Components.PomodoroStatusDisplay();

        this.mainLayout.SuspendLayout();
        this.tlpButtons.SuspendLayout();
        this.SuspendLayout();

        // ---------------------------------------------------------
        // 1. 主布局容器 (mainLayout)
        // ---------------------------------------------------------
        this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
        this.mainLayout.ColumnCount = 1;
        this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

        // 定义行：Spring - Time - Status - ButtonArea - Spring
        this.mainLayout.RowCount = 5;
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F)); // Top Spring
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F)); // Time
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));  // Status
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));       // Buttons
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F)); // Bottom Spring

        this.mainLayout.Controls.Add(this.lblPomodoroTime, 0, 1);
        this.mainLayout.Controls.Add(this.pomodoroStatusDisplay1, 0, 2);
        this.mainLayout.Controls.Add(this.tlpButtons, 0, 3);

        // ---------------------------------------------------------
        // 2. 按钮组布局 (tlpButtons) - 3行2列
        // ---------------------------------------------------------
        this.tlpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpButtons.AutoSize = true;
        this.tlpButtons.ColumnCount = 2;
        this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));

        this.tlpButtons.RowCount = 3;
        this.tlpButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
        this.tlpButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
        this.tlpButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));

        // Row 0: Start/Pause | Skip/Next
        this.tlpButtons.Controls.Add(this.btnStartPomodoro, 0, 0);
        this.tlpButtons.Controls.Add(this.btnNextState, 1, 0);

        // Row 1: +1 Min | Stats
        this.tlpButtons.Controls.Add(this.btnAddMinute, 0, 1);
        this.tlpButtons.Controls.Add(this.btnShowStats, 1, 1);

        // Row 2: Settings | Reset
        this.tlpButtons.Controls.Add(this.btnPomodoroSettings, 0, 2);
        this.tlpButtons.Controls.Add(this.btnPomodoroReset, 1, 2);

        // ---------------------------------------------------------
        // 3. 样式调整
        // ---------------------------------------------------------
        // 时间显示
        this.lblPomodoroTime.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblPomodoroTime.AutoSize = false;
        this.lblPomodoroTime.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
        this.lblPomodoroTime.Font = new System.Drawing.Font("Microsoft YaHei UI", 24F, System.Drawing.FontStyle.Bold);
        this.lblPomodoroTime.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.TextColor;
        this.lblPomodoroTime.Text = "25:00";

        // 状态图标
        this.pomodoroStatusDisplay1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pomodoroStatusDisplay1.BackColor = System.Drawing.Color.Transparent;
        this.pomodoroStatusDisplay1.IconSize = 24;

        // 按钮统一样式
        Size btnSize = new System.Drawing.Size(140, 45); // 稍微变窄以适应两列

        void InitBtn(ThemedButton btn, string text)
        {
            btn.Text = text;
            btn.Size = btnSize;
            btn.Anchor = System.Windows.Forms.AnchorStyles.None;
        }

        InitBtn(this.btnStartPomodoro, "Start");
        this.btnStartPomodoro.Click += new System.EventHandler(this.BtnStartPomodoro_Click);

        InitBtn(this.btnNextState, "Skip");
        // 点击事件在主代码中绑定

        InitBtn(this.btnAddMinute, "+1 Min");
        // 点击事件在主代码中绑定

        InitBtn(this.btnShowStats, "Stats");
        // 点击事件在主代码中绑定

        InitBtn(this.btnPomodoroSettings, "Settings");
        this.btnPomodoroSettings.Click += new System.EventHandler(this.BtnPomodoroSettings_Click);

        InitBtn(this.btnPomodoroReset, "Reset");
        this.btnPomodoroReset.Click += new System.EventHandler(this.BtnPomodoroReset_Click);

        // 
        // PomodoroControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.Controls.Add(this.mainLayout);
        this.Name = "PomodoroControl";
        this.Size = new System.Drawing.Size(549, 600);

        this.mainLayout.ResumeLayout(false);
        this.mainLayout.PerformLayout();
        this.tlpButtons.ResumeLayout(false);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.TableLayoutPanel mainLayout;
    private System.Windows.Forms.TableLayoutPanel tlpButtons;

    private DiabloTwoMFTimer.UI.Components.ThemedButton btnPomodoroReset;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnStartPomodoro;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnPomodoroSettings;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnNextState;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnAddMinute;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnShowStats;

    private PomodoroTimeDisplayLabel lblPomodoroTime;
    private DiabloTwoMFTimer.UI.Components.PomodoroStatusDisplay pomodoroStatusDisplay1;
    #endregion
}