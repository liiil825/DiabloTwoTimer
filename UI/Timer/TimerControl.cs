using System;
using System.Drawing;
using System.Windows.Forms;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Timer
{
    public partial class TimerControl : UserControl
    {
        // 服务层引用
        private readonly TimerService _timerService;
        
        // 组件引用
        private StatisticsControl? statisticsControl;
        private HistoryControl? historyControl;
        private CharacterSceneControl? characterSceneControl;

        // 控件字段定义
        private Button? btnStatusIndicator;
        private Label? lblTimeDisplay;

        // 事件
        public event EventHandler? TimerStateChanged;

        public TimerControl()
        {
            InitializeComponent();
            
            // 获取TimerService单例实例
            _timerService = TimerService.Instance;
            
            // 订阅服务事件
            SubscribeToServiceEvents();
            
            // 注册语言变更事件
            LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
            
            UpdateUI();
        }

        #region Properties
        public bool IsTimerRunning => _timerService.IsRunning;
        public Models.CharacterProfile? CurrentProfile 
        { 
            get => _timerService.CurrentProfile;
            set => _timerService.CurrentProfile = value; // value可以为null，因为_timerService.CurrentProfile现在是可空类型
        }
        #endregion

        #region Service Event Handlers
        private void SubscribeToServiceEvents()
        {
            _timerService.TimeUpdated += OnTimeUpdated;
            _timerService.TimerRunningStateChanged += OnTimerRunningStateChanged;
            _timerService.TimerPauseStateChanged += OnTimerPauseStateChanged;
            _timerService.TimerReset += OnTimerReset;
            _timerService.RunCompleted += OnRunCompleted;
        }

        private void UnsubscribeFromServiceEvents()
        {
            _timerService.TimeUpdated -= OnTimeUpdated;
            _timerService.TimerRunningStateChanged -= OnTimerRunningStateChanged;
            _timerService.TimerPauseStateChanged -= OnTimerPauseStateChanged;
            _timerService.TimerReset -= OnTimerReset;
            _timerService.RunCompleted -= OnRunCompleted;
        }

        private void OnTimeUpdated(string timeString)
        {
            if (lblTimeDisplay != null && lblTimeDisplay.InvokeRequired)
            {
                lblTimeDisplay.Invoke(new Action<string>(OnTimeUpdated), timeString);
            }
            else if (lblTimeDisplay != null)
            {
                lblTimeDisplay.Text = timeString;
                
                // 根据时间长度调整字体大小
                var elapsed = _timerService.GetElapsedTime();
                if (elapsed.Hours > 9)
                {
                    lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 24F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
                }
                else if (elapsed.Hours > 0)
                {
                    lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 28F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
                }
                else
                {
                    lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 30F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
                }
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
            
            TimerStateChanged?.Invoke(this, EventArgs.Empty);
            UpdateStatistics();
        }

        private void OnTimerPauseStateChanged(bool isPaused)
        {
            // 可以在这里处理暂停状态的特殊UI显示
            TimerStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnTimerReset()
        {
            if (lblTimeDisplay != null && lblTimeDisplay.InvokeRequired)
            {
                lblTimeDisplay.Invoke(new Action(OnTimerReset));
            }
            else if (lblTimeDisplay != null)
            {
                lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 30F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
                lblTimeDisplay.Text = "00:00:00:0";
            }
            
            UpdateStatistics();
        }

        private void OnRunCompleted(TimeSpan runTime)
        {
            // 使用HistoryControl来记录新的运行时间
            historyControl?.AddRunRecord(runTime);
            UpdateStatistics();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 设置角色和场景信息
        /// </summary>
        public void SetCharacterAndScene(string character, string scene)
        {
            _timerService.CurrentCharacter = character;
            _timerService.CurrentScene = scene;
            
            characterSceneControl?.SetCharacterAndScene(character, scene);
            UpdateUI();
        }

        /// <summary>
        /// 通过快捷键触发开始/停止计时
        /// </summary>
        public void ToggleTimer()
        {
            LogManager.WriteDebugLog("TimerControl", $"ToggleTimer 调用（快捷键触发），当前状态: isTimerRunning={_timerService.IsRunning}");
            _timerService.Toggle();
        }

        /// <summary>
        /// 暂停/恢复计时
        /// </summary>
        public void TogglePause()
        {
            _timerService.TogglePause();
        }

        /// <summary>
        /// 重置计时器
        /// </summary>
        public void ResetTimerExternally()
        {
            _timerService.Reset();
        }

        /// <summary>
        /// 同步更新角色和场景信息
        /// </summary>
        public void SyncWithProfileManager()
        {
            TryGetProfileInfoFromMainForm();
            historyControl?.LoadProfileHistoryData(CurrentProfile, _timerService.CurrentScene, _timerService.CurrentCharacter, ProfileService.Instance.CurrentDifficulty);
            UpdateUI();
            LogManager.WriteDebugLog("TimerControl", $"已同步角色和场景信息: {_timerService.CurrentCharacter} - {_timerService.CurrentScene}");
        }

        /// <summary>
        /// 当切换到计时器Tab时调用
        /// </summary>
        public void OnTabSelected()
        {
            SyncWithProfileManager();
            LogManager.WriteDebugLog("TimerControl", "计时器Tab被选中，已自动加载角色档案数据");
        }

        /// <summary>
        /// 在应用程序关闭时调用
        /// </summary>
        public void OnApplicationClosing()
        {
            _timerService.OnApplicationClosing();
        }
        #endregion

        #region Private Methods
        private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            // 更新状态指示按钮颜色
            if (btnStatusIndicator != null)
            {
                btnStatusIndicator.BackColor = _timerService.IsRunning ? Color.Green : Color.Red;
            }

            // 更新时间显示
            if (!_timerService.IsRunning)
            {
                if (lblTimeDisplay != null)
                {
                    lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 30F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
                    lblTimeDisplay.Text = "00:00:00:0";
                }
            }

            UpdateStatistics();
            UpdateCharacterSceneInfo();
        }

        /// <summary>
        /// 公共方法，供外部调用刷新UI
        /// </summary>
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
                TimeSpan fastestTime = historyControl.FastestTime;
                var runHistory = historyControl.RunHistory;
                
                statisticsControl.UpdateStatistics(runCount, fastestTime, runHistory);
            }
            
            historyControl?.UpdateHistory(historyControl.RunHistory);
        }

        private void UpdateCharacterSceneInfo()
        {
            if (characterSceneControl != null && CurrentProfile != null)
            {
                // 获取本地化的场景名称
                string localizedSceneName = LanguageManager.GetString(_timerService.CurrentScene);
                
                // 获取游戏难度文本
                string difficultyText = GetCurrentDifficultyText();
                
                characterSceneControl.UpdateCharacterSceneInfo(
                    _timerService.CurrentCharacter, 
                    CurrentProfile, 
                    localizedSceneName, 
                    difficultyText);
            }
        }

        private void TryGetProfileInfoFromMainForm()
        {
            var mainForm = this.FindForm() as MainForm;
            if (mainForm != null && mainForm.ProfileManager != null)
            {
                var profileManager = mainForm.ProfileManager;
                if (profileManager.CurrentProfile != null)
                {
                    CurrentProfile = profileManager.CurrentProfile;
                    _timerService.CurrentCharacter = profileManager.CurrentProfile.Name;
                    _timerService.CurrentScene = profileManager.CurrentScene;
                }
                else
                {
                    LogManager.WriteDebugLog("TimerControl", "ProfileManager.CurrentProfile 为 null，无法获取角色信息");
                }
            }
        }

        private string GetCurrentDifficultyText()
        {
            var difficulty = ProfileService.Instance.CurrentDifficulty;
            return SceneManager.GetLocalizedDifficultyName(difficulty);
        }
        #endregion

        #region UI Initialization
        private void InitializeComponent()
        {
            // 状态指示按钮
            btnStatusIndicator = new Button
            {
                Enabled = false,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(16, 16),
                Location = new Point(15, 45),
                Name = "btnStatusIndicator",
                TabIndex = 0,
                TabStop = false,
                BackColor = Color.Red,
                FlatAppearance = { BorderSize = 0 }
            };

            // 主要计时显示标签
            lblTimeDisplay = new Label
            {
                AutoSize = false,
                Font = new Font("Microsoft YaHei UI", 30F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134))),
                Location = new Point(15, 30),
                Name = "lblTimeDisplay",
                Size = new Size(290, 64),
                TextAlign = ContentAlignment.MiddleCenter,
                TabIndex = 1,
                Text = "00:00:00:0"
            };
            
            // 初始化统计信息控件
            statisticsControl = new StatisticsControl
            {
                Location = new Point(5, 100),
                Name = "statisticsControl",
                Size = new Size(290, 75),
                TabIndex = 5,
                Parent = this
            };

            // 初始化历史记录控件
            historyControl = new HistoryControl
            {
                Location = new Point(15, 170),
                Name = "historyControl",
                Size = new Size(290, 90),
                TabIndex = 3,
                Parent = this
            };
            
            // 初始化角色场景信息组件
            characterSceneControl = new CharacterSceneControl
            {
                Location = new Point(15, 270),
                Name = "characterSceneControl",
                Size = new Size(290, 40),
                TabIndex = 4
            };
            
            SuspendLayout();
            
            // TimerControl - 主控件设置
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnStatusIndicator);
            Controls.Add(lblTimeDisplay);
            Controls.Add(statisticsControl);
            Controls.Add(historyControl);
            Controls.Add(characterSceneControl);
            Name = "TimerControl";
            Size = new Size(320, 320);
            ResumeLayout(false);
            PerformLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnsubscribeFromServiceEvents();
                LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}