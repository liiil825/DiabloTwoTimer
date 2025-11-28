using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Services; // 假设存在
using DiabloTwoMFTimer.Utils; // 假设存在

namespace DiabloTwoMFTimer.UI.Settings
{
    public partial class SettingsControl : UserControl
    {
        // 枚举定义
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

        // 事件定义
        public event EventHandler<WindowPositionChangedEventArgs>? WindowPositionChanged;
        public event EventHandler<LanguageChangedEventArgs>? LanguageChanged;
        public event EventHandler<AlwaysOnTopChangedEventArgs>? AlwaysOnTopChanged;
        public event EventHandler<AllHotkeysChangedEventArgs>? HotkeysChanged;
        public event EventHandler<TimerSettingsChangedEventArgs>? TimerSettingsChanged;

        // IAppSettings 字段
        private readonly IAppSettings _appSettings = null!;

        public SettingsControl()
        {
            InitializeComponent();
            RefreshUI();
        }

        public SettingsControl(IAppSettings appSettings)
            : this()
        {
            _appSettings = appSettings;
        }

        public void RefreshUI()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(RefreshUI));
                return;
            }

            // 刷新自身（按钮等）
            btnConfirmSettings.Text = LanguageManager.GetString("ConfirmSettings");
            tabPageGeneral.Text = LanguageManager.GetString("General");
            tabPageHotkeys.Text = LanguageManager.GetString("Hotkeys");
            tabPageTimer.Text = LanguageManager.GetString("TimerSettings");

            // 刷新子组件
            generalSettings.RefreshUI();
            hotkeySettings.RefreshUI();
            timerSettings.RefreshUI();
        }

        public void InitializeData(Services.IAppSettings settings)
        {
            generalSettings.LoadSettings(settings);
            hotkeySettings.LoadHotkeys(settings);
            timerSettings.LoadSettings(settings);
        }

        private void BtnConfirmSettings_Click(object? sender, EventArgs e)
        {
            // 直接修改IAppSettings并保存
            // 更新窗口位置
            _appSettings.WindowPosition = SettingsManager.WindowPositionToString(generalSettings.SelectedPosition);

            // 更新语言
            _appSettings.Language = SettingsManager.LanguageToString(generalSettings.SelectedLanguage);

            // 更新始终置顶设置
            _appSettings.AlwaysOnTop = generalSettings.IsAlwaysOnTop;

            // 更新快捷键设置
            _appSettings.HotkeyStartOrNext = hotkeySettings.StartOrNextRunHotkey;
            _appSettings.HotkeyPause = hotkeySettings.PauseHotkey;
            _appSettings.HotkeyDeleteHistory = hotkeySettings.DeleteHistoryHotkey;
            _appSettings.HotkeyRecordLoot = hotkeySettings.RecordLootHotkey;

            // 更新计时器设置
            _appSettings.TimerShowPomodoro = timerSettings.TimerShowPomodoro;
            _appSettings.TimerShowLootDrops = timerSettings.TimerShowLootDrops;
            _appSettings.TimerSyncStartPomodoro = timerSettings.TimerSyncStartPomodoro;
            _appSettings.TimerSyncPausePomodoro = timerSettings.TimerSyncPausePomodoro;
            _appSettings.GenerateRoomName = timerSettings.GenerateRoomName;

            // 保存设置
            SettingsManager.SaveSettings(_appSettings);

            // 显示成功提示
            Utils.Toast.Success(Utils.LanguageManager.GetString("SuccessSettingsChanged", "设置修改成功"));

            // 触发所有设置事件，用于其他UI更新
            WindowPositionChanged?.Invoke(this, new WindowPositionChangedEventArgs(generalSettings.SelectedPosition));
            LanguageChanged?.Invoke(this, new LanguageChangedEventArgs(generalSettings.SelectedLanguage));
            AlwaysOnTopChanged?.Invoke(this, new AlwaysOnTopChangedEventArgs(generalSettings.IsAlwaysOnTop));
            HotkeysChanged?.Invoke(
                this,
                new AllHotkeysChangedEventArgs(
                    hotkeySettings.StartOrNextRunHotkey,
                    hotkeySettings.PauseHotkey,
                    hotkeySettings.DeleteHistoryHotkey,
                    hotkeySettings.RecordLootHotkey
                )
            );
            TimerSettingsChanged?.Invoke(
                this,
                new TimerSettingsChangedEventArgs(
                    timerSettings.TimerShowPomodoro,
                    timerSettings.TimerShowLootDrops,
                    timerSettings.TimerSyncStartPomodoro,
                    timerSettings.TimerSyncPausePomodoro,
                    timerSettings.GenerateRoomName
                )
            );
        }

        // ... (其他代码不变) ...

        // 新增：包含所有快捷键的事件参数类
        public class AllHotkeysChangedEventArgs(Keys start, Keys pause, Keys delete, Keys record) : EventArgs
        {
            public Keys StartHotkey { get; } = start;
            public Keys PauseHotkey { get; } = pause;
            public Keys DeleteHotkey { get; } = delete;
            public Keys RecordHotkey { get; } = record;
        }

        public void ApplyWindowPosition(Form form)
        {
            MoveWindowToPosition(form, generalSettings.SelectedPosition);
        }

        // 静态辅助方法保持不变
        public static void MoveWindowToPosition(Form form, WindowPosition position)
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

        // 事件参数类
        public class WindowPositionChangedEventArgs(WindowPosition position) : EventArgs
        {
            public WindowPosition Position { get; } = position;
        }

        public class LanguageChangedEventArgs(LanguageOption language) : EventArgs
        {
            public LanguageOption Language { get; } = language;
        }

        public class AlwaysOnTopChangedEventArgs(bool isAlwaysOnTop) : EventArgs
        {
            public bool IsAlwaysOnTop { get; } = isAlwaysOnTop;
        }

        public class HotkeyChangedEventArgs(Keys hotkey) : EventArgs
        {
            public Keys Hotkey { get; } = hotkey;
        }

        // 新增：计时器设置事件参数类
        public class TimerSettingsChangedEventArgs(
            bool showPomodoro,
            bool showLootDrops,
            bool syncStartPomodoro,
            bool syncPausePomodoro,
            bool generateRoomName
        ) : EventArgs
        {
            public bool ShowPomodoro { get; } = showPomodoro;
            public bool ShowLootDrops { get; } = showLootDrops;
            public bool SyncStartPomodoro { get; } = syncStartPomodoro;
            public bool SyncPausePomodoro { get; } = syncPausePomodoro;
            public bool GenerateRoomName { get; } = generateRoomName;
        }
    }
}
