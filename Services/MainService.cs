using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI;
using DiabloTwoMFTimer.UI.Pomodoro;
using DiabloTwoMFTimer.UI.Profiles;
using DiabloTwoMFTimer.UI.Settings;
using DiabloTwoMFTimer.UI.Timer;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.Services
{
    public interface IMainServices
    {
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
        IPomodoroTimerService pomodoroTimerService,
        IAppSettings appSettings
    ) : IMainServices, IDisposable
    {
        #region Fields and Properties
        private readonly IProfileService _profileService = profileService;
        private readonly ITimerService _timerService = timerService;
        private readonly ITimerHistoryService _timerHistoryService = timerHistoryService;
        private readonly IPomodoroTimerService _pomodoroTimerService = pomodoroTimerService;
        private readonly IAppSettings _appSettings = appSettings;

        private MainForm? _mainForm;
        private ProfileManager? _profileManager;
        private PomodoroControl? _pomodoroControl;
        private TimerControl? _timerControl;
        private SettingsControl? _settingsControl;

        // 热键相关
        private const int WM_HOTKEY = 0x0312;
        private const int MOD_ALT = 0x0001;
        private const int MOD_CONTROL = 0x0002;
        private const int MOD_SHIFT = 0x0004;

        private const int HOTKEY_ID_STARTSTOP = 1;
        private const int HOTKEY_ID_PAUSE = 2;
        private const int HOTKEY_ID_DELETE_HISTORY = 3;
        private const int HOTKEY_ID_RECORD_LOOT = 4;
        private Keys _currentStartOrNextRunHotkey;
        private Keys _currentPauseHotkey;
        private Keys _currentDeleteHistoryHotkey;
        private Keys _currentRecordLootHotkey;

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
        public void InitializeMainForm(MainForm mainForm)
        {
            _mainForm = mainForm;
            InitializeControlInstances(); // 在这里初始化控件实例
            InitializeControls();
        }

        /// <summary>
        /// 切换到指定的Tab页
        /// </summary>
        public void SetActiveTabPage(Models.TabPage tabPage)
        {
            if (_mainForm?.TabControl != null)
            {
                int tabIndex = (int)tabPage;
                if (tabIndex >= 0 && tabIndex < _mainForm.TabControl.TabCount)
                {
                    _mainForm.TabControl.SelectedIndex = tabIndex;
                }
            }
        }

        /// <summary>
        /// 初始化应用程序
        /// </summary>
        public void InitializeApplication()
        {
            // 1. 从配置中初始化当前热键变量
            _currentStartOrNextRunHotkey = _appSettings.HotkeyStartOrNext;
            _currentPauseHotkey = _appSettings.HotkeyPause;
            _currentDeleteHistoryHotkey = _appSettings.HotkeyDeleteHistory;
            _currentRecordLootHotkey = _appSettings.HotkeyRecordLoot;

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
        public void ProcessHotKeyMessage(Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();

                switch (id)
                {
                    case HOTKEY_ID_STARTSTOP:
                        if (
                            _mainForm?.TabControl != null
                            && _mainForm.TabControl.SelectedIndex != (int)Models.TabPage.Timer
                        )
                        {
                            SetActiveTabPage(Models.TabPage.Timer);
                            bool hasIncompleteRecord = _profileService?.HasIncompleteRecord() ?? false;
                            if (hasIncompleteRecord)
                            {
                                _timerControl?.TogglePause();
                            }
                            else
                            {
                                _timerControl?.ToggleTimer();
                            }
                        }
                        else
                        {
                            _timerControl?.ToggleTimer();
                        }

                        break;
                    case HOTKEY_ID_PAUSE:
                        SetActiveTabPage(Models.TabPage.Timer);
                        _timerControl?.TogglePause();
                        break;
                    case HOTKEY_ID_DELETE_HISTORY:
                        if (
                            _timerControl != null
                            && _mainForm != null
                            && _mainForm.TabControl != null
                            && _mainForm.TabControl.SelectedIndex == (int)Models.TabPage.Timer
                        )
                        {
                            // 异步调用删除选中记录的方法
                            _ = _timerControl.DeleteSelectedRecordAsync();
                        }
                        break;
                    case HOTKEY_ID_RECORD_LOOT:
                        // 切换到计时界面
                        SetActiveTabPage(Models.TabPage.Timer);
                        // 显示掉落记录弹窗
                        if (_mainForm != null)
                        {
                            using var lootForm = new UI.Timer.RecordLootForm(_profileService, _timerHistoryService);
                            // 订阅掉落记录保存成功事件
                            lootForm.LootRecordSaved += OnLootRecordSaved;
                            lootForm.ShowDialog(_mainForm);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 处理标签页切换
        /// </summary>
        public void HandleTabChanged()
        {
            UpdateUI();

            // 当切换到计时器标签页时，调用OnTabSelected方法以加载角色档案数据
            if (TabControl != null && _timerControl != null && TabControl.SelectedIndex == (int)Models.TabPage.Timer)
            {
                _timerControl.HandleTabSelected();
            }
            // 当切换到档案标签页时，重新刷新ProfileManager的UI，确保按钮文本根据最新的计时器状态正确显示
            else if (
                TabControl != null
                && _profileManager != null
                && TabControl.SelectedIndex == (int)Models.TabPage.Profile
            )
            {
                _profileManager.RefreshUI();
            }
        }

        /// <summary>
        /// 刷新UI
        /// </summary>
        public void RefreshUI()
        {
            UpdateUI();
        }

        /// <summary>
        /// 当掉落记录保存成功时触发的事件处理程序
        /// </summary>
        private void OnLootRecordSaved(object? sender, EventArgs e)
        {
            // 刷新UI以显示新添加的掉落记录
            UpdateUI();
        }

        /// <summary>
        /// 处理应用程序关闭
        /// </summary>
        public void HandleApplicationClosing()
        {
            _timerControl?.HandleApplicationClosing();
            UnregisterHotKeys();
        }

        /// <summary>
        /// 应用窗口设置
        /// </summary>
        public void ApplyWindowSettings()
        {
            if (_mainForm != null && _appSettings != null && Screen.PrimaryScreen != null)
            {
                _mainForm.TopMost = _appSettings.AlwaysOnTop;

                var position = SettingsManager.StringToWindowPosition(_appSettings.WindowPosition);
                SettingsControl.MoveWindowToPosition(_mainForm, position);
            }
        }
        #endregion

        #region Private Methods
        private void InitializeControlInstances()
        {
            // 修复：传递 this 作为 IMainServices 参数
            _profileManager = new ProfileManager(
                _profileService,
                _appSettings,
                _timerService,
                _pomodoroTimerService,
                this
            );
            _timerControl = new TimerControl(
                _profileService,
                _timerService,
                _timerHistoryService,
                _pomodoroTimerService
            );
            _pomodoroControl = new PomodoroControl(_pomodoroTimerService, _appSettings, _profileService);
            _settingsControl = new SettingsControl(_appSettings);
            _settingsControl.InitializeData(_appSettings);
        }

        private void InitializeControls()
        {
            if (
                _mainForm == null
                || _profileManager == null
                || _timerControl == null
                || _pomodoroControl == null
                || _settingsControl == null
            )
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

        private void SubscribeToEvents()
        {
            // 订阅计时器事件
            if (_timerControl != null)
            {
                _timerControl.TimerStateChanged += OnTimerTimerStateChanged;
            }
            // 订阅设置事件
            if (_settingsControl != null)
            {
                _settingsControl.WindowPositionChanged += OnWindowPositionChanged;
                _settingsControl.LanguageChanged += OnLanguageChanged;
                _settingsControl.AlwaysOnTopChanged += OnAlwaysOnTopChanged;
                _settingsControl.HotkeysChanged += OnHotkeysChanged;
                _settingsControl.TimerSettingsChanged += OnTimerSettingsChanged;
            }
        }

        /// <summary>
        /// 加载设置到番茄钟服务
        /// </summary>
        private void LoadSettings()
        {
            _pomodoroTimerService.LoadSettings(_appSettings);
        }

        private void InitializeLanguageSupport()
        {
            // 在应用启动时，根据IAppSettings中的Language设置来切换语言
            LanguageManager.SwitchLanguage(
                _appSettings.Language == "Chinese" ? LanguageManager.Chinese : LanguageManager.English
            );
            LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
        }

        private void UpdateUI()
        {
            if (_mainForm == null)
                return;

            _mainForm.UpdateFormTitle(LanguageManager.GetString("FormTitle"));

            // 更新选项卡标题
            if (_mainForm.TabControl != null && _mainForm.TabControl.TabPages.Count >= 4)
            {
                _mainForm.TabControl.TabPages[(int)Models.TabPage.Profile].Text = LanguageManager.GetString(
                    "TabProfile"
                );
                _mainForm.TabControl.TabPages[(int)Models.TabPage.Timer].Text = LanguageManager.GetString("TabTimer");
                _mainForm.TabControl.TabPages[(int)Models.TabPage.Pomodoro].Text = LanguageManager.GetString(
                    "TabPomodoro"
                );
                _mainForm.TabControl.TabPages[(int)Models.TabPage.Settings].Text = LanguageManager.GetString(
                    "TabSettings"
                );
            }

            // 更新各功能控件的UI
            _profileManager?.RefreshUI();
            _timerControl?.RefreshUI();
            _pomodoroControl?.RefreshUI();
            _settingsControl?.RefreshUI();
        }

        private void RegisterHotkeys()
        {
            UnregisterHotKeys();

            RegisterHotKey(_currentStartOrNextRunHotkey, HOTKEY_ID_STARTSTOP);
            RegisterHotKey(_currentPauseHotkey, HOTKEY_ID_PAUSE);
            RegisterHotKey(_currentDeleteHistoryHotkey, HOTKEY_ID_DELETE_HISTORY);
            RegisterHotKey(_currentRecordLootHotkey, HOTKEY_ID_RECORD_LOOT);
            // 不再注册删除历史和记录战利品的热键，因为它们未被使用
        }

        private void UnregisterHotKeys()
        {
            if (_mainForm != null && !_mainForm.IsDisposed && _mainForm.IsHandleCreated)
            {
                UnregisterHotKey(_mainForm.Handle, HOTKEY_ID_STARTSTOP);
                UnregisterHotKey(_mainForm.Handle, HOTKEY_ID_PAUSE);
                UnregisterHotKey(_mainForm.Handle, HOTKEY_ID_DELETE_HISTORY);
                UnregisterHotKey(_mainForm.Handle, HOTKEY_ID_RECORD_LOOT);
            }
        }

        private void RegisterHotKey(Keys keys, int id)
        {
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
        private void OnTimerTimerStateChanged(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        private void OnLanguageChanged(object? sender, SettingsControl.LanguageChangedEventArgs e)
        {
            // 先更新IAppSettings中的Language属性
            if (_appSettings != null)
            {
                _appSettings.Language = SettingsManager.LanguageToString(e.Language);
            }

            // 再调用LanguageManager.SwitchLanguage触发语言变更事件
            if (e.Language == SettingsControl.LanguageOption.Chinese)
            {
                LanguageManager.SwitchLanguage(LanguageManager.Chinese);
            }
            else
            {
                LanguageManager.SwitchLanguage(LanguageManager.English);
            }
        }

        private void OnAlwaysOnTopChanged(object? sender, SettingsControl.AlwaysOnTopChangedEventArgs e)
        {
            if (_mainForm != null)
            {
                _mainForm.TopMost = e.IsAlwaysOnTop;
            }

            if (_appSettings != null)
            {
                _appSettings.AlwaysOnTop = e.IsAlwaysOnTop;
            }
        }

        private void OnHotkeysChanged(object? sender, SettingsControl.AllHotkeysChangedEventArgs e)
        {
            // 更新内存变量
            _currentStartOrNextRunHotkey = e.StartHotkey;
            _currentPauseHotkey = e.PauseHotkey;
            _currentDeleteHistoryHotkey = e.DeleteHotkey;
            _currentRecordLootHotkey = e.RecordHotkey;

            // 重新注册热键使其生效
            RegisterHotkeys();
        }

        private void OnTimerSettingsChanged(object? sender, SettingsControl.TimerSettingsChangedEventArgs e)
        {
            // 更新配置文件中的计时器设置
            if (_appSettings != null)
            {
                _appSettings.TimerShowPomodoro = e.ShowPomodoro;
                _appSettings.TimerShowLootDrops = e.ShowLootDrops;
                _appSettings.TimerSyncStartPomodoro = e.SyncStartPomodoro;
                _appSettings.TimerSyncPausePomodoro = e.SyncPausePomodoro;
                _appSettings.GenerateRoomName = e.GenerateRoomName;
            }

            // 应用掉落记录显示设置到UI
            if (_timerControl != null)
            {
                _timerControl.SetLootRecordsVisible(e.ShowLootDrops);
            }

            // 更新UI
            UpdateUI();

            // 跳转到计时界面
            SetActiveTabPage(Models.TabPage.Timer);
        }

        private void OnWindowPositionChanged(object? sender, SettingsControl.WindowPositionChangedEventArgs e)
        {
            if (_mainForm != null)
            {
                SettingsControl.MoveWindowToPosition(_mainForm, e.Position);
            }

            if (_appSettings != null)
            {
                _appSettings.WindowPosition = SettingsManager.WindowPositionToString(e.Position);
                SettingsManager.SaveSettings(_appSettings);
            }
        }

        private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e)
        {
            UpdateUI();
        }
        #endregion

        #region IDisposable Implementation
        public void Dispose()
        {
            UnregisterHotKeys();

            if (_timerControl != null)
            {
                _timerControl.TimerStateChanged -= OnTimerTimerStateChanged;
            }
            if (_settingsControl != null)
            {
                _settingsControl.WindowPositionChanged -= OnWindowPositionChanged;
                _settingsControl.LanguageChanged -= OnLanguageChanged;
                _settingsControl.AlwaysOnTopChanged -= OnAlwaysOnTopChanged;
                _settingsControl.HotkeysChanged -= OnHotkeysChanged;
                _settingsControl.TimerSettingsChanged -= OnTimerSettingsChanged;
            }

            LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
        }
        #endregion
    }
}
