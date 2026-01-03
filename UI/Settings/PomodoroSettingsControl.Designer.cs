namespace DiabloTwoMFTimer.UI.Settings;

partial class PomodoroSettingsControl
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

    private void InitializeComponent()
    {
        this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
        this.grpPomodoroSettings = new DiabloTwoMFTimer.UI.Components.ThemedGroupBox();
        this.tlpMode = new System.Windows.Forms.TableLayoutPanel();
        this.lblMode = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.cmbMode = new DiabloTwoMFTimer.UI.Components.ThemedComboBox();
        this.tlpWorkTime = new System.Windows.Forms.TableLayoutPanel();
        this.lblWorkTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudWorkTimeMin = new System.Windows.Forms.NumericUpDown();
        this.lblWorkMinUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudWorkTimeSec = new System.Windows.Forms.NumericUpDown();
        this.lblWorkSecUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.tlpShortBreakTime = new System.Windows.Forms.TableLayoutPanel();
        this.lblShortBreakTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudShortBreakTimeMin = new System.Windows.Forms.NumericUpDown();
        this.lblShortBreakMinUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudShortBreakTimeSec = new System.Windows.Forms.NumericUpDown();
        this.lblShortBreakSecUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.tlpLongBreakTime = new System.Windows.Forms.TableLayoutPanel();
        this.lblLongBreakTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudLongBreakTimeMin = new System.Windows.Forms.NumericUpDown();
        this.lblLongBreakMinUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudLongBreakTimeSec = new System.Windows.Forms.NumericUpDown();
        this.lblLongBreakSecUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.tlpWarningLongTime = new System.Windows.Forms.TableLayoutPanel();
        this.lblWarningLongTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudWarningLongTime = new System.Windows.Forms.NumericUpDown();
        this.lblWarningLongTimeUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.tlpWarningShortTime = new System.Windows.Forms.TableLayoutPanel();
        this.lblWarningShortTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudWarningShortTime = new System.Windows.Forms.NumericUpDown();
        this.lblWarningShortTimeUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();

        ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeMin)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeSec)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeMin)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeSec)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeMin)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeSec)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudWarningLongTime)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudWarningShortTime)).BeginInit();
        this.tlpMain.SuspendLayout();
        this.grpPomodoroSettings.SuspendLayout();
        this.tlpMode.SuspendLayout();
        this.tlpWorkTime.SuspendLayout();
        this.tlpShortBreakTime.SuspendLayout();
        this.tlpLongBreakTime.SuspendLayout();
        this.tlpWarningLongTime.SuspendLayout();
        this.tlpWarningShortTime.SuspendLayout();
        this.SuspendLayout();

        // 
        // tlpMain
        // 
        this.tlpMain.AutoSize = true;
        this.tlpMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpMain.ColumnCount = 1;
        this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpMain.Controls.Add(this.grpPomodoroSettings, 0, 0);
        this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpMain.Location = new System.Drawing.Point(0, 0);
        this.tlpMain.Name = "tlpMain";
        this.tlpMain.Padding = new System.Windows.Forms.Padding(10);
        this.tlpMain.RowCount = 1;
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpMain.Size = new System.Drawing.Size(610, 350);
        this.tlpMain.TabIndex = 0;

        // 
        // grpPomodoroSettings
        // 
        this.grpPomodoroSettings.AutoSize = true;
        this.grpPomodoroSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.grpPomodoroSettings.Controls.Add(this.tlpMode);
        this.grpPomodoroSettings.Controls.Add(this.tlpWorkTime);
        this.grpPomodoroSettings.Controls.Add(this.tlpShortBreakTime);
        this.grpPomodoroSettings.Controls.Add(this.tlpLongBreakTime);
        this.grpPomodoroSettings.Controls.Add(this.tlpWarningLongTime);
        this.grpPomodoroSettings.Controls.Add(this.tlpWarningShortTime);
        this.grpPomodoroSettings.Dock = System.Windows.Forms.DockStyle.Fill;
        this.grpPomodoroSettings.Location = new System.Drawing.Point(13, 13);
        this.grpPomodoroSettings.Name = "grpPomodoroSettings";
        this.grpPomodoroSettings.Padding = new System.Windows.Forms.Padding(10);
        this.grpPomodoroSettings.Size = new System.Drawing.Size(584, 324);
        this.grpPomodoroSettings.TabIndex = 0;
        this.grpPomodoroSettings.TabStop = false;
        this.grpPomodoroSettings.Text = "番茄设置";

        // 
        // tlpMode
        // 
        this.tlpMode.AutoSize = true;
        this.tlpMode.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpMode.ColumnCount = 2;
        this.tlpMode.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpMode.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpMode.Controls.Add(this.lblMode, 0, 0);
        this.tlpMode.Controls.Add(this.cmbMode, 1, 0);
        this.tlpMode.Dock = System.Windows.Forms.DockStyle.Top;
        this.tlpMode.Location = new System.Drawing.Point(13, 13);
        this.tlpMode.Name = "tlpMode";
        this.tlpMode.RowCount = 1;
        this.tlpMode.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpMode.Size = new System.Drawing.Size(564, 40);
        this.tlpMode.TabIndex = 0;

        // 
        // lblMode
        // 
        this.lblMode.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblMode.AutoSize = true;
        this.lblMode.Location = new System.Drawing.Point(3, 10);
        this.lblMode.Name = "lblMode";
        this.lblMode.Size = new System.Drawing.Size(50, 20);
        this.lblMode.TabIndex = 0;
        this.lblMode.Text = "模式:";

        // 
        // cmbMode
        // 
        this.cmbMode.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.cmbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbMode.FormattingEnabled = true;
        this.cmbMode.Location = new System.Drawing.Point(59, 7);
        this.cmbMode.Name = "cmbMode";
        this.cmbMode.Size = new System.Drawing.Size(200, 28);
        this.cmbMode.Margin = new System.Windows.Forms.Padding(5, 5, 5, 15);
        this.cmbMode.TabIndex = 1;

        // 
        // tlpWorkTime
        // 
        this.tlpWorkTime.AutoSize = true;
        this.tlpWorkTime.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpWorkTime.ColumnCount = 5;
        this.tlpWorkTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpWorkTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
        this.tlpWorkTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpWorkTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
        this.tlpWorkTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpWorkTime.Controls.Add(this.lblWorkTime, 0, 0);
        this.tlpWorkTime.Controls.Add(this.nudWorkTimeMin, 1, 0);
        this.tlpWorkTime.Controls.Add(this.lblWorkMinUnit, 2, 0);
        this.tlpWorkTime.Controls.Add(this.nudWorkTimeSec, 3, 0);
        this.tlpWorkTime.Controls.Add(this.lblWorkSecUnit, 4, 0);
        this.tlpWorkTime.Dock = System.Windows.Forms.DockStyle.Top;
        this.tlpWorkTime.Location = new System.Drawing.Point(13, 59);
        this.tlpWorkTime.Name = "tlpWorkTime";
        this.tlpWorkTime.RowCount = 1;
        this.tlpWorkTime.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpWorkTime.Size = new System.Drawing.Size(564, 40);
        this.tlpWorkTime.TabIndex = 1;

        // 
        // lblWorkTime
        // 
        this.lblWorkTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblWorkTime.AutoSize = true;
        this.lblWorkTime.Location = new System.Drawing.Point(3, 10);
        this.lblWorkTime.Name = "lblWorkTime";
        this.lblWorkTime.Size = new System.Drawing.Size(80, 20);
        this.lblWorkTime.TabIndex = 0;
        this.lblWorkTime.Text = "工作时间:";

        // 
        // nudWorkTimeMin
        // 
        this.nudWorkTimeMin.Dock = System.Windows.Forms.DockStyle.Fill;
        this.nudWorkTimeMin.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.SurfaceColor;
        this.nudWorkTimeMin.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.TextColor;
        this.nudWorkTimeMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.nudWorkTimeMin.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
        this.nudWorkTimeMin.Margin = new System.Windows.Forms.Padding(5, 5, 5, 15);
        this.nudWorkTimeMin.Name = "nudWorkTimeMin";
        this.nudWorkTimeMin.Value = new decimal(new int[] { 25, 0, 0, 0 });

        // 
        // lblWorkMinUnit
        // 
        this.lblWorkMinUnit.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblWorkMinUnit.AutoSize = true;
        this.lblWorkMinUnit.Location = new System.Drawing.Point(189, 10);
        this.lblWorkMinUnit.Name = "lblWorkMinUnit";
        this.lblWorkMinUnit.Size = new System.Drawing.Size(30, 20);
        this.lblWorkMinUnit.TabIndex = 2;
        this.lblWorkMinUnit.Text = "分";

        // 
        // nudWorkTimeSec
        // 
        this.nudWorkTimeSec.Dock = System.Windows.Forms.DockStyle.Fill;
        this.nudWorkTimeSec.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.SurfaceColor;
        this.nudWorkTimeSec.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.TextColor;
        this.nudWorkTimeSec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.nudWorkTimeSec.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
        this.nudWorkTimeSec.Margin = new System.Windows.Forms.Padding(5, 5, 5, 15);
        this.nudWorkTimeSec.Name = "nudWorkTimeSec";

        // 
        // lblWorkSecUnit
        // 
        this.lblWorkSecUnit.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblWorkSecUnit.AutoSize = true;
        this.lblWorkSecUnit.Location = new System.Drawing.Point(325, 10);
        this.lblWorkSecUnit.Name = "lblWorkSecUnit";
        this.lblWorkSecUnit.Size = new System.Drawing.Size(30, 20);
        this.lblWorkSecUnit.TabIndex = 4;
        this.lblWorkSecUnit.Text = "秒";

        // 
        // tlpShortBreakTime
        // 
        this.tlpShortBreakTime.AutoSize = true;
        this.tlpShortBreakTime.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpShortBreakTime.ColumnCount = 5;
        this.tlpShortBreakTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpShortBreakTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
        this.tlpShortBreakTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpShortBreakTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
        this.tlpShortBreakTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpShortBreakTime.Controls.Add(this.lblShortBreakTime, 0, 0);
        this.tlpShortBreakTime.Controls.Add(this.nudShortBreakTimeMin, 1, 0);
        this.tlpShortBreakTime.Controls.Add(this.lblShortBreakMinUnit, 2, 0);
        this.tlpShortBreakTime.Controls.Add(this.nudShortBreakTimeSec, 3, 0);
        this.tlpShortBreakTime.Controls.Add(this.lblShortBreakSecUnit, 4, 0);
        this.tlpShortBreakTime.Dock = System.Windows.Forms.DockStyle.Top;
        this.tlpShortBreakTime.Location = new System.Drawing.Point(13, 105);
        this.tlpShortBreakTime.Name = "tlpShortBreakTime";
        this.tlpShortBreakTime.RowCount = 1;
        this.tlpShortBreakTime.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpShortBreakTime.Size = new System.Drawing.Size(564, 40);
        this.tlpShortBreakTime.TabIndex = 2;

        // 
        // lblShortBreakTime
        // 
        this.lblShortBreakTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblShortBreakTime.AutoSize = true;
        this.lblShortBreakTime.Location = new System.Drawing.Point(3, 10);
        this.lblShortBreakTime.Name = "lblShortBreakTime";
        this.lblShortBreakTime.Size = new System.Drawing.Size(90, 20);
        this.lblShortBreakTime.TabIndex = 0;
        this.lblShortBreakTime.Text = "短休息时间:";

        // 
        // nudShortBreakTimeMin
        // 
        this.nudShortBreakTimeMin.Dock = System.Windows.Forms.DockStyle.Fill;
        this.nudShortBreakTimeMin.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.SurfaceColor;
        this.nudShortBreakTimeMin.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.TextColor;
        this.nudShortBreakTimeMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.nudShortBreakTimeMin.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
        this.nudShortBreakTimeMin.Margin = new System.Windows.Forms.Padding(5, 5, 5, 15);
        this.nudShortBreakTimeMin.Name = "nudShortBreakTimeMin";
        this.nudShortBreakTimeMin.Value = new decimal(new int[] { 5, 0, 0, 0 });

        // 
        // lblShortBreakMinUnit
        // 
        this.lblShortBreakMinUnit.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblShortBreakMinUnit.AutoSize = true;
        this.lblShortBreakMinUnit.Location = new System.Drawing.Point(199, 10);
        this.lblShortBreakMinUnit.Name = "lblShortBreakMinUnit";
        this.lblShortBreakMinUnit.Size = new System.Drawing.Size(30, 20);
        this.lblShortBreakMinUnit.TabIndex = 2;
        this.lblShortBreakMinUnit.Text = "分";

        // 
        // nudShortBreakTimeSec
        // 
        this.nudShortBreakTimeSec.Dock = System.Windows.Forms.DockStyle.Fill;
        this.nudShortBreakTimeSec.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.SurfaceColor;
        this.nudShortBreakTimeSec.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.TextColor;
        this.nudShortBreakTimeSec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.nudShortBreakTimeSec.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
        this.nudShortBreakTimeSec.Margin = new System.Windows.Forms.Padding(5, 5, 5, 15);
        this.nudShortBreakTimeSec.Name = "nudShortBreakTimeSec";

        // 
        // lblShortBreakSecUnit
        // 
        this.lblShortBreakSecUnit.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblShortBreakSecUnit.AutoSize = true;
        this.lblShortBreakSecUnit.Location = new System.Drawing.Point(335, 10);
        this.lblShortBreakSecUnit.Name = "lblShortBreakSecUnit";
        this.lblShortBreakSecUnit.Size = new System.Drawing.Size(30, 20);
        this.lblShortBreakSecUnit.TabIndex = 4;
        this.lblShortBreakSecUnit.Text = "秒";

        // 
        // tlpLongBreakTime
        // 
        this.tlpLongBreakTime.AutoSize = true;
        this.tlpLongBreakTime.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpLongBreakTime.ColumnCount = 5;
        this.tlpLongBreakTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpLongBreakTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
        this.tlpLongBreakTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpLongBreakTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
        this.tlpLongBreakTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpLongBreakTime.Controls.Add(this.lblLongBreakTime, 0, 0);
        this.tlpLongBreakTime.Controls.Add(this.nudLongBreakTimeMin, 1, 0);
        this.tlpLongBreakTime.Controls.Add(this.lblLongBreakMinUnit, 2, 0);
        this.tlpLongBreakTime.Controls.Add(this.nudLongBreakTimeSec, 3, 0);
        this.tlpLongBreakTime.Controls.Add(this.lblLongBreakSecUnit, 4, 0);
        this.tlpLongBreakTime.Dock = System.Windows.Forms.DockStyle.Top;
        this.tlpLongBreakTime.Location = new System.Drawing.Point(13, 151);
        this.tlpLongBreakTime.Name = "tlpLongBreakTime";
        this.tlpLongBreakTime.RowCount = 1;
        this.tlpLongBreakTime.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpLongBreakTime.Size = new System.Drawing.Size(564, 40);
        this.tlpLongBreakTime.TabIndex = 3;

        // 
        // lblLongBreakTime
        // 
        this.lblLongBreakTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblLongBreakTime.AutoSize = true;
        this.lblLongBreakTime.Location = new System.Drawing.Point(3, 10);
        this.lblLongBreakTime.Name = "lblLongBreakTime";
        this.lblLongBreakTime.Size = new System.Drawing.Size(90, 20);
        this.lblLongBreakTime.TabIndex = 0;
        this.lblLongBreakTime.Text = "长休息时间:";

        // 
        // nudLongBreakTimeMin
        // 
        this.nudLongBreakTimeMin.Dock = System.Windows.Forms.DockStyle.Fill;
        this.nudLongBreakTimeMin.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.SurfaceColor;
        this.nudLongBreakTimeMin.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.TextColor;
        this.nudLongBreakTimeMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.nudLongBreakTimeMin.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
        this.nudLongBreakTimeMin.Margin = new System.Windows.Forms.Padding(5, 5, 5, 15);
        this.nudLongBreakTimeMin.Name = "nudLongBreakTimeMin";
        this.nudLongBreakTimeMin.Value = new decimal(new int[] { 15, 0, 0, 0 });

        // 
        // lblLongBreakMinUnit
        // 
        this.lblLongBreakMinUnit.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblLongBreakMinUnit.AutoSize = true;
        this.lblLongBreakMinUnit.Location = new System.Drawing.Point(199, 10);
        this.lblLongBreakMinUnit.Name = "lblLongBreakMinUnit";
        this.lblLongBreakMinUnit.Size = new System.Drawing.Size(30, 20);
        this.lblLongBreakMinUnit.TabIndex = 2;
        this.lblLongBreakMinUnit.Text = "分";

        // 
        // nudLongBreakTimeSec
        // 
        this.nudLongBreakTimeSec.Dock = System.Windows.Forms.DockStyle.Fill;
        this.nudLongBreakTimeSec.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.SurfaceColor;
        this.nudLongBreakTimeSec.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.TextColor;
        this.nudLongBreakTimeSec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.nudLongBreakTimeSec.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
        this.nudLongBreakTimeSec.Margin = new System.Windows.Forms.Padding(5, 5, 5, 15);
        this.nudLongBreakTimeSec.Name = "nudLongBreakTimeSec";

        // 
        // lblLongBreakSecUnit
        // 
        this.lblLongBreakSecUnit.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblLongBreakSecUnit.AutoSize = true;
        this.lblLongBreakSecUnit.Location = new System.Drawing.Point(335, 10);
        this.lblLongBreakSecUnit.Name = "lblLongBreakSecUnit";
        this.lblLongBreakSecUnit.Size = new System.Drawing.Size(30, 20);
        this.lblLongBreakSecUnit.TabIndex = 4;
        this.lblLongBreakSecUnit.Text = "秒";

        // 
        // tlpWarningLongTime
        // 
        this.tlpWarningLongTime.AutoSize = true;
        this.tlpWarningLongTime.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpWarningLongTime.ColumnCount = 3;
        this.tlpWarningLongTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpWarningLongTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
        this.tlpWarningLongTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpWarningLongTime.Controls.Add(this.lblWarningLongTime, 0, 0);
        this.tlpWarningLongTime.Controls.Add(this.nudWarningLongTime, 1, 0);
        this.tlpWarningLongTime.Controls.Add(this.lblWarningLongTimeUnit, 2, 0);
        this.tlpWarningLongTime.Dock = System.Windows.Forms.DockStyle.Top;
        this.tlpWarningLongTime.Location = new System.Drawing.Point(13, 197);
        this.tlpWarningLongTime.Name = "tlpWarningLongTime";
        this.tlpWarningLongTime.RowCount = 1;
        this.tlpWarningLongTime.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpWarningLongTime.Size = new System.Drawing.Size(564, 40);
        this.tlpWarningLongTime.TabIndex = 4;

        // 
        // lblWarningLongTime
        // 
        this.lblWarningLongTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblWarningLongTime.AutoSize = true;
        this.lblWarningLongTime.Location = new System.Drawing.Point(3, 10);
        this.lblWarningLongTime.Name = "lblWarningLongTime";
        this.lblWarningLongTime.Size = new System.Drawing.Size(80, 20);
        this.lblWarningLongTime.TabIndex = 0;
        this.lblWarningLongTime.Text = "时间提示1:";

        // 
        // nudWarningLongTime
        // 
        this.nudWarningLongTime.Dock = System.Windows.Forms.DockStyle.Fill;
        this.nudWarningLongTime.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.SurfaceColor;
        this.nudWarningLongTime.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.TextColor;
        this.nudWarningLongTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.nudWarningLongTime.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
        this.nudWarningLongTime.Margin = new System.Windows.Forms.Padding(5, 5, 5, 15);
        this.nudWarningLongTime.Name = "nudWarningLongTime";
        this.nudWarningLongTime.Value = new decimal(new int[] { 45, 0, 0, 0 });

        // 
        // lblWarningLongTimeUnit
        // 
        this.lblWarningLongTimeUnit.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblWarningLongTimeUnit.AutoSize = true;
        this.lblWarningLongTimeUnit.Location = new System.Drawing.Point(189, 10);
        this.lblWarningLongTimeUnit.Name = "lblWarningLongTimeUnit";
        this.lblWarningLongTimeUnit.Size = new System.Drawing.Size(30, 20);
        this.lblWarningLongTimeUnit.TabIndex = 2;
        this.lblWarningLongTimeUnit.Text = "秒";

        // 
        // tlpWarningShortTime
        // 
        this.tlpWarningShortTime.AutoSize = true;
        this.tlpWarningShortTime.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpWarningShortTime.ColumnCount = 3;
        this.tlpWarningShortTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpWarningShortTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
        this.tlpWarningShortTime.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpWarningShortTime.Controls.Add(this.lblWarningShortTime, 0, 0);
        this.tlpWarningShortTime.Controls.Add(this.nudWarningShortTime, 1, 0);
        this.tlpWarningShortTime.Controls.Add(this.lblWarningShortTimeUnit, 2, 0);
        this.tlpWarningShortTime.Dock = System.Windows.Forms.DockStyle.Top;
        this.tlpWarningShortTime.Location = new System.Drawing.Point(13, 243);
        this.tlpWarningShortTime.Name = "tlpWarningShortTime";
        this.tlpWarningShortTime.RowCount = 1;
        this.tlpWarningShortTime.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpWarningShortTime.Size = new System.Drawing.Size(564, 40);
        this.tlpWarningShortTime.TabIndex = 5;

        // 
        // lblWarningShortTime
        // 
        this.lblWarningShortTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblWarningShortTime.AutoSize = true;
        this.lblWarningShortTime.Location = new System.Drawing.Point(3, 10);
        this.lblWarningShortTime.Name = "lblWarningShortTime";
        this.lblWarningShortTime.Size = new System.Drawing.Size(80, 20);
        this.lblWarningShortTime.TabIndex = 0;
        this.lblWarningShortTime.Text = "时间提示2:";

        // 
        // nudWarningShortTime
        // 
        this.nudWarningShortTime.Dock = System.Windows.Forms.DockStyle.Fill;
        this.nudWarningShortTime.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.SurfaceColor;
        this.nudWarningShortTime.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.TextColor;
        this.nudWarningShortTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.nudWarningShortTime.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
        this.nudWarningShortTime.Margin = new System.Windows.Forms.Padding(5, 5, 5, 15);
        this.nudWarningShortTime.Name = "nudWarningShortTime";
        this.nudWarningShortTime.Value = new decimal(new int[] { 3, 0, 0, 0 });

        // 
        // lblWarningShortTimeUnit
        // 
        this.lblWarningShortTimeUnit.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblWarningShortTimeUnit.AutoSize = true;
        this.lblWarningShortTimeUnit.Location = new System.Drawing.Point(189, 10);
        this.lblWarningShortTimeUnit.Name = "lblWarningShortTimeUnit";
        this.lblWarningShortTimeUnit.Size = new System.Drawing.Size(30, 20);
        this.lblWarningShortTimeUnit.TabIndex = 2;
        this.lblWarningShortTimeUnit.Text = "秒";

        // 
        // PomodoroSettingsControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.AutoScroll = true;
        this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.Controls.Add(this.tlpMain);
        this.Name = "PomodoroSettingsControl";
        this.Size = new System.Drawing.Size(600, 300);

        ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeMin)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeSec)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeMin)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeSec)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeMin)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeSec)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudWarningLongTime)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudWarningShortTime)).EndInit();
        this.tlpMain.ResumeLayout(false);
        this.tlpMain.PerformLayout();
        this.tlpMode.ResumeLayout(false);
        this.tlpMode.PerformLayout();
        this.tlpWorkTime.ResumeLayout(false);
        this.tlpWorkTime.PerformLayout();
        this.tlpShortBreakTime.ResumeLayout(false);
        this.tlpShortBreakTime.PerformLayout();
        this.tlpLongBreakTime.ResumeLayout(false);
        this.tlpLongBreakTime.PerformLayout();
        this.tlpWarningLongTime.ResumeLayout(false);
        this.tlpWarningLongTime.PerformLayout();
        this.tlpWarningShortTime.ResumeLayout(false);
        this.tlpWarningShortTime.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.TableLayoutPanel tlpMain;
    private System.Windows.Forms.TableLayoutPanel tlpMode;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblMode;
    private DiabloTwoMFTimer.UI.Components.ThemedComboBox cmbMode;
    private System.Windows.Forms.TableLayoutPanel tlpWorkTime;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblWorkTime;
    private System.Windows.Forms.NumericUpDown nudWorkTimeMin;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblWorkMinUnit;
    private System.Windows.Forms.NumericUpDown nudWorkTimeSec;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblWorkSecUnit;
    private System.Windows.Forms.TableLayoutPanel tlpShortBreakTime;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblShortBreakTime;
    private System.Windows.Forms.NumericUpDown nudShortBreakTimeMin;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblShortBreakMinUnit;
    private System.Windows.Forms.NumericUpDown nudShortBreakTimeSec;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblShortBreakSecUnit;
    private System.Windows.Forms.TableLayoutPanel tlpLongBreakTime;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblLongBreakTime;
    private System.Windows.Forms.NumericUpDown nudLongBreakTimeMin;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblLongBreakMinUnit;
    private System.Windows.Forms.NumericUpDown nudLongBreakTimeSec;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblLongBreakSecUnit;
    private System.Windows.Forms.TableLayoutPanel tlpWarningLongTime;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblWarningLongTime;
    private System.Windows.Forms.NumericUpDown nudWarningLongTime;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblWarningLongTimeUnit;
    private System.Windows.Forms.TableLayoutPanel tlpWarningShortTime;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblWarningShortTime;
    private System.Windows.Forms.NumericUpDown nudWarningShortTime;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblWarningShortTimeUnit;
    private DiabloTwoMFTimer.UI.Components.ThemedGroupBox grpPomodoroSettings;
}
