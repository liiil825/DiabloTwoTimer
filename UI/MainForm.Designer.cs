#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DTwoMFTimerHelper.UI;

partial class MainForm
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        tabControl = new TabControl();
        tabProfilePage = new System.Windows.Forms.TabPage();
        tabTimerPage = new System.Windows.Forms.TabPage();
        tabPomodoroPage = new System.Windows.Forms.TabPage();
        tabSettingsPage = new System.Windows.Forms.TabPage();
        tabControl.SuspendLayout();
        SuspendLayout();

        // tabControl
        tabControl.Controls.Add(tabProfilePage);
        tabControl.Controls.Add(tabTimerPage);
        tabControl.Controls.Add(tabPomodoroPage);
        tabControl.Controls.Add(tabSettingsPage);
        tabControl.Dock = DockStyle.Fill;
        tabControl.Location = new Point(0, 0);
        tabControl.Margin = new Padding(6);
        tabControl.Name = "tabControl";
        tabControl.SelectedIndex = 0;
        tabControl.Size = new Size(894, 813);
        tabControl.TabIndex = 1;
        tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;

        // tabProfilePage
        tabProfilePage.Location = new Point(4, 37);
        tabProfilePage.Name = "tabProfilePage";
        tabProfilePage.Size = new Size(886, 813);
        tabProfilePage.TabIndex = 0;
        tabProfilePage.Text = "Profile";
        tabProfilePage.UseVisualStyleBackColor = true;

        // tabTimerPage
        tabTimerPage.Location = new Point(4, 37);
        tabTimerPage.Name = "tabTimerPage";
        tabTimerPage.Size = new Size(886, 813);
        tabTimerPage.TabIndex = 1;
        tabTimerPage.UseVisualStyleBackColor = true;

        // tabPomodoroPage
        tabPomodoroPage.Location = new Point(4, 37);
        tabPomodoroPage.Name = "tabPomodoroPage";
        tabPomodoroPage.Size = new Size(886, 813);
        tabPomodoroPage.TabIndex = 2;
        tabPomodoroPage.UseVisualStyleBackColor = true;

        // tabSettingsPage
        tabSettingsPage.Location = new Point(4, 37);
        tabSettingsPage.Name = "tabSettingsPage";
        tabSettingsPage.Size = new Size(886, 837);
        tabSettingsPage.TabIndex = 3;
        tabSettingsPage.UseVisualStyleBackColor = true;

        // MainForm
        AutoScaleDimensions = new SizeF(13F, 28F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(894, 950);
        Controls.Add(tabControl);
        Margin = new Padding(6);
        Name = "MainForm";
        tabControl.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage tabProfilePage;
    private System.Windows.Forms.TabPage tabTimerPage;
    private System.Windows.Forms.TabPage tabPomodoroPage;
    private System.Windows.Forms.TabPage tabSettingsPage;
}