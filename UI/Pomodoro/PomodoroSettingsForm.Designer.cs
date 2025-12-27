using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Pomodoro;

partial class PomodoroSettingsForm
{
    private System.ComponentModel.IContainer components;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.tlpContent = new System.Windows.Forms.TableLayoutPanel();

        // Controls
        this.lblWorkTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudWorkTimeMin = new System.Windows.Forms.NumericUpDown();
        this.lblWorkMinUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudWorkTimeSec = new System.Windows.Forms.NumericUpDown();
        this.lblWorkSecUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();

        this.lblShortBreakTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudShortBreakTimeMin = new System.Windows.Forms.NumericUpDown();
        this.lblShortBreakMinUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudShortBreakTimeSec = new System.Windows.Forms.NumericUpDown();
        this.lblShortBreakSecUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();

        this.lblLongBreakTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudLongBreakTimeMin = new System.Windows.Forms.NumericUpDown();
        this.lblLongBreakMinUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudLongBreakTimeSec = new System.Windows.Forms.NumericUpDown();
        this.lblLongBreakSecUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();

        this.lblWarningLongTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudWarningLongTime = new System.Windows.Forms.NumericUpDown();
        this.lblWarningLongTimeUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();

        this.lblWarningShortTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.nudWarningShortTime = new System.Windows.Forms.NumericUpDown();
        this.lblWarningShortTimeUnit = new DiabloTwoMFTimer.UI.Components.ThemedLabel();

