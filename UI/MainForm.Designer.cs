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
        if (disposing)
        {
            if (_notifyIcon != null) _notifyIcon.Dispose();
            if (_components != null) _components.Dispose();
            if (components != null) components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void ConfigureNavButton(Button btn, string text)
    {
        btn.Dock = DockStyle.Fill;
        btn.Text = text;
        btn.Margin = new Padding(0); // 紧贴
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 0;
        // 绑定事件
        btn.Click += NavButton_Click;
    }

    private void InitializeComponent()
    {
        this.tabControl = new DiabloTwoMFTimer.UI.Components.ThemedTabControl();
        this.tabProfilePage = new System.Windows.Forms.TabPage();
        this.tabTimerPage = new System.Windows.Forms.TabPage();
        this.tabPomodoroPage = new System.Windows.Forms.TabPage();
        this.tabSettingsPage = new System.Windows.Forms.TabPage();
        this.tabMinimizePage = new System.Windows.Forms.TabPage(); // 新增：最小化用的 Tab
        this.tlpNavigation = new System.Windows.Forms.TableLayoutPanel();
        this.btnNavProfile = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnNavTimer = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnNavPomodoro = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnNavSettings = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnNavMinimize = new DiabloTwoMFTimer.UI.Components.ThemedButton(); // 新增声明

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
        this.tabControl.Padding = new System.Drawing.Point(0, 0);
        this.tabControl.BackColor = AppTheme.BackColor;
        this.tabControl.Name = "tabControl";
        this.tabControl.SelectedIndex = 0;
        this.tabControl.TabIndex = 1;

        // --- 核心修改：布局设置 ---
        this.tabControl.ItemSize = new System.Drawing.Size(UISizeConstants.TabItemWidth, UISizeConstants.TabItemHeight);
        this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
        // -------------------------
        this.tabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);

        this.tlpNavigation.ColumnCount = 5;
        this.tlpNavigation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
        this.tlpNavigation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
        this.tlpNavigation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
        this.tlpNavigation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
        this.tlpNavigation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));

        this.tlpNavigation.Controls.Add(this.btnNavProfile, 0, 0);
        this.tlpNavigation.Controls.Add(this.btnNavTimer, 1, 0);
        this.tlpNavigation.Controls.Add(this.btnNavPomodoro, 2, 0);
        this.tlpNavigation.Controls.Add(this.btnNavSettings, 3, 0);
        this.tlpNavigation.Controls.Add(this.btnNavMinimize, 4, 0);

        this.tlpNavigation.Dock = System.Windows.Forms.DockStyle.Top;
        this.tlpNavigation.Location = new System.Drawing.Point(0, 0);
        this.tlpNavigation.Name = "tlpNavigation";
        this.tlpNavigation.RowCount = 1;
        this.tlpNavigation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        // 高度将在构造函数中设置，这里先给个默认值
        this.tlpNavigation.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, Theme.UISizeConstants.TabItemHeight);
        this.tlpNavigation.TabIndex = 0;

        // 配置按钮 (去除边框，设为透明，文字颜色等在 CS 中统一管理)
        ConfigureNavButton(btnNavProfile, "Profile");
        ConfigureNavButton(btnNavTimer, "Timer");
        ConfigureNavButton(btnNavPomodoro, "Tomato");
        ConfigureNavButton(btnNavSettings, "Settings");
        ConfigureNavButton(btnNavMinimize, "x"); // 下划线表示最小化

        // 
        // tabControl
        // 
        // 关键：因为 tlpNavigation Dock=Top，tabControl 设为 Fill 会自动填充剩余区域
        this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tabControl.Location = new System.Drawing.Point(0, 40); // Y坐标会被自动调整

        // 
        // tabProfilePage
        // 
        this.tabProfilePage.Location = new System.Drawing.Point(0, UISizeConstants.TabItemHeight);
        this.tabProfilePage.Name = "tabProfilePage";
        this.tabProfilePage.Padding = new System.Windows.Forms.Padding(0);
        this.tabProfilePage.TabIndex = 0;
        this.tabProfilePage.Text = "Profile";
        this.tabProfilePage.UseVisualStyleBackColor = true;

        // 
        // tabTimerPage
        // 
        this.tabTimerPage.Location = new System.Drawing.Point(0, UISizeConstants.TabItemHeight);
        this.tabTimerPage.Name = "tabTimerPage";
        this.tabTimerPage.Padding = new System.Windows.Forms.Padding(0);
        this.tabTimerPage.TabIndex = 1;
        this.tabTimerPage.Text = "Timer";
        this.tabTimerPage.UseVisualStyleBackColor = true;

        // 
        // tabPomodoroPage
        // 
        this.tabPomodoroPage.Location = new System.Drawing.Point(0, UISizeConstants.TabItemHeight);
        this.tabPomodoroPage.Name = "tabPomodoroPage";
        this.tabPomodoroPage.Padding = new System.Windows.Forms.Padding(0);
        this.tabPomodoroPage.TabIndex = 2;
        this.tabPomodoroPage.Text = "Tomato";
        this.tabPomodoroPage.UseVisualStyleBackColor = true;

        // 
        // tabSettingsPage
        // 
        this.tabSettingsPage.Location = new System.Drawing.Point(0, UISizeConstants.TabItemHeight);
        this.tabSettingsPage.Name = "tabSettingsPage";
        this.tabSettingsPage.Padding = new System.Windows.Forms.Padding(0);
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
        this.Controls.Add(this.tlpNavigation);
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
    private System.Windows.Forms.TableLayoutPanel tlpNavigation; // 新增导航容器
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnNavProfile;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnNavTimer;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnNavPomodoro;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnNavSettings;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnNavMinimize; // 新增声明
}