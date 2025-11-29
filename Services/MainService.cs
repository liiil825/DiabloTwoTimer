using System;
using System.Runtime.InteropServices;
using System.Windows.Forms; // 仅保留用于 Keys, Message, Form (作为参数)
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.Services;

public interface IMainServices
{
    // 初始化应用逻辑，传入窗口句柄用于注册热键
    void InitializeApplication(IntPtr windowHandle);

    // 处理关闭逻辑
    void HandleApplicationClosing();

    // 处理 Windows 消息（热键）
    void ProcessHotKeyMessage(Message msg);

    // 应用窗口设置（变为无状态的辅助方法，或者通过事件通知）
    void ApplyWindowSettings(Form form);

    // 触发 UI 刷新的请求
    void RequestRefresh();

    // --- 事件定义：用于通知 UI 层执行操作 ---

    void SetActiveTabPage(Models.TabPage tabPage);

    // 请求切换 Tab
    event Action<Models.TabPage>? OnRequestTabChange;

    // 请求 UI 刷新
    event Action? OnRequestRefreshUI;

    // 请求删除历史记录 (对应原来的 DeleteSelectedRecordAsync)
    event Action? OnRequestDeleteHistory;

    // 请求显示战利品记录窗口 (对应原来的 RecordLootForm)
    event Action? OnRequestRecordLoot;

    void ReloadHotkeys();
}

