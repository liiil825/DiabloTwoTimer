using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.UI;
using DTwoMFTimerHelper.UI.Settings;
using DTwoMFTimerHelper.UI.Pomodoro;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Timer {
    public partial class TimerControl : UserControl {
        // 服务层引用
        private readonly ITimerService? _timerService;
        private readonly IProfileService? _profileService;
        private readonly ITimerHistoryService? _historyService;
        private readonly PomodoroTimerService? _pomodoroTimerService;

        // 组件引用 (这里去掉初始化，统一在 InitializeComponent 中处理)
        private StatisticsControl? statisticsControl;
        private HistoryControl? historyControl;
        private CharacterSceneControl? characterSceneControl;
        private LootRecordsControl? lootRecordsControl;

        // private PomodoroDisplayControl pomodoroDisplayControl;
        private AntdUI.LabelTime labelTime1 = null!; // 如果这是第三方控件，请确保引用正确

        // 控件字段定义
        private Label btnStatusIndicator = null!;
        private Button toggleLootButton = null!;
        private PomodoroTimeDisplayLabel pomodoroTime = null!;
        private Label lblTimeDisplay = null!;

        public TimerControl() {
            InitializeComponent();

            // 设置圆形指示器
            // 建议放在构造函数末尾，确保控件尺寸已初始化
            if (btnStatusIndicator != null) {
                using System.Drawing.Drawing2D.GraphicsPath path = new();
                path.AddEllipse(0, 0, btnStatusIndicator.Width, btnStatusIndicator.Height);
                btnStatusIndicator.Region = new System.Drawing.Region(path);
            }
        }

        public TimerControl(IProfileService profileService, ITimerService timerService, ITimerHistoryService historyService) : this() {
            _timerService = timerService;
            _profileService = profileService;
            _historyService = historyService;

            // 初始化子控件的服务引用
            characterSceneControl?.Initialize(_profileService);
            historyControl?.Initialize(_historyService);

            // 订阅服务事件
            SubscribeToServiceEvents();

            // 注册语言变更事件
            LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
        }

        /// <summary>
        /// 重载构造函数，用于注入番茄计时器服务
        /// </summary>
        /// <param name="profileService"></param>
        /// <param name="timerService"></param>
        /// <param name="historyService"></param>
        /// <param name="pomodoroTimerService"></param>
        public TimerControl(IProfileService profileService, ITimerService timerService, ITimerHistoryService historyService, PomodoroTimerService pomodoroTimerService) : this(profileService, timerService, historyService) {
            _pomodoroTimerService = pomodoroTimerService;
        }

        // 2. 【关键修复】重写 OnLoad 方法
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            // 在这里加载数据，此时控件已经创建完成
            if (!DesignMode && _profileService != null) {
                // 加载历史数据
                LoadProfileHistoryData();

                // 根据角色档案的ShowLoot设置初始化掉落记录控件的可见性
                InitializeLootRecordsVisibility();
                // 更新界面状态
                UpdateUI();
            }

            // 确保番茄计时器显示正确的值（即使设置是在绑定后加载的）
            if (_pomodoroTimerService != null && pomodoroTime != null) {
                // 强制更新显示，确保显示设置的值而不是默认值
                // 这里我们通过重新绑定服务来触发更新
                pomodoroTime.BindService(_pomodoroTimerService);
            }
        }

        private void InitializeLootRecordsVisibility() {
            if (lootRecordsControl != null) {
                // 根据应用设置的ShowLoot设置初始化掉落记录控件的可见性
                var settings = Services.SettingsManager.LoadSettings();
                lootRecordsControl.Visible = settings.TimerShowLootDrops;
                bool isVisible = lootRecordsControl.Visible;

                // 更新按钮文本
                toggleLootButton.Text = isVisible ? Utils.LanguageManager.GetString("HideLoot", "隐藏掉落") : Utils.LanguageManager.GetString("ShowLoot", "显示掉落");

                // 设置初始高度
                this.Size = new Size(this.Width, isVisible ? UISizeConstants.TimerControlHeightWithLoot : UISizeConstants.TimerControlHeightWithoutLoot);
            }
        }

        // 事件
        public event EventHandler? TimerStateChanged;

        /// <summary>
        /// 获取番茄时间显示控件实例
        /// </summary>
        // public PomodoroDisplayControl? PomodoroDisplay => pomodoroDisplayControl;

        public bool IsTimerRunning => _timerService?.IsRunning ?? false;

        #region Service Event Handlers
        private void SubscribeToServiceEvents() {
            if (_timerService == null || _profileService == null) return;
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

        private void UnsubscribeFromServiceEvents() {
            if (_timerService == null || _profileService == null) return;

            _timerService.TimeUpdatedEvent -= OnTimeUpdated;
            _timerService.TimerRunningStateChangedEvent -= OnTimerRunningStateChanged;
            _timerService.TimerPauseStateChangedEvent -= OnTimerPauseStateChanged;
            _timerService.TimerResetEvent -= OnTimerReset;
            _timerService.RunCompletedEvent -= OnRunCompleted;

            _profileService.CurrentProfileChangedEvent -= OnProfileChanged;
            _profileService.CurrentSceneChangedEvent -= OnSceneChanged;
            _profileService.CurrentDifficultyChangedEvent -= OnDifficultyChanged;
        }

        private void OnTimeUpdated(string timeString) {
            if (_timerService == null) return;

            if (lblTimeDisplay != null && lblTimeDisplay.InvokeRequired) {
                lblTimeDisplay.Invoke(new Action<string>(OnTimeUpdated), timeString);
            }
            else if (lblTimeDisplay != null) {
                lblTimeDisplay.Text = timeString;
            }
        }

        private void OnTimerRunningStateChanged(bool isRunning) {
            if (btnStatusIndicator != null && btnStatusIndicator.InvokeRequired) {
                btnStatusIndicator.Invoke(new Action<bool>(OnTimerRunningStateChanged), isRunning);
            }
            else if (btnStatusIndicator != null) {
                btnStatusIndicator.BackColor = isRunning ? Color.Green : Color.Red;
            }

            TimerStateChanged?.Invoke(this, EventArgs.Empty);
            UpdateStatistics();
        }

        private void OnTimerPauseStateChanged(bool isPaused) {
            TimerStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnProfileChanged(Models.CharacterProfile? profile) {
            // 在切换到新角色前，先保存当前角色的ShowLoot设置
            SaveShowLootSetting();

            LoadProfileHistoryData();
            UpdateCharacterSceneInfo();

            // 更新掉落记录
            if (lootRecordsControl != null && profile != null) {
                lootRecordsControl.UpdateLootRecords(profile.LootRecords);
                // 重新初始化控件可见性（现在基于应用设置而非角色档案）
                InitializeLootRecordsVisibility();
            }
        }

        private void OnSceneChanged(string scene) {
            LoadProfileHistoryData();
            UpdateCharacterSceneInfo();
        }

        private void OnDifficultyChanged(Models.GameDifficulty difficulty) {
            LoadProfileHistoryData();
            UpdateCharacterSceneInfo();
        }

        // 【关键修复】确保加载后刷新列表
        private void LoadProfileHistoryData() {
            LogManager.WriteDebugLog("TimerControl", "LoadProfileHistoryData");
            if (historyControl != null && _profileService != null) {
                var profile = _profileService.CurrentProfile;
                var scene = _profileService.CurrentScene;
                var characterName = profile?.Name ?? "";
                var difficulty = _profileService.CurrentDifficulty;
                LogManager.WriteDebugLog("TimerControl", $"LoadProfileHistoryData: profile={profile?.Name}, scene={scene}, characterName={characterName}, difficulty={difficulty}");

                // 1. 尝试加载数据
                historyControl.LoadProfileHistoryData(profile, scene, characterName, difficulty);

                // 2. 更新掉落记录
                if (lootRecordsControl != null && profile != null) {
                    lootRecordsControl.UpdateLootRecords(profile.LootRecords);
                }
            }
        }

        private void OnTimerReset() {
            if (lblTimeDisplay != null && lblTimeDisplay.InvokeRequired) {
                lblTimeDisplay.Invoke(new Action(OnTimerReset));
            }
            else if (lblTimeDisplay != null) {
                lblTimeDisplay.Text = "00:00:00.0";
            }
            UpdateStatistics();
        }

        private void OnRunCompleted(TimeSpan runTime) {
            historyControl?.AddRunRecord(runTime);
            UpdateStatistics();
        }
        #endregion

        #region Public Methods
        public void ToggleTimer() {
            _timerService?.StartOrNextRun();
        }

        public void TogglePause() {
            _timerService?.TogglePause();
        }

        public void HandleExternalReset() {
            _timerService?.Reset();
        }

        public void HandleTabSelected() {
            LoadProfileHistoryData();
            UpdateUI();
        }

        public void HandleApplicationClosing() {
            // 保存当前角色档案的ShowLoot设置
            SaveShowLootSetting();
            _timerService?.HandleApplicationClosing();
        }

        private void SaveShowLootSetting() {
            // 保存应用设置的TimerShowLootDrops设置
            if (lootRecordsControl != null) {
                var settings = Services.SettingsManager.LoadSettings();
                settings.TimerShowLootDrops = lootRecordsControl.Visible;
                Services.SettingsManager.SaveSettings(settings);
            }
        }

        public async Task<bool> DeleteSelectedRecordAsync() {
            if (historyControl != null) {
                return await historyControl.DeleteSelectedRecordAsync();
            }
            return false;
        }
        #endregion

        #region Private Methods
        private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e) {
            UpdateUI();
        }

        private void UpdateUI() {
            if (btnStatusIndicator != null && _timerService != null) {
                btnStatusIndicator.BackColor = _timerService.IsRunning ? Color.Green : Color.Red;
            }

            if (_timerService != null && !_timerService.IsRunning && !_timerService.IsPaused) {
                if (lblTimeDisplay != null) {
                    lblTimeDisplay.Text = "00:00:00.0";
                }
            }

            UpdateStatistics();
            UpdateCharacterSceneInfo();
            UpdateLootRecords();
        }

        public void RefreshUI() {
            if (this.InvokeRequired) {
                this.Invoke(new Action(UpdateUI));
            }
            else {
                UpdateUI();
            }
        }

        private void UpdateStatistics() {
            if (statisticsControl != null && historyControl != null) {
                int runCount = historyControl.RunCount;
                TimeSpan fastestTime = historyControl.FastestTime;
                var runHistory = historyControl.RunHistory;
                statisticsControl.UpdateStatistics(runCount, fastestTime, runHistory);
            }
            historyControl?.UpdateHistory(historyControl.RunHistory);
        }

        private void UpdateCharacterSceneInfo() {
            characterSceneControl?.UpdateCharacterSceneInfo();
        }

        private void UpdateLootRecords() {
            if (lootRecordsControl != null && _profileService != null && _profileService.CurrentProfile != null) {
                lootRecordsControl.UpdateLootRecords(_profileService.CurrentProfile.LootRecords);
            }
        }
        #endregion

        #region UI Initialization
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TimerControl));
            btnStatusIndicator = new Label();
            lblTimeDisplay = new Label();
            statisticsControl = new StatisticsControl();
            historyControl = new HistoryControl();
            characterSceneControl = new CharacterSceneControl();
            lootRecordsControl = new LootRecordsControl();
            labelTime1 = new AntdUI.LabelTime();
            toggleLootButton = new Button();
            pomodoroTime = new PomodoroTimeDisplayLabel();
            SuspendLayout();
            // 
            // btnStatusIndicator
            // 
            btnStatusIndicator.Location = new Point(15, 19);
            btnStatusIndicator.Margin = new Padding(6);
            btnStatusIndicator.Name = "btnStatusIndicator";
            btnStatusIndicator.Size = new Size(24, 24);
            btnStatusIndicator.TabIndex = 0;
            // 
            // lblTimeDisplay
            // 
            lblTimeDisplay.AutoSize = true;
            lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 28F, FontStyle.Regular, GraphicsUnit.Point, 134);
            lblTimeDisplay.Location = new Point(20, 61);
            lblTimeDisplay.Margin = new Padding(6, 0, 6, 0);
            lblTimeDisplay.Name = "lblTimeDisplay";
            lblTimeDisplay.Size = new Size(303, 86);
            lblTimeDisplay.TabIndex = 1;
            lblTimeDisplay.Text = "00:00:00";
            lblTimeDisplay.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // statisticsControl
            // 
            statisticsControl.AverageTime = TimeSpan.Parse("00:00:00");
            statisticsControl.FastestTime = TimeSpan.Parse("00:00:00");
            statisticsControl.Location = new Point(9, 157);
            statisticsControl.Margin = new Padding(9, 8, 9, 8);
            statisticsControl.Name = "statisticsControl";
            statisticsControl.RunCount = 0;
            statisticsControl.Size = new Size(421, 116);
            statisticsControl.TabIndex = 2;
            // 
            // historyControl
            // 
            historyControl.Location = new Point(9, 289);
            historyControl.Margin = new Padding(9, 8, 9, 8);
            historyControl.Name = "historyControl";
            historyControl.Size = new Size(421, 117);
            historyControl.TabIndex = 3;
            // 
            // characterSceneControl
            // 
            characterSceneControl.Location = new Point(9, 409);
            characterSceneControl.Margin = new Padding(6);
            characterSceneControl.Name = "characterSceneControl";
            characterSceneControl.Size = new Size(421, 80);
            characterSceneControl.TabIndex = 4;
            // 
            // lootRecordsControl
            // 
            lootRecordsControl.Location = new Point(9, 495);
            lootRecordsControl.Margin = new Padding(9, 8, 9, 8);
            lootRecordsControl.Name = "lootRecordsControl";
            lootRecordsControl.Size = new Size(421, 100);
            lootRecordsControl.TabIndex = 6;
            // 
            // labelTime1
            // 
            labelTime1.Location = new Point(60, 9);
            labelTime1.Name = "labelTime1";
            labelTime1.ShowTime = false;
            labelTime1.Size = new Size(135, 40);
            labelTime1.TabIndex = 5;
            labelTime1.Text = "labelTime1";
            // 
            // toggleLootButton
            // 
            toggleLootButton.Location = new Point(299, 414);
            toggleLootButton.Name = "toggleLootButton";
            toggleLootButton.Size = new Size(131, 40);
            toggleLootButton.TabIndex = 7;
            toggleLootButton.Text = "ShowLoot";
            toggleLootButton.UseVisualStyleBackColor = true;
            toggleLootButton.Click += ToggleLootButton_Click;
            // 
            // pomodoroTime
            // 
            pomodoroTime.AutoSize = true;
            pomodoroTime.Font = new Font("微软雅黑", 16F, FontStyle.Bold);
            pomodoroTime.Location = new Point(299, 9);
            pomodoroTime.Name = "pomodoroTime";
            pomodoroTime.ShowMilliseconds = false;
            pomodoroTime.Size = new Size(125, 50);
            pomodoroTime.TabIndex = 8;
            pomodoroTime.Text = "00:00";
            // 
            // TimerControl
            // 
            AutoScaleDimensions = new SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pomodoroTime);
            Controls.Add(toggleLootButton);
            Controls.Add(lootRecordsControl);
            Controls.Add(labelTime1);
            Controls.Add(characterSceneControl);
            Controls.Add(historyControl);
            Controls.Add(statisticsControl);
            Controls.Add(lblTimeDisplay);
            Controls.Add(btnStatusIndicator);
            Margin = new Padding(6);
            Name = "TimerControl";
            Size = new Size(667, 850);
            ResumeLayout(false);
            PerformLayout();
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                UnsubscribeFromServiceEvents();
                LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
            }
            base.Dispose(disposing);
        }
        #endregion

        private void ToggleLootButton_Click(object? sender, EventArgs e) {
            if (lootRecordsControl != null) {
                // 确保状态正确切换
                lootRecordsControl.Visible = !lootRecordsControl.Visible;
                bool isVisible = lootRecordsControl.Visible;

                // 加载应用设置
                var settings = Services.SettingsManager.LoadSettings();

                // 更新按钮文本
                toggleLootButton.Text = isVisible ? Utils.LanguageManager.GetString("HideLoot", "隐藏掉落") : Utils.LanguageManager.GetString("ShowLoot", "显示掉落");

                // 调整TimerControl的高度
                int newHeight = isVisible ? UISizeConstants.TimerControlHeightWithLoot : UISizeConstants.TimerControlHeightWithoutLoot;
                int heightChange = newHeight - this.Height;
                this.Size = new Size(this.Width, newHeight);

                // 调整父窗体（MainForm）的高度
                if (this.ParentForm != null) {
                    int newFormHeight = this.ParentForm.ClientSize.Height + heightChange;
                    this.ParentForm.ClientSize = new Size(this.ParentForm.ClientSize.Width, newFormHeight);

                    // 如果窗口位置设置为下方，重新应用窗口位置以保持在底部
                    var windowPosition = SettingsControl.WindowPosition.BottomLeft; // 默认位置
                    if (!string.IsNullOrEmpty(settings.WindowPosition)) {
                        windowPosition = Services.SettingsManager.StringToWindowPosition(settings.WindowPosition);
                    }

                    if (windowPosition == SettingsControl.WindowPosition.BottomLeft ||
                        windowPosition == SettingsControl.WindowPosition.BottomCenter ||
                        windowPosition == SettingsControl.WindowPosition.BottomRight) {
                        SettingsControl.MoveWindowToPosition(this.ParentForm, windowPosition);
                    }
                }

                // 更新应用设置的TimerShowLootDrops设置
                settings.TimerShowLootDrops = isVisible;
                Services.SettingsManager.SaveSettings(settings);
            }
        }
    }
}