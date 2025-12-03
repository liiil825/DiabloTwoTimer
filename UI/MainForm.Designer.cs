#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI;

partial class MainForm
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

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        this.tabControl = new DiabloTwoMFTimer.UI.Components.ThemedTabControl();
        this.tabProfilePage = new System.Windows.Forms.TabPage();
        this.tabTimerPage = new System.Windows.Forms.TabPage();
        this.tabPomodoroPage = new System.Windows.Forms.TabPage();
        this.tabSettingsPage = new System.Windows.Forms.TabPage();
        this.tabMinimizePage = new System.Windows.Forms.TabPage(); // 新增：最小化用的 Tab

        this.tabControl.SuspendLayout();
        this.SuspendLayout();

        // 
        // tabControl
        // 
        this.tabControl.Controls.Add(this.tabProfilePage);
        this.tabControl.Controls.Add(this.tabTimerPage);
        this.tabControl.Controls.Add(this.tabPomodoroPage);
        this.tabControl.Controls.Add(this.tabSettingsPage);
        this.tabControl.Controls.Add(this.tabMinimizePage); // 添加到集合中

        this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tabControl.Margin = new System.Windows.Forms.Padding(0);
        this.tabControl.BackColor = AppTheme.BackColor;
        this.tabControl.Name = "tabControl";
        this.tabControl.SelectedIndex = 0;
        this.tabControl.TabIndex = 1;

        // --- 核心修改：布局设置 ---
        this.tabControl.ItemSize = new System.Drawing.Size(95, 40);
        this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
        // -------------------------
        this.tabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);

        // 
        // tabProfilePage
        // 
        this.tabProfilePage.Location = new System.Drawing.Point(4, 44);
        this.tabProfilePage.Name = "tabProfilePage";
        this.tabProfilePage.Padding = new System.Windows.Forms.Padding(3);
        this.tabProfilePage.TabIndex = 0;
        this.tabProfilePage.Text = "Profile";
        this.tabProfilePage.UseVisualStyleBackColor = true;

        // 
        // tabTimerPage
        // 
        this.tabTimerPage.Location = new System.Drawing.Point(4, 44);
        this.tabTimerPage.Name = "tabTimerPage";
        this.tabTimerPage.Padding = new System.Windows.Forms.Padding(3);
        this.tabTimerPage.TabIndex = 1;
        this.tabTimerPage.Text = "Timer";
        this.tabTimerPage.UseVisualStyleBackColor = true;

        // 
        // tabPomodoroPage
        // 
        this.tabPomodoroPage.Location = new System.Drawing.Point(4, 44);
        this.tabPomodoroPage.Name = "tabPomodoroPage";
        this.tabPomodoroPage.Padding = new System.Windows.Forms.Padding(3);
        this.tabPomodoroPage.TabIndex = 2;
        this.tabPomodoroPage.Text = "Tomato";
        this.tabPomodoroPage.UseVisualStyleBackColor = true;

        // 
        // tabSettingsPage
        // 
        this.tabSettingsPage.Location = new System.Drawing.Point(4, 44);
        this.tabSettingsPage.Name = "tabSettingsPage";
        this.tabSettingsPage.Padding = new System.Windows.Forms.Padding(3);
        this.tabSettingsPage.TabIndex = 3;
        this.tabSettingsPage.Text = "Settings";
        this.tabSettingsPage.UseVisualStyleBackColor = true;

        // 
        // tabMinimizePage (新增)
        // 
        // 不需要设置 Padding 或 Content，因为它只作为一个按钮使用
        this.tabMinimizePage.Name = "tabMinimizePage";
        this.tabMinimizePage.TabIndex = 4;
        this.tabMinimizePage.Text = "_"; // 显示 _
        this.tabMinimizePage.UseVisualStyleBackColor = true;

        // 
        // MainForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.ClientSize = new System.Drawing.Size(UISizeConstants.ClientWidth, UISizeConstants.ClientHeightWithoutLoot);

        // 无边框
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;

        // 只添加 tabControl，移除了之前的 btnClose
        this.Controls.Add(this.tabControl);

        this.Margin = new System.Windows.Forms.Padding(6);
        this.Name = "MainForm";
        this.Text = "D2R Helper";
        this.Icon = new System.Drawing.Icon("Resources\\d2r.ico");

        this.tabControl.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    #endregion

    private DiabloTwoMFTimer.UI.Components.ThemedTabControl tabControl;
    private System.Windows.Forms.TabPage tabProfilePage;
    private System.Windows.Forms.TabPage tabTimerPage;
    private System.Windows.Forms.TabPage tabPomodoroPage;
    private System.Windows.Forms.TabPage tabSettingsPage;
    private System.Windows.Forms.TabPage tabMinimizePage; // 新增声明
}