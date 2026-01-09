using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Settings;

public partial class TimerSettingsControl : UserControl
{
    public TimerSettingsControl()
    {
        InitializeComponent();
    }

    // 加载设置
    public void LoadSettings(IAppSettings settings)
    {
        chkShowPomodoro.Checked = settings.TimerShowPomodoro;
        chkSyncStartPomodoro.Checked = settings.TimerSyncStartPomodoro;
        chkSyncPausePomodoro.Checked = settings.TimerSyncPausePomodoro;
        chkGenerateRoomName.Checked = settings.GenerateRoomName;

        // 加载截图相关设置
        chkScreenshotOnLoot.Checked = settings.ScreenshotOnLoot;
        chkHideWindowOnScreenshot.Checked = settings.HideWindowOnScreenshot;

        chkShowTime.Checked = settings.TimerShowTimerTime;
        chkShowStats.Checked = settings.TimerShowStatistics;
        chkShowHistory.Checked = settings.TimerShowHistory;
        chkShowLoot.Checked = settings.TimerShowLootDrops;
        chkShowAccount.Checked = settings.TimerShowAccountInfo;
        numAvgCount.Value = settings.TimerAverageRunCount;
    }

    // 刷新UI（支持国际化）
    public void RefreshUI()
    {
        this.SafeInvoke(() =>
        {
            grpTimerSettings!.Text = LanguageManager.GetString("TimerSettingsGroup");
            chkShowPomodoro!.Text = LanguageManager.GetString("TimerShowPomodoro");
            chkSyncStartPomodoro!.Text = LanguageManager.GetString("TimerSyncStartPomodoro");
            chkSyncPausePomodoro!.Text = LanguageManager.GetString("TimerSyncPausePomodoro");
            chkGenerateRoomName!.Text = LanguageManager.GetString("TimerGenerateRoomName");
            chkScreenshotOnLoot!.Text = LanguageManager.GetString("TimerScreenshotOnLoot");
            chkHideWindowOnScreenshot!.Text = LanguageManager.GetString("TimerHideWindowOnScreenshot");
            grpDisplay!.Text = LanguageManager.GetString("Settings_DisplayGroup") ?? "Display Settings";
            chkShowTime!.Text = LanguageManager.GetString("Settings_ShowTime") ?? "Show Timer";
            chkShowStats!.Text = LanguageManager.GetString("Settings_ShowStats") ?? "Show Statistics";
            chkShowHistory!.Text = LanguageManager.GetString("Settings_ShowHistory") ?? "Show History";
            chkShowLoot!.Text = LanguageManager.GetString("Settings_ShowLoot") ?? "Show Loot Records";
            chkShowAccount!.Text = LanguageManager.GetString("Settings_ShowAccount") ?? "Show Account Info";
            lblAvgCount!.Text = LanguageManager.GetString("Settings_AvgCount") ?? "Average Run Sample (0=All):";
        });
    }

    public bool SaveSettings(IAppSettings settings)
    {
        // settings.TimerShowLootDrops = TimerShowLootDrops;
        settings.TimerSyncStartPomodoro = chkSyncStartPomodoro.Checked;
        settings.TimerSyncPausePomodoro = chkSyncPausePomodoro.Checked;
        settings.GenerateRoomName = chkGenerateRoomName.Checked;
        settings.ScreenshotOnLoot = chkScreenshotOnLoot.Checked;
        settings.HideWindowOnScreenshot = chkHideWindowOnScreenshot.Checked;
        settings.TimerShowPomodoro = chkShowPomodoro.Checked;
        settings.TimerShowTimerTime = chkShowTime.Checked;
        settings.TimerShowStatistics = chkShowStats.Checked;
        settings.TimerShowHistory = chkShowHistory.Checked;
        settings.TimerShowLootDrops = chkShowLoot.Checked;
        settings.TimerShowAccountInfo = chkShowAccount.Checked;
        settings.TimerAverageRunCount = (int)numAvgCount.Value;
        return true;
    }
}
