using System;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.UI.Form;
using DiabloTwoMFTimer.Utils;

#nullable disable

namespace DiabloTwoMFTimer.UI.Pomodoro;

public partial class PomodoroSettingsForm : BaseForm
{
    private readonly IAppSettings _appSettings = null!;

    // 属性
    public int WorkTimeMinutes { get; private set; }
    public int WorkTimeSeconds { get; private set; }
    public int ShortBreakMinutes { get; private set; }
    public int ShortBreakSeconds { get; private set; }
    public int LongBreakMinutes { get; private set; }
    public int LongBreakSeconds { get; private set; }
    public int WarningLongTime { get; private set; }
    public int WarningShortTime { get; private set; }

    // 辅助类用于 ComboBox
    private class ModeItem
    {
        public PomodoroMode Value { get; set; }
        public string Text { get; set; } = "";
        public override string ToString() => Text;
    }

    public PomodoroSettingsForm()
    {
        InitializeComponent();
    }

    public PomodoroSettingsForm(
        IAppSettings appSettings,
        int workTimeMinutes,
        int workTimeSeconds,
        int shortBreakMinutes,
        int shortBreakSeconds,
        int longBreakMinutes,
        int longBreakSeconds,
        int warningLongTime,
        int warningShortTime
    )
        : this()
    {
        _appSettings = appSettings;

        WorkTimeMinutes = workTimeMinutes;
        WorkTimeSeconds = workTimeSeconds;
        ShortBreakMinutes = shortBreakMinutes;
        ShortBreakSeconds = shortBreakSeconds;
        LongBreakMinutes = longBreakMinutes;
        LongBreakSeconds = longBreakSeconds;
        WarningLongTime = warningLongTime;
        WarningShortTime = warningShortTime;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!this.DesignMode)
        {
            if (WorkTimeMinutes == 0 && ShortBreakMinutes == 0 && LongBreakMinutes == 0)
            {
                LoadSettings();
            }
            UpdateUI();
        }
    }

    private void LoadSettings()
    {
        WorkTimeMinutes = _appSettings.WorkTimeMinutes;
        WorkTimeSeconds = _appSettings.WorkTimeSeconds;
        ShortBreakMinutes = _appSettings.ShortBreakMinutes;
        ShortBreakSeconds = _appSettings.ShortBreakSeconds;
        LongBreakMinutes = _appSettings.LongBreakMinutes;
        LongBreakSeconds = _appSettings.LongBreakSeconds;
        WarningLongTime = _appSettings.PomodoroWarningLongTime;
        WarningShortTime = _appSettings.PomodoroWarningShortTime;
    }

    protected override void UpdateUI()
    {
        base.UpdateUI();

        // 1. 设置数值
        nudWorkTimeMin.Value = Math.Max(nudWorkTimeMin.Minimum, Math.Min(nudWorkTimeMin.Maximum, WorkTimeMinutes));
        nudWorkTimeSec.Value = Math.Max(nudWorkTimeSec.Minimum, Math.Min(nudWorkTimeSec.Maximum, WorkTimeSeconds));

        nudShortBreakTimeMin.Value = Math.Max(
            nudShortBreakTimeMin.Minimum,
            Math.Min(nudShortBreakTimeMin.Maximum, ShortBreakMinutes)
        );
        nudShortBreakTimeSec.Value = Math.Max(
            nudShortBreakTimeSec.Minimum,
            Math.Min(nudShortBreakTimeSec.Maximum, ShortBreakSeconds)
        );

        nudLongBreakTimeMin.Value = Math.Max(
            nudLongBreakTimeMin.Minimum,
            Math.Min(nudLongBreakTimeMin.Maximum, LongBreakMinutes)
        );
        nudLongBreakTimeSec.Value = Math.Max(
            nudLongBreakTimeSec.Minimum,
            Math.Min(nudLongBreakTimeSec.Maximum, LongBreakSeconds)
        );

        nudWarningLongTime.Value = Math.Max(
            nudWarningLongTime.Minimum,
            Math.Min(nudWarningLongTime.Maximum, WarningLongTime)
        );
        nudWarningShortTime.Value = Math.Max(
            nudWarningShortTime.Minimum,
            Math.Min(nudWarningShortTime.Maximum, WarningShortTime)
        );

        // 2. 本地化文本
        this.Text = LanguageManager.GetString("PomodoroSettings") ?? "番茄时钟设置";

        // 模式选择
        lblMode.Text = LanguageManager.GetString("PomodoroMode", "Mode:");
        cmbMode.Items.Clear();
        cmbMode.Items.Add(new ModeItem { Value = PomodoroMode.Automatic, Text = LanguageManager.GetString("PomodoroMode_Auto", "Auto") });
        cmbMode.Items.Add(new ModeItem { Value = PomodoroMode.SemiAuto, Text = LanguageManager.GetString("PomodoroMode_Semi", "Semi-Auto") });
        cmbMode.Items.Add(new ModeItem { Value = PomodoroMode.Manual, Text = LanguageManager.GetString("PomodoroMode_Manual", "Manual") });

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
        lblWarningLongTime.Text = LanguageManager.GetString("PomodoroWarningLongTime") ?? "提前长时间提示:";
        lblWarningShortTime.Text = LanguageManager.GetString("PomodoroWarningShortTime") ?? "提前短时间提示:";

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

    protected override void BtnConfirm_Click(object sender, EventArgs e)
    {
        WorkTimeMinutes = (int)nudWorkTimeMin.Value;
        WorkTimeSeconds = (int)nudWorkTimeSec.Value;
        ShortBreakMinutes = (int)nudShortBreakTimeMin.Value;
        ShortBreakSeconds = (int)nudShortBreakTimeSec.Value;
        LongBreakMinutes = (int)nudLongBreakTimeMin.Value;
        LongBreakSeconds = (int)nudLongBreakTimeSec.Value;
        WarningLongTime = (int)nudWarningLongTime.Value;
        WarningShortTime = (int)nudWarningShortTime.Value;

        // 如果有注入的_appSettings，直接更新设置
        _appSettings.WorkTimeMinutes = WorkTimeMinutes;
        _appSettings.WorkTimeSeconds = WorkTimeSeconds;
        _appSettings.ShortBreakMinutes = ShortBreakMinutes;
        _appSettings.ShortBreakSeconds = ShortBreakSeconds;
        _appSettings.LongBreakMinutes = LongBreakMinutes;
        _appSettings.LongBreakSeconds = LongBreakSeconds;
        _appSettings.PomodoroWarningLongTime = WarningLongTime;
        _appSettings.PomodoroWarningShortTime = WarningShortTime;

        // 更新模式
        if (cmbMode.SelectedItem is ModeItem item)
        {
            _appSettings.PomodoroMode = item.Value;
        }

        base.BtnConfirm_Click(sender, e);
    }
}