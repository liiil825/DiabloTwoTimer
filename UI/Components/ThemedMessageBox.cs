using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Form;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Components;

public class ThemedMessageBox : BaseForm
{
    private Label lblMessage;
    private MessageBoxButtons _buttons;

    public ThemedMessageBox(string message, string title, MessageBoxButtons buttons)
    {
        this._buttons = buttons;
        this.Text = title; // BaseForm 会自动将其设置到标题栏 Label

        // --- 1. 构建消息内容 ---
        lblMessage = new Label
        {
            Text = message,
            ForeColor = AppTheme.TextColor,
            Font = AppTheme.MainFont,
            AutoSize = true, // 关键：让高度随内容自动撑开
            // 限制最大宽度以强制换行 (弹窗总宽 - 左右边距)
            MaximumSize = new Size(UISizeConstants.BaseFormWidth - 60, 0),
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill
        };

        // 使用 TableLayoutPanel 作为容器来实现居中布局
        // 直接放在 pnlContent 里也可以，但 TLP 能更好地处理垂直/水平居中
        TableLayoutPanel tlp = new TableLayoutPanel();
        tlp.Dock = DockStyle.Fill;
        tlp.AutoSize = true;
        tlp.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        tlp.ColumnCount = 1;
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tlp.RowCount = 1;
        tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        tlp.Controls.Add(lblMessage, 0, 0);
        // 增加一点内边距，让文字不要贴着边框
        tlp.Padding = new Padding(10, 20, 10, 20);

        // --- 2. 将内容添加到 BaseForm 的 pnlContent ---
        this.pnlContent.Controls.Add(tlp);

        // --- 3. 配置按钮可见性 ---
        ConfigureButtons();
    }

    private void ConfigureButtons()
    {
        switch (_buttons)
        {
            case MessageBoxButtons.OK:
                btnConfirm.Visible = true;
                btnCancel.Visible = false;
                break;
            case MessageBoxButtons.OKCancel:
            case MessageBoxButtons.YesNo:
                btnConfirm.Visible = true;
                btnCancel.Visible = true;
                break;
        }
    }

    // 重写 BaseForm 的 UpdateUI，确保按钮文字正确
    // BaseForm 会在 OnLoad 时调用此方法
    protected override void UpdateUI()
    {
        base.UpdateUI(); // 先让基类设置标题和默认 Confirm/Cancel

        // 覆盖特定类型的按钮文本
        switch (_buttons)
        {
            case MessageBoxButtons.OK:
                // 如果是 OK，显示 "确认" 或 "OK"
                btnConfirm.Text = LanguageManager.GetString("Confirm") ?? "OK";
                break;

            case MessageBoxButtons.YesNo:
                // 如果是 YesNo，显示 "是/否" 或 "Yes/No"
                // 尝试获取资源，没有则使用硬编码 fallback
                btnConfirm.Text = LanguageManager.GetString("Yes") ?? "Yes";
                btnCancel.Text = LanguageManager.GetString("No") ?? "No";
                break;

                // OKCancel 默认使用 Confirm/Cancel 即可，不需要额外覆盖
        }
    }

    // 静态调用方法
    public static DialogResult Show(
        string message,
        string title = "Message",
        MessageBoxButtons buttons = MessageBoxButtons.OK
    )
    {
        using var msgBox = new ThemedMessageBox(message, title, buttons);
        return msgBox.ShowDialog();
    }

    // 按钮点击处理
    protected override void BtnConfirm_Click(object? sender, EventArgs e)
    {
        if (_buttons == MessageBoxButtons.YesNo)
        {
            this.DialogResult = DialogResult.Yes;
        }
        else
        {
            this.DialogResult = DialogResult.OK;
        }
        this.Close();
    }

    protected override void BtnCancel_Click(object? sender, EventArgs e)
    {
        if (_buttons == MessageBoxButtons.YesNo)
        {
            this.DialogResult = DialogResult.No;
        }
        else
        {
            this.DialogResult = DialogResult.Cancel;
        }
        this.Close();
    }
}