        // 【新增】模式选择
        this.lblMode = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.cmbMode = new DiabloTwoMFTimer.UI.Components.ThemedComboBox();

        ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeMin)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeSec)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeMin)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeSec)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeMin)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeSec)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudWarningLongTime)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudWarningShortTime)).BeginInit();

        this.pnlContent.SuspendLayout();
        this.tlpContent.SuspendLayout();
        this.SuspendLayout();

        //
        // tlpContent (6 Rows, 5 Cols)
        //
        this.tlpContent.AutoSize = true;
        this.tlpContent.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpContent.ColumnCount = 5;
        this.tlpContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize)); // Label
        this.tlpContent.ColumnStyles.Add(
            new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F)
        ); // Input 1
        this.tlpContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize)); // Unit 1
        this.tlpContent.ColumnStyles.Add(
            new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F)
        ); // Input 2
        this.tlpContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize)); // Unit 2

        this.tlpContent.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpContent.RowCount = 6;
        for (int i = 0; i < 6; i++)
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

        // Row 0: Mode Selection (新增)
        this.tlpContent.Controls.Add(this.lblMode, 0, 0);
        this.tlpContent.Controls.Add(this.cmbMode, 1, 0);
        this.tlpContent.SetColumnSpan(this.cmbMode, 4); // ComboBox 占满后续列

        // Row 1: Work
        AddRow(1, lblWorkTime, nudWorkTimeMin, lblWorkMinUnit, nudWorkTimeSec, lblWorkSecUnit);
        // Row 2: Short Break
        AddRow(
            2,
            lblShortBreakTime,
            nudShortBreakTimeMin,
            lblShortBreakMinUnit,
            nudShortBreakTimeSec,
            lblShortBreakSecUnit
        );
        // Row 3: Long Break
        AddRow(3, lblLongBreakTime, nudLongBreakTimeMin, lblLongBreakMinUnit, nudLongBreakTimeSec, lblLongBreakSecUnit);

        // Row 4: Warning 1
        this.tlpContent.Controls.Add(this.lblWarningLongTime, 0, 4);
        this.tlpContent.Controls.Add(this.nudWarningLongTime, 1, 4);
        this.tlpContent.Controls.Add(this.lblWarningLongTimeUnit, 2, 4);

        // Row 5: Warning 2
        this.tlpContent.Controls.Add(this.lblWarningShortTime, 0, 5);
        this.tlpContent.Controls.Add(this.nudWarningShortTime, 1, 5);
        this.tlpContent.Controls.Add(this.lblWarningShortTimeUnit, 2, 5);

        // 统一控件样式方法
        void ConfigureNud(System.Windows.Forms.NumericUpDown nud, decimal max)
        {
            nud.Dock = System.Windows.Forms.DockStyle.Fill;
            nud.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.SurfaceColor;
            nud.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.TextColor;
            nud.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            nud.Maximum = max;
            nud.Margin = new System.Windows.Forms.Padding(5, 5, 5, 15);
        }

        void ConfigureLabel(System.Windows.Forms.Label lbl, string text)
        {
            lbl.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lbl.AutoSize = true;
            lbl.Text = text;
            lbl.Margin = new System.Windows.Forms.Padding(0, 0, 10, 15);
        }

        // Apply Styles
        ConfigureLabel(lblMode, "Mode:");

        ConfigureLabel(lblWorkTime, "Work Time:");
        ConfigureLabel(lblShortBreakTime, "Short Break:");
        ConfigureLabel(lblLongBreakTime, "Long Break:");
        ConfigureLabel(lblWarningLongTime, "Warning Long:");
        ConfigureLabel(lblWarningShortTime, "Warning Short:");

        ConfigureLabel(lblWorkMinUnit, "min");
        ConfigureLabel(lblWorkSecUnit, "sec");
        ConfigureLabel(lblShortBreakMinUnit, "min");
        ConfigureLabel(lblShortBreakSecUnit, "sec");
        ConfigureLabel(lblLongBreakMinUnit, "min");
        ConfigureLabel(lblLongBreakSecUnit, "sec");
        ConfigureLabel(lblWarningLongTimeUnit, "sec");
        ConfigureLabel(lblWarningShortTimeUnit, "sec");

        ConfigureNud(nudWorkTimeMin, 999);
        ConfigureNud(nudWorkTimeSec, 59);
        ConfigureNud(nudShortBreakTimeMin, 999);
        ConfigureNud(nudShortBreakTimeSec, 59);
        ConfigureNud(nudLongBreakTimeMin, 999);
        ConfigureNud(nudLongBreakTimeSec, 59);
        ConfigureNud(nudWarningLongTime, 300);
        ConfigureNud(nudWarningShortTime, 60);

        // ComboBox style
        this.cmbMode.Dock = System.Windows.Forms.DockStyle.Fill;
        this.cmbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbMode.Margin = new System.Windows.Forms.Padding(5, 5, 5, 15);

        //
        // PomodoroSettingsForm
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.MinimumSize = new System.Drawing.Size(450, 0);

        this.pnlContent.Controls.Add(this.tlpContent);
        this.Name = "PomodoroSettingsForm";
        this.Text = "Pomodoro Settings";

        ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeMin)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeSec)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeMin)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeSec)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeMin)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeSec)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudWarningLongTime)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nudWarningShortTime)).EndInit();

        this.pnlContent.ResumeLayout(false);
        this.pnlContent.PerformLayout();
        this.tlpContent.ResumeLayout(false);
        this.tlpContent.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void AddRow(int row, Control c1, Control c2, Control c3, Control c4, Control c5)
    {
        this.tlpContent.Controls.Add(c1, 0, row);
        this.tlpContent.Controls.Add(c2, 1, row);
        this.tlpContent.Controls.Add(c3, 2, row);
        this.tlpContent.Controls.Add(c4, 3, row);
        this.tlpContent.Controls.Add(c5, 4, row);
    }

    private System.Windows.Forms.TableLayoutPanel tlpContent;

    // ... Controls definitions (same as before) ...
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblMode; // 新增
    private DiabloTwoMFTimer.UI.Components.ThemedComboBox cmbMode; // 新增

    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblWorkTime;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblShortBreakTime;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblLongBreakTime;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblWorkMinUnit;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblShortBreakMinUnit;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblLongBreakMinUnit;
    private System.Windows.Forms.NumericUpDown nudWorkTimeMin;
    private System.Windows.Forms.NumericUpDown nudWorkTimeSec;
    private System.Windows.Forms.NumericUpDown nudShortBreakTimeMin;
    private System.Windows.Forms.NumericUpDown nudShortBreakTimeSec;
    private System.Windows.Forms.NumericUpDown nudLongBreakTimeMin;
    private System.Windows.Forms.NumericUpDown nudLongBreakTimeSec;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblWorkSecUnit;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblShortBreakSecUnit;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblLongBreakSecUnit;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblWarningLongTime;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblWarningShortTime;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblWarningLongTimeUnit;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblWarningShortTimeUnit;
    private System.Windows.Forms.NumericUpDown nudWarningLongTime;
    private System.Windows.Forms.NumericUpDown nudWarningShortTime;
}
