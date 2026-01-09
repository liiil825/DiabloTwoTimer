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
        this.grpDisplay = new DiabloTwoMFTimer.UI.Components.ThemedGroupBox();
        this.tlpDisplay = new System.Windows.Forms.TableLayoutPanel();
        this.chkShowTime = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();
        this.chkShowStats = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();
        this.chkShowHistory = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();
        this.chkShowLoot = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();
        this.chkShowAccount = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();

        // 平均数统计行容器
        this.pnlAvg = new System.Windows.Forms.FlowLayoutPanel();
        this.lblAvgCount = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.txtAvgCount = new DiabloTwoMFTimer.UI.Components.ThemedTextBox();

        this.lblAvgCount = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.numAvgCount = new System.Windows.Forms.NumericUpDown();
        this.grpTimerSettings = new DiabloTwoMFTimer.UI.Components.ThemedGroupBox();
        this.flpTimerParams = new System.Windows.Forms.FlowLayoutPanel();

        this.chkShowPomodoro = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();
        this.chkSyncStartPomodoro = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();
        this.chkSyncPausePomodoro = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();
        this.chkGenerateRoomName = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();
        this.chkScreenshotOnLoot = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();
        this.chkHideWindowOnScreenshot = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();

        this.tlpMain.SuspendLayout();
        this.grpTimerSettings.SuspendLayout();
        this.flpTimerParams.SuspendLayout();
        this.SuspendLayout();

        this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;

        // 
        // tlpMain
        // 
        this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpMain.ColumnCount = 1;
        this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpMain.Controls.Add(this.grpDisplay, 0, 0);
        this.tlpMain.RowCount = 2; // 假设后面还有别的
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpMain.Padding = new System.Windows.Forms.Padding(0);

        // 
        // grpDisplay
        // 
        this.grpDisplay.Dock = System.Windows.Forms.DockStyle.Top;
        this.grpDisplay.AutoSize = true;
        this.grpDisplay.Controls.Add(this.tlpDisplay);
        this.grpDisplay.Name = "grpDisplay";
        this.grpDisplay.Padding = new System.Windows.Forms.Padding(10); // 增加内边距
        this.grpDisplay.Text = "Display Settings";

        // 
        // tlpDisplay
        // 
        this.tlpDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpDisplay.AutoSize = true;
        this.tlpDisplay.ColumnCount = 1;
        this.tlpDisplay.RowCount = 7;
        this.tlpDisplay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

        // 添加控件，增加 Margin 保持间距
        System.Windows.Forms.Padding itemMargin = new System.Windows.Forms.Padding(3, 5, 3, 5);
        this.chkShowTime.Margin = itemMargin;
        this.chkShowStats.Margin = itemMargin;
        this.chkShowHistory.Margin = itemMargin;
        this.chkShowLoot.Margin = itemMargin;
        this.chkShowAccount.Margin = itemMargin;
        this.pnlAvg.Margin = itemMargin;

        this.tlpDisplay.Controls.Add(this.chkShowPomodoro, 0, 0);
        this.tlpDisplay.Controls.Add(this.chkShowTime, 0, 1);
        this.tlpDisplay.Controls.Add(this.chkShowStats, 0, 2);
        this.tlpDisplay.Controls.Add(this.chkShowHistory, 0, 3);
        this.tlpDisplay.Controls.Add(this.chkShowLoot, 0, 4);
        this.tlpDisplay.Controls.Add(this.chkShowAccount, 0, 5);
        this.tlpDisplay.Controls.Add(this.pnlAvg, 0, 6);

        // 
        // CheckBoxes (Properties)
        this.chkShowPomodoro.AutoSize = true;
        this.chkShowTime.AutoSize = true;
        this.chkShowStats.AutoSize = true;
        this.chkShowHistory.AutoSize = true;
        this.chkShowLoot.AutoSize = true;
        this.chkShowAccount.AutoSize = true;

        // 
        // pnlAvg
        // 
        this.pnlAvg.AutoSize = true;
        this.pnlAvg.Controls.Add(this.lblAvgCount);
        this.pnlAvg.Controls.Add(this.txtAvgCount);
        this.pnlAvg.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pnlAvg.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
        this.pnlAvg.WrapContents = false;

        // 
        // lblAvgCount
        // 
        this.lblAvgCount.AutoSize = true;
        this.lblAvgCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
        // this.lblAvgCount.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
        this.lblAvgCount.Margin = itemMargin;
        this.lblAvgCount.Text = "Average Run Count:";

        // 
        // txtAvgCount (Replacement for NumericUpDown)
        // 
        this.txtAvgCount.Size = new System.Drawing.Size(80, 30);
        this.txtAvgCount.Anchor = System.Windows.Forms.AnchorStyles.Left;

        // 添加到主布局 (假设放在最上面或者合适位置)
        this.tlpMain.Controls.Add(this.grpDisplay, 0, 0); // 放在第一位

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
        this.flpTimerParams.Controls.Add(this.chkSyncStartPomodoro);
        this.flpTimerParams.Controls.Add(this.chkSyncPausePomodoro);
        this.flpTimerParams.Controls.Add(this.chkGenerateRoomName);
        this.flpTimerParams.Controls.Add(this.chkScreenshotOnLoot);
        this.flpTimerParams.Controls.Add(this.chkHideWindowOnScreenshot);
        this.flpTimerParams.Dock = System.Windows.Forms.DockStyle.Fill;
        this.flpTimerParams.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        this.flpTimerParams.Location = new System.Drawing.Point(3, 20);
        this.flpTimerParams.Name = "flpTimerParams";
        this.flpTimerParams.Padding = new System.Windows.Forms.Padding(5);
        this.flpTimerParams.Size = new System.Drawing.Size(318, 227);
        this.flpTimerParams.TabIndex = 0;

        // 设置 Checkbox 属性
        void SetCheck(System.Windows.Forms.CheckBox chk, string text)
        {
            chk.AutoSize = true;
            chk.Margin = itemMargin;
            chk.Text = text;
        }

        SetCheck(chkSyncStartPomodoro, "同步开启番茄钟");
        SetCheck(chkSyncPausePomodoro, "同步暂停番茄钟");
        SetCheck(chkGenerateRoomName, "生成房间名称");
        SetCheck(chkScreenshotOnLoot, "掉落保存截图");
        SetCheck(chkHideWindowOnScreenshot, "截图隐藏界面");

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
    private DiabloTwoMFTimer.UI.Components.ThemedGroupBox grpDisplay;
    private System.Windows.Forms.TableLayoutPanel tlpDisplay;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkShowTime;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkShowStats;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkShowHistory;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkShowLoot;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkShowAccount;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblAvgCount;
    private System.Windows.Forms.NumericUpDown numAvgCount;

    private System.Windows.Forms.FlowLayoutPanel flpTimerParams;
    private DiabloTwoMFTimer.UI.Components.ThemedGroupBox grpTimerSettings;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkShowPomodoro;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkSyncStartPomodoro;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkSyncPausePomodoro;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkGenerateRoomName;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkScreenshotOnLoot;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkHideWindowOnScreenshot;
    private System.Windows.Forms.FlowLayoutPanel pnlAvg;
    private DiabloTwoMFTimer.UI.Components.ThemedTextBox txtAvgCount; // Changed
}