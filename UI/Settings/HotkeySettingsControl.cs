using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Settings;

public partial class HotkeySettingsControl : UserControl
{
    // 样式定义
    private readonly Color ColorNormal = Color.White;
    private readonly Color ColorEditing = Color.AliceBlue;

    //防止焦点回弹的标志位
    private bool _isUpdating = false;

    public Keys StartOrNextRunHotkey { get; private set; }
    public Keys PauseHotkey { get; private set; }
    public Keys DeleteHistoryHotkey { get; private set; }
    public Keys RecordLootHotkey { get; private set; }

    public HotkeySettingsControl()
    {
        InitializeComponent();
    }

    // --- 核心修复逻辑 ---

    private void OnTextBoxEnter(object? sender, EventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        // 修复关键：如果是刚刚修改完触发的焦点回弹，不要显示提示语，直接返回
        if (_isUpdating)
        {
            _isUpdating = false;
            return;
        }

        textBox.BackColor = ColorEditing;
        textBox.Text = "请按快捷键 (Esc取消)";
    }

    private void OnTextBoxLeave(object? sender, EventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        textBox.BackColor = ColorNormal;

        // 还原/更新文本
        string tag = textBox.Tag?.ToString() ?? "";
        Keys currentKey = Keys.None;
        switch (tag)
        {
            case "StartNext":
                currentKey = StartOrNextRunHotkey;
                break;
            case "Pause":
                currentKey = PauseHotkey;
                break;
            case "Delete":
                currentKey = DeleteHistoryHotkey;
                break;
            case "Record":
                currentKey = RecordLootHotkey;
                break;
        }
        textBox.Text = FormatKeyString(currentKey);

        // 确保标志位复位（防呆）
        _isUpdating = false;
    }

    private void OnHotkeyInput(object? sender, KeyEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        e.SuppressKeyPress = true;

        // Esc: 取消
        if (e.KeyCode == Keys.Escape)
        {
            // 主动转移焦点到父控件，比 ActiveControl=null 更稳健
            this.Focus();
            return;
        }

        // Delete/Back: 清除
        if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
        {
            // 标记正在更新，防止Enter事件覆盖文本
            _isUpdating = true;
            UpdateHotkey(textBox, Keys.None);
            this.Focus();
            return;
        }

        // 忽略单一控制键
        if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu)
        {
            return;
        }

        Keys keyData = e.KeyCode;
        if (e.Control)
            keyData |= Keys.Control;
        if (e.Shift)
            keyData |= Keys.Shift;
        if (e.Alt)
            keyData |= Keys.Alt;

        // 标记正在更新，防止Enter事件覆盖文本
        _isUpdating = true;

        UpdateHotkey(textBox, keyData);

        // 尝试转移焦点，这会触发 Leave -> Enter(回弹)
        // 但因为我们设置了 _isUpdating = true，回弹的 Enter 会被忽略
        this.Focus();
    }

    private void UpdateHotkey(TextBox textBox, Keys newKey)
    {
        string tag = textBox.Tag?.ToString() ?? "";
        switch (tag)
        {
            case "StartNext":
                StartOrNextRunHotkey = newKey;
                break;
            case "Pause":
                PauseHotkey = newKey;
                break;
            case "Delete":
                DeleteHistoryHotkey = newKey;
                break;
            case "Record":
                RecordLootHotkey = newKey;
                break;
        }
        // 立即更新文本，让用户立刻看到结果
        textBox.Text = FormatKeyString(newKey);
        // 立即恢复颜色
        textBox.BackColor = ColorNormal;
    }

    private string FormatKeyString(Keys key)
    {
        if (key == Keys.None)
            return "无 (None)";
        var converter = new KeysConverter();
        return converter.ConvertToString(key) ?? "None";
    }

    public void LoadHotkeys(Services.IAppSettings settings)
    {
        StartOrNextRunHotkey = settings.HotkeyStartOrNext;
        PauseHotkey = settings.HotkeyPause;
        DeleteHistoryHotkey = settings.HotkeyDeleteHistory;
        RecordLootHotkey = settings.HotkeyRecordLoot;

        txtStartNext.Text = FormatKeyString(StartOrNextRunHotkey);
        txtPause.Text = FormatKeyString(PauseHotkey);
        txtDeleteHistory.Text = FormatKeyString(DeleteHistoryHotkey);
        txtRecordLoot.Text = FormatKeyString(RecordLootHotkey);
    }

    public void RefreshUI()
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(RefreshUI));
            return;
        }
        if (grpHotkeys == null)
            return;
        try
        {
            grpHotkeys.Text = LanguageManager.GetString("HotkeySettingsGroup");
            lblStartNext.Text = LanguageManager.GetString("HotkeyStartNext");
            lblPause.Text = LanguageManager.GetString("HotkeyPause");
            lblDeleteHistory.Text = LanguageManager.GetString("HotkeyDeleteHistory");
            lblRecordLoot.Text = LanguageManager.GetString("HotkeyRecordLoot");

            LoadHotkeys(
                new Services.AppSettings
                {
                    HotkeyStartOrNext = StartOrNextRunHotkey,
                    HotkeyPause = PauseHotkey,
                    HotkeyDeleteHistory = DeleteHistoryHotkey,
                    HotkeyRecordLoot = RecordLootHotkey,
                }
            );
        }
        catch { }
    }
}
