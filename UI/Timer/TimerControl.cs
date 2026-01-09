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
            InitializeLootRecordsVisibility();
            InitializePomodoroVisibility();

            // 【修改】不在这里直接滚动，而是触发一次 Layout 后再滚
            // 强制重新布局
            this.PerformLayout();

            this.BeginInvoke(
                new Action(() =>
                {
                    // 此时控件应该已经有尺寸了
                    ScrollToBottom();
                    ClearAllSelections();
                })
            );
            UpdateAllModulesVisibility();

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
            // SetLootRecordsVisible(_appSettings.TimerShowLootDrops);
            UpdateAllModulesVisibility();
            UpdateUI();
            _messenger.Publish(new AdjustWindowHeightMessage());
        });
    }

    private void UpdateAllModulesVisibility()
    {
        if (tlpMain == null) return;

        bool showTime = _appSettings.TimerShowTimerTime;
        bool showStats = _appSettings.TimerShowStatistics;
        bool showHistory = _appSettings.TimerShowHistory;
        bool showLoot = _appSettings.TimerShowLootDrops;
        bool showAccount = _appSettings.TimerShowAccountInfo;
        bool showPomodoro = _appSettings.TimerShowPomodoro;

        if (lblTimeDisplay != null) lblTimeDisplay.Visible = showTime;
        if (pomodoroTime != null) pomodoroTime.Visible = showPomodoro;

        void SetRowVisible(int rowIndex, bool visible, SizeType sizeType = SizeType.AutoSize, float height = 0)
        {
            if (rowIndex >= tlpMain.RowCount) return;
            if (visible)
            {
                if (sizeType == SizeType.Percent)
                    tlpMain.RowStyles[rowIndex] = new RowStyle(SizeType.Percent, height > 0 ? height : 100F);
                else
                    tlpMain.RowStyles[rowIndex] = new RowStyle(SizeType.AutoSize);
                var ctrl = tlpMain.GetControlFromPosition(0, rowIndex);
                if (ctrl != null) ctrl.Visible = true;
            }
            else
            {
                tlpMain.RowStyles[rowIndex] = new RowStyle(SizeType.Absolute, 0F);
                var ctrl = tlpMain.GetControlFromPosition(0, rowIndex);
                if (ctrl != null) ctrl.Visible = false;
            }
        }

        SetRowVisible(2, showStats, SizeType.AutoSize);

        if (showHistory && showLoot)
        {
            SetRowVisible(3, true, SizeType.Percent, 40F);
            SetRowVisible(4, true, SizeType.Percent, 60F);
        }
        else if (showHistory)
        {
            SetRowVisible(3, true, SizeType.Percent, 100F);
            SetRowVisible(4, false);
        }
        else if (showLoot)
        {
            SetRowVisible(3, false);
            SetRowVisible(4, true, SizeType.Percent, 100F);
        }
        else
        {
            SetRowVisible(3, false);
            SetRowVisible(4, false);
        }

        SetRowVisible(5, showAccount, SizeType.AutoSize);

        if (toggleLootButton != null)
        {
            toggleLootButton.Text = showLoot ? "\uE70E" : "\uE70D";
        }
    }

    // 处理切换掉落记录可见性消息
    private void OnToggleLootVisibility(ToggleLootVisibilityMessage message)
    {
        // Messenger 可能在任意线程回调，必须使用 SafeInvoke
        this.SafeInvoke(() =>
        {
            // 调用现有的 ToggleLootButton_Click 逻辑
            ToggleLootButton_Click(null, EventArgs.Empty);
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
        _messenger.Subscribe<ToggleLootVisibilityMessage>(OnToggleLootVisibility);

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
        _messenger.Unsubscribe<ToggleLootVisibilityMessage>(OnToggleLootVisibility);
        LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
    }

    private void OnTimeUpdated(string timeString)
    {
        // 直接针对具体的 label 调用 SafeInvoke
        lblTimeDisplay?.SafeInvoke(() =>
        {
            if (lblTimeDisplay.Text != timeString)
            {
                lblTimeDisplay.Text = timeString;
            }

            // 同时更新顶部的小时间（包含日期）
            if (labelTime1 != null)
            {
                // 获取当前时间字符串
                string nowText = DateTime.Now.ToString("HH:mm ddd MM-dd");
                // 【优化】同样检查是否变化
                if (labelTime1.Text != nowText)
                {
                    labelTime1.Text = nowText;
                }
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
        btnStatusIndicator?.SafeInvoke(() =>
        {
            btnStatusIndicator.BackColor = !isPaused ? Color.Green : Color.Red;
        });
        TimerStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnProfileChanged(Models.CharacterProfile? profile)
    {
        SaveShowLootSetting();
        LoadProfileHistoryData();
        UpdateCharacterSceneInfo();
        UpdateLootRecords();
        InitializeLootRecordsVisibility();
    }

    private void OnSceneChanged(string scene)
    {
        HandleContextChanged();
    }

    private void LoadProfileHistoryData()
    {
        // ... 原有日志代码 ...
        if (historyControl != null && _profileService != null)
        {
            var profile = _profileService.CurrentProfile;
            var scene = _profileService.CurrentScene;
            var difficulty = _profileService.CurrentDifficulty;
            LogManager.WriteDebugLog(
                "TimerControl",
                $"LoadProfileHistoryData: profile={profile?.Name}, scene={scene}, difficulty={difficulty}"
            );
            // ... 原有加载历史逻辑 ...
            historyControl.LoadProfileHistoryData(profile, scene, difficulty);
            ClearAllSelections();
            historyControl?.ScrollToBottom();
        }
    }

    // 核心逻辑：添加记录完成后，强制焦点回到历史列表
    private void OnRunCompleted(TimeSpan runTime)
    {
        // 1. 数据层添加
        _historyService?.AddRunRecord(runTime); // 添加历史记录
        UpdateStatistics();

        // 强制焦点控制
        this.SafeInvoke(SetFocusToNewHistoryRecord);
    }

    private void SetFocusToNewHistoryRecord()
    {
        // 修改 3a: 完成计时时，选中历史记录，同时清除掉落列表选中 (互斥)
        lootRecordsControl?.ClearSelection();
        historyControl?.SelectLastRow();
    }

    // 新增处理掉落添加的方法 (供 MainForm 调用)
    public void HandleLootAdded()
    {
        this.SafeInvoke(() =>
        {
            // 1. 刷新掉落数据 (会重新排序)
            UpdateLootRecords();

            // 2. 选中掉落列表的最后一项 (新添加的)
            lootRecordsControl?.SelectLastRow();

            // 3. 清除历史记录的选中状态 (互斥)
            historyControl?.ClearSelection();
        });
    }

    // 路由删除逻辑
    public async Task<bool> DeleteSelectedRecordAsync()
    {
        // 逻辑：如果 Loot 控件有选中记录，则删除 Loot；否则默认删除 History
        if (lootRecordsControl != null && (lootRecordsControl.HasFocus || lootRecordsControl.HasSelectedRecords))
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

    // 删除最后一个时间记录
    public async Task<bool> DeleteLastHistoryRecordAsync()
    {
        if (historyControl != null && historyControl.RunHistory.Count > 0)
        {
            // 选择最后一行
            historyControl.SelectLastRow();
            // 删除选中的记录
            bool result = await historyControl.DeleteSelectedRecordAsync();
            if (result)
            {
                UpdateStatistics();
            }
            return result;
        }
        return false;
    }

    // 删除最后一个掉落记录
    public async Task<bool> DeleteLastLootRecordAsync()
    {
        if (lootRecordsControl != null)
        {
            // 选择最后一行
            lootRecordsControl.SelectLastRow();
            // 删除选中的记录
            bool result = await lootRecordsControl.DeleteSelectedLootAsync();
            if (result)
            {
                UpdateStatistics();
            }
            return result;
        }
        return false;
    }

    // UpdateLootRecords 辅助方法更新
    private void UpdateLootRecords()
    {
        if (lootRecordsControl != null && _profileService != null && _profileService.CurrentProfile != null)
        {
            string currentScene = _profileService.CurrentScene ?? string.Empty;
            lootRecordsControl.UpdateLootRecords(_profileService.CurrentProfile, currentScene);
            lootRecordsControl.ScrollToBottom();
        }
    }

    private void OnDifficultyChanged(Models.GameDifficulty difficulty)
    {
        HandleContextChanged();
    }

    /// <summary>
    /// 处理场景或难度变化的统一方法
    /// </summary>
    private void HandleContextChanged()
    {
        LoadProfileHistoryData();
        UpdateCharacterSceneInfo();
        UpdateStatistics();
        UpdateLootRecords();
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
        // 使用BeginInvoke确保在UI更新完成后执行滚动
        this.BeginInvoke(
            new Action(() =>
            {
                ScrollToBottom();
            })
        );
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
            if (!_timerService.IsStopped)
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
        // 更新按钮图标
        // 使用 Segoe MDL2 Assets 字体图标
        toggleLootButton.Text = isVisible ? "\uE70E" : "\uE70D";

        // 2. 动态调整 TableLayoutPanel 的行高
        // 索引说明：
        // 0: Top
        // 1: Time
        // 2: Stats
        // 3: History (Percent)
        // 4: Loot (Percent) - 【修改点：现在操作索引4】
        // 5: Bottom Info (Fixed)
        if (isVisible)
        {
            tlpMain.RowStyles[4] = new RowStyle(SizeType.Percent, 80F);
        }
        else
        {
            // 隐藏掉落时：历史占满剩余空间 (100%)，掉落高度强行设为 0
            tlpMain.RowStyles[4] = new RowStyle(SizeType.Absolute, 0F);
        }
        // --- 新增滚动逻辑 ---
        // 使用 BeginInvoke 是关键：它会把这个操作排入 UI 线程的消息队列末尾
        // 确保在 TableLayoutPanel 完成布局调整（重新计算高度）之后再执行滚动
        // 否则，滚动计算时可能会用到旧的高度，导致滚不到底
        this.BeginInvoke(
            new Action(() =>
            {
                ScrollToBottom();
            })
        );
    }

    private void ScrollToBottom()
    {
        historyControl?.ScrollToBottom();
        if (lootRecordsControl?.Visible == true)
        {
            lootRecordsControl?.ScrollToBottom();
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
            historyControl?.SelectLastRow();
            // historyControl?.ClearSelection();
            _appSettings.TimerShowLootDrops = isVisible;
            _appSettings.Save();
            _messenger.Publish(new TimerShowLootDropsChangedMessage());
            _messenger.Publish(new TimerSettingsChangedMessage());
        }
    }
}
