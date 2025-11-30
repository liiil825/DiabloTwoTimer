using System.Drawing;
using DiabloTwoMFTimer.UI.Components;

namespace DiabloTwoMFTimer.UI.Pomodoro;
partial class PomodoroControl
{
    /// <summary> 
    /// 必需的设计器变量。
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    /// <summary> 
    /// 清理所有正在使用的资源。
    /// </summary>
    /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region 组件设计器生成的代码

    /// <summary> 
    /// 设计器支持所需的方法 - 不要修改
    /// 使用代码编辑器修改此方法的内容。
    /// </summary>
    private void InitializeComponent()
    {
        // 1. 初始化控件
        // 使用 TableLayout 进行垂直居中布局
        this.mainLayout = new System.Windows.Forms.TableLayoutPanel();

        // 替换为 ThemedButton
        this.btnStartPomodoro = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnPomodoroSettings = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnPomodoroReset = new DiabloTwoMFTimer.UI.Components.ThemedButton();

        // 保持自定义控件
        this.lblPomodoroTime = new DiabloTwoMFTimer.UI.Pomodoro.PomodoroTimeDisplayLabel();
        this.pomodoroStatusDisplay1 = new DiabloTwoMFTimer.UI.Components.PomodoroStatusDisplay();

        this.SuspendLayout();

        // ---------------------------------------------------------
        // 1. 主布局容器 (mainLayout)
        // ---------------------------------------------------------
        this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
        this.mainLayout.ColumnCount = 1;
        this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

        // 定义行：我们使用 "上弹簧 - 内容 - 下弹簧" 的结构来实现垂直居中
        this.mainLayout.RowCount = 7;

        // Row 0: 顶部弹簧 (占 50% 剩余空间)
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));

        // Row 1: 时间显示 (固定高度)
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));

        // Row 2: 番茄状态 (小番茄图标)
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));

        // Row 3: 开始按钮
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));

        // Row 4: 设置按钮
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));

        // Row 5: 重置按钮
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));

        // Row 6: 底部弹簧 (占 50% 剩余空间)
        this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));

        // 添加控件 (注意 Row 索引)
        this.mainLayout.Controls.Add(this.lblPomodoroTime, 0, 1);
        this.mainLayout.Controls.Add(this.pomodoroStatusDisplay1, 0, 2);
        this.mainLayout.Controls.Add(this.btnStartPomodoro, 0, 3);
        this.mainLayout.Controls.Add(this.btnPomodoroSettings, 0, 4);
        this.mainLayout.Controls.Add(this.btnPomodoroReset, 0, 5);

        // ---------------------------------------------------------
        // 2. 控件样式设置
        // ---------------------------------------------------------

        // 时间显示
        this.lblPomodoroTime.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblPomodoroTime.AutoSize = false; // 必须关掉 AutoSize 才能居中
        this.lblPomodoroTime.TextAlign = System.Drawing.ContentAlignment.BottomCenter; // 靠下一点，接近图标
        this.lblPomodoroTime.Font = new System.Drawing.Font("Microsoft YaHei UI", 24F, System.Drawing.FontStyle.Bold);
        // 颜色由 PomodoroTimeDisplayLabel 内部逻辑控制，但我们可以给个默认深色背景下的亮色
        this.lblPomodoroTime.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.TextColor;
        this.lblPomodoroTime.Text = "25:00";

        // 状态图标 (小番茄)
        this.pomodoroStatusDisplay1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pomodoroStatusDisplay1.BackColor = System.Drawing.Color.Transparent;
        // IconSize 和 Spacing 可以在这里微调
        this.pomodoroStatusDisplay1.IconSize = 24;

        // 按钮通用设置 (居中、大小)
        Size btnSize = new System.Drawing.Size(200, 45);

        // 开始按钮
        this.btnStartPomodoro.Text = "开始";
        this.btnStartPomodoro.Size = btnSize;
        this.btnStartPomodoro.Anchor = System.Windows.Forms.AnchorStyles.None; // 居中
        this.btnStartPomodoro.Click += new System.EventHandler(this.BtnStartPomodoro_Click);

        // 设置按钮
        this.btnPomodoroSettings.Text = "设置";
        this.btnPomodoroSettings.Size = btnSize;
        this.btnPomodoroSettings.Anchor = System.Windows.Forms.AnchorStyles.None;
        this.btnPomodoroSettings.Click += new System.EventHandler(this.BtnPomodoroSettings_Click);

        // 重置按钮
        this.btnPomodoroReset.Text = "重置";
        this.btnPomodoroReset.Size = btnSize;
        this.btnPomodoroReset.Anchor = System.Windows.Forms.AnchorStyles.None;
        this.btnPomodoroReset.Click += new System.EventHandler(this.BtnPomodoroReset_Click);

        // 
        // PomodoroControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor; // 全局深色背景
        this.Controls.Add(this.mainLayout);
        this.Name = "PomodoroControl";
        this.Size = new System.Drawing.Size(549, 600);

        this.ResumeLayout(false);
        this.PerformLayout();
    }

    // 记得在类中添加这个字段
    private System.Windows.Forms.TableLayoutPanel mainLayout;

    // 修改现有的字段类型定义 (从 Button 改为 ThemedButton)
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnPomodoroReset;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnStartPomodoro;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnPomodoroSettings;
    #endregion

    private PomodoroTimeDisplayLabel lblPomodoroTime;
    private DiabloTwoMFTimer.UI.Components.PomodoroStatusDisplay pomodoroStatusDisplay1;
}
