using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.UI.Theme;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Settings;

public partial class SettingsControl : UserControl
{
    public enum WindowPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
    }

    public enum LanguageOption
    {
        Chinese,
        English,
    }

    private readonly IAppSettings _appSettings = null!;
    private readonly IMessenger _messenger = null!;

    public SettingsControl()
    {
        InitializeComponent();
        btnSetGeneral.Click += (s, e) => SwitchTab(0, btnSetGeneral);
        btnSetHotkeys.Click += (s, e) => SwitchTab(1, btnSetHotkeys);
        btnSetTimer.Click += (s, e) => SwitchTab(2, btnSetTimer);

        // 默认选中
        SwitchTab(0, btnSetGeneral);
        RefreshUI();
    }

    public SettingsControl(IAppSettings appSettings, IMessenger messenger)
        : this()
    {
        _appSettings = appSettings;
        _messenger = messenger;

        hotkeySettings.SetMessenger(_messenger);

        InitializeData(_appSettings);
        SubscribeMessages();
    }
    private void SwitchTab(int index, ThemedButton activeBtn)
    {
        tabControl.SelectedIndex = index;

        // 1. 样式逻辑 (使用 IsSelected)
        var buttons = new[] { btnSetGeneral, btnSetHotkeys, btnSetTimer };
        foreach (var btn in buttons)
        {
            btn.IsSelected = (btn == activeBtn);
        }
    }

    private void SubscribeMessages()
    {
        _messenger.Subscribe<TimerShowLootDropsChangedMessage>(OnTimerShowLootDropsChanged);
    }

    private void OnTimerShowLootDropsChanged(TimerShowLootDropsChangedMessage _)
    {
        this.SafeInvoke(() =>
        {
            timerSettings.LoadSettings(_appSettings);
        });
    }

    public void RefreshUI()
    {
        this.SafeInvoke(() =>
        {
            btnConfirmSettings.Text = LanguageManager.GetString("ConfirmSettings");
            btnSetGeneral.Text = LanguageManager.GetString("General");
            btnSetHotkeys.Text = LanguageManager.GetString("Hotkeys");
            btnSetTimer.Text = LanguageManager.GetString("TimerSettings");

            generalSettings.RefreshUI();
            hotkeySettings.RefreshUI();
            timerSettings.RefreshUI();
        });
    }

    public void InitializeData(IAppSettings settings)
    {
        generalSettings.LoadSettings(settings);
        hotkeySettings.LoadHotkeys(settings);
        timerSettings.LoadSettings(settings);
    }

    private void BtnConfirmSettings_Click(object? sender, EventArgs e)
    {
        _appSettings.WindowPosition = AppSettings.WindowPositionToString(generalSettings.SelectedPosition);
        _appSettings.Language = AppSettings.LanguageToString(generalSettings.SelectedLanguage);
        _appSettings.AlwaysOnTop = generalSettings.IsAlwaysOnTop;

        _appSettings.HotkeyStartOrNext = hotkeySettings.StartOrNextRunHotkey;
        _appSettings.HotkeyPause = hotkeySettings.PauseHotkey;
        _appSettings.HotkeyDeleteHistory = hotkeySettings.DeleteHistoryHotkey;
        _appSettings.HotkeyRecordLoot = hotkeySettings.RecordLootHotkey;

        _appSettings.TimerShowPomodoro = timerSettings.TimerShowPomodoro;
        _appSettings.TimerShowLootDrops = timerSettings.TimerShowLootDrops;
        _appSettings.TimerSyncStartPomodoro = timerSettings.TimerSyncStartPomodoro;
        _appSettings.TimerSyncPausePomodoro = timerSettings.TimerSyncPausePomodoro;
        _appSettings.GenerateRoomName = timerSettings.GenerateRoomName;

        // 保存缩放设置
        float oldScale = _appSettings.UiScale;
        float newScale = generalSettings.SelectedUiScale;

        _appSettings.UiScale = newScale;
        _appSettings.Save();

        string langCode = (generalSettings.SelectedLanguage == LanguageOption.Chinese) ? "zh-CN" : "en-US";
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
                MessageBoxButtons.YesNo); // 使用 YesNo 按钮

            if (result == DialogResult.Yes)
            {
                Application.Restart();
                Environment.Exit(0); // 确保当前进程立即终止
            }
        }
        else
        {
            Utils.Toast.Success(Utils.LanguageManager.GetString("SuccessSettingsChanged", "设置修改成功"));
        }
    }

    public void ApplyWindowPosition(System.Windows.Forms.Form form)
    {
        MoveWindowToPosition(form, generalSettings.SelectedPosition);
    }

    public static void MoveWindowToPosition(System.Windows.Forms.Form form, WindowPosition position)
    {
        Rectangle screenBounds = Screen.GetWorkingArea(form);
        int x,
            y;
        switch (position)
        {
            case WindowPosition.TopLeft:
                x = screenBounds.Left;
                y = screenBounds.Top;
                break;
            case WindowPosition.TopCenter:
                x = screenBounds.Left + (screenBounds.Width - form.Width) / 2;
                y = screenBounds.Top;
                break;
            case WindowPosition.TopRight:
                x = screenBounds.Right - form.Width;
                y = screenBounds.Top;
                break;
            case WindowPosition.BottomLeft:
                x = screenBounds.Left;
                y = screenBounds.Bottom - form.Height;
                break;
            case WindowPosition.BottomCenter:
                x = screenBounds.Left + (screenBounds.Width - form.Width) / 2;
                y = screenBounds.Bottom - form.Height;
                break;
            case WindowPosition.BottomRight:
                x = screenBounds.Right - form.Width;
                y = screenBounds.Bottom - form.Height;
                break;
            default:
                return;
        }
        form.Location = new Point(x, y);
    }
}
