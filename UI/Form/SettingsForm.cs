using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace DiabloTwoMFTimer.UI.Form;

public partial class SettingsForm : BaseForm
{
    private readonly IAppSettings _appSettings;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessenger _messenger;

    // 用于存储所有的快捷键提示标签，方便统一显隐
    private readonly List<Label> _shortcutBadges = new();
    private bool _showShortcuts = false; // 默认隐藏

    public SettingsForm(
        IAppSettings appSettings,
        IServiceProvider serviceProvider,
        IMessenger messenger)
    {
        InitializeComponent();

        // 设置窗口属性
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.CenterScreen;

        _appSettings = appSettings;
        _serviceProvider = serviceProvider;
        _messenger = messenger;

        this.Text = Utils.LanguageManager.GetString("Settings.Title");

        // 开启键盘预览并绑定事件
        this.KeyPreview = true;
        this.KeyDown += SettingsForm_KeyDown;

        // 去掉构造函数里的 Size 设置，移到 OnLoad
        SetupTabs();
        InitializeData(_appSettings);
        RefreshUI();
    }

    private void SetupTabs()
    {
        // 添加番茄设置Tab页面的内容
        var pomodoroSettings = _serviceProvider.GetRequiredService<UI.Settings.PomodoroSettingsControl>();
        tabPagePomodoro.Text = Utils.LanguageManager.GetString("Settings.Tab.Pomodoro");
        pomodoroSettings.Dock = System.Windows.Forms.DockStyle.Fill;
        tabPagePomodoro.Controls.Add(pomodoroSettings);

        // 添加音频设置Tab页面的内容
        var audioSettings = _serviceProvider.GetRequiredService<UI.Settings.AudioSettingsControl>();
        tabPageAudio.Text = Utils.LanguageManager.GetString("Settings.Tab.Audio");
        audioSettings.Dock = System.Windows.Forms.DockStyle.Fill;
        tabPageAudio.Controls.Add(audioSettings);

        // 添加关于设置Tab页面的内容
        var aboutSettings = _serviceProvider.GetRequiredService<UI.Settings.AboutSettingsControl>();
        tabPageAbout.Text = Utils.LanguageManager.GetString("Settings.Tab.About");
        aboutSettings.Dock = System.Windows.Forms.DockStyle.Fill;
        tabPageAbout.Controls.Add(aboutSettings);

        // 扩展导航按钮数组
        btnSetGeneral.Click += (s, e) => SwitchTab(0, btnSetGeneral);
        btnSetHotkeys.Click += (s, e) => SwitchTab(1, btnSetHotkeys);
        btnSetTimer.Click += (s, e) => SwitchTab(2, btnSetTimer);
        btnSetPomodoro.Click += (s, e) => SwitchTab(3, btnSetPomodoro);
        btnSetAudio.Click += (s, e) => SwitchTab(4, btnSetAudio);
        btnAbout.Click += (s, e) => SwitchTab(5, btnAbout);

        // 为Tab按钮添加快捷键提示标签
        AttachKeyBadge(btnSetGeneral, "1");
        AttachKeyBadge(btnSetHotkeys, "2");
        AttachKeyBadge(btnSetTimer, "3");
        AttachKeyBadge(btnSetPomodoro, "4");
        AttachKeyBadge(btnSetAudio, "5");
        AttachKeyBadge(btnAbout, "6");
        AttachKeyBadge(btnConfirmSettings, "Z");

        // 默认选中
        SwitchTab(0, btnSetGeneral);
    }

    private void SwitchTab(int index, ThemedButton activeBtn)
    {
        tabControl.SelectedIndex = index;

        // 1. 样式逻辑 (使用 IsSelected)
        var buttons = new[] { btnSetGeneral, btnSetHotkeys, btnSetTimer, btnSetPomodoro, btnSetAudio, btnAbout };
        foreach (var btn in buttons)
        {
            btn.IsSelected = (btn == activeBtn);
        }
    }

    private void BtnSetPomodoro_Click(object sender, EventArgs e)
    {
        SwitchTab(3, btnSetPomodoro);
    }

