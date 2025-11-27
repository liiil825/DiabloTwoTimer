using DTwoMFTimerHelper.UI.Components;

namespace DTwoMFTimerHelper.UI.Pomodoro;
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
        btnPomodoroReset = new System.Windows.Forms.Button();
        btnStartPomodoro = new System.Windows.Forms.Button();
        btnPomodoroSettings = new System.Windows.Forms.Button();
        lblPomodoroTime = new PomodoroTimeDisplayLabel();
        pomodoroStatusDisplay1 = new PomodoroStatusDisplay();
        SuspendLayout();
        // 
        // btnPomodoroReset
        // 
        btnPomodoroReset.Location = new System.Drawing.Point(111, 385);
        btnPomodoroReset.Name = "btnPomodoroReset";
        btnPomodoroReset.Size = new System.Drawing.Size(205, 75);
        btnPomodoroReset.TabIndex = 4;
        btnPomodoroReset.Text = "重置";
        btnPomodoroReset.UseVisualStyleBackColor = true;
        btnPomodoroReset.Click += BtnPomodoroReset_Click;
        // 
        // btnStartPomodoro
        // 
        btnStartPomodoro.Location = new System.Drawing.Point(111, 171);
        btnStartPomodoro.Name = "btnStartPomodoro";
        btnStartPomodoro.Size = new System.Drawing.Size(205, 75);
        btnStartPomodoro.TabIndex = 2;
        btnStartPomodoro.Text = "开始";
        btnStartPomodoro.UseVisualStyleBackColor = true;
        btnStartPomodoro.Click += BtnStartPomodoro_Click;
        // 
        // btnPomodoroSettings
        // 
        btnPomodoroSettings.Location = new System.Drawing.Point(111, 279);
        btnPomodoroSettings.Name = "btnPomodoroSettings";
        btnPomodoroSettings.Size = new System.Drawing.Size(205, 75);
        btnPomodoroSettings.TabIndex = 3;
        btnPomodoroSettings.Text = "设置";
        btnPomodoroSettings.UseVisualStyleBackColor = true;
        btnPomodoroSettings.Click += BtnPomodoroSettings_Click;
        // 
        // lblPomodoroTime
        // 
        lblPomodoroTime.AutoSize = true;
        lblPomodoroTime.Font = new System.Drawing.Font("微软雅黑", 16F, System.Drawing.FontStyle.Bold);
        lblPomodoroTime.Location = new System.Drawing.Point(111, 29);
        lblPomodoroTime.Name = "lblPomodoroTime";
        lblPomodoroTime.ShowMilliseconds = true;
        lblPomodoroTime.Size = new System.Drawing.Size(216, 50);
        lblPomodoroTime.TabIndex = 0;
        lblPomodoroTime.Text = "25:00:00:0";
        // 
        // pomodoroStatusDisplay1
        // 
        this.pomodoroStatusDisplay1.BackColor = System.Drawing.Color.Transparent; // 修改1：透明背景
        this.pomodoroStatusDisplay1.BigIconImage = null;
        this.pomodoroStatusDisplay1.IconSize = 28;
        this.pomodoroStatusDisplay1.IconSpacing = 6;
        // 修改2 & 3：位置从0开始，宽度占满父容器 (549是父容器宽度)
        this.pomodoroStatusDisplay1.Location = new System.Drawing.Point(0, 100);
        this.pomodoroStatusDisplay1.Name = "pomodoroStatusDisplay1";
        this.pomodoroStatusDisplay1.Size = new System.Drawing.Size(549, 40);
        this.pomodoroStatusDisplay1.SmallIconImage = null;
        this.pomodoroStatusDisplay1.TabIndex = 6;
        this.pomodoroStatusDisplay1.TotalCompletedCount = 0;
        // 
        // PomodoroControl
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        Controls.Add(pomodoroStatusDisplay1);
        Controls.Add(btnPomodoroSettings);
        Controls.Add(btnPomodoroReset);
        Controls.Add(btnStartPomodoro);
        Controls.Add(lblPomodoroTime);
        Margin = new System.Windows.Forms.Padding(6);
        Name = "PomodoroControl";
        Size = new System.Drawing.Size(549, 562);
        ResumeLayout(false);
        PerformLayout();
    }
    #endregion

    private System.Windows.Forms.Button btnPomodoroReset;
    private System.Windows.Forms.Button btnStartPomodoro;
    private System.Windows.Forms.Button btnPomodoroSettings;
    private PomodoroTimeDisplayLabel lblPomodoroTime;
    private DTwoMFTimerHelper.UI.Components.PomodoroStatusDisplay pomodoroStatusDisplay1;
}
