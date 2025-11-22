using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Timer {
    public partial class TimerControl : UserControl {
        // 服务层引用
        private readonly ITimerService? _timerService;
        private readonly IProfileService? _profileService;
        private readonly ITimerHistoryService? _historyService;

        // 组件引用 (这里去掉初始化，统一在 InitializeComponent 中处理)
        private StatisticsControl statisticsControl;
        private HistoryControl historyControl;
        private CharacterSceneControl characterSceneControl;
        private AntdUI.LabelTime labelTime1; // 如果这是第三方控件，请确保引用正确

        // 控件字段定义
        private Label btnStatusIndicator;
        private Label lblTimeDisplay;

        public TimerControl() {
            InitializeComponent();

            // 设置圆形指示器
            // 建议放在构造函数末尾，确保控件尺寸已初始化
            if (btnStatusIndicator != null) {
                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath()) {
                    path.AddEllipse(0, 0, btnStatusIndicator.Width, btnStatusIndicator.Height);
                    btnStatusIndicator.Region = new System.Drawing.Region(path);
                }
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

        // 2. 【关键修复】重写 OnLoad 方法
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            // 在这里加载数据，此时控件已经创建完成
            if (!DesignMode && _profileService != null) {
                // 加载历史数据
                LoadProfileHistoryData();
                // 更新界面状态
                UpdateUI();
            }
        }

        // 事件
        public event EventHandler? TimerStateChanged;

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
            LoadProfileHistoryData();
            UpdateCharacterSceneInfo();
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
            _timerService?.HandleApplicationClosing();
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
        #endregion

        #region UI Initialization
        private void InitializeComponent() {
            btnStatusIndicator = new Label();
            lblTimeDisplay = new Label();
            statisticsControl = new StatisticsControl();
            historyControl = new HistoryControl();
            characterSceneControl = new CharacterSceneControl();
            labelTime1 = new AntdUI.LabelTime();
            SuspendLayout();
            // 
            // btnStatusIndicator
            // 
            btnStatusIndicator.Location = new Point(20, 19);
            btnStatusIndicator.Margin = new Padding(6);
            btnStatusIndicator.Name = "btnStatusIndicator";
            btnStatusIndicator.Size = new Size(24, 24);
            btnStatusIndicator.TabIndex = 0;
            btnStatusIndicator.Click += btnStatusIndicator_Click;
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
            statisticsControl.Size = new Size(605, 116);
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
            characterSceneControl.Size = new Size(605, 80);
            characterSceneControl.TabIndex = 4;
            // 
            // labelTime1
            // 
            labelTime1.Location = new Point(65, 9);
            labelTime1.Name = "labelTime1";
            labelTime1.ShowTime = false;
            labelTime1.Size = new Size(135, 40);
            labelTime1.TabIndex = 5;
            labelTime1.Text = "labelTime1";
            // 
            // TimerControl
            // 
            AutoScaleDimensions = new SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(labelTime1);
            Controls.Add(characterSceneControl);
            Controls.Add(historyControl);
            Controls.Add(statisticsControl);
            Controls.Add(lblTimeDisplay);
            Controls.Add(btnStatusIndicator);
            Margin = new Padding(6);
            Name = "TimerControl";
            Size = new Size(667, 627);
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

        private void btnStatusIndicator_Click(object sender, EventArgs e) {

        }
    }
}