using System;
using System.ComponentModel; // 必须引用，用于 Browsable 等特性
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Form;

public partial class BaseForm : System.Windows.Forms.Form
{
    public BaseForm()
    {
        InitializeComponent();
    }

    // --- 【修复】将属性定义移到这里 ---
    [Browsable(true)]
    [Category("Appearance")]
    public string ConfirmButtonText { get; set; } = "Confirm";

    [Browsable(true)]
    [Category("Appearance")]
    public string CancelButtonText { get; set; } = "Cancel";
    // ---------------------------------

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
        // 绘制边框
        using var pen = new Pen(AppTheme.AccentColor, 1);
        e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
    }

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    public static extern int SendMessage(nint hWnd, int Msg, int wParam, int lParam);

    // Designer.cs 中绑定了这个事件
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
        // 增加空值检查，防止设计器报错
        if (btnConfirm != null)
            btnConfirm.Text = LanguageManager.GetString("Confirm") ?? ConfirmButtonText;

        if (btnCancel != null)
            btnCancel.Text = LanguageManager.GetString("Cancel") ?? CancelButtonText;

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
            // 如果焦点在按钮上，让按钮自己处理，否则默认触发确认
            if (btnConfirm != null && !btnConfirm.Focused && btnCancel != null && !btnCancel.Focused)
            {
                BtnConfirm_Click(null, EventArgs.Empty);
                return true;
            }
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    // 注意：Designer.cs 中绑定了 btnConfirm.Click -> BtnConfirm_Click
    protected virtual void BtnConfirm_Click(object? sender, EventArgs e)
    {
        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    // 注意：Designer.cs 中绑定了 btnCancel.Click -> BtnCancel_Click
    protected virtual void BtnCancel_Click(object? sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}