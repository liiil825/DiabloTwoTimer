using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.UI.Settings;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Timer;

public partial class TimerControl : UserControl
{
    // 服务层引用
    private readonly ITimerService _timerService = null!;
    private readonly IProfileService _profileService = null!;
    private readonly ITimerHistoryService _historyService = null!;
    private readonly IPomodoroTimerService _pomodoroTimerService = null!;
    private readonly IMessenger _messenger = null!;
    private readonly IAppSettings _appSettings = null!;
    private readonly ISceneService _sceneService = null!;

    public TimerControl()
    {
        InitializeComponent();
        InitializeInteractionLogic(); // 【新增】初始化交互逻辑

        // 设置圆形指示器
        // 建议放在构造函数末尾，确保控件尺寸已初始化
        if (btnStatusIndicator != null)
        {
            using System.Drawing.Drawing2D.GraphicsPath path = new();
            path.AddEllipse(0, 0, btnStatusIndicator.Width, btnStatusIndicator.Height);
            btnStatusIndicator.Region = new System.Drawing.Region(path);
        }
    }

    /// <summary>
    /// 重载构造函数，用于注入番茄计时器服务
    /// </summary>
    /// <param name="profileService"></param>
    /// <param name="timerService"></param>
    /// <param name="historyService"></param>
    /// <param name="pomodoroTimerService"></param>
    public TimerControl(
        IProfileService profileService,
        ITimerService timerService,
        ITimerHistoryService historyService,
        IPomodoroTimerService pomodoroTimerService,
        IAppSettings appSettings,
        ISceneService sceneService,
        IMessenger messenger // 新增参数
    )
        : this()
    {
        _timerService = timerService;
        _profileService = profileService;
        _historyService = historyService;
        _pomodoroTimerService = pomodoroTimerService;
        _appSettings = appSettings;
        _messenger = messenger; // 赋值新增字段
        _sceneService = sceneService;

        this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        // 初始化子控件的服务引用
        characterSceneControl?.Initialize(_profileService, _appSettings, _sceneService);
        historyControl?.Initialize(_historyService, _profileService);
        lootRecordsControl?.Initialize(_profileService, _sceneService);

        // 订阅服务事件
        SubscribeToServiceEvents();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!DesignMode)
        {
            LoadProfileHistoryData();

            // 根据角色档案的ShowLoot设置初始化掉落记录控件的可见性
            InitializeLootRecordsVisibility();
            // 根据设置初始化番茄时间显示
            InitializePomodoroVisibility();
            // 【关键】首次加载时，清除所有选中状态，防止默认选中第一行掉落
            ClearAllSelections();
            // 更新界面状态
            UpdateUI();
        }

