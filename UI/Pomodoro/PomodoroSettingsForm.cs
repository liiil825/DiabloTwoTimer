using System;
using System.ComponentModel;
using System.Windows.Forms;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.UI.Common;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Pomodoro
{
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
                // 如果是默认初始化（全0），尝试加载配置
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

            // 2. 本地化文本 - 所有 Label 都需要更新
            this.Text = LanguageManager.GetString("PomodoroSettings") ?? "番茄时钟设置";

            // 行标题
            lblWorkTime.Text = LanguageManager.GetString("WorkTime") ?? "工作时间:";
            lblShortBreakTime.Text = LanguageManager.GetString("ShortBreakTime") ?? "短休息时间:";
            lblLongBreakTime.Text = LanguageManager.GetString("LongBreakTime") ?? "长休息时间:";
            lblWarningLongTime.Text = LanguageManager.GetString("PomodoroWarningLongTime") ?? "提前长时间提示:";
            lblWarningShortTime.Text = LanguageManager.GetString("PomodoroWarningShortTime") ?? "提前短时间提示:";

            // 单位 (分)
            string strMin = LanguageManager.GetString("Minutes") ?? "分";
            lblWorkMinUnit.Text = strMin;
            lblShortBreakMinUnit.Text = strMin;
            lblLongBreakMinUnit.Text = strMin;

            // 单位 (秒)
            string strSec = LanguageManager.GetString("Seconds") ?? "秒";
            lblWorkSecUnit.Text = strSec;
            lblShortBreakSecUnit.Text = strSec;
            lblLongBreakSecUnit.Text = strSec;
            lblWarningLongTimeUnit.Text = strSec;
            lblWarningShortTimeUnit.Text = strSec;

            // 按钮
            if (btnConfirm != null)
                btnConfirm.Text = LanguageManager.GetString("Save") ?? "保存";
            if (btnCancel != null)
                btnCancel.Text = LanguageManager.GetString("Cancel") ?? "取消";
        }

        protected override void BtnConfirm_Click(object? sender, EventArgs e)
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

            base.BtnConfirm_Click(sender, e);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}