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
        this.tabPageGeneral = new System.Windows.Forms.TabPage();
        this.generalSettings = new DiabloTwoMFTimer.UI.Settings.GeneralSettingsControl();
        this.tabPageHotkeys = new System.Windows.Forms.TabPage();
        this.hotkeySettings = new DiabloTwoMFTimer.UI.Settings.HotkeySettingsControl();
        this.tabPageTimer = new System.Windows.Forms.TabPage();
        this.timerSettings = new DiabloTwoMFTimer.UI.Settings.TimerSettingsControl();

        this.panelBottom = new System.Windows.Forms.Panel();
        this.btnConfirmSettings = new DiabloTwoMFTimer.UI.Components.ThemedButton();

        this.tlpMain.SuspendLayout();
        this.tabControl.SuspendLayout();
        this.tabPageGeneral.SuspendLayout();
        this.tabPageHotkeys.SuspendLayout();
        this.tabPageTimer.SuspendLayout();
        this.panelBottom.SuspendLayout();
        this.SuspendLayout();


        // 
        // tlpMain
        // 
        this.tlpMain.ColumnCount = 1;
        this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        // 重要：SettingsControl 之前的 tlpMain 可能包含了 TabControl。
        // 现在我们需要把 tlpSettingsNav 加在 tlpMain 的最上面，或者让 tlpSettingsNav 和 tabControl 并在 tlpMain 里。
        // 假设 tlpMain 是垂直布局：
        // 1. 修改 tlpMain 的行定义，从 2 行改为 3 行
        this.tlpMain.RowCount = 3;
        this.tlpMain.RowStyles.Clear();
        // 行 0：顶部自定义导航栏 (高度写死)
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, Theme.UISizeConstants.TabItemHeight));
        // 行 1：中部 Tab 内容容器 (自动填满 100%)
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        // 行 2：底部确认按钮区域 (固定高度)
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, Theme.UISizeConstants.TabItemHeight));

        // 2. 清除控件原有的 Dock=Top，全部改为 Dock=Fill
        this.tlpSettingsNav.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panelBottom.Dock = System.Windows.Forms.DockStyle.Fill;

        // 3. 将控件严格放入对应的“坑位”
        this.tlpMain.Controls.Add(this.tlpSettingsNav, 0, 0); // 导航放在第 0 行
        this.tlpMain.Controls.Add(this.tabControl, 0, 1);    // 容器放在第 1 行 (内容区不会再往上跑了)
        this.tlpMain.Controls.Add(this.panelBottom, 0, 2);   // 按钮放在第 2 行

        // 4. 其他修正：确保 tlpMain 始终 Dock=Fill 填充整个 SettingsControl
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
        // tlpSettingsNav
        // 
        this.tlpSettingsNav.ColumnCount = 3;
        this.tlpSettingsNav.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
        this.tlpSettingsNav.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
        this.tlpSettingsNav.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
        this.tlpSettingsNav.Controls.Add(this.btnSetGeneral, 0, 0);
        this.tlpSettingsNav.Controls.Add(this.btnSetHotkeys, 1, 0);
        this.tlpSettingsNav.Controls.Add(this.btnSetTimer, 2, 0);
        this.tlpSettingsNav.Dock = System.Windows.Forms.DockStyle.Top; // 停靠在顶部
        this.tlpSettingsNav.Location = new System.Drawing.Point(0, 0);
        this.tlpSettingsNav.RowCount = 1;
        this.tlpSettingsNav.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        // 这里不需要用 TabItemHeight，可以稍微矮一点，或者保持一致
        this.tlpSettingsNav.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, Theme.UISizeConstants.TabItemHeight);

        // 
        // tabPageGeneral
        // 
        this.tabPageGeneral.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.tabPageGeneral.Controls.Add(this.generalSettings);
        this.tabPageGeneral.Location = new System.Drawing.Point(0, 37); // TabControl内部计算的位置
        this.tabPageGeneral.Name = "tabPageGeneral";
        this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
        this.tabPageGeneral.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, Theme.UISizeConstants.SettingTabPageHeight);
        this.tabPageGeneral.TabIndex = 0;
        this.tabPageGeneral.Text = "通用";

        // 
        // generalSettings
        // 
        this.generalSettings.Dock = System.Windows.Forms.DockStyle.Fill;
        this.generalSettings.Location = new System.Drawing.Point(0, 3);
        this.generalSettings.Name = "generalSettings";
        this.generalSettings.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, Theme.UISizeConstants.SettingTabPageHeight);
        this.generalSettings.TabIndex = 0;

        // 
        // tabPageHotkeys
        // 
        this.tabPageHotkeys.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.tabPageHotkeys.Controls.Add(this.hotkeySettings);
        this.tabPageHotkeys.Location = new System.Drawing.Point(0, 37);
        this.tabPageHotkeys.Name = "tabPageHotkeys";
        this.tabPageHotkeys.Padding = new System.Windows.Forms.Padding(3);
        this.tabPageHotkeys.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, Theme.UISizeConstants.SettingTabPageHeight);
        this.tabPageHotkeys.TabIndex = 1;
        this.tabPageHotkeys.Text = "快捷键";

        // 
        // hotkeySettings
        // 
        this.hotkeySettings.Dock = System.Windows.Forms.DockStyle.Fill;
        this.hotkeySettings.Location = new System.Drawing.Point(0, 3);
        this.hotkeySettings.Name = "hotkeySettings";
        this.hotkeySettings.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, Theme.UISizeConstants.SettingTabPageHeight);
        this.hotkeySettings.TabIndex = 0;

        // 
        // tabPageTimer
        // 
        this.tabPageTimer.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.tabPageTimer.Controls.Add(this.timerSettings);
        this.tabPageTimer.Location = new System.Drawing.Point(0, 37);
        this.tabPageTimer.Name = "tabPageTimer";
        this.tabPageTimer.Padding = new System.Windows.Forms.Padding(3);
        this.tabPageTimer.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, Theme.UISizeConstants.SettingTabPageHeight);
        this.tabPageTimer.TabIndex = 2;
        this.tabPageTimer.Text = "计时器";

        // 
        // timerSettings
        // 
        this.timerSettings.Dock = System.Windows.Forms.DockStyle.Fill;
        this.timerSettings.Location = new System.Drawing.Point(0, 3);
        this.timerSettings.Name = "timerSettings";
        this.timerSettings.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, Theme.UISizeConstants.SettingTabPageHeight);
        this.timerSettings.TabIndex = 0;

        // 
        // panelBottom
        // 
        this.panelBottom.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.panelBottom.Controls.Add(this.btnConfirmSettings);
        this.panelBottom.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panelBottom.Location = new System.Drawing.Point(0, 389);
        this.panelBottom.Name = "panelBottom";
        this.panelBottom.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, 44);
        this.panelBottom.TabIndex = 1;

        // 
        // btnConfirmSettings
        // 
        this.btnConfirmSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.btnConfirmSettings.Location = new System.Drawing.Point(Theme.UISizeConstants.ClientWidth - 90, 6);
        this.btnConfirmSettings.Name = "btnConfirmSettings";
        this.btnConfirmSettings.Size = new System.Drawing.Size(80, 30);
        this.btnConfirmSettings.TabIndex = 0;
        this.btnConfirmSettings.Text = "确认";
        this.btnConfirmSettings.Click += new System.EventHandler(this.BtnConfirmSettings_Click);

        // 
        // SettingsControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.Controls.Add(this.tlpMain);
        this.Name = "SettingsControl";
        this.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, 436);

        this.tlpMain.ResumeLayout(false);
        this.tabControl.ResumeLayout(false);
        this.tabPageGeneral.ResumeLayout(false);
        this.tabPageHotkeys.ResumeLayout(false);
        this.tabPageTimer.ResumeLayout(false);
        this.panelBottom.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tlpMain;
    private DiabloTwoMFTimer.UI.Components.ThemedTabControl tabControl;
    private System.Windows.Forms.TabPage tabPageGeneral;
    private System.Windows.Forms.TabPage tabPageHotkeys;
    private System.Windows.Forms.TabPage tabPageTimer;
    private System.Windows.Forms.Panel panelBottom;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnConfirmSettings;
    private GeneralSettingsControl generalSettings;
    private HotkeySettingsControl hotkeySettings;
    private TimerSettingsControl timerSettings;
    private System.Windows.Forms.TableLayoutPanel tlpSettingsNav;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnSetGeneral;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnSetHotkeys;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnSetTimer;
}