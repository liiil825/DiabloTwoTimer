namespace DiabloTwoMFTimer.UI.Settings;

partial class SettingsControl
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
        this.tabControl = new DiabloTwoMFTimer.UI.Components.ThemedTabControl();
        this.tlpSettingsNav = new System.Windows.Forms.TableLayoutPanel();
        this.btnSetGeneral = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnSetHotkeys = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnSetTimer = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnAbout = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.tabPageGeneral = new System.Windows.Forms.TabPage();
        this.generalSettings = new DiabloTwoMFTimer.UI.Settings.GeneralSettingsControl();
        this.tabPageHotkeys = new System.Windows.Forms.TabPage();
        this.hotkeySettings = new DiabloTwoMFTimer.UI.Settings.HotkeySettingsControl();
        this.tabPageTimer = new System.Windows.Forms.TabPage();
        this.timerSettings = new DiabloTwoMFTimer.UI.Settings.TimerSettingsControl();

        // 替换 Panel 为 TableLayoutPanel
        this.tlpBottomBar = new System.Windows.Forms.TableLayoutPanel();
        this.btnConfirmSettings = new DiabloTwoMFTimer.UI.Components.ThemedButton();

        this.tlpMain.SuspendLayout();
        this.tabControl.SuspendLayout();
        this.tabPageGeneral.SuspendLayout();
        this.tabPageHotkeys.SuspendLayout();
        this.tabPageTimer.SuspendLayout();
        this.tlpBottomBar.SuspendLayout();
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

        // 【修改点 2】 Row 2: Bottom Buttons (AutoSize)
        // 改为 AutoSize，它将紧紧包裹住 tlpBottomBar 的高度，不再有多余留白
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

        this.tlpMain.Controls.Add(this.tlpSettingsNav, 0, 0);
        this.tlpMain.Controls.Add(this.tabControl, 0, 1);
        this.tlpMain.Controls.Add(this.tlpBottomBar, 0, 2); // 放入新的 TLP

        this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpMain.Location = new System.Drawing.Point(0, 0);
        this.tlpMain.Name = "tlpMain";
        this.tlpMain.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, Theme.UISizeConstants.TabPageHeight);
        this.tlpMain.TabIndex = 0;

        // 
        // tabControl
        // 
        this.tabControl.Controls.Add(this.tabPageGeneral);
        this.tabControl.Controls.Add(this.tabPageHotkeys);
        this.tabControl.Controls.Add(this.tabPageTimer);
        this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tabControl.Location = new System.Drawing.Point(0);
        this.tabControl.Name = "tabControl";
        this.tabControl.SelectedIndex = 0;
        this.tabControl.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, Theme.UISizeConstants.TabPageHeight);
        this.tabControl.TabIndex = 0;

        // 
        // tlpSettingsNav (保持不变)
        // 
        this.tlpSettingsNav.ColumnCount = 3;
        this.tlpSettingsNav.ColumnStyles.Clear();
        this.tlpSettingsNav.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
        this.tlpSettingsNav.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
        this.tlpSettingsNav.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
        this.tlpSettingsNav.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpSettingsNav.Margin = new System.Windows.Forms.Padding(0);
        this.tlpSettingsNav.Controls.Add(this.btnSetGeneral, 0, 0);
        this.tlpSettingsNav.Controls.Add(this.btnSetHotkeys, 1, 0);
        this.tlpSettingsNav.Controls.Add(this.btnSetTimer, 2, 0);

        this.btnSetGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
        this.btnSetGeneral.Margin = new System.Windows.Forms.Padding(0);

        this.btnSetHotkeys.Dock = System.Windows.Forms.DockStyle.Fill;
        this.btnSetHotkeys.Margin = new System.Windows.Forms.Padding(0);

        this.btnSetTimer.Dock = System.Windows.Forms.DockStyle.Fill;
        this.btnSetTimer.Margin = new System.Windows.Forms.Padding(0);

        // 
        // Tab Pages (保持不变)
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
        // tlpBottomBar (新容器: TableLayoutPanel)
        // 
        this.tlpBottomBar.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.tlpBottomBar.AutoSize = true;
        this.tlpBottomBar.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpBottomBar.ColumnCount = 3;
        // Col 0: Auto (About Button)
        this.tlpBottomBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        // Col 1: 100% (Spacer - 撑开中间距离)
        this.tlpBottomBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        // Col 2: Auto (Confirm Button)
        this.tlpBottomBar.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));

        this.tlpBottomBar.Controls.Add(this.btnAbout, 0, 0);
        this.tlpBottomBar.Controls.Add(this.btnConfirmSettings, 2, 0);

        this.tlpBottomBar.Dock = System.Windows.Forms.DockStyle.Fill; // 填充 tlpMain 的 Bottom 单元格
        this.tlpBottomBar.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5); // 上下给一点点留白(5px)
        this.tlpBottomBar.Name = "tlpBottomBar";
        this.tlpBottomBar.RowCount = 1;
        this.tlpBottomBar.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpBottomBar.TabIndex = 1;

        // 
        // btnAbout
        // 
        this.btnAbout.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.btnAbout.Name = "btnAbout";
        this.btnAbout.Size = new System.Drawing.Size(80, 30);
        this.btnAbout.TabIndex = 1;
        this.btnAbout.Text = "关于";
        this.btnAbout.Margin = new System.Windows.Forms.Padding(0);
        this.btnAbout.Click += new System.EventHandler(this.BtnAbout_Click);

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
        // SettingsControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.Controls.Add(this.tlpMain);
        this.Name = "SettingsControl";
        this.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, Theme.UISizeConstants.ClientHeightWithLoot);

        this.tlpMain.ResumeLayout(false);
        this.tabControl.ResumeLayout(false);
        this.tabPageGeneral.ResumeLayout(false);
        this.tabPageHotkeys.ResumeLayout(false);
        this.tabPageTimer.ResumeLayout(false);
        this.tlpBottomBar.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tlpMain;
    private DiabloTwoMFTimer.UI.Components.ThemedTabControl tabControl;
    private System.Windows.Forms.TabPage tabPageGeneral;
    private System.Windows.Forms.TabPage tabPageHotkeys;
    private System.Windows.Forms.TabPage tabPageTimer;
    private System.Windows.Forms.TableLayoutPanel tlpBottomBar; // 替换了 panelBottom
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnConfirmSettings;
    private GeneralSettingsControl generalSettings;
    private HotkeySettingsControl hotkeySettings;
    private TimerSettingsControl timerSettings;
    private System.Windows.Forms.TableLayoutPanel tlpSettingsNav;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnSetGeneral;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnSetHotkeys;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnSetTimer;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnAbout;
}