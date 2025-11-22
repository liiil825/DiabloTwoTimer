using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.UI;
using DTwoMFTimerHelper.UI.Timer;
using DTwoMFTimerHelper.UI.Pomodoro;
using DTwoMFTimerHelper.UI.Settings;
using DTwoMFTimerHelper.UI.Profiles;

namespace DTwoMFTimerHelper.Services {
    public interface IMainServices {
        void InitializeMainForm(MainForm mainForm);
        void HandleTabChanged();
        void RefreshUI();
        void InitializeApplication();
        void ApplyWindowSettings();
        void ProcessHotKeyMessage(Message msg);
        void HandleApplicationClosing();
        void SetActiveTabPage(Models.TabPage tabPage);
    }

    public class MainServices(
        IProfileService profileService,
        ITimerService timerService,
        ITimerHistoryService timerHistoryService,
        PomodoroTimerService pomodoroTimerService) : IMainServices, IDisposable {

        #region Fields and Properties
        private readonly IProfileService _profileService = profileService;
        private readonly ITimerService _timerService = timerService;
        private readonly ITimerHistoryService _timerHistoryService = timerHistoryService;
        private readonly PomodoroTimerService _pomodoroTimerService = pomodoroTimerService;

        private MainForm? _mainForm;
        private ProfileManager? _profileManager;
        private PomodoroControl? _pomodoroControl;
        private TimerControl? _timerControl;
        private SettingsControl? _settingsControl;
        private AppSettings? _appSettings;
        private bool _disposed = false;

        // 热键相关
        private const int WM_HOTKEY = 0x0312;
        private const int MOD_ALT = 0x0001;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;
        private const int MOD_WIN = 0x0008;

        private const int HOTKEY_ID_STARTSTOP = 1;
        private const int HOTKEY_ID_PAUSE = 2;
        private const int HOTKEY_ID_DELETE_HISTORY = 3;
        private const int HOTKEY_ID_RECORD_LOOT = 4;

        private Keys _currentStartStopHotkey = Keys.Q | Keys.Alt;
        private Keys _currentPauseHotkey = Keys.Space | Keys.Control;
        private Keys _currentDeleteHistoryHotkey = Keys.D | Keys.Control;
        private Keys _currentRecordLootHotkey = Keys.A | Keys.Alt;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public ProfileManager? ProfileManager => _profileManager;
        public TimerControl? TimerControl => _timerControl;
        public TabControl? TabControl => _mainForm?.TabControl;
        #endregion

        #region Public Methods
        /// <summary>
        /// 初始化主窗体引用和相关组件
        /// 程序第一层加载时调用
        /// </summary>
        public void InitializeMainForm(MainForm mainForm) {
            _mainForm = mainForm;
            InitializeControlInstances(); // 在这里初始化控件实例
            InitializeControls();
        }

        /// <summary>
        /// 切换到指定的Tab页
        /// </summary>
        public void SetActiveTabPage(Models.TabPage tabPage) {
            if (_mainForm?.TabControl != null) {
                int tabIndex = (int)tabPage;
                if (tabIndex >= 0 && tabIndex < _mainForm.TabControl.TabCount) {
                    _mainForm.TabControl.SelectedIndex = tabIndex;
                }
            }
        }

        /// <summary>
        /// 初始化应用程序
        /// </summary>
        public void InitializeApplication() {
            LoadSettings();
            InitializeLanguageSupport();
            ApplyWindowSettings();
            RegisterHotkeys();

            // 加载上次使用的角色档案
            _profileManager?.LoadLastUsedProfile();

            // 在所有初始化完成后，触发恢复未完成记录的请求
            _timerService.RestoreIncompleteRecord();
            UpdateUI();
        }

        /// <summary>
        /// 处理热键消息
        /// </summary>
        public void ProcessHotKeyMessage(Message m) {
            if (m.Msg == WM_HOTKEY) {
                int id = m.WParam.ToInt32();

                switch (id) {
                    case HOTKEY_ID_STARTSTOP:
                        if (_mainForm?.TabControl != null && _mainForm.TabControl.SelectedIndex != (int)Models.TabPage.Timer) {
                            SetActiveTabPage(Models.TabPage.Timer);
                            bool hasIncompleteRecord = _profileService?.HasIncompleteRecord() ?? false;
                            if (hasIncompleteRecord) {
                                _timerControl?.TogglePause();
                            }
                            else {
                                _timerControl?.ToggleTimer();
                            }
                        }
                        else {
                            _timerControl?.ToggleTimer();
                        }

                        break;
                    case HOTKEY_ID_PAUSE:
                        SetActiveTabPage(Models.TabPage.Timer);
                        _timerControl?.TogglePause();
                        break;
                    case HOTKEY_ID_DELETE_HISTORY:
                        if (_timerControl != null && _mainForm != null &&
                        _mainForm.TabControl != null &&
                         _mainForm.TabControl.SelectedIndex == (int)Models.TabPage.Timer) {
                            // 异步调用删除选中记录的方法
                            _ = _timerControl.DeleteSelectedRecordAsync();
                        }
                        break;
                    case HOTKEY_ID_RECORD_LOOT:
                        // 切换到计时界面
                        SetActiveTabPage(Models.TabPage.Timer);
                        // 显示掉落记录弹窗
                        if (_mainForm != null) {
                            using (var lootForm = new UI.Timer.RecordLootForm(_profileService, _timerHistoryService)) {
                                lootForm.ShowDialog(_mainForm);
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 处理标签页切换
        /// </summary>
        public void HandleTabChanged() {
            UpdateUI();

            // 当切换到计时器标签页时，调用OnTabSelected方法以加载角色档案数据
            if (TabControl != null && _timerControl != null && TabControl.SelectedIndex == (int)Models.TabPage.Timer) {
                _timerControl.HandleTabSelected();
            }
            // 当切换到档案标签页时，重新刷新ProfileManager的UI，确保按钮文本根据最新的计时器状态正确显示
            else if (TabControl != null && _profileManager != null && TabControl.SelectedIndex == (int)Models.TabPage.Profile) {
                _profileManager.RefreshUI();
            }
        }

        /// <summary>
        /// 刷新UI
        /// </summary>
        public void RefreshUI() {
            UpdateUI();
        }

        /// <summary>
        /// 处理应用程序关闭
        /// </summary>
        public void HandleApplicationClosing() {
            _timerControl?.HandleApplicationClosing();
            UnregisterHotKeys();
        }

        /// <summary>
        /// 应用窗口设置
        /// </summary>
        public void ApplyWindowSettings() {
            if (_mainForm != null && _appSettings != null && Screen.PrimaryScreen != null) {
                _mainForm.TopMost = _appSettings.AlwaysOnTop;

                var position = SettingsManager.StringToWindowPosition(_appSettings.WindowPosition);
                SettingsControl.MoveWindowToPosition(_mainForm, position);
            }
        }
        #endregion

        #region Private Methods
        private void InitializeControlInstances() {
            // 修复：传递 this 作为 IMainServices 参数
            _profileManager = new ProfileManager(_profileService, _timerService, this);
            _timerControl = new TimerControl(_profileService, _timerService, _timerHistoryService);
            _pomodoroControl = new PomodoroControl(_pomodoroTimerService);
            _settingsControl = new SettingsControl();
        }

        private void InitializeControls() {
            if (_mainForm == null || _profileManager == null || _timerControl == null || _pomodoroControl == null || _settingsControl == null)
                return;

            // 设置控件的Dock属性
            _profileManager.Dock = DockStyle.Fill;
            _timerControl.Dock = DockStyle.Fill;
            _pomodoroControl.Dock = DockStyle.Fill;
            _settingsControl.Dock = DockStyle.Fill;

            // 添加到对应的TabPage
            _mainForm.TabProfilePage?.Controls.Add(_profileManager);
            _mainForm.TabTimerPage?.Controls.Add(_timerControl);
            _mainForm.TabPomodoroPage?.Controls.Add(_pomodoroControl);
            _mainForm.TabSettingsPage?.Controls.Add(_settingsControl);

            // 订阅事件
            SubscribeToEvents();
        }

        private void SubscribeToEvents() {
            // 订阅计时器事件
            if (_timerControl != null) {
                _timerControl.TimerStateChanged += OnTimerTimerStateChanged;
            }

            // 订阅设置事件
            if (_settingsControl != null) {
                _settingsControl.WindowPositionChanged += OnWindowPositionChanged;
                _settingsControl.LanguageChanged += OnLanguageChanged;
                _settingsControl.AlwaysOnTopChanged += OnAlwaysOnTopChanged;
                _settingsControl.StartStopHotkeyChanged += OnStartStopHotkeyChanged;
                _settingsControl.PauseHotkeyChanged += OnPauseHotkeyChanged;
            }
        }

        private void LoadSettings() {
            _appSettings = SettingsManager.LoadSettings();
        }

        private void InitializeLanguageSupport() {
            LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
        }

        private void UpdateUI() {
            if (_mainForm == null)
                return;

            _mainForm.UpdateFormTitle(LanguageManager.GetString("FormTitle"));

            // 更新选项卡标题
            if (_mainForm.TabControl != null && _mainForm.TabControl.TabPages.Count >= 4) {
                _mainForm.TabControl.TabPages[(int)Models.TabPage.Profile].Text = LanguageManager.GetString("TabProfile");
                _mainForm.TabControl.TabPages[(int)Models.TabPage.Timer].Text = LanguageManager.GetString("TabTimer");
                _mainForm.TabControl.TabPages[(int)Models.TabPage.Pomodoro].Text = LanguageManager.GetString("TabPomodoro");
                _mainForm.TabControl.TabPages[(int)Models.TabPage.Settings].Text = LanguageManager.GetString("TabSettings");
            }

            // 更新各功能控件的UI
            _profileManager?.RefreshUI();
            _timerControl?.RefreshUI();
            _pomodoroControl?.RefreshUI();
            _settingsControl?.RefreshUI();
        }

        private void RegisterHotkeys() {
            UnregisterHotKeys();

            RegisterHotKey(_currentStartStopHotkey, HOTKEY_ID_STARTSTOP);
            RegisterHotKey(_currentPauseHotkey, HOTKEY_ID_PAUSE);
            RegisterHotKey(_currentDeleteHistoryHotkey, HOTKEY_ID_DELETE_HISTORY);
            RegisterHotKey(_currentRecordLootHotkey, HOTKEY_ID_RECORD_LOOT);
        }

        private void UnregisterHotKeys() {
            if (_mainForm != null && !_mainForm.IsDisposed && _mainForm.IsHandleCreated) {
                UnregisterHotKey(_mainForm.Handle, HOTKEY_ID_STARTSTOP);
                UnregisterHotKey(_mainForm.Handle, HOTKEY_ID_PAUSE);
                UnregisterHotKey(_mainForm.Handle, HOTKEY_ID_DELETE_HISTORY);
                UnregisterHotKey(_mainForm.Handle, HOTKEY_ID_RECORD_LOOT);
            }
        }

        private void RegisterHotKey(Keys keys, int id) {
            if (_mainForm == null || _mainForm.IsDisposed || !_mainForm.IsHandleCreated)
                return;

            int modifiers = 0;

            if ((keys & Keys.Alt) == Keys.Alt)
                modifiers |= MOD_ALT;
            if ((keys & Keys.Control) == Keys.Control)
                modifiers |= MOD_CONTROL;
            if ((keys & Keys.Shift) == Keys.Shift)
                modifiers |= MOD_SHIFT;

            int keyCode = (int)(keys & Keys.KeyCode);

            RegisterHotKey(_mainForm.Handle, id, modifiers, keyCode);
        }
        #endregion

        #region Event Handlers
        private void OnTimerTimerStateChanged(object? sender, EventArgs e) {
            UpdateUI();
        }

        private void OnLanguageChanged(object? sender, SettingsControl.LanguageChangedEventArgs e) {
            if (e.Language == SettingsControl.LanguageOption.Chinese) {
                LanguageManager.SwitchLanguage(LanguageManager.Chinese);
            }
            else {
                LanguageManager.SwitchLanguage(LanguageManager.English);
            }

            if (_appSettings != null) {
                _appSettings.Language = SettingsManager.LanguageToString(e.Language);
                SettingsManager.SaveSettings(_appSettings);
            }
        }

        private void OnAlwaysOnTopChanged(object? sender, SettingsControl.AlwaysOnTopChangedEventArgs e) {
            if (_mainForm != null) {
                _mainForm.TopMost = e.IsAlwaysOnTop;
            }

            if (_appSettings != null) {
                _appSettings.AlwaysOnTop = e.IsAlwaysOnTop;
                SettingsManager.SaveSettings(_appSettings);
            }
        }

        private void OnStartStopHotkeyChanged(object? sender, SettingsControl.HotkeyChangedEventArgs e) {
            _currentStartStopHotkey = e.Hotkey;
            RegisterHotkeys();
        }

        private void OnPauseHotkeyChanged(object? sender, SettingsControl.HotkeyChangedEventArgs e) {
            _currentPauseHotkey = e.Hotkey;
            RegisterHotkeys();
        }

        private void OnWindowPositionChanged(object? sender, SettingsControl.WindowPositionChangedEventArgs e) {
            if (_mainForm != null) {
                SettingsControl.MoveWindowToPosition(_mainForm, e.Position);
            }

            if (_appSettings != null) {
                _appSettings.WindowPosition = SettingsManager.WindowPositionToString(e.Position);
                SettingsManager.SaveSettings(_appSettings);
            }
        }

        private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e) {
            UpdateUI();
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose() {
            UnregisterHotKeys();

            if (_timerControl != null) {
                _timerControl.TimerStateChanged -= OnTimerTimerStateChanged;
            }
            if (_settingsControl != null) {
                _settingsControl.WindowPositionChanged -= OnWindowPositionChanged;
                _settingsControl.LanguageChanged -= OnLanguageChanged;
                _settingsControl.AlwaysOnTopChanged -= OnAlwaysOnTopChanged;
                _settingsControl.StartStopHotkeyChanged -= OnStartStopHotkeyChanged;
                _settingsControl.PauseHotkeyChanged -= OnPauseHotkeyChanged;
            }

            LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
        }
        #endregion
    }
}