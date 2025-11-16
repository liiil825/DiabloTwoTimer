using System;
using System.Windows.Forms;
using System.Drawing;

namespace DTwoMFTimerHelper.UI
{
    public partial class MainForm : Form
    {
        // 服务实例
        private readonly MainServices _mainServices;

        // UI控件 - 改为internal以便服务类访问
        internal TabControl? TabControl => tabControl;
        internal TabPage? TabProfilePage => tabProfilePage;
        internal TabPage? TabTimerPage => tabTimerPage;
        internal TabPage? TabPomodoroPage => tabPomodoroPage;
        internal TabPage? TabSettingsPage => tabSettingsPage;

        private System.Windows.Forms.TabControl? tabControl;
        private System.Windows.Forms.TabPage? tabProfilePage;
        private System.Windows.Forms.TabPage? tabTimerPage;
        private System.Windows.Forms.TabPage? tabPomodoroPage;
        private System.Windows.Forms.TabPage? tabSettingsPage;

        public MainForm()
        {
            InitializeComponent();

            // 获取服务实例并设置主窗体引用
            _mainServices = MainServices.Instance;
            _mainServices.SetMainForm(this);

            // 初始化应用程序
            _mainServices.InitializeApplication();

            // 立即刷新UI以确保Tab标题正确显示
            _mainServices.RefreshUI();

            // 设置窗体属性
            InitializeForm();

            // 添加Shown事件处理，确保窗口显示后正确应用位置设置
            this.Shown += (sender, e) => OnMainForm_Shown(sender ?? this, e);
        }

        private void OnMainForm_Shown(object sender, EventArgs e)
        {
            // 窗口显示后再次应用窗口位置设置，确保正确显示在右上角
            _mainServices.ApplyWindowSettings();
        }

        private void InitializeForm()
        {
            this.Size = new Size(480, 600);
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = true;
            this.Visible = true;
        }

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
            tabControl.Size = new Size(894, 878);
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
            ClientSize = new Size(894, 878);
            Controls.Add(tabControl);
            Margin = new Padding(6);
            Name = "MainForm";
            tabControl.ResumeLayout(false);
            ResumeLayout(false);
        }

        /// <summary>
        /// 更新窗体标题（供服务类调用）
        /// </summary>
        public void UpdateFormTitle(string title)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateFormTitle), title);
            }
            else
            {
                this.Text = title;
            }
        }

        /// <summary>
        /// 刷新UI（供外部调用）
        /// </summary>
        public void RefreshUI()
        {
            _mainServices.RefreshUI();
        }

        private void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
        {
            _mainServices.HandleTabChanged();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            _mainServices.ProcessHotKeyMessage(m);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _mainServices.HandleApplicationClosing();
            base.OnFormClosing(e);
        }
    }
}