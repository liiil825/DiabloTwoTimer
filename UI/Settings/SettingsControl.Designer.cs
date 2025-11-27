#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DTwoMFTimerHelper.UI.Settings;
partial class SettingsControl
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        tabControl = new TabControl();
        tabPageGeneral = new TabPage();
        generalSettings = new GeneralSettingsControl();
        tabPageHotkeys = new TabPage();
        hotkeySettings = new HotkeySettingsControl();
        tabPageTimer = new TabPage();
        timerSettings = new TimerSettingsControl();
        btnConfirmSettings = new Button();
        panelBottom = new Panel();
        tabControl.SuspendLayout();
        tabPageGeneral.SuspendLayout();
        tabPageHotkeys.SuspendLayout();
        tabPageTimer.SuspendLayout();
        panelBottom.SuspendLayout();
        SuspendLayout();
        //
        // tabControl
        //
        tabControl.Controls.Add(tabPageGeneral);
        tabControl.Controls.Add(tabPageHotkeys);
        tabControl.Controls.Add(tabPageTimer);
        tabControl.Dock = DockStyle.Fill;
        tabControl.Location = new Point(0, 0);
        tabControl.Name = "tabControl";
        tabControl.SelectedIndex = 0;
        tabControl.Size = new Size(371, 391);
        tabControl.TabIndex = 0;
        //
        // tabPageGeneral
        //
        tabPageGeneral.Controls.Add(generalSettings);
        tabPageGeneral.Location = new Point(4, 37);
        tabPageGeneral.Name = "tabPageGeneral";
        tabPageGeneral.Padding = new Padding(3);
        tabPageGeneral.Size = new Size(363, 350);
        tabPageGeneral.TabIndex = 0;
        tabPageGeneral.Text = "通用";
        //
        // generalSettings
        //
        generalSettings.AutoScroll = true;
        generalSettings.Dock = DockStyle.Fill;
        generalSettings.Location = new Point(3, 3);
        generalSettings.Name = "generalSettings";
        generalSettings.Size = new Size(357, 344);
        generalSettings.TabIndex = 0;
        //
        // tabPageHotkeys
        //
        tabPageHotkeys.Controls.Add(hotkeySettings);
        tabPageHotkeys.Location = new Point(4, 37);
        tabPageHotkeys.Name = "tabPageHotkeys";
        tabPageHotkeys.Padding = new Padding(3);
        tabPageHotkeys.Size = new Size(363, 350);
        tabPageHotkeys.TabIndex = 1;
        tabPageHotkeys.Text = "快捷键";
        //
        // tabPageTimer
        //
        tabPageTimer.Controls.Add(timerSettings);
        tabPageTimer.Location = new Point(4, 37);
        tabPageTimer.Name = "tabPageTimer";
        tabPageTimer.Padding = new Padding(3);
        tabPageTimer.Size = new Size(363, 350);
        tabPageTimer.TabIndex = 2;
        tabPageTimer.Text = "计时器";
        //
        // hotkeySettings
        //
        hotkeySettings.AutoScroll = true;
        hotkeySettings.Dock = DockStyle.Fill;
        hotkeySettings.Location = new Point(3, 3);
        hotkeySettings.Name = "hotkeySettings";
        hotkeySettings.Size = new Size(357, 344);
        hotkeySettings.TabIndex = 0;
        //
        // timerSettings
        //
        timerSettings.AutoScroll = true;
        timerSettings.Dock = DockStyle.Fill;
        timerSettings.Location = new Point(3, 3);
        timerSettings.Name = "timerSettings";
        timerSettings.Size = new Size(357, 344);
        timerSettings.TabIndex = 0;
        //
        // btnConfirmSettings
        //
        btnConfirmSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnConfirmSettings.Location = new Point(271, 6);
        btnConfirmSettings.Name = "btnConfirmSettings";
        btnConfirmSettings.Size = new Size(80, 30);
        btnConfirmSettings.TabIndex = 0;
        btnConfirmSettings.Text = "确认";
        btnConfirmSettings.Click += BtnConfirmSettings_Click;
        //
        // panelBottom
        //
        panelBottom.Controls.Add(btnConfirmSettings);
        panelBottom.Dock = DockStyle.Bottom;
        panelBottom.Location = new Point(0, 391);
        panelBottom.Name = "panelBottom";
        panelBottom.Size = new Size(371, 45);
        panelBottom.TabIndex = 1;
        //
        // SettingsControl
        //
        Controls.Add(tabControl);
        Controls.Add(panelBottom);
        Name = "SettingsControl";
        Size = new Size(371, 436);
        tabControl.ResumeLayout(false);
        tabPageGeneral.ResumeLayout(false);
        tabPageHotkeys.ResumeLayout(false);
        tabPageTimer.ResumeLayout(false);
        panelBottom.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private TabControl tabControl;
    private TabPage tabPageGeneral;
    private TabPage tabPageHotkeys;
    private Button btnConfirmSettings;
    private Panel panelBottom;
    private GeneralSettingsControl generalSettings;
    private HotkeySettingsControl hotkeySettings;
    private TimerSettingsControl timerSettings;
    private TabPage tabPageTimer;
}