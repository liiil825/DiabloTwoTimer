using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace DiabloTwoMFTimer.UI.Form;

public partial class SettingsForm : BaseForm
{
    private readonly IAppSettings _appSettings;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessenger _messenger;

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

        // 去掉构造函数里的 Size 设置，移到 OnLoad
        SetupTabs();
        InitializeData(_appSettings);
        RefreshUI();

        // 动态调整按钮大小
        AdjustButtonSize();
    }

    private void SetupTabs()
    {
        // 添加音频设置Tab页面的内容
        var audioSettings = _serviceProvider.GetRequiredService<UI.Settings.AudioSettingsControl>();
        tabPageAudio.Text = Utils.LanguageManager.GetString("Settings.Tab.Audio");
        audioSettings.Dock = System.Windows.Forms.DockStyle.Fill;
        tabPageAudio.Controls.Add(audioSettings);

        // 扩展导航按钮数组
        btnSetGeneral.Click += (s, e) => SwitchTab(0, btnSetGeneral);
        btnSetHotkeys.Click += (s, e) => SwitchTab(1, btnSetHotkeys);
        btnSetTimer.Click += (s, e) => SwitchTab(2, btnSetTimer);
        btnSetAudio.Click += (s, e) => SwitchTab(3, btnSetAudio);

        // 默认选中
        SwitchTab(0, btnSetGeneral);
    }

    private void SwitchTab(int index, ThemedButton activeBtn)
    {
        tabControl.SelectedIndex = index;

        // 1. 样式逻辑 (使用 IsSelected)
        var buttons = new[] { btnSetGeneral, btnSetHotkeys, btnSetTimer, btnSetAudio };
        foreach (var btn in buttons)
        {
            btn.IsSelected = (btn == activeBtn);
        }
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

            generalSettings.RefreshUI();
            hotkeySettings.RefreshUI();
            timerSettings.RefreshUI();

            // 刷新音频设置UI
            var audioSettings = tabControl.TabPages[3].Controls[0] as UI.Settings.AudioSettingsControl;
            if (audioSettings != null)
            {
                audioSettings.RefreshUI();
            }

            // 动态调整按钮大小
            AdjustButtonSize();
        });
    }

    private void AdjustButtonSize()
    {
        // 基础尺寸
        int baseWidth = 80;
        int baseHeight = 30;

        // 根据界面大小设置调整尺寸
        int newWidth, newHeight;
        float uiScale = _appSettings.UiScale;

        // 如果用户没有手动设置，使用自动计算的缩放比例
        float scaleFactor = uiScale > 0.1f ? uiScale : Utils.ScaleHelper.ScaleFactor;

        if (scaleFactor < 1.3f) // 小尺寸
        {
            newWidth = Utils.ScaleHelper.Scale(baseWidth - 10);
            newHeight = Utils.ScaleHelper.Scale(baseHeight - 5);
        }
        else if (scaleFactor < 2.0f) // 中尺寸
        {
            newWidth = Utils.ScaleHelper.Scale(baseWidth);
            newHeight = Utils.ScaleHelper.Scale(baseHeight);
        }
        else // 大尺寸
        {
            newWidth = Utils.ScaleHelper.Scale(baseWidth + 20);
            newHeight = Utils.ScaleHelper.Scale(baseHeight + 10);
        }

        // 应用新尺寸
        if (btnConfirmSettings != null)
        {
            btnConfirmSettings.Size = new System.Drawing.Size(newWidth, newHeight);
        }
    }

    public void InitializeData(IAppSettings settings)
    {
        generalSettings.LoadSettings(settings);
        hotkeySettings.LoadHotkeys(settings);
        timerSettings.LoadSettings(settings);

        // 音频设置已在构造函数中初始化数据
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // _appSettings.Save();
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