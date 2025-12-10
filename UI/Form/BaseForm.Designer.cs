using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.UI.Theme;

namespace DiabloTwoMFTimer.UI.Form;

partial class BaseForm
{
    private System.ComponentModel.IContainer components = null;

    protected ThemedButton btnConfirm;
    protected ThemedButton btnCancel;
    protected System.Windows.Forms.Panel pnlContent;

    private System.Windows.Forms.Panel pnlTitleBar;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Panel pnlBottomButtons;
    private System.Windows.Forms.FlowLayoutPanel flpButtons;

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
        this.pnlTitleBar = new System.Windows.Forms.Panel();
        this.lblTitle = new System.Windows.Forms.Label();
        this.btnClose = new System.Windows.Forms.Button();
        this.pnlBottomButtons = new System.Windows.Forms.Panel();
        this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
        this.btnCancel = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.btnConfirm = new DiabloTwoMFTimer.UI.Components.ThemedButton();
        this.pnlContent = new System.Windows.Forms.Panel();

        this.pnlTitleBar.SuspendLayout();
        this.pnlBottomButtons.SuspendLayout();
        this.flpButtons.SuspendLayout();
        this.SuspendLayout();

        // 
        // pnlTitleBar
        // 
        this.pnlTitleBar.BackColor = AppTheme.SurfaceColor;
        this.pnlTitleBar.Controls.Add(this.lblTitle);
        this.pnlTitleBar.Controls.Add(this.btnClose);
        this.pnlTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
        this.pnlTitleBar.Height = 35;
        this.pnlTitleBar.MouseDown += PnlTitleBar_MouseDown;

        // 
        // lblTitle
        // 
        this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblTitle.Font = Theme.AppTheme.SmallTitleFont;
        this.lblTitle.ForeColor = AppTheme.AccentColor;
        this.lblTitle.TextAlign = ContentAlignment.MiddleLeft;
        this.lblTitle.Padding = new Padding(10, 0, 0, 0);
        this.lblTitle.MouseDown += PnlTitleBar_MouseDown;

        // 
        // btnClose
        // 
        this.btnClose.Dock = System.Windows.Forms.DockStyle.Right;
        this.btnClose.Width = 40;
        this.btnClose.FlatStyle = FlatStyle.Flat;
        this.btnClose.FlatAppearance.BorderSize = 0;
        this.btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(232, 17, 35);
        this.btnClose.ForeColor = AppTheme.TextSecondaryColor;
        this.btnClose.Text = "×";
        this.btnClose.Font = Theme.AppTheme.ArialFont;
        this.btnClose.Cursor = Cursors.Hand;
        this.btnClose.Click += (s, e) => this.Close();

        // 
        // pnlBottomButtons
        // 
        this.pnlBottomButtons.Controls.Add(this.flpButtons);
        this.pnlBottomButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.pnlBottomButtons.Height = 60;
        this.pnlBottomButtons.BackColor = AppTheme.BackColor;

        // 
        // flpButtons
        // 
        this.flpButtons.Controls.Add(this.btnCancel);
        this.flpButtons.Controls.Add(this.btnConfirm);
        this.flpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
        this.flpButtons.FlowDirection = FlowDirection.RightToLeft;
        this.flpButtons.Padding = new Padding(0, 8, 15, 8);

        // 
        // btnConfirm
        // 
        this.btnConfirm.Size = new Size(80, 34);
        this.btnConfirm.Text = "Confirm";
        this.btnConfirm.Margin = new Padding(10, 0, 10, 0);
        this.btnConfirm.Click += new System.EventHandler(this.BtnConfirm_Click);

        // 
        // btnCancel
        // 
        this.btnCancel.Size = new Size(80, 34);
        this.btnCancel.Text = "Cancel";
        this.btnCancel.Margin = new Padding(0);
        this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);

        // 
        // pnlContent
        // 
        // 【核心修复】必须开启 AutoSize，否则 BaseForm 会把它压扁
        this.pnlContent.AutoSize = true;
        this.pnlContent.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pnlContent.BackColor = AppTheme.BackColor;
        this.pnlContent.Padding = new Padding(20);

        // 
        // BaseForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = AppTheme.BackColor;
        this.AutoSize = true;
        this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterScreen;

        // 【关键顺序】: Dock 布局优先级与添加顺序相反 (后添加的先布局)
        // 1. 先加 Content (Index 0, Z-Order Top) -> Dock Last (Fill)
        this.Controls.Add(this.pnlContent);
        // 2. 后加 Top/Bottom (Index > 0) -> Dock First
        this.Controls.Add(this.pnlBottomButtons);
        this.Controls.Add(this.pnlTitleBar);

        this.pnlTitleBar.ResumeLayout(false);
        this.pnlBottomButtons.ResumeLayout(false);
        this.flpButtons.ResumeLayout(false);
        this.ResumeLayout(false);
        this.PerformLayout(); // 触发布局计算
    }
}