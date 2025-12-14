using System.Drawing;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.Utils;

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
        UnSubscribeEvents();
        UnSubscribeToMessages();
        base.Dispose(disposing);
    }

    #region 组件设计器生成的代码

    private void InitializeComponent()
    {
        this.mainLayout = new System.Windows.Forms.TableLayoutPanel();

        // 按钮容器
        this.tlpButtons = new System.Windows.Forms.TableLayoutPanel();

        this.btnStartPomodoro = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnPomodoroSettings = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnPomodoroReset = new DiabloTwoMFTimer.UI.Components.ThemedButton();
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
        // 【修改 2】调整顶部弹簧比例：从 30% -> 40%，把时间往下推，不再贴顶
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F)); // Time
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));  // Status
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));       // Buttons (自适应)
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F)); // Bottom Spring

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

        // 【修改 1 & 3】
        // 1. 减小容器内边距 (Padding)：30 -> 15。这会让按钮自动变宽。
        this.tlpButtons.Padding = new System.Windows.Forms.Padding(ScaleHelper.Scale(15), 0, ScaleHelper.Scale(15), 0);

        this.tlpButtons.RowCount = 3;
        // 【修改 1】行高改为 AutoSize，不强制指定高度。
        // 这样按钮的高度就会由组件自身 (ThemedButton 默认为 40px) 决定，与档案页保持一致。
        this.tlpButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

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

        // 【修改】按钮统一样式逻辑
        void InitBtn(ThemedButton btn, string text)
        {
            btn.Text = text;

            // 1. 使用 Dock.Fill 填满格子
            btn.Dock = System.Windows.Forms.DockStyle.Fill;

            // 2. 移除 Anchor 和固定 Size，完全交给布局管理器
            // btn.Size = ... (移除)

            // 3. 设置 Margin 控制间距
            // 左右间距设小 (Scale(3)) -> 让两个按钮靠得更近
            // 上下间距设大 (Scale(8)) -> 拉开行距
            btn.Margin = new System.Windows.Forms.Padding(
                ScaleHelper.Scale(3), // Left (更紧凑)
                ScaleHelper.Scale(8), // Top (更宽松)
                ScaleHelper.Scale(3), // Right
                ScaleHelper.Scale(8)  // Bottom
            );
        }

        InitBtn(this.btnStartPomodoro, "Start");
        this.btnStartPomodoro.Click += new System.EventHandler(this.BtnStartPomodoro_Click);

        InitBtn(this.btnNextState, "Skip");

        InitBtn(this.btnAddMinute, "+1 Min");

        InitBtn(this.btnShowStats, "Stats");

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