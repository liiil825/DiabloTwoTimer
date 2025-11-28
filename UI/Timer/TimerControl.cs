using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.UI.Pomodoro;
using DiabloTwoMFTimer.UI.Settings;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Timer
{
    public partial class TimerControl : UserControl
    {
        // 服务层引用
        private readonly ITimerService _timerService = null!;
        private readonly IProfileService _profileService = null!;
        private readonly ITimerHistoryService _historyService = null!;
        private readonly IPomodoroTimerService _pomodoroTimerService = null!;

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
            IPomodoroTimerService pomodoroTimerService
        )
            : this()
        {
            _timerService = timerService;
            _profileService = profileService;
            _historyService = historyService;
            _pomodoroTimerService = pomodoroTimerService;

            // 初始化子控件的服务引用
            characterSceneControl?.Initialize(_profileService);
            historyControl?.Initialize(_historyService);

            // 订阅服务事件
            SubscribeToServiceEvents();

            // 注册语言变更事件
            LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
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
            var settings = Services.SettingsManager.LoadSettings();
            pomodoroTime.Visible = settings.TimerShowPomodoro;
        }

        private void InitializeLootRecordsVisibility()
        {
            if (lootRecordsControl != null)
            {
                // 根据应用设置的TimerShowLootDrops设置初始化掉落记录控件的可见性
                var settings = Services.SettingsManager.LoadSettings();
                lootRecordsControl.Visible = settings.TimerShowLootDrops;
                bool isVisible = lootRecordsControl.Visible;

                // 更新按钮文本
                toggleLootButton.Text = isVisible
                    ? Utils.LanguageManager.GetString("HideLoot", "隐藏掉落")
                    : Utils.LanguageManager.GetString("ShowLoot", "显示掉落");

                // 设置初始高度
                this.Size = new Size(
                    this.Width,
                    isVisible
                        ? UISizeConstants.TimerControlHeightWithLoot
                        : UISizeConstants.TimerControlHeightWithoutLoot
                );
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
        }

        private void OnTimeUpdated(string timeString)
        {
            if (_timerService == null)
                return;

            if (lblTimeDisplay != null && lblTimeDisplay.InvokeRequired)
            {
                lblTimeDisplay.Invoke(new Action<string>(OnTimeUpdated), timeString);
            }
            else if (lblTimeDisplay != null)
            {
                lblTimeDisplay.Text = timeString;
            }
        }

        private void OnTimerRunningStateChanged(bool isRunning)
        {
            if (btnStatusIndicator != null && btnStatusIndicator.InvokeRequired)
            {
                btnStatusIndicator.Invoke(new Action<bool>(OnTimerRunningStateChanged), isRunning);
            }
            else if (btnStatusIndicator != null)
            {
                btnStatusIndicator.BackColor = isRunning ? Color.Green : Color.Red;
            }

            // 番茄时钟同步启动逻辑现在在PomodoroTimerService中处理

            TimerStateChanged?.Invoke(this, EventArgs.Empty);
            UpdateStatistics();
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
            // 1. 数据层添加（这会触发 HistoryControl 的 Grid 刷新，但现在 Grid 刷新不会清除选中了）
            historyControl?.AddRunRecord(runTime);
            UpdateStatistics();

            // 2. 强制焦点控制
            // 使用 Invoke 确保在 UI 刷新完成后执行
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(SetFocusToNewHistoryRecord));
            }
            else
            {
                SetFocusToNewHistoryRecord();
            }
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
            if (lblTimeDisplay != null && lblTimeDisplay.InvokeRequired)
            {
                lblTimeDisplay.Invoke(new Action(OnTimerReset));
            }
            else if (lblTimeDisplay != null)
            {
                lblTimeDisplay.Text = "00:00:00.0";
            }
            UpdateStatistics();
        }
        #endregion

        #region Public Methods
        public void ToggleTimer()
        {
            _timerService?.StartOrNextRun();
        }

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
                var settings = Services.SettingsManager.LoadSettings();
                settings.TimerShowLootDrops = lootRecordsControl.Visible;
                Services.SettingsManager.SaveSettings(settings);
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
                var settings = Services.SettingsManager.LoadSettings();
                pomodoroTime.Visible = settings.TimerShowPomodoro;
            }

            UpdateStatistics();
            UpdateCharacterSceneInfo();
            UpdateLootRecords();
        }

        public void RefreshUI()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(UpdateUI));
            }
            else
            {
                UpdateUI();
            }
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
            if (lootRecordsControl != null)
            {
                // 设置可见性
                lootRecordsControl.Visible = isVisible;

                // 更新按钮文本
                toggleLootButton.Text = isVisible
                    ? Utils.LanguageManager.GetString("HideLoot", "隐藏掉落")
                    : Utils.LanguageManager.GetString("ShowLoot", "显示掉落");

                // 调整TimerControl的高度
                int newHeight = isVisible
                    ? UISizeConstants.TimerControlHeightWithLoot
                    : UISizeConstants.TimerControlHeightWithoutLoot;
                int heightChange = newHeight - this.Height;
                this.Size = new Size(this.Width, newHeight);

                // 调整父窗体（MainForm）的高度
                if (this.ParentForm != null)
                {
                    int newFormHeight = this.ParentForm.ClientSize.Height + heightChange;
                    this.ParentForm.ClientSize = new Size(this.ParentForm.ClientSize.Width, newFormHeight);

                    // 如果窗口位置设置为下方，重新应用窗口位置以保持在底部
                    var settings = Services.SettingsManager.LoadSettings();
                    var windowPosition = SettingsControl.WindowPosition.BottomLeft; // 默认位置
                    if (!string.IsNullOrEmpty(settings.WindowPosition))
                    {
                        windowPosition = Services.SettingsManager.StringToWindowPosition(settings.WindowPosition);
                    }

                    if (
                        windowPosition == SettingsControl.WindowPosition.BottomLeft
                        || windowPosition == SettingsControl.WindowPosition.BottomCenter
                        || windowPosition == SettingsControl.WindowPosition.BottomRight
                    )
                    {
                        SettingsControl.MoveWindowToPosition(this.ParentForm, windowPosition);
                    }
                }
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
                var settings = Services.SettingsManager.LoadSettings();

                settings.TimerShowLootDrops = isVisible;
                Services.SettingsManager.SaveSettings(settings);
            }
        }
    }
}