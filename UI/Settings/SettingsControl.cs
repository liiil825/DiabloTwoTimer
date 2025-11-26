using System;
using System.Windows.Forms;
using System.Drawing;
using DTwoMFTimerHelper.Utils; // 假设存在
using DTwoMFTimerHelper.Services; // 假设存在

namespace DTwoMFTimerHelper.UI.Settings {
    public partial class SettingsControl : UserControl {
        // 枚举定义
        public enum WindowPosition { TopLeft, TopCenter, TopRight, BottomLeft, BottomCenter, BottomRight }
        public enum LanguageOption { Chinese, English }

        // 事件定义
        public event EventHandler<WindowPositionChangedEventArgs>? WindowPositionChanged;
        public event EventHandler<LanguageChangedEventArgs>? LanguageChanged;
        public event EventHandler<AlwaysOnTopChangedEventArgs>? AlwaysOnTopChanged;
        public event EventHandler<AllHotkeysChangedEventArgs>? HotkeysChanged;
        public event EventHandler<TimerSettingsChangedEventArgs>? TimerSettingsChanged;

        // 控件引用
        private TabControl tabControl = null!;
        private TabPage tabPageGeneral = null!;
        private TabPage tabPageHotkeys = null!;
        private Button btnConfirmSettings = null!;
        private Panel panelBottom = null!; // 用于放置按钮的底部面板

        // 子组件引用
        private GeneralSettingsControl generalSettings = null!;
        private HotkeySettingsControl hotkeySettings = null!;
        private TimerSettingsControl timerSettings = null!;
        private TabPage tabPageTimer = null!;

        // IAppSettings 字段
        private readonly IAppSettings _appSettings;

        public SettingsControl() {
            InitializeComponent();
            RefreshUI();
        }

        public SettingsControl(IAppSettings appSettings) : this() {
            _appSettings = appSettings;
        }

        private void InitializeComponent() {
            tabControl = new TabControl();
            tabPageGeneral = new TabPage();
            generalSettings = new GeneralSettingsControl();
            tabPageHotkeys = new TabPage();
            hotkeySettings = new HotkeySettingsControl();
            tabPageTimer = new TabPage();
            timerSettings = new TimerSettingsControl();
            btnConfirmSettings = new Button();
            panelBottom = new Panel();
            tabControl.SuspendLayout();
            tabPageGeneral.SuspendLayout();
            tabPageHotkeys.SuspendLayout();
            tabPageTimer.SuspendLayout();
            panelBottom.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabPageGeneral);
            tabControl.Controls.Add(tabPageHotkeys);
            tabControl.Controls.Add(tabPageTimer);
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(0, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(371, 391);
            tabControl.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            tabPageGeneral.Controls.Add(generalSettings);
            tabPageGeneral.Location = new Point(4, 37);
            tabPageGeneral.Name = "tabPageGeneral";
            tabPageGeneral.Padding = new Padding(3);
            tabPageGeneral.Size = new Size(363, 350);
            tabPageGeneral.TabIndex = 0;
            tabPageGeneral.Text = "通用";
            // 
            // generalSettings
            // 
            generalSettings.AutoScroll = true;
            generalSettings.Dock = DockStyle.Fill;
            generalSettings.Location = new Point(3, 3);
            generalSettings.Name = "generalSettings";
            generalSettings.Size = new Size(357, 344);
            generalSettings.TabIndex = 0;
            // 
            // tabPageHotkeys
            // 
            tabPageHotkeys.Controls.Add(hotkeySettings);
            tabPageHotkeys.Location = new Point(4, 37);
            tabPageHotkeys.Name = "tabPageHotkeys";
            tabPageHotkeys.Padding = new Padding(3);
            tabPageHotkeys.Size = new Size(363, 350);
            tabPageHotkeys.TabIndex = 1;
            tabPageHotkeys.Text = "快捷键";
            // 
            // tabPageTimer
            // 
            tabPageTimer.Controls.Add(timerSettings);
            tabPageTimer.Location = new Point(4, 37);
            tabPageTimer.Name = "tabPageTimer";
            tabPageTimer.Padding = new Padding(3);
            tabPageTimer.Size = new Size(363, 350);
            tabPageTimer.TabIndex = 2;
            tabPageTimer.Text = "计时器";
            // 
            // hotkeySettings
            // 
            hotkeySettings.AutoScroll = true;
            hotkeySettings.Dock = DockStyle.Fill;
            hotkeySettings.Location = new Point(3, 3);
            hotkeySettings.Name = "hotkeySettings";
            hotkeySettings.Size = new Size(357, 344);
            hotkeySettings.TabIndex = 0;
            // 
            // timerSettings
            // 
            timerSettings.AutoScroll = true;
            timerSettings.Dock = DockStyle.Fill;
            timerSettings.Location = new Point(3, 3);
            timerSettings.Name = "timerSettings";
            timerSettings.Size = new Size(357, 344);
            timerSettings.TabIndex = 0;
            // 
            // btnConfirmSettings
            // 
            btnConfirmSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnConfirmSettings.Location = new Point(271, 6);
            btnConfirmSettings.Name = "btnConfirmSettings";
            btnConfirmSettings.Size = new Size(80, 30);
            btnConfirmSettings.TabIndex = 0;
            btnConfirmSettings.Text = "确认";
            btnConfirmSettings.Click += BtnConfirmSettings_Click;
            // 
            // panelBottom
            // 
            panelBottom.Controls.Add(btnConfirmSettings);
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Location = new Point(0, 391);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(371, 45);
            panelBottom.TabIndex = 1;
            // 
            // SettingsControl
            // 
            Controls.Add(tabControl);
            Controls.Add(panelBottom);
            Name = "SettingsControl";
            Size = new Size(371, 436);
            tabControl.ResumeLayout(false);
            tabPageGeneral.ResumeLayout(false);
            tabPageHotkeys.ResumeLayout(false);
            tabPageTimer.ResumeLayout(false);
            panelBottom.ResumeLayout(false);
            ResumeLayout(false);
        }

