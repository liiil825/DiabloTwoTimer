using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Form;

public partial class BaseForm : System.Windows.Forms.Form
{
    // --- 新增：字体和图标定义 ---
    protected readonly Font _iconFont = new Font("Segoe MDL2 Assets", 10F, FontStyle.Bold);
    protected const string ICON_CONFIRM = "\uE73E"; // CheckMark
    protected const string ICON_CANCEL = "\uE711"; // ChromeClose

    public BaseForm()
    {
        InitializeComponent();
    }

    // 这些属性保留，但在 UpdateUI 中我们会优先使用图标
    [Browsable(true)]
    [Category("Appearance")]
    public string ConfirmButtonText { get; set; } = "Confirm";

    [Browsable(true)]
    [Category("Appearance")]
    public string CancelButtonText { get; set; } = "Cancel";

    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set
        {
            base.Text = value;
            if (lblTitle != null)
                lblTitle.Text = value;
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using var pen = new Pen(AppTheme.AccentColor, 1);
        e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
    }

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    public static extern int SendMessage(nint hWnd, int Msg, int wParam, int lParam);

    private void PnlTitleBar_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ReleaseCapture();
            SendMessage(Handle, 0xA1, 0x2, 0);
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!this.DesignMode)
            UpdateUI();
    }

    protected virtual void UpdateUI()
    {
        // 核心修改：应用图标字体和字符
        if (btnConfirm != null)
        {
            btnConfirm.Font = _iconFont;
            btnConfirm.Text = ICON_CONFIRM; // 使用打钩图标

            btnConfirm.BackColor = AppTheme.Colors.ButtonBackColor;
            btnConfirm.ForeColor = AppTheme.AccentColor;
        }

        if (btnCancel != null)
        {
            btnCancel.Font = _iconFont;
            btnCancel.Text = ICON_CANCEL; // 使用关闭图标

            btnCancel.BackColor = AppTheme.Colors.ButtonBackColor;
            btnCancel.ForeColor = AppTheme.Colors.TextSecondaryColor;
        }

        if (lblTitle != null)
            lblTitle.Text = Text;
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Escape)
        {
            BtnCancel_Click(null, EventArgs.Empty);
            return true;
        }
        if (keyData == Keys.Enter)
        {
            if (btnConfirm != null && !btnConfirm.Focused && btnCancel != null && !btnCancel.Focused)
            {
                btnConfirm.PerformClick();
                return true;
            }
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    protected virtual void BtnConfirm_Click(object? sender, EventArgs e)
    {
        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    protected virtual void BtnCancel_Click(object? sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}