public class MainServices(
    IProfileService profileService,
    ITimerService timerService,
    ITimerHistoryService timerHistoryService,
    IPomodoroTimerService pomodoroTimerService,
    IAppSettings appSettings
    ) : IMainServices, IDisposable
{
    #region Services & Fields
    private readonly IProfileService _profileService = profileService;
    private readonly ITimerService _timerService = timerService;
    private readonly ITimerHistoryService _timerHistoryService = timerHistoryService;
    private readonly IPomodoroTimerService _pomodoroTimerService = pomodoroTimerService;
    private readonly IAppSettings _appSettings = appSettings;

    // 仅持有句柄用于注册热键，不持有 Form 对象
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

    private Keys _currentStartOrNextRunHotkey;
    private Keys _currentPauseHotkey;
    private Keys _currentDeleteHistoryHotkey;
    private Keys _currentRecordLootHotkey;

    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    #endregion

    #region Events
    public event Action<Models.TabPage>? OnRequestTabChange;
    public event Action? OnRequestRefreshUI;
    public event Action? OnRequestDeleteHistory;
    public event Action? OnRequestRecordLoot;

    #endregion

    #region Public Methods

    /// <summary>
    /// 初始化应用程序逻辑
    /// </summary>
    /// <param name="windowHandle">主窗口句柄，用于注册热键</param>
    public void InitializeApplication(IntPtr windowHandle)
    {
        _windowHandle = windowHandle;

        // 1. 从配置中初始化当前热键变量
        _currentStartOrNextRunHotkey = _appSettings.HotkeyStartOrNext;
        _currentPauseHotkey = _appSettings.HotkeyPause;
        _currentDeleteHistoryHotkey = _appSettings.HotkeyDeleteHistory;
        _currentRecordLootHotkey = _appSettings.HotkeyRecordLoot;

        InitializeLanguageSupport();
        RegisterHotkeys();

        // 加载上次使用的角色档案 (不再直接操作 ProfileManager，而是依赖 ProfileService)
        // 注意：UI 层的 ProfileManager 需要自己监听 ProfileService 的变化或者在初始化时读取
        LoadLastUsedProfile();

        // 恢复未完成记录
        _timerService.RestoreIncompleteRecord();

        // 通知 UI 刷新
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

    /// <summary>
    /// 处理热键消息
    /// </summary>
    public void ProcessHotKeyMessage(Message m)
    {
        if (m.Msg != WM_HOTKEY) return;

        int id = m.WParam.ToInt32();

        switch (id)
        {
            case HOTKEY_ID_STARTSTOP:
                // 逻辑：如果热键触发，强制切到 Timer 页面，并执行开始/停止
                OnRequestTabChange?.Invoke(Models.TabPage.Timer);
                _timerService.StartOrNextRun();
                break;

            case HOTKEY_ID_PAUSE:
                OnRequestTabChange?.Invoke(Models.TabPage.Timer);
                _timerService.TogglePause();
                break;

            case HOTKEY_ID_DELETE_HISTORY:
                // 逻辑：只有在 Timer 页面才能删除 (判断逻辑也可以移给 UI 层，或者在这里判断)
                // 这里我们选择触发事件，让 UI 层决定是否响应以及如何响应
                OnRequestDeleteHistory?.Invoke();
                break;

            case HOTKEY_ID_RECORD_LOOT:
                OnRequestTabChange?.Invoke(Models.TabPage.Timer);
                // 触发事件，让 UI 层弹出 Loot 窗口
                OnRequestRecordLoot?.Invoke();
                break;
        }
    }

    /// <summary>
    /// 应用窗口设置 (无状态辅助方法)
    /// </summary>
    public void ApplyWindowSettings(Form form)
    {
        if (form != null && _appSettings != null)
        {
            form.TopMost = _appSettings.AlwaysOnTop;

            if (Screen.PrimaryScreen != null)
            {
                var position = SettingsManager.StringToWindowPosition(_appSettings.WindowPosition);
                UI.Settings.SettingsControl.MoveWindowToPosition(form, position);
            }
        }
    }

    public void SetActiveTabPage(Models.TabPage tabPage)
    {
        // 这里的 OnRequestTabChange 会被 MainForm 监听到，
        // 然后由 MainForm 去真正执行 tabControl.SelectedIndex = ...
        OnRequestTabChange?.Invoke(tabPage);
    }

    public void ReloadHotkeys()
    {
        // 1. 重新从配置读取热键到内存变量
        _currentStartOrNextRunHotkey = _appSettings.HotkeyStartOrNext;
        _currentPauseHotkey = _appSettings.HotkeyPause;
        _currentDeleteHistoryHotkey = _appSettings.HotkeyDeleteHistory;
        _currentRecordLootHotkey = _appSettings.HotkeyRecordLoot;

        // 2. 重新注册
        RegisterHotkeys();

        // 记录日志方便调试
        Utils.LogManager.WriteDebugLog("MainServices", "热键已重新注册");
    }

    #endregion

    #region Private Methods

    private void LoadLastUsedProfile()
    {
        // 这一步是纯数据逻辑，没问题
        if (!string.IsNullOrEmpty(_appSettings.LastUsedProfile))
        {
            var profile = Utils.DataHelper.LoadProfileByName(_appSettings.LastUsedProfile);
            if (profile != null)
            {
                // 设置当前 Profile，这将触发 ProfileService 的事件，
                // UI 层的 ProfileManager 和 TimerControl 会监听到这个变化并自动更新
                _profileService.SwitchCharacter(profile);
            }
        }
    }

    private void InitializeLanguageSupport()
    {
        LanguageManager.SwitchLanguage(
            _appSettings.Language == "Chinese" ? LanguageManager.Chinese : LanguageManager.English
        );
        // MainService 本身不需要监听语言变化来更新 UI，因为它不再持有 UI 控件
        // 只需要确保设置了正确的语言即可
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
        }
    }

    private void RegisterHotKey(Keys keys, int id)
    {
        if (_windowHandle == IntPtr.Zero) return;

        int modifiers = 0;
        if ((keys & Keys.Alt) == Keys.Alt) modifiers |= MOD_ALT;
        if ((keys & Keys.Control) == Keys.Control) modifiers |= MOD_CONTROL;
        if ((keys & Keys.Shift) == Keys.Shift) modifiers |= MOD_SHIFT;

        int keyCode = (int)(keys & Keys.KeyCode);
        RegisterHotKey(_windowHandle, id, modifiers, keyCode);
    }

    #endregion

    public void Dispose()
    {
        UnregisterHotKeys();
        // 取消订阅其他可能的事件
    }
}