        pomodoroTime.BindService(_pomodoroTimerService);
    }

    private void ClearAllSelections()
    {
        lootRecordsControl?.ClearSelection();
        historyControl?.ClearSelection();
        // 让焦点落在一个“无害”的控件上，比如时间显示Label（虽然Label不能获得焦点）
        // 或者直接设 ActiveControl = null
        this.ActiveControl = null;
    }

    private void InitializePomodoroVisibility()
    {
        // 根据应用设置的TimerShowPomodoro设置初始化番茄时间控件的可见性
        pomodoroTime.Visible = _appSettings.TimerShowPomodoro;
    }

    private void InitializeLootRecordsVisibility()
    {
        if (lootRecordsControl != null)
        {
            // 根据应用设置的TimerShowLootDrops设置初始化掉落记录控件的可见性
            lootRecordsControl.Visible = _appSettings.TimerShowLootDrops;
            bool isVisible = lootRecordsControl.Visible;
            SetLootRecordsVisible(isVisible);
        }
    }

    // 互斥逻辑：一个动，另一个清
    private void InitializeInteractionLogic()
    {
        if (historyControl != null && lootRecordsControl != null)
        {
            historyControl.InteractionOccurred += (s, e) => lootRecordsControl.ClearSelection();
            lootRecordsControl.InteractionOccurred += (s, e) => historyControl.ClearSelection();
        }
    }

    // [重构] 消息处理方法
    private void OnTimerSettingsChanged(TimerSettingsChangedMessage message)
    {
        // Messenger 可能在任意线程回调，必须使用 SafeInvoke
        this.SafeInvoke(() =>
        {
            // 响应 ShowLootDrops 变化
            SetLootRecordsVisible(message.ShowLootDrops);

            // 响应 ShowPomodoro 变化
            if (pomodoroTime != null)
            {
                pomodoroTime.Visible = message.ShowPomodoro;
            }

            // 其他设置如果需要立即响应 UI 变化也可以在这里处理
        });
    }

    // 事件
    public event EventHandler? TimerStateChanged;

    public bool IsTimerRunning => _timerService?.IsRunning ?? false;

    #region Service Event Handlers
    private void SubscribeToServiceEvents()
    {
        if (_timerService == null || _profileService == null)
            return;
        // 订阅TimerService事件
        _timerService.TimeUpdatedEvent += OnTimeUpdated;
        _timerService.TimerRunningStateChangedEvent += OnTimerRunningStateChanged;
        _timerService.TimerPauseStateChangedEvent += OnTimerPauseStateChanged;
        _timerService.TimerResetEvent += OnTimerReset;
        _timerService.RunCompletedEvent += OnRunCompleted;

        // 订阅ProfileService事件
        _profileService.CurrentProfileChangedEvent += OnProfileChanged;
        _profileService.CurrentSceneChangedEvent += OnSceneChanged;
        _profileService.CurrentDifficultyChangedEvent += OnDifficultyChanged;

        _messenger.Subscribe<TimerSettingsChangedMessage>(OnTimerSettingsChanged);

        // 注册语言变更事件
        LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
    }

    private void UnsubscribeFromServiceEvents()
    {
        if (_timerService == null || _profileService == null)
            return;

        _timerService.TimeUpdatedEvent -= OnTimeUpdated;
        _timerService.TimerRunningStateChangedEvent -= OnTimerRunningStateChanged;
        _timerService.TimerPauseStateChangedEvent -= OnTimerPauseStateChanged;
        _timerService.TimerResetEvent -= OnTimerReset;
        _timerService.RunCompletedEvent -= OnRunCompleted;

        _profileService.CurrentProfileChangedEvent -= OnProfileChanged;
        _profileService.CurrentSceneChangedEvent -= OnSceneChanged;
        _profileService.CurrentDifficultyChangedEvent -= OnDifficultyChanged;

        _messenger.Unsubscribe<TimerSettingsChangedMessage>(OnTimerSettingsChanged);
        LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
    }

    private void OnTimeUpdated(string timeString)
    {
        // 直接针对具体的 label 调用 SafeInvoke
        lblTimeDisplay?.SafeInvoke(() =>
        {
            lblTimeDisplay.Text = timeString;

            // 【新增】同时更新顶部的小时间（包含日期）
            if (labelTime1 != null)
            {
                // 格式：10:03 周日 11-30
                labelTime1.Text = DateTime.Now.ToString("HH:mm ddd MM-dd");
            }
        });
    }

    private void OnTimerRunningStateChanged(bool isRunning)
    {
        // 针对指示灯
        btnStatusIndicator?.SafeInvoke(() =>
        {
            btnStatusIndicator.BackColor = isRunning ? Color.Green : Color.Red;
        });

        // 针对整个控件的其他更新 (TimerStateChanged 事件和 UpdateStatistics)
        this.SafeInvoke(() =>
        {
            TimerStateChanged?.Invoke(this, EventArgs.Empty);
            UpdateStatistics();
        });
    }

    private void OnTimerPauseStateChanged(bool isPaused)
    {
        TimerStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnProfileChanged(Models.CharacterProfile? profile)
    {
        SaveShowLootSetting();
        LoadProfileHistoryData();
        UpdateCharacterSceneInfo();

        if (lootRecordsControl != null && profile != null && _profileService != null)
        {
            string currentScene = _profileService.CurrentScene ?? string.Empty;
            // 传递 profile 对象
            lootRecordsControl.UpdateLootRecords(profile, currentScene);
            InitializeLootRecordsVisibility();
        }
    }

    private void OnSceneChanged(string scene)
    {
        LoadProfileHistoryData();
        UpdateCharacterSceneInfo();
        // 场景切换也需要刷新 Loot 显示过滤
        if (lootRecordsControl != null && _profileService?.CurrentProfile != null)
        {
            lootRecordsControl.UpdateLootRecords(_profileService.CurrentProfile, scene);
        }
    }

    private void LoadProfileHistoryData()
    {
        // ... 原有日志代码 ...
        if (historyControl != null && _profileService != null)
        {
            var profile = _profileService.CurrentProfile;
            var scene = _profileService.CurrentScene;
            var characterName = profile?.Name ?? "";
            var difficulty = _profileService.CurrentDifficulty;
            LogManager.WriteDebugLog(
                "TimerControl",
                $"LoadProfileHistoryData: profile={profile?.Name}, scene={scene}, characterName={characterName}, difficulty={difficulty}"
            );
            // ... 原有加载历史逻辑 ...
            historyControl.LoadProfileHistoryData(profile, scene, characterName, difficulty);

            // 更新 Loot
            if (lootRecordsControl != null && profile != null)
            {
                string currentScene = _profileService.CurrentScene ?? string.Empty;
                lootRecordsControl.UpdateLootRecords(profile, currentScene);
            }
            ClearAllSelections();
        }
    }

    // ... OnTimerRunningStateChanged 等方法保持不变 ...

    // 【修改 4】 核心逻辑：添加记录完成后，强制焦点回到历史列表
    private void OnRunCompleted(TimeSpan runTime)
    {
        // 1. 数据层添加
        historyControl?.AddRunRecord(runTime);
        UpdateStatistics();

        // 2. 强制焦点控制 (直接用 this.SafeInvoke)
        this.SafeInvoke(SetFocusToNewHistoryRecord);
    }

    private void SetFocusToNewHistoryRecord()
    {
        // 确保掉落列表不被选中
        lootRecordsControl?.ClearSelection();

        // 强制选中历史列表最后一行
        historyControl?.SelectLastRow();
    }

    // 【修改 5】 路由删除逻辑
    public async Task<bool> DeleteSelectedRecordAsync()
    {
        // 逻辑：如果 Loot 控件拥有焦点，则删除 Loot；否则默认删除 History

        if (lootRecordsControl != null && lootRecordsControl.HasFocus)
        {
            return await lootRecordsControl.DeleteSelectedLootAsync();
        }

        if (historyControl != null)
        {
            bool result = await historyControl.DeleteSelectedRecordAsync();
            if (result)
            {
                UpdateStatistics();
            }
            return result;
        }
        return false;
    }

    // ... UpdateUI, UpdateStatistics 保持不变 ...

    // 【修改 6】 UpdateLootRecords 辅助方法更新
    private void UpdateLootRecords()
    {
        if (lootRecordsControl != null && _profileService != null && _profileService.CurrentProfile != null)
        {
            string currentScene = _profileService.CurrentScene ?? string.Empty;
            // 传递 Profile
            lootRecordsControl.UpdateLootRecords(_profileService.CurrentProfile, currentScene);
        }
    }

    private void OnDifficultyChanged(Models.GameDifficulty difficulty)
    {
        LoadProfileHistoryData();
        UpdateCharacterSceneInfo();
    }

    private void OnTimerReset()
    {
        this.SafeInvoke(() =>
        {
            if (lblTimeDisplay != null)
                lblTimeDisplay.Text = "00:00:00.0";
            UpdateStatistics();
        });
    }
    #endregion

    #region Public Methods
    public void TogglePause()
    {
        _timerService?.TogglePause();
    }

    public void HandleExternalReset()
    {
        _timerService?.Reset();
    }

    public void HandleTabSelected()
    {
        LoadProfileHistoryData();
        UpdateUI();
    }

    public void HandleApplicationClosing()
    {
        // 保存当前角色档案的ShowLoot设置
        SaveShowLootSetting();
        _timerService?.HandleApplicationClosing();
    }

    private void SaveShowLootSetting()
    {
        // 保存应用设置的TimerShowLootDrops设置
        if (lootRecordsControl != null)
        {
            _appSettings.TimerShowLootDrops = lootRecordsControl.Visible;
            _appSettings.Save();
        }
    }
    #endregion

    #region Private Methods
    private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (btnStatusIndicator != null && _timerService != null)
        {
            btnStatusIndicator.BackColor = _timerService.IsRunning ? Color.Green : Color.Red;
        }

        if (_timerService != null && !_timerService.IsRunning && !_timerService.IsPaused)
        {
            if (lblTimeDisplay != null)
            {
                lblTimeDisplay.Text = "00:00:00.0";
            }
        }

        // 根据设置更新番茄时间显示
        if (pomodoroTime != null)
        {
            pomodoroTime.Visible = _appSettings.TimerShowPomodoro;
        }

        UpdateStatistics();
        UpdateCharacterSceneInfo();
        UpdateLootRecords();
    }

    public void RefreshUI()
    {
        this.SafeInvoke(UpdateUI);
    }

    private void UpdateStatistics()
    {
        if (statisticsControl != null && historyControl != null)
        {
            int runCount = historyControl.RunCount;
            // 如果计时器正在运行，说明已经添加了一条未完成的记录，所以显示次数+1
            if (_timerService.IsRunning)
            {
                runCount++;
            }
            TimeSpan fastestTime = historyControl.FastestTime;
            var runHistory = historyControl.RunHistory;
            statisticsControl.UpdateStatistics(runCount, fastestTime, runHistory);
        }
        historyControl?.UpdateHistory(historyControl.RunHistory);
    }

    private void UpdateCharacterSceneInfo()
    {
        characterSceneControl?.UpdateCharacterSceneInfo();
    }
    #endregion

    /// <summary>
    /// 设置掉落记录控件的可见性，并调整相关UI元素
    /// </summary>
    /// <param name="isVisible">是否可见</param>
    public void SetLootRecordsVisible(bool isVisible)
    {
        if (lootRecordsControl == null)
            return;

        lootRecordsControl.Visible = isVisible;
        // 更新按钮文本
        toggleLootButton.Text = isVisible
            ? Utils.LanguageManager.GetString("HideLoot", "隐藏掉落")
            : Utils.LanguageManager.GetString("ShowLoot", "显示掉落");

        // 2. 动态调整 TableLayoutPanel 的行高
        // 索引：3=History, 5=Loot
        if (isVisible)
        {
            // 注意：因为窗体变高了，所以即使是 50% 也足够显示内容
            mainLayout.RowStyles[5] = new RowStyle(SizeType.Percent, 80F);
        }
        else
        {
            // 隐藏掉落时：历史占满剩余空间 (100%)，掉落高度强行设为 0
            mainLayout.RowStyles[5] = new RowStyle(SizeType.Absolute, 0F);
        }
    }

    private void ToggleLootButton_Click(object? sender, EventArgs e)
    {
        if (lootRecordsControl != null)
        {
            // 切换可见性
            bool isVisible = !lootRecordsControl.Visible;

            // 调用封装的方法
            SetLootRecordsVisible(isVisible);

            // 更新应用设置

            _appSettings.TimerShowLootDrops = isVisible;
            _appSettings.Save();
            _messenger.Publish(new TimerShowLootDropsChangedMessage());
            _messenger.Publish(new TimerSettingsChangedMessage(
                _appSettings.TimerShowPomodoro,
                isVisible, // 当前最新的 Loot 状态
                _appSettings.TimerSyncStartPomodoro,
                _appSettings.TimerSyncPausePomodoro,
                _appSettings.GenerateRoomName
            ));
        }
    }
}
