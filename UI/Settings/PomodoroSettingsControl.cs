using System;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Settings;

public partial class PomodoroSettingsControl : UserControl
{
    private readonly IAppSettings _appSettings;

    // 辅助类用于 ComboBox
    private class ModeItem
    {
        public PomodoroMode Value { get; set; }
        public string Text { get; set; } = "";
        public override string ToString() => Text;
    }

    public PomodoroSettingsControl(IAppSettings appSettings)
    {
        _appSettings = appSettings;
        InitializeComponent();
        RefreshUI();
    }

    public void RefreshUI()
    {
        // 1. 设置数值
        nudWorkTimeMin.Value = Math.Max(nudWorkTimeMin.Minimum, Math.Min(nudWorkTimeMin.Maximum, _appSettings.WorkTimeMinutes));
        nudWorkTimeSec.Value = Math.Max(nudWorkTimeSec.Minimum, Math.Min(nudWorkTimeSec.Maximum, _appSettings.WorkTimeSeconds));

        nudShortBreakTimeMin.Value = Math.Max(
            nudShortBreakTimeMin.Minimum,
            Math.Min(nudShortBreakTimeMin.Maximum, _appSettings.ShortBreakMinutes)
        );
        nudShortBreakTimeSec.Value = Math.Max(
            nudShortBreakTimeSec.Minimum,
            Math.Min(nudShortBreakTimeSec.Maximum, _appSettings.ShortBreakSeconds)
        );

        nudLongBreakTimeMin.Value = Math.Max(
            nudLongBreakTimeMin.Minimum,
            Math.Min(nudLongBreakTimeMin.Maximum, _appSettings.LongBreakMinutes)
        );
        nudLongBreakTimeSec.Value = Math.Max(
            nudLongBreakTimeSec.Minimum,
            Math.Min(nudLongBreakTimeSec.Maximum, _appSettings.LongBreakSeconds)
        );

        nudWarningLongTime.Value = Math.Max(
            nudWarningLongTime.Minimum,
            Math.Min(nudWarningLongTime.Maximum, _appSettings.PomodoroWarningLongTime)
        );
        nudWarningShortTime.Value = Math.Max(
            nudWarningShortTime.Minimum,
            Math.Min(nudWarningShortTime.Maximum, _appSettings.PomodoroWarningShortTime)
        );

        // 2. 本地化文本
        lblMode.Text = LanguageManager.GetString("PomodoroMode", "模式:");
        cmbMode.Items.Clear();
        cmbMode.Items.Add(new ModeItem { Value = PomodoroMode.Automatic, Text = LanguageManager.GetString("PomodoroMode_Auto", "自动 (严格模式)") });
        cmbMode.Items.Add(new ModeItem { Value = PomodoroMode.SemiAuto, Text = LanguageManager.GetString("PomodoroMode_Semi", "半自动") });
        cmbMode.Items.Add(new ModeItem { Value = PomodoroMode.Manual, Text = LanguageManager.GetString("PomodoroMode_Manual", "手动") });

        // 选中当前值
        foreach (ModeItem item in cmbMode.Items)
        {
            if (item.Value == _appSettings.PomodoroMode)
            {
                cmbMode.SelectedItem = item;
                break;
            }
        }

        // 行标题
        lblWorkTime.Text = LanguageManager.GetString("WorkTime") ?? "工作时间:";
        lblShortBreakTime.Text = LanguageManager.GetString("ShortBreakTime") ?? "短休息时间:";
        lblLongBreakTime.Text = LanguageManager.GetString("LongBreakTime") ?? "长休息时间:";
        lblWarningLongTime.Text = LanguageManager.GetString("PomodoroWarningLongTime") ?? "时间提示1:";
        lblWarningShortTime.Text = LanguageManager.GetString("PomodoroWarningShortTime") ?? "时间提示2:";

        // 单位 (分/秒)
        string strMin = LanguageManager.GetString("Minutes") ?? "分";
        string strSec = LanguageManager.GetString("Seconds") ?? "秒";
        lblWorkMinUnit.Text = strMin;
        lblShortBreakMinUnit.Text = strMin;
        lblLongBreakMinUnit.Text = strMin;
        lblWorkSecUnit.Text = strSec;
        lblShortBreakSecUnit.Text = strSec;
        lblLongBreakSecUnit.Text = strSec;
        lblWarningLongTimeUnit.Text = strSec;
        lblWarningShortTimeUnit.Text = strSec;
    }

    public void SaveSettings()
    {
        _appSettings.WorkTimeMinutes = (int)nudWorkTimeMin.Value;
        _appSettings.WorkTimeSeconds = (int)nudWorkTimeSec.Value;
        _appSettings.ShortBreakMinutes = (int)nudShortBreakTimeMin.Value;
        _appSettings.ShortBreakSeconds = (int)nudShortBreakTimeSec.Value;
        _appSettings.LongBreakMinutes = (int)nudLongBreakTimeMin.Value;
        _appSettings.LongBreakSeconds = (int)nudLongBreakTimeSec.Value;
        _appSettings.PomodoroWarningLongTime = (int)nudWarningLongTime.Value;
        _appSettings.PomodoroWarningShortTime = (int)nudWarningShortTime.Value;

        // 更新模式
        if (cmbMode.SelectedItem is ModeItem item)
        {
            _appSettings.PomodoroMode = item.Value;
        }
    }
}