    public void RefreshUI()
    {
        this.SafeInvoke(() =>
        {
            btnConfirmSettings!.Text = LanguageManager.GetString("Settings.Title");
            btnSetGeneral!.Text = LanguageManager.GetString("Settings.Tab.General");
            btnSetHotkeys!.Text = LanguageManager.GetString("Settings.Tab.Hotkeys");
            btnSetTimer!.Text = LanguageManager.GetString("Settings.Tab.Timer");
            btnSetAudio!.Text = LanguageManager.GetString("Settings.Tab.Audio");
            btnAbout!.Text = LanguageManager.GetString("Settings.Tab.About");

            generalSettings.RefreshUI();
            hotkeySettings.RefreshUI();
            timerSettings.RefreshUI();

            // 刷新音频设置UI
            var audioSettings = tabControl.TabPages[3].Controls[0] as UI.Settings.AudioSettingsControl;
            if (audioSettings != null)
            {
                audioSettings.RefreshUI();
            }

            // 刷新番茄设置UI
            var pomodoroSettings = tabControl.TabPages[3].Controls[0] as UI.Settings.PomodoroSettingsControl;
            if (pomodoroSettings != null)
            {
                pomodoroSettings.RefreshUI();
            }

            // 刷新关于设置UI
            var aboutSettings = tabControl.TabPages[5].Controls[0] as UI.Settings.AboutSettingsControl;
            if (aboutSettings != null)
            {
                aboutSettings.UpdateUI();
            }
        });
    }

    public void InitializeData(IAppSettings settings)
    {
        generalSettings.LoadSettings(settings);
        hotkeySettings.LoadHotkeys(settings);
        timerSettings.LoadSettings(settings);

        // 番茄设置已在构造函数中初始化数据
    }

