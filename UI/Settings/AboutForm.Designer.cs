namespace DiabloTwoMFTimer.UI.Form
{
    partial class AboutForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tlpContent = new System.Windows.Forms.TableLayoutPanel();
            this.lblAppName = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.lblVersion = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.lblAuthor = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.btnGithub = new DiabloTwoMFTimer.UI.Components.ThemedButton();
            this.btnBilibili = new DiabloTwoMFTimer.UI.Components.ThemedButton();

            this.pnlContent.SuspendLayout();
            this.tlpContent.SuspendLayout();
            this.SuspendLayout();

            // 
            // tlpContent
            // 
            this.tlpContent.AutoSize = true;
            this.tlpContent.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpContent.ColumnCount = 1;
            this.tlpContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

            this.tlpContent.Controls.Add(this.lblAppName, 0, 0);
            this.tlpContent.Controls.Add(this.lblVersion, 0, 1);
            this.tlpContent.Controls.Add(this.lblAuthor, 0, 2);
            this.tlpContent.Controls.Add(this.btnGithub, 0, 3);
            this.tlpContent.Controls.Add(this.btnBilibili, 0, 4);

            this.tlpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpContent.RowCount = 5;
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Name
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Version
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Author
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F)); // Github
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F)); // Bilibili
            this.tlpContent.Padding = new System.Windows.Forms.Padding(20);

            // 
            // lblAppName
            // 
            this.lblAppName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAppName.Font = DiabloTwoMFTimer.UI.Theme.AppTheme.BigTitleFont;
            this.lblAppName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAppName.Name = "lblAppName";
            this.lblAppName.Text = "Diablo II Timer";
            this.lblAppName.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);

            // 
            // lblVersion
            // 
            this.lblVersion.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Text = "v1.0.0";
            this.lblVersion.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.TextSecondaryColor;

            // 
            // lblAuthor
            // 
            this.lblAuthor.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAuthor.Name = "lblAuthor";
            this.lblAuthor.Text = "Author: liiil825";
            this.lblAuthor.Margin = new System.Windows.Forms.Padding(0, 0, 0, 20);

            // 
            // btnGithub
            // 
            this.btnGithub.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnGithub.Size = new System.Drawing.Size(200, 45);
            this.btnGithub.Name = "btnGithub";
            this.btnGithub.Text = "GitHub";
            this.btnGithub.Click += new System.EventHandler(this.BtnGithub_Click);
            // TODO: 如果有资源文件，可以在这里设置 Image
            // this.btnGithub.Image = Properties.Resources.github_icon;
            // this.btnGithub.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;

            // 
            // btnBilibili
            // 
            this.btnBilibili.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnBilibili.Size = new System.Drawing.Size(200, 45);
            this.btnBilibili.Name = "btnBilibili";
            this.btnBilibili.Text = "Bilibili";
            this.btnBilibili.Click += new System.EventHandler(this.BtnBilibili_Click);
            // TODO: 如果有资源文件，可以在这里设置 Image
            // this.btnBilibili.Image = Properties.Resources.bilibili_icon;
            // this.btnBilibili.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;

            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.MinimumSize = new System.Drawing.Size(350, 0);

            this.pnlContent.Controls.Add(this.tlpContent);
            this.Name = "AboutForm";
            this.Text = "About";

            // 隐藏底部的 Confirm/Cancel 按钮，因为关于页面通常只需要一个关闭
            this.btnConfirm.Visible = false;
            this.btnCancel.Visible = false;
            // 或者你可以把 btnConfirm 改成 "关闭" 并居中，这里演示直接隐藏底部按钮区，使用右上角关闭
            // 如果 BaseForm 允许，可以隐藏 pnlBottomButtons，或者保留它作为空白区

            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.tlpContent.ResumeLayout(false);
            this.tlpContent.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TableLayoutPanel tlpContent;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblAppName;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblVersion;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblAuthor;
        private DiabloTwoMFTimer.UI.Components.ThemedButton btnGithub;
        private DiabloTwoMFTimer.UI.Components.ThemedButton btnBilibili;
    }
}