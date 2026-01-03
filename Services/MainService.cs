using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace DiabloTwoMFTimer.Services;

public class MainServices(
    IServiceProvider serviceProvider,
    IProfileService profileService,
    ITimerService timerService,
    ITimerHistoryService timerHistoryService,
    IPomodoroTimerService pomodoroTimerService,
    IProfileRepository profileRepository,
    IAppSettings appSettings,
    IMessenger messenger
) : IMainService, IDisposable
{
    #region Services & Fields
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IProfileRepository _profileRepository = profileRepository;
    private readonly IProfileService _profileService = profileService;
    private readonly ITimerService _timerService = timerService;
    private readonly ITimerHistoryService _timerHistoryService = timerHistoryService;
    private readonly IPomodoroTimerService _pomodoroTimerService = pomodoroTimerService;
    private readonly IAppSettings _appSettings = appSettings;
    private readonly IMessenger _messenger = messenger;

    private IntPtr _windowHandle;

    // 热键常量
    private const int WM_HOTKEY = 0x0312;
    private const int MOD_ALT = 0x0001;
    private const int MOD_CONTROL = 0x0002;
    private const int MOD_SHIFT = 0x0004;

    private const int HOTKEY_ID_STARTSTOP = 1;
    private const int HOTKEY_ID_PAUSE = 2;
    private const int HOTKEY_ID_DELETE_HISTORY = 3;
    private const int HOTKEY_ID_RECORD_LOOT = 4;
    private const int HOTKEY_ID_LEADER = 5;

    private Keys _currentStartOrNextRunHotkey;
    private Keys _currentPauseHotkey;
    private Keys _currentDeleteHistoryHotkey;
    private Keys _currentRecordLootHotkey;
    private Keys _currentLeaderHotkey;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    #endregion

    #region Events
    public event Action<Models.TabPage>? OnRequestTabChange;
    public event Action? OnRequestRefreshUI;
    public event Action? OnRequestDeleteHistory;
    public event Action? OnRequestDeleteLastHistory;
    public event Action? OnRequestDeleteLastLoot;
    #endregion

    #region Public Methods

    // 请求删除选中的记录
    public void RequestDeleteSelectedRecord()
    {
        OnRequestDeleteHistory?.Invoke();
    }

    // 请求删除最后一个时间记录
    public void RequestDeleteLastHistory()
    {
        OnRequestDeleteLastHistory?.Invoke();
    }

    // 请求删除最后一个掉落记录
    public void RequestDeleteLastLoot()
    {
        OnRequestDeleteLastLoot?.Invoke();
    }

    #endregion

    #region Public Methods

    public void InitializeApplication(IntPtr windowHandle)
    {
        _windowHandle = windowHandle;

        _currentStartOrNextRunHotkey = _appSettings.HotkeyStartOrNext;
        _currentPauseHotkey = _appSettings.HotkeyPause;
        _currentDeleteHistoryHotkey = _appSettings.HotkeyDeleteHistory;
        _currentRecordLootHotkey = _appSettings.HotkeyRecordLoot;
        _currentLeaderHotkey = _appSettings.HotkeyLeader;

        InitializeLanguageSupport();
        RegisterHotkeys();

        _messenger.Subscribe<LanguageChangedMessage>(OnLanguageChanged);
        _messenger.Subscribe<TimerSettingsChangedMessage>(OnTimerSettingsChanged);
        _messenger.Subscribe<HotkeysChangedMessage>(_ => ReloadHotkeys());

        // --- 新增：监听暂停和恢复消息 ---
        _messenger.Subscribe<SuspendHotkeysMessage>(_ => UnregisterHotKeys());
        _messenger.Subscribe<ResumeHotkeysMessage>(_ => RegisterHotkeys());

        LoadLastUsedProfile();
        _timerService.RestoreIncompleteRecord();
        RequestRefresh();
    }

    public void RequestRefresh()
    {
        OnRequestRefreshUI?.Invoke();
    }

    public void HandleApplicationClosing()
    {
        _timerService.HandleApplicationClosing();
        UnregisterHotKeys();
    }

    public void ProcessHotKeyMessage(Message m)
    {
        if (m.Msg != WM_HOTKEY)
            return;

        int id = m.WParam.ToInt32();

        switch (id)
        {
            case HOTKEY_ID_STARTSTOP:
                OnRequestTabChange?.Invoke(Models.TabPage.Timer);
                _timerService.StartOrNextRun();
                break;

            case HOTKEY_ID_PAUSE:
                OnRequestTabChange?.Invoke(Models.TabPage.Timer);
                _timerService.TogglePause();
                break;

            case HOTKEY_ID_DELETE_HISTORY:
                OnRequestDeleteHistory?.Invoke();
                break;

            case HOTKEY_ID_RECORD_LOOT:
                OnRequestTabChange?.Invoke(Models.TabPage.Timer);
                _messenger.Publish(new ShowRecordLootFormMessage());
                break;

            case HOTKEY_ID_LEADER:
                _messenger.Publish(new ShowLeaderKeyFormMessage());
                break;
        }
    }

    public void ApplyWindowSettings(Form _)
    {
        _messenger.Publish(new WindowPositionChangedMessage());
    }

    public void SetActiveTabPage(Models.TabPage tabPage)
    {
        OnRequestTabChange?.Invoke(tabPage);
    }

    public void ReloadHotkeys()
    {
        _currentStartOrNextRunHotkey = _appSettings.HotkeyStartOrNext;
        _currentPauseHotkey = _appSettings.HotkeyPause;
        _currentDeleteHistoryHotkey = _appSettings.HotkeyDeleteHistory;
        _currentRecordLootHotkey = _appSettings.HotkeyRecordLoot;
        _currentLeaderHotkey = _appSettings.HotkeyLeader;

        RegisterHotkeys();
        Utils.LogManager.WriteDebugLog("MainServices", "热键已重新注册");
    }

    public void UpdateWindowHandle(IntPtr newHandle)
    {
        if (_windowHandle != newHandle)
        {
            // 先注销旧句柄的热键（虽然旧句柄可能已经销毁，但为了保险）
            UnregisterHotKeys();

            // 更新句柄
            _windowHandle = newHandle;

            // 重新注册
            RegisterHotkeys();
            Utils.LogManager.WriteDebugLog("MainServices", $"窗口句柄已更新为 {newHandle}，热键已重新注册");
        }
    }

    #endregion

    #region Private Methods

    private void LoadLastUsedProfile()
    {
        if (!string.IsNullOrEmpty(_appSettings.LastUsedProfile))
        {
            var profile = _profileRepository.GetByName(_appSettings.LastUsedProfile);
            if (profile != null)
            {
                _profileService.SwitchCharacter(profile);
            }
        }
    }

    private void InitializeLanguageSupport()
    {
        LanguageManager.SwitchLanguage(
            _appSettings.Language == "Chinese" ? LanguageManager.Chinese : LanguageManager.English
        );
    }

    private void OnTimerSettingsChanged(TimerSettingsChangedMessage _)
    {
        OnRequestTabChange?.Invoke(Models.TabPage.Timer);
        RequestRefresh();
    }

    private void OnLanguageChanged(LanguageChangedMessage message)
    {
        LanguageManager.SwitchLanguage(message.LanguageCode);
        RequestRefresh();
    }

    private void RegisterHotkeys()
    {
        UnregisterHotKeys();

        if (_windowHandle != IntPtr.Zero)
        {
            RegisterHotKey(_currentStartOrNextRunHotkey, HOTKEY_ID_STARTSTOP);
            RegisterHotKey(_currentPauseHotkey, HOTKEY_ID_PAUSE);
            RegisterHotKey(_currentDeleteHistoryHotkey, HOTKEY_ID_DELETE_HISTORY);
            RegisterHotKey(_currentRecordLootHotkey, HOTKEY_ID_RECORD_LOOT);
            RegisterHotKey(_currentLeaderHotkey, HOTKEY_ID_LEADER);
        }
    }

    private void UnregisterHotKeys()
    {
        if (_windowHandle != IntPtr.Zero)
        {
            UnregisterHotKey(_windowHandle, HOTKEY_ID_STARTSTOP);
            UnregisterHotKey(_windowHandle, HOTKEY_ID_PAUSE);
            UnregisterHotKey(_windowHandle, HOTKEY_ID_DELETE_HISTORY);
            UnregisterHotKey(_windowHandle, HOTKEY_ID_RECORD_LOOT);
            UnregisterHotKey(_windowHandle, HOTKEY_ID_LEADER);
        }
    }

    private void RegisterHotKey(Keys keys, int id)
    {
        if (_windowHandle == IntPtr.Zero || keys == Keys.None)
            return;

        int modifiers = 0;
        if ((keys & Keys.Alt) == Keys.Alt)
            modifiers |= MOD_ALT;
        if ((keys & Keys.Control) == Keys.Control)
            modifiers |= MOD_CONTROL;
        if ((keys & Keys.Shift) == Keys.Shift)
            modifiers |= MOD_SHIFT;

        int keyCode = (int)(keys & Keys.KeyCode);
        RegisterHotKey(_windowHandle, id, modifiers, keyCode);
    }

    #endregion

    public void Dispose()
    {
        UnregisterHotKeys();
        _messenger.Unsubscribe<TimerSettingsChangedMessage>(OnTimerSettingsChanged);
    }

    // 设置番茄钟模式
    public void SetPomodoroMode(Models.PomodoroMode mode)
    {
        _appSettings.PomodoroMode = mode;
        _appSettings.Save();
        _messenger.Publish(new Models.PomodoroModeChangedMessage(mode));

        // 显示成功提示
        string modeName = LanguageManager.GetString($"PomodoroMode_{mode}");
        Utils.Toast.Success($"番茄钟模式已设置为: {modeName}");
    }

    public void RequestShowSettings()
    {
        // 使用 IServiceProvider 创建一个新的 SettingsForm 实例
        // 这样可以确保每次打开都是新的，且 Form 内部的依赖 (如 AudioService) 也会被自动注入
        var settingsForm = _serviceProvider.GetRequiredService<UI.Form.SettingsForm>();

        // 以模态对话框方式显示
        settingsForm.ShowDialog();

        // ShowDialog 会阻塞直到窗口关闭，关闭后资源由 Form 自身 Dispose 处理
        // (因为我们在 SettingsForm 里没写特殊逻辑，它关闭时会自动 Dispose)
    }

    public void RequestShowAbout()
    {
        // 使用 IServiceProvider 创建一个新的 AboutForm 实例
        var aboutForm = _serviceProvider.GetRequiredService<UI.Form.AboutForm>();

        // 以模态对话框方式显示
        aboutForm.ShowDialog();

        // ShowDialog 会阻塞直到窗口关闭，关闭后资源由 Form 自身 Dispose 处理
    }
}