    // 键盘事件处理逻辑
    private void SettingsForm_KeyDown(object? sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.H:
                // 切换提示显示/隐藏
                ToggleShortcutsVisibility();
                e.SuppressKeyPress = true;
                break;

            case Keys.D1:
            case Keys.NumPad1:
                SwitchTab(0, btnSetGeneral);
                break;

            case Keys.D2:
            case Keys.NumPad2:
                SwitchTab(1, btnSetHotkeys);
                break;

            case Keys.D3:
            case Keys.NumPad3:
                SwitchTab(2, btnSetTimer);
                break;

            case Keys.D4:
            case Keys.NumPad4:
                SwitchTab(3, btnSetPomodoro);
                break;

            case Keys.D5:
            case Keys.NumPad5:
                SwitchTab(4, btnSetAudio);
                break;

            case Keys.D6:
            case Keys.NumPad6:
                SwitchTab(5, btnAbout);
                break;

            case Keys.Z:
                if (btnConfirmSettings.Visible && btnConfirmSettings.Enabled)
                    BtnConfirmSettings_Click(btnConfirmSettings, EventArgs.Empty);
                break;
        }
    }

    // 动态添加快捷键徽标 (Badge)
    private void AttachKeyBadge(Control targetControl, string keyText)
    {
        var lblBadge = new Label
        {
            Text = keyText,
            Font = new Font("Consolas", 8F, FontStyle.Bold),
            ForeColor = Color.Gold,
            BackColor = Color.FromArgb(180, 0, 0, 0),
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleCenter,
            Cursor = Cursors.Hand,
            Visible = _showShortcuts,
        };

        // 计算位置：右上角
        lblBadge.Location = new Point(targetControl.Width - 15, 2);
        lblBadge.Anchor = AnchorStyles.Top | AnchorStyles.Right;

        // 点击徽标也能触发按钮点击
        lblBadge.Click += (s, e) =>
        {
            if (targetControl is Button btn)
            {
                btn.PerformClick();
            }
            else if (targetControl is ThemedButton themedBtn)
            {
                // 对于ThemedButton，查找对应的Click事件处理
                if (themedBtn == btnSetGeneral)
                    SwitchTab(0, btnSetGeneral);
                else if (themedBtn == btnSetHotkeys)
                    SwitchTab(1, btnSetHotkeys);
                else if (themedBtn == btnSetTimer)
                    SwitchTab(2, btnSetTimer);
                else if (themedBtn == btnSetAudio)
                    SwitchTab(3, btnSetAudio);
                else if (themedBtn == btnAbout)
                    SwitchTab(4, btnAbout);
            }
        };

        targetControl.Controls.Add(lblBadge);
        lblBadge.BringToFront();

        _shortcutBadges.Add(lblBadge);
    }

    private void ToggleShortcutsVisibility()
    {
        _showShortcuts = !_showShortcuts;
        foreach (var badge in _shortcutBadges)
        {
            badge.Visible = _showShortcuts;
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _appSettings.Save();
        // _messenger.Publish(new Models.TimerSettingsChangedMessage());
        base.OnFormClosing(e);
    }

    private void BtnConfirmSettings_Click(object? sender, EventArgs e)
    {
        _appSettings.WindowPosition = AppSettings.WindowPositionToString(generalSettings.SelectedPosition);
        _appSettings.Language = AppSettings.LanguageToString(generalSettings.SelectedLanguage);
        _appSettings.AlwaysOnTop = generalSettings.IsAlwaysOnTop;

        _appSettings.HotkeyLeader = hotkeySettings.LeaderHotkey;
        _appSettings.HotkeyStartOrNext = hotkeySettings.StartOrNextRunHotkey;
        _appSettings.HotkeyPause = hotkeySettings.PauseHotkey;
        _appSettings.HotkeyDeleteHistory = hotkeySettings.DeleteHistoryHotkey;
        _appSettings.HotkeyRecordLoot = hotkeySettings.RecordLootHotkey;

        _appSettings.TimerShowPomodoro = timerSettings.TimerShowPomodoro;
        _appSettings.TimerShowLootDrops = timerSettings.TimerShowLootDrops;
        _appSettings.TimerSyncStartPomodoro = timerSettings.TimerSyncStartPomodoro;
        _appSettings.TimerSyncPausePomodoro = timerSettings.TimerSyncPausePomodoro;
        _appSettings.GenerateRoomName = timerSettings.GenerateRoomName;
        LogManager.WriteDebugLog(
            "SettingsControl",
            $"保存设置 timerSettings.ScreenshotOnLoot={timerSettings.ScreenshotOnLoot}"
        );
        _appSettings.ScreenshotOnLoot = timerSettings.ScreenshotOnLoot;
        _appSettings.HideWindowOnScreenshot = timerSettings.HideWindowOnScreenshot;
        _appSettings.Opacity = generalSettings.SelectedOpacity;
        // 保存缩放设置
        float oldScale = _appSettings.UiScale;
        float newScale = generalSettings.SelectedUiScale;

        _appSettings.UiScale = newScale;
        _appSettings.Save();

        string langCode = (generalSettings.SelectedLanguage == LanguageOption.Chinese) ? "zh-CN" : "en-US";
        _messenger.Publish(new OpacityChangedMessage());
        _messenger.Publish(new LanguageChangedMessage(langCode));
        _messenger.Publish(
            new TimerSettingsChangedMessage(
                timerSettings.TimerShowPomodoro,
                timerSettings.TimerShowLootDrops,
                timerSettings.TimerSyncStartPomodoro,
                timerSettings.TimerSyncPausePomodoro,
                timerSettings.GenerateRoomName
            )
        );
        _messenger.Publish(new WindowPositionChangedMessage());
        _messenger.Publish(new AlwaysOnTopChangedMessage());
        _messenger.Publish(new HotkeysChangedMessage());
        // 4. 【核心修改】检查缩放变化并提示重启
        if (Math.Abs(oldScale - newScale) > 0.01f)
        {
            var result = DiabloTwoMFTimer.UI.Components.ThemedMessageBox.Show(
                "界面缩放设置已保存。需要重启程序才能完全生效。\n\n是否立即重启？",
                "需要重启",
                MessageBoxButtons.YesNo
            ); // 使用 YesNo 按钮

            if (result == DialogResult.Yes)
            {
                Application.Restart();
                Application.Exit();
            }
        }
        else
        {
            Utils.Toast.Success(Utils.LanguageManager.GetString("SuccessSettingsChanged", "设置修改成功"));
        }
    }
}