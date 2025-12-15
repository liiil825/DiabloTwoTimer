using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Settings;

public partial class HotkeySettingsControl : UserControl
{
    private readonly Color ColorNormal = AppTheme.SurfaceColor;
    private readonly Color ColorEditing = Color.FromArgb(60, 60, 70);

    private bool _isUpdating = false;
    private IMessenger? _messenger;

    public Keys LeaderHotkey { get; private set; } // 新增
    public Keys StartOrNextRunHotkey { get; private set; }
    public Keys PauseHotkey { get; private set; }
    public Keys DeleteHistoryHotkey { get; private set; }
    public Keys RecordLootHotkey { get; private set; }

    public HotkeySettingsControl()
    {
        InitializeComponent();
        InitializeTextBoxStyles();
    }

    public void SetMessenger(IMessenger messenger)
    {
        _messenger = messenger;
    }

    private void InitializeTextBoxStyles()
    {
        ApplyStyle(txtLeaderKey); // 新增
        ApplyStyle(txtStartNext);
        ApplyStyle(txtPause);
        ApplyStyle(txtDeleteHistory);
        ApplyStyle(txtRecordLoot);
    }

    private void ApplyStyle(TextBox tb)
    {
        tb.BackColor = ColorNormal;
        tb.ForeColor = AppTheme.TextColor;
        tb.BorderStyle = BorderStyle.FixedSingle;
    }

    private void OnTextBoxEnter(object? sender, EventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        if (_isUpdating)
        {
            _isUpdating = false;
            return;
        }

        // 暂停全局热键
        _messenger?.Publish(new SuspendHotkeysMessage());

        textBox.BackColor = ColorEditing;
        textBox.ForeColor = AppTheme.AccentColor;

        // --- 核心修改：提示信息显示在底部标签，而不是输入框内 ---
        // 尝试从语言文件获取，如果为空则使用默认
        string hint =
            LanguageManager.GetString("HotkeyPressToSetDetail") ?? "请按下快捷键 (Esc取消, Backspace/Delete清除)";
        lblStatus.Text = hint;
        lblStatus.ForeColor = AppTheme.Colors.Primary;

        // 可选：输入框内可以显示 "..." 表示正在等待
        textBox.Text = LanguageManager.GetString("PleasePressKey") ?? "请按下快捷键";
    }

    private void OnTextBoxLeave(object? sender, EventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        // 恢复全局热键
        _messenger?.Publish(new ResumeHotkeysMessage());

        textBox.BackColor = ColorNormal;
        textBox.ForeColor = AppTheme.TextColor;

        // 清除底部提示
        lblStatus.Text = "";

        // 还原显示当前保存的键值
        RestoreTextBoxValue(textBox);

        _isUpdating = false;
    }

    private void RestoreTextBoxValue(TextBox textBox)
    {
        string tag = textBox.Tag?.ToString() ?? "";
        Keys currentKey = Keys.None;
        switch (tag)
        {
            case "Leader":
                currentKey = LeaderHotkey;
                break;
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
    }

    private void OnHotkeyInput(object? sender, KeyEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        e.SuppressKeyPress = true;

        // Esc: 取消
        if (e.KeyCode == Keys.Escape)
        {
            this.Parent?.Focus();
            return;
        }

        // Back/Delete: 清除热键 (设为 None)
        if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
        {
            _isUpdating = true;
            UpdateHotkey(textBox, Keys.None);
            this.Parent?.Focus();
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

        _isUpdating = true;
        UpdateHotkey(textBox, keyData);
        this.Parent?.Focus();
    }

    private void UpdateHotkey(TextBox textBox, Keys newKey)
    {
        string tag = textBox.Tag?.ToString() ?? "";
        switch (tag)
        {
            case "Leader":
                LeaderHotkey = newKey;
                break;
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
        textBox.Text = FormatKeyString(newKey);
        textBox.BackColor = ColorNormal;
        textBox.ForeColor = AppTheme.TextColor;
    }

    private string FormatKeyString(Keys key)
    {
        if (key == Keys.None)
            return LanguageManager.GetString("HotkeyNone") ?? "无 (None)";

        var converter = new KeysConverter();
        return converter.ConvertToString(key) ?? "None";
    }

    public void LoadHotkeys(IAppSettings settings)
    {

        LeaderHotkey = settings.HotkeyLeader;
        StartOrNextRunHotkey = settings.HotkeyStartOrNext;
        PauseHotkey = settings.HotkeyPause;
        DeleteHistoryHotkey = settings.HotkeyDeleteHistory;
        RecordLootHotkey = settings.HotkeyRecordLoot;

        txtLeaderKey.Text = FormatKeyString(LeaderHotkey); // 更新 UI
        txtStartNext.Text = FormatKeyString(StartOrNextRunHotkey);
        txtPause.Text = FormatKeyString(PauseHotkey);
        txtDeleteHistory.Text = FormatKeyString(DeleteHistoryHotkey);
        txtRecordLoot.Text = FormatKeyString(RecordLootHotkey);
    }

    public void RefreshUI()
    {
        this.SafeInvoke(() =>
        {
            if (grpHotkeys == null)
                return;
            try
            {
                grpHotkeys.Text = LanguageManager.GetString("HotkeySettingsGroup");

                // 确保添加 LeaderKey 的本地化字符串
                lblLeaderKey.Text = LanguageManager.GetString("HotkeyLeader") ?? "Leader Key:";

                lblStartNext.Text = LanguageManager.GetString("HotkeyStartNext");
                lblPause.Text = LanguageManager.GetString("HotkeyPause");
                lblDeleteHistory.Text = LanguageManager.GetString("HotkeyDeleteHistory");
                lblRecordLoot.Text = LanguageManager.GetString("HotkeyRecordLoot");

                // 这里保存逻辑需要根据你的 Settings 实现来调整
                // 通常应该传递包含新属性的对象
                LoadHotkeys(
                    new Services.AppSettings
                    {
                        HotkeyLeader = LeaderHotkey,
                        HotkeyStartOrNext = StartOrNextRunHotkey,
                        HotkeyPause = PauseHotkey,
                        HotkeyDeleteHistory = DeleteHistoryHotkey,
                        HotkeyRecordLoot = RecordLootHotkey,
                    }
                );
            }
            catch { }
        });
    }
}
