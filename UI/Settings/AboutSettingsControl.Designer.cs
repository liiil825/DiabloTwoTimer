namespace DiabloTwoMFTimer.UI.Settings;

partial class AboutSettingsControl
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

    private void InitializeComponent()
    {
        this.pnlMain = new System.Windows.Forms.Panel();
        this.lblAppName = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.lblVersion = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.lblAuthor = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.btnGithub = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnBilibili = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.pnlMain.SuspendLayout();
        this.SuspendLayout();

        // 
        // pnlMain
        // 
        this.pnlMain.Controls.Add(this.lblAppName);
        this.pnlMain.Controls.Add(this.lblVersion);
        this.pnlMain.Controls.Add(this.lblAuthor);
        this.pnlMain.Controls.Add(this.btnGithub);
        this.pnlMain.Controls.Add(this.btnBilibili);
        this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pnlMain.Location = new System.Drawing.Point(0, 0);
        this.pnlMain.Name = "pnlMain";
        this.pnlMain.Size = new System.Drawing.Size(600, 300);
        this.pnlMain.TabIndex = 0;

        // 
        // lblAppName
        // 
        this.lblAppName.AutoSize = true;
        this.lblAppName.Font = DiabloTwoMFTimer.UI.Theme.AppTheme.BigTitleFont;
        this.lblAppName.ForeColor = System.Drawing.Color.White;
        this.lblAppName.Margin = new System.Windows.Forms.Padding(20, 20, 0, 10);
        this.lblAppName.Name = "lblAppName";
        this.lblAppName.Size = new System.Drawing.Size(225, 32);
        this.lblAppName.Text = "Diablo II Timer";
        this.lblAppName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.lblAppName.Location = new System.Drawing.Point(20, 20);

        // 
        // lblVersion
        // 
        this.lblVersion.AutoSize = true;
        this.lblVersion.Font = new System.Drawing.Font("Microsoft YaHei", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.lblVersion.ForeColor = System.Drawing.Color.LightGray;
        this.lblVersion.Margin = new System.Windows.Forms.Padding(20, 10, 0, 10);
        this.lblVersion.Name = "lblVersion";
        this.lblVersion.Size = new System.Drawing.Size(80, 25);
        this.lblVersion.Text = "版本";
        this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.lblVersion.Location = new System.Drawing.Point(20, 72);

        // 
        // lblAuthor
        // 
        this.lblAuthor.AutoSize = true;
        this.lblAuthor.Font = new System.Drawing.Font("Microsoft YaHei", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.lblAuthor.ForeColor = System.Drawing.Color.White;
        this.lblAuthor.Margin = new System.Windows.Forms.Padding(20, 10, 0, 10);
        this.lblAuthor.Name = "lblAuthor";
        this.lblAuthor.Size = new System.Drawing.Size(80, 25);
        this.lblAuthor.Text = "作者";
        this.lblAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.lblAuthor.Location = new System.Drawing.Point(20, 117);

        // 
        // btnGithub
        // 
        this.btnGithub.Margin = new System.Windows.Forms.Padding(20, 10, 0, 10);
        this.btnGithub.Size = new System.Drawing.Size(150, 40);
        this.btnGithub.Name = "btnGithub";
        this.btnGithub.Text = "GitHub";
        this.btnGithub.Click += new System.EventHandler(this.BtnGithub_Click);
        this.btnGithub.Location = new System.Drawing.Point(20, 162);

        // 
        // btnBilibili
        // 
        this.btnBilibili.Margin = new System.Windows.Forms.Padding(20, 10, 0, 10);
        this.btnBilibili.Size = new System.Drawing.Size(150, 40);
        this.btnBilibili.Name = "btnBilibili";
        this.btnBilibili.Text = "Bilibili";
        this.btnBilibili.Click += new System.EventHandler(this.BtnBilibili_Click);
        this.btnBilibili.Location = new System.Drawing.Point(20, 212);

        // 
        // AboutSettingsControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this.pnlMain);
        this.Name = "AboutSettingsControl";
        this.Size = new System.Drawing.Size(600, 300);
        this.pnlMain.ResumeLayout(false);
        this.pnlMain.PerformLayout();
        this.ResumeLayout(false);
    }

    private System.Windows.Forms.Panel pnlMain;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblAppName;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblVersion;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblAuthor;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnGithub;
    private DiabloTwoMFTimer.UI.Components.ThemedButton btnBilibili;
}