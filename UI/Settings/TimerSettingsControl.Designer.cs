namespace DiabloTwoMFTimer.UI.Settings;

partial class TimerSettingsControl
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

    #region Component Designer generated code

    private void InitializeComponent()
    {
        this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
        this.grpTimerSettings = new DiabloTwoMFTimer.UI.Components.ThemedGroupBox();
        this.flpTimerParams = new System.Windows.Forms.FlowLayoutPanel();

        this.chkShowPomodoro = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();
        this.chkShowLootDrops = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();
        this.chkSyncStartPomodoro = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();
        this.chkSyncPausePomodoro = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();
        this.chkGenerateRoomName = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();

        this.tlpMain.SuspendLayout();
        this.grpTimerSettings.SuspendLayout();
        this.flpTimerParams.SuspendLayout();
        this.SuspendLayout();

        this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;

        // 
        // tlpMain
        // 
        this.tlpMain.ColumnCount = 1;
        this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpMain.Controls.Add(this.grpTimerSettings, 0, 0);
        this.tlpMain.Dock = System.Windows.Forms.DockStyle.Top; // 改为 Top
        this.tlpMain.AutoSize = true; // 开启自动大小
        this.tlpMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpMain.Location = new System.Drawing.Point(0, 0);
        this.tlpMain.Name = "tlpMain";
        this.tlpMain.Padding = new System.Windows.Forms.Padding(10);
        this.tlpMain.RowCount = 2;
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpMain.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, Theme.UISizeConstants.SettingTabPageHeight);
        this.tlpMain.TabIndex = 0;

        // 
        // grpTimerSettings
        // 
        this.grpTimerSettings.AutoSize = true;
        this.grpTimerSettings.Controls.Add(this.flpTimerParams);
        this.grpTimerSettings.Dock = System.Windows.Forms.DockStyle.Fill;
        this.grpTimerSettings.Location = new System.Drawing.Point(13, 13);
        this.grpTimerSettings.Name = "grpTimerSettings";
        this.grpTimerSettings.Padding = new System.Windows.Forms.Padding(3, 20, 3, 3);
        this.grpTimerSettings.Size = new System.Drawing.Size(324, 250);
        this.grpTimerSettings.TabIndex = 0;
        this.grpTimerSettings.TabStop = false;
        this.grpTimerSettings.Text = "计时器设置";

        // 
        // flpTimerParams
        // 
        this.flpTimerParams.AutoSize = true;
        this.flpTimerParams.Controls.Add(this.chkShowPomodoro);
        this.flpTimerParams.Controls.Add(this.chkShowLootDrops);
        this.flpTimerParams.Controls.Add(this.chkSyncStartPomodoro);
        this.flpTimerParams.Controls.Add(this.chkSyncPausePomodoro);
        this.flpTimerParams.Controls.Add(this.chkGenerateRoomName);
        this.flpTimerParams.Dock = System.Windows.Forms.DockStyle.Fill;
        this.flpTimerParams.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        this.flpTimerParams.Location = new System.Drawing.Point(3, 20);
        this.flpTimerParams.Name = "flpTimerParams";
        this.flpTimerParams.Padding = new System.Windows.Forms.Padding(5);
        this.flpTimerParams.Size = new System.Drawing.Size(318, 227);
        this.flpTimerParams.TabIndex = 0;

        // 设置 Checkbox 属性
        void SetCheck(System.Windows.Forms.CheckBox chk, string text, System.EventHandler handler)
        {
            chk.AutoSize = true;
            chk.Margin = new System.Windows.Forms.Padding(5, 10, 5, 10);
            chk.Text = text;
            if (handler != null) chk.CheckedChanged += handler;
        }

        SetCheck(chkShowPomodoro, "是否显示番茄钟", this.OnShowPomodoroChanged);
        SetCheck(chkShowLootDrops, "是否展示掉落", this.OnShowLootDropsChanged);
        SetCheck(chkSyncStartPomodoro, "同步开启番茄钟", this.OnSyncStartPomodoroChanged);
        SetCheck(chkSyncPausePomodoro, "同步暂停番茄钟", this.OnSyncPausePomodoroChanged);
        SetCheck(chkGenerateRoomName, "生成房间名称", this.OnGenerateRoomNameChanged);

        // 
        // TimerSettingsControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.AutoScroll = true;
        this.Controls.Add(this.tlpMain);
        this.Name = "TimerSettingsControl";
        this.Size = new System.Drawing.Size(350, 420);

        this.tlpMain.ResumeLayout(false);
        this.tlpMain.PerformLayout();
        this.grpTimerSettings.ResumeLayout(false);
        this.grpTimerSettings.PerformLayout();
        this.flpTimerParams.ResumeLayout(false);
        this.flpTimerParams.PerformLayout();
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tlpMain;
    private System.Windows.Forms.FlowLayoutPanel flpTimerParams;
    private DiabloTwoMFTimer.UI.Components.ThemedGroupBox grpTimerSettings;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkShowPomodoro;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkShowLootDrops;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkSyncStartPomodoro;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkSyncPausePomodoro;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkGenerateRoomName;
}