using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Form; // 这里的 Form 是命名空间，不是类 Form

// 继承自 BaseForm
public class CloseOptionForm : BaseForm
{
    // 对外公开属性，供 MainForm 读取
    public bool IsCloseAppSelected { get; private set; }

    private ThemedCheckBox _chkClose = null!;
    private ThemedLabel _lblMessage = null!;

    public CloseOptionForm()
    {
        InitializeCustomUI();
    }

    private void InitializeCustomUI()
    {
        // 1. 【核心修复】调整 Z-Order 解决 BaseForm 的布局 Bug
        // 将 pnlContent 提到最前，使其在布局计算中排在最后 (Last Docked)
        // 这样它就会填充 Top(Title) 和 Bottom(Buttons) 剩余的空间，而不是覆盖它们
        this.pnlContent.BringToFront();

        // 2. 设置窗体属性
        // 关闭自动大小，使用固定大小
        this.AutoSize = false;
        this.MinimumSize = new Size(ScaleHelper.Scale(400), ScaleHelper.Scale(240));
        this.Size = new Size(ScaleHelper.Scale(400), ScaleHelper.Scale(240));
        this.StartPosition = FormStartPosition.CenterScreen; // 强制居中显示

        // 确保内容面板也关闭自动大小
        this.pnlContent.AutoSize = false;

        // 3. 初始化内容控件
        // 提示文字
        _lblMessage = new ThemedLabel
        {
            Text = "程序将缩小到系统托盘继续运行。\n您希望怎么做？",
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Top, // 停靠在 pnlContent 顶部
            Width = this.pnlContent.Width,
            Height = ScaleHelper.Scale(80),
            Font = AppTheme.MainFont
        };

        // 退出选项复选框
        _chkClose = new ThemedCheckBox
        {
            AutoSize = true,
            Checked = false,
            Cursor = Cursors.Hand,
            Font = AppTheme.MainFont,
            ForeColor = AppTheme.AccentColor
        };

        // 4. 将控件加入到 BaseForm 的 pnlContent 容器中
        this.pnlContent.Controls.Add(_lblMessage);
        this.pnlContent.Controls.Add(_chkClose);

        // 5. 手动居中 CheckBox (因为 Panel 内部相对布局比较灵活)
        // 监听 Layout 事件，确保窗口缩放或 DPI 变化时位置正确
        this.pnlContent.Layout += (s, e) =>
        {
            // 在 Message 标签下方居中显示
            int chkX = (this.pnlContent.Width - _chkClose.Width) / 2;
            int chkY = _lblMessage.Bottom + ScaleHelper.Scale(10);
            _chkClose.Location = new Point(chkX, chkY);
        };

        // 6. 绑定逻辑
        _chkClose.CheckedChanged += (s, e) => IsCloseAppSelected = _chkClose.Checked;
        // 订阅语言变更事件
        LanguageManager.OnLanguageChanged += OnLanguageChanged;

        // 7. 更新界面文本
        UpdateUIText();

        // 触发一次布局以定位 CheckBox
        this.pnlContent.PerformLayout();
    }

    private void UpdateUIText()
    {
        // 更新窗体标题
        this.Text = LanguageManager.GetString("CloseOptionTitle");

        // 更新底部按钮文字
        this.ConfirmButtonText = LanguageManager.GetString("Confirm");
        this.CancelButtonText = LanguageManager.GetString("Cancel");

        // 更新提示文字
        _lblMessage.Text = LanguageManager.GetString("CloseOptionMessage");

        // 更新复选框文字
        _chkClose.Text = LanguageManager.GetString("CloseOptionExitApp");

        // 确保 BaseForm 的按钮使用我们新定义的颜色 (如果 BaseForm 没有自动应用)
        if (this.btnConfirm != null) this.btnConfirm.BackColor = AppTheme.ButtonBackColor;
        if (this.btnCancel != null) this.btnCancel.BackColor = AppTheme.ButtonBackColor;
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        UpdateUIText();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        // 取消订阅语言变更事件，避免内存泄漏
        LanguageManager.OnLanguageChanged -= OnLanguageChanged;
    }
}