using DiabloTwoMFTimer.UI.Settings;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Form;

partial class SettingsForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        // 初始化 SettingsForm 特有的控件
        this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
        this.tabControl = new DiabloTwoMFTimer.UI.Components.ThemedTabControl();
        this.tlpSettingsNav = new System.Windows.Forms.TableLayoutPanel();
        this.btnSetGeneral = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnSetHotkeys = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnSetTimer = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnSetAudio = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnAbout = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnSetPomodoro = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.tabPageGeneral = new System.Windows.Forms.TabPage();
        this.generalSettings = new DiabloTwoMFTimer.UI.Settings.GeneralSettingsControl();
        this.tabPageHotkeys = new System.Windows.Forms.TabPage();
        this.hotkeySettings = new DiabloTwoMFTimer.UI.Settings.HotkeySettingsControl();
        this.tabPageTimer = new System.Windows.Forms.TabPage();
        this.timerSettings = new DiabloTwoMFTimer.UI.Settings.TimerSettingsControl();
        this.tabPagePomodoro = new System.Windows.Forms.TabPage();
        this.tabPageAudio = new System.Windows.Forms.TabPage();
        this.tabPageAbout = new System.Windows.Forms.TabPage();
        this.btnConfirmSettings = new DiabloTwoMFTimer.UI.Components.ThemedButton();

        this.tlpMain.SuspendLayout();
        this.tabControl.SuspendLayout();
        this.tabPageGeneral.SuspendLayout();
        this.tabPageHotkeys.SuspendLayout();
        this.tabPageTimer.SuspendLayout();
        this.SuspendLayout();

        // 
        // tlpMain
        // 
        this.tlpMain.ColumnCount = 1;
        this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpMain.RowCount = 3;
        this.tlpMain.RowStyles.Clear();

        // Row 0: Top Nav
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, Theme.UISizeConstants.TabItemHeight));
        // Row 1: Content (100% Fill)
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

        // Row 2: Bottom Buttons (AutoSize)
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

        this.tlpMain.Controls.Add(this.tlpSettingsNav, 0, 0);
        this.tlpMain.Controls.Add(this.tabControl, 0, 1);

        this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpMain.Location = new System.Drawing.Point(0, 0);
        this.tlpMain.Name = "tlpMain";
        this.tlpMain.Size = new System.Drawing.Size(Theme.UISizeConstants.SettingsFormWidth, Theme.UISizeConstants.SettingsFormHeight);
        this.tlpMain.TabIndex = 0;

        // 
        // tabPageAudio
        // 
        this.tabPageAudio.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.tabPageAudio.Location = new System.Drawing.Point(4, 25);
        this.tabPageAudio.Name = "tabPageAudio";
        this.tabPageAudio.Padding = new System.Windows.Forms.Padding(3);
        this.tabPageAudio.Size = new System.Drawing.Size(Theme.UISizeConstants.SettingsFormWidth - 8, Theme.UISizeConstants.SettingsFormHeight - Theme.UISizeConstants.TabItemHeight);
        this.tabPageAudio.TabIndex = 3;
        this.tabPageAudio.Text = "声音";
        this.tabPageAudio.UseVisualStyleBackColor = false;

        // 
        // tabPagePomodoro
        // 
        this.tabPagePomodoro.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.tabPagePomodoro.Location = new System.Drawing.Point(4, 25);
        this.tabPagePomodoro.Name = "tabPagePomodoro";
        this.tabPagePomodoro.Padding = new System.Windows.Forms.Padding(3);
        this.tabPagePomodoro.Size = new System.Drawing.Size(Theme.UISizeConstants.SettingsFormWidth - 8, Theme.UISizeConstants.SettingsFormHeight - Theme.UISizeConstants.TabItemHeight - 29);
        this.tabPagePomodoro.TabIndex = 3;
        this.tabPagePomodoro.Text = "番茄";
        this.tabPagePomodoro.UseVisualStyleBackColor = false;

        // 
        // tabControl
        // 
        this.tabControl.Controls.Add(this.tabPageGeneral);
        this.tabControl.Controls.Add(this.tabPageHotkeys);
        this.tabControl.Controls.Add(this.tabPageTimer);
        this.tabControl.Controls.Add(this.tabPagePomodoro);
        this.tabControl.Controls.Add(this.tabPageAudio);
        this.tabControl.Controls.Add(this.tabPageAbout);
        this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tabControl.Location = new System.Drawing.Point(0, Theme.UISizeConstants.TabItemHeight);
        this.tabControl.Name = "tabControl";
        this.tabControl.SelectedIndex = 0;
        this.tabControl.Size = new System.Drawing.Size(Theme.UISizeConstants.SettingsFormWidth, Theme.UISizeConstants.SettingsFormHeight - Theme.UISizeConstants.TabItemHeight);
        this.tabControl.TabIndex = 0;

        // 
        // tabPageAbout
        // 
        this.tabPageAbout.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.tabPageAbout.Location = new System.Drawing.Point(4, 25);
        this.tabPageAbout.Name = "tabPageAbout";
        this.tabPageAbout.Padding = new System.Windows.Forms.Padding(3);
        this.tabPageAbout.Size = new System.Drawing.Size(Theme.UISizeConstants.SettingsFormWidth - 8, UISizeConstants.SettingsFormHeight - Theme.UISizeConstants.TabItemHeight);
        this.tabPageAbout.TabIndex = 4;
        this.tabPageAbout.Text = "关于";
        this.tabPageAbout.UseVisualStyleBackColor = false;

        // 
        // tlpSettingsNav
        // 
        this.tlpSettingsNav.ColumnCount = 6;
        this.tlpSettingsNav.ColumnStyles.Clear();
        this.tlpSettingsNav.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.67F));
        this.tlpSettingsNav.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.67F));
        this.tlpSettingsNav.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.67F));
        this.tlpSettingsNav.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.67F));
        this.tlpSettingsNav.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.67F));
        this.tlpSettingsNav.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.67F));
        this.tlpSettingsNav.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpSettingsNav.Margin = new System.Windows.Forms.Padding(0);
        this.tlpSettingsNav.Controls.Add(this.btnSetGeneral, 0, 0);
        this.tlpSettingsNav.Controls.Add(this.btnSetHotkeys, 1, 0);
        this.tlpSettingsNav.Controls.Add(this.btnSetTimer, 2, 0);
        this.tlpSettingsNav.Controls.Add(this.btnSetPomodoro, 3, 0);
        this.tlpSettingsNav.Controls.Add(this.btnSetAudio, 4, 0);
        this.tlpSettingsNav.Controls.Add(this.btnAbout, 5, 0);

        // 
        // btnSetPomodoro
        // 
        this.btnSetPomodoro.Dock = System.Windows.Forms.DockStyle.Fill;
        this.btnSetPomodoro.Margin = new System.Windows.Forms.Padding(0);
        this.btnSetPomodoro.Name = "btnSetPomodoro";
        this.btnSetPomodoro.Text = "番茄";
        this.btnSetPomodoro.Click += new System.EventHandler(this.BtnSetPomodoro_Click);

        this.btnSetGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
        this.btnSetGeneral.Margin = new System.Windows.Forms.Padding(0);

        this.btnSetHotkeys.Dock = System.Windows.Forms.DockStyle.Fill;
        this.btnSetHotkeys.Margin = new System.Windows.Forms.Padding(0);

        this.btnSetTimer.Dock = System.Windows.Forms.DockStyle.Fill;
        this.btnSetTimer.Margin = new System.Windows.Forms.Padding(0);

        this.btnSetAudio.Dock = System.Windows.Forms.DockStyle.Fill;
        this.btnSetAudio.Margin = new System.Windows.Forms.Padding(0);

        this.btnAbout.Dock = System.Windows.Forms.DockStyle.Fill;
        this.btnAbout.Margin = new System.Windows.Forms.Padding(0);

        // 
        // Tab Pages
        // 
        this.tabPageGeneral.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.tabPageGeneral.Controls.Add(this.generalSettings);
        this.tabPageGeneral.Name = "tabPageGeneral";
        this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
        this.tabPageGeneral.TabIndex = 0;
        this.tabPageGeneral.Text = "通用";

        this.generalSettings.Dock = System.Windows.Forms.DockStyle.Fill;
        this.generalSettings.Name = "generalSettings";
        this.generalSettings.TabIndex = 0;

        this.tabPageHotkeys.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.tabPageHotkeys.Controls.Add(this.hotkeySettings);
        this.tabPageHotkeys.Name = "tabPageHotkeys";
        this.tabPageHotkeys.Padding = new System.Windows.Forms.Padding(3);
        this.tabPageHotkeys.TabIndex = 1;
        this.tabPageHotkeys.Text = "快捷键";

        this.hotkeySettings.Dock = System.Windows.Forms.DockStyle.Fill;
        this.hotkeySettings.Name = "hotkeySettings";
        this.hotkeySettings.TabIndex = 0;

        this.tabPageTimer.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.tabPageTimer.Controls.Add(this.timerSettings);
        this.tabPageTimer.Name = "tabPageTimer";
        this.tabPageTimer.Padding = new System.Windows.Forms.Padding(3);
        this.tabPageTimer.TabIndex = 2;
        this.tabPageTimer.Text = "计时器";

        this.timerSettings.Dock = System.Windows.Forms.DockStyle.Fill;
        this.timerSettings.Name = "timerSettings";
        this.timerSettings.TabIndex = 0;

        // 
        // btnConfirmSettings
        // 
        this.btnConfirmSettings.Anchor = System.Windows.Forms.AnchorStyles.Right;
        this.btnConfirmSettings.Name = "btnConfirmSettings";
        this.btnConfirmSettings.Size = new System.Drawing.Size(80, 30);
        this.btnConfirmSettings.TabIndex = 0;
        this.btnConfirmSettings.Text = "确认";
        this.btnConfirmSettings.Margin = new System.Windows.Forms.Padding(0);
        this.btnConfirmSettings.Click += new System.EventHandler(this.BtnConfirmSettings_Click);

        // 
        // SettingsForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.MinimumSize = new System.Drawing.Size(Theme.UISizeConstants.SettingsFormWidth, Theme.UISizeConstants.SettingsFormHeight);
        this.Name = "SettingsForm";

        // 将 SettingsForm 的内容添加到 BaseForm 的 pnlContent 中
        this.pnlContent.Controls.Add(this.tlpMain);

        // 设置按钮事件
        this.btnConfirm.Click += this.BtnConfirmSettings_Click;

        this.tlpMain.ResumeLayout(false);
        this.tabControl.ResumeLayout(false);
        this.tabPageGeneral.ResumeLayout(false);
        this.tabPageHotkeys.ResumeLayout(false);
        this.tabPageTimer.ResumeLayout(false);
        this.tabPageAudio.ResumeLayout(false);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.TableLayoutPanel tlpMain;
    private DiabloTwoMFTimer.UI.Components.ThemedTabControl tabControl;
    private System.Windows.Forms.TabPage tabPageGeneral;
    private System.Windows.Forms.TabPage tabPageHotkeys;
    private System.Windows.Forms.TabPage tabPageTimer;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnConfirmSettings;
    private GeneralSettingsControl generalSettings;
    private HotkeySettingsControl hotkeySettings;
    private TimerSettingsControl timerSettings;
    private System.Windows.Forms.TableLayoutPanel tlpSettingsNav;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnSetGeneral;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnSetHotkeys;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnSetTimer;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnSetPomodoro;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnSetAudio;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnAbout;
    private System.Windows.Forms.TabPage tabPageAudio;
    private System.Windows.Forms.TabPage tabPageAbout;
    private System.Windows.Forms.TabPage tabPagePomodoro;
}