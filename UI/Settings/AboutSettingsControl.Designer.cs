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
    {// 初始化 SettingsForm 特有的控件
        this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
        this.lblAppName = new System.Windows.Forms.Label();
        this.lblTitle = new System.Windows.Forms.Label();
        this.lblVersion = new System.Windows.Forms.Label();
        this.lblAuthor = new System.Windows.Forms.Label();
        this.btnGithub = new System.Windows.Forms.Button();
        this.btnBilibili = new System.Windows.Forms.Button();
        this.tlpMain.SuspendLayout();
        this.SuspendLayout();

        // 
        // tlpMain
        // 
        this.tlpMain.ColumnCount = 2;
        this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.tlpMain.Controls.Add(this.lblAppName, 0, 0);
        this.tlpMain.Controls.Add(this.btnGithub, 0, 4);
        this.tlpMain.Controls.Add(this.btnBilibili, 1, 4);
        this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpMain.Location = new System.Drawing.Point(0, 0);
        this.tlpMain.Name = "tlpMain";
        this.tlpMain.RowCount = 5;
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
        this.tlpMain.Size = new System.Drawing.Size(600, 200);
        this.tlpMain.TabIndex = 0;

        // 
        // lblAppName
        // 
        this.lblAppName.AutoSize = true;
        this.lblAppName.Font = new System.Drawing.Font("Microsoft YaHei", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.lblAppName.ForeColor = System.Drawing.Color.White;
        this.lblAppName.Location = new System.Drawing.Point(200, 10);
        this.lblAppName.Name = "lblAppName";
        this.lblAppName.Size = new System.Drawing.Size(200, 31);
        this.lblAppName.TabIndex = 0;
        this.lblAppName.Text = "Diablo II Timer";
        this.lblAppName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.tlpMain.SetColumnSpan(this.lblAppName, 2);
        this.tlpMain.SetRow(this.lblAppName, 0);
        this.tlpMain.SetColumn(this.lblAppName, 0);
        this.tlpMain.Controls.Add(this.lblAppName, 0, 0);

        // 
        // lblTitle
        // 
        this.lblTitle.AutoSize = true;
        this.lblTitle.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
        this.lblTitle.Location = new System.Drawing.Point(280, 70);
        this.lblTitle.Name = "lblTitle";
        this.lblTitle.Size = new System.Drawing.Size(40, 22);
        this.lblTitle.TabIndex = 1;
        this.lblTitle.Text = "关于";
        this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.tlpMain.SetColumnSpan(this.lblTitle, 2);
        this.tlpMain.SetRow(this.lblTitle, 1);
        this.tlpMain.SetColumn(this.lblTitle, 0);
        this.tlpMain.Controls.Add(this.lblTitle, 0, 1);

        // 
        // lblVersion
        // 
        this.lblVersion.AutoSize = true;
        this.lblVersion.Location = new System.Drawing.Point(280, 100);
        this.lblVersion.Name = "lblVersion";
        this.lblVersion.Size = new System.Drawing.Size(40, 16);
        this.lblVersion.TabIndex = 2;
        this.lblVersion.Text = "版本";
        this.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.tlpMain.SetColumnSpan(this.lblVersion, 2);
        this.tlpMain.SetRow(this.lblVersion, 2);
        this.tlpMain.SetColumn(this.lblVersion, 0);
        this.tlpMain.Controls.Add(this.lblVersion, 0, 2);

        // 
        // lblAuthor
        // 
        this.lblAuthor.AutoSize = true;
        this.lblAuthor.Location = new System.Drawing.Point(280, 130);
        this.lblAuthor.Name = "lblAuthor";
        this.lblAuthor.Size = new System.Drawing.Size(40, 16);
        this.lblAuthor.TabIndex = 3;
        this.lblAuthor.Text = "作者";
        this.lblAuthor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.tlpMain.SetColumnSpan(this.lblAuthor, 2);
        this.tlpMain.SetRow(this.lblAuthor, 3);
        this.tlpMain.SetColumn(this.lblAuthor, 0);
        this.tlpMain.Controls.Add(this.lblAuthor, 0, 3);

        // 
        // btnGithub
        // 
        this.btnGithub.Anchor = System.Windows.Forms.AnchorStyles.None;
        this.btnGithub.Location = new System.Drawing.Point(125, 115);
        this.btnGithub.Name = "btnGithub";
        this.btnGithub.Size = new System.Drawing.Size(150, 30);
        this.btnGithub.TabIndex = 3;
        this.btnGithub.Text = "GitHub";
        this.btnGithub.UseVisualStyleBackColor = true;
        this.btnGithub.Click += new System.EventHandler(this.BtnGithub_Click);

        // 
        // btnBilibili
        // 
        this.btnBilibili.Anchor = System.Windows.Forms.AnchorStyles.None;
        this.btnBilibili.Location = new System.Drawing.Point(325, 115);
        this.btnBilibili.Name = "btnBilibili";
        this.btnBilibili.Size = new System.Drawing.Size(150, 30);
        this.btnBilibili.TabIndex = 4;
        this.btnBilibili.Text = "Bilibili";
        this.btnBilibili.UseVisualStyleBackColor = true;
        this.btnBilibili.Click += new System.EventHandler(this.BtnBilibili_Click);

        // 
        // AboutSettingsControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.Controls.Add(this.tlpMain);
        this.Name = "AboutSettingsControl";
        this.Size = new System.Drawing.Size(600, 200);
        this.tlpMain.ResumeLayout(false);
        this.tlpMain.PerformLayout();
        this.ResumeLayout(false);
    }

    private System.Windows.Forms.TableLayoutPanel tlpMain;
    private System.Windows.Forms.Label lblAppName;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Label lblVersion;
    private System.Windows.Forms.Label lblAuthor;
    private System.Windows.Forms.Button btnGithub;
    private System.Windows.Forms.Button btnBilibili;
}