using System;
using System.Drawing;
using System.Windows.Forms;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Settings
{
    public partial class TimerSettingsControl : UserControl
    {
        // 属性定义
        public bool TimerShowPomodoro { get; private set; }
        public bool TimerShowLootDrops { get; private set; }
        public bool TimerSyncStartPomodoro { get; private set; }
        public bool TimerSyncPausePomodoro { get; private set; }
        public bool GenerateRoomName { get; private set; }

        public TimerSettingsControl()
        {
            InitializeComponent();
        }

        // 事件处理
        private void OnShowPomodoroChanged(object? sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                TimerShowPomodoro = checkBox.Checked;
            }
        }

        private void OnShowLootDropsChanged(object? sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                TimerShowLootDrops = checkBox.Checked;
            }
        }

        private void OnSyncStartPomodoroChanged(object? sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                TimerSyncStartPomodoro = checkBox.Checked;
            }
        }

        private void OnSyncPausePomodoroChanged(object? sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                TimerSyncPausePomodoro = checkBox.Checked;
            }
        }

        private void OnGenerateRoomNameChanged(object? sender, EventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                GenerateRoomName = checkBox.Checked;
            }
        }

        // 加载设置
        public void LoadSettings(Services.IAppSettings settings)
        {
            TimerShowPomodoro = settings.TimerShowPomodoro;
            TimerShowLootDrops = settings.TimerShowLootDrops;
            TimerSyncStartPomodoro = settings.TimerSyncStartPomodoro;
            TimerSyncPausePomodoro = settings.TimerSyncPausePomodoro;
            GenerateRoomName = settings.GenerateRoomName;

            chkShowPomodoro.Checked = TimerShowPomodoro;
            chkShowLootDrops.Checked = TimerShowLootDrops;
            chkSyncStartPomodoro.Checked = TimerSyncStartPomodoro;
            chkSyncPausePomodoro.Checked = TimerSyncPausePomodoro;
            chkGenerateRoomName.Checked = GenerateRoomName;
        }

        // 刷新UI（支持国际化）
        public void RefreshUI()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(RefreshUI));
                return;
            }
            if (grpTimerSettings == null)
                return;
            try
            {
                grpTimerSettings.Text = LanguageManager.GetString("TimerSettingsGroup");
                chkShowPomodoro.Text = LanguageManager.GetString("TimerShowPomodoro");
                chkShowLootDrops.Text = LanguageManager.GetString("TimerShowLootDrops");
                chkSyncStartPomodoro.Text = LanguageManager.GetString("TimerSyncStartPomodoro");
                chkSyncPausePomodoro.Text = LanguageManager.GetString("TimerSyncPausePomodoro");
                chkGenerateRoomName.Text = LanguageManager.GetString("TimerGenerateRoomName");
            }
            catch { }
        }
    }
}