        public void RefreshUI() {
            if (this.InvokeRequired) { this.Invoke(new Action(RefreshUI)); return; }

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

        public void InitializeData(Services.IAppSettings settings) {
            generalSettings.LoadSettings(settings);
            hotkeySettings.LoadHotkeys(settings);
            timerSettings.LoadSettings(settings);
        }

        private void BtnConfirmSettings_Click(object? sender, EventArgs e) {
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

            // 保存设置
            SettingsManager.SaveSettings(_appSettings);

            // 显示成功提示
            Utils.Toast.Success(Utils.LanguageManager.GetString("SuccessSettingsChanged", "设置修改成功"));

            // 触发所有设置事件，用于其他UI更新
            WindowPositionChanged?.Invoke(this, new WindowPositionChangedEventArgs(generalSettings.SelectedPosition));
            LanguageChanged?.Invoke(this, new LanguageChangedEventArgs(generalSettings.SelectedLanguage));
            AlwaysOnTopChanged?.Invoke(this, new AlwaysOnTopChangedEventArgs(generalSettings.IsAlwaysOnTop));
            HotkeysChanged?.Invoke(this, new AllHotkeysChangedEventArgs(
                hotkeySettings.StartOrNextRunHotkey,
                hotkeySettings.PauseHotkey,
                hotkeySettings.DeleteHistoryHotkey,
                hotkeySettings.RecordLootHotkey
            ));
            TimerSettingsChanged?.Invoke(this, new TimerSettingsChangedEventArgs(
                timerSettings.TimerShowPomodoro,
                timerSettings.TimerShowLootDrops,
                timerSettings.TimerSyncStartPomodoro
            ));
        }

        // ... (其他代码不变) ...

        // 新增：包含所有快捷键的事件参数类
        public class AllHotkeysChangedEventArgs(Keys start, Keys pause, Keys delete, Keys record) : EventArgs {
            public Keys StartHotkey { get; } = start;
            public Keys PauseHotkey { get; } = pause;
            public Keys DeleteHotkey { get; } = delete;
            public Keys RecordHotkey { get; } = record;
        }



        public void ApplyWindowPosition(Form form) {
            MoveWindowToPosition(form, generalSettings.SelectedPosition);
        }

        // 静态辅助方法保持不变
        public static void MoveWindowToPosition(Form form, WindowPosition position) {
            Rectangle screenBounds = Screen.GetWorkingArea(form);
            int x, y;
            switch (position) {
                case WindowPosition.TopLeft: x = screenBounds.Left; y = screenBounds.Top; break;
                case WindowPosition.TopCenter: x = screenBounds.Left + (screenBounds.Width - form.Width) / 2; y = screenBounds.Top; break;
                case WindowPosition.TopRight: x = screenBounds.Right - form.Width; y = screenBounds.Top; break;
                case WindowPosition.BottomLeft: x = screenBounds.Left; y = screenBounds.Bottom - form.Height; break;
                case WindowPosition.BottomCenter: x = screenBounds.Left + (screenBounds.Width - form.Width) / 2; y = screenBounds.Bottom - form.Height; break;
                case WindowPosition.BottomRight: x = screenBounds.Right - form.Width; y = screenBounds.Bottom - form.Height; break;
                default: return;
            }
            form.Location = new Point(x, y);
        }

        // 事件参数类
        public class WindowPositionChangedEventArgs(WindowPosition position) : EventArgs { public WindowPosition Position { get; } = position; }
        public class LanguageChangedEventArgs(LanguageOption language) : EventArgs { public LanguageOption Language { get; } = language; }
        public class AlwaysOnTopChangedEventArgs(bool isAlwaysOnTop) : EventArgs { public bool IsAlwaysOnTop { get; } = isAlwaysOnTop; }
        public class HotkeyChangedEventArgs(Keys hotkey) : EventArgs { public Keys Hotkey { get; } = hotkey; }

        // 新增：计时器设置事件参数类
        public class TimerSettingsChangedEventArgs(bool showPomodoro, bool showLootDrops, bool syncStartPomodoro) : EventArgs {
            public bool ShowPomodoro { get; } = showPomodoro;
            public bool ShowLootDrops { get; } = showLootDrops;
            public bool SyncStartPomodoro { get; } = syncStartPomodoro;
        }
    }
}