using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.ComponentModel;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Profiles {
    public partial class ProfileManager : UserControl {
        private readonly ITimerService _timerService;
        private readonly IProfileService _profileService;
        private readonly IMainServices _mainServices;

        public ProfileManager(
        IProfileService profileService,
        ITimerService timerService,
        IMainServices mainServices) {
            _profileService = profileService;
            _timerService = timerService;
            _mainServices = mainServices;

            InitializeComponent();
            LoadFarmingScenes();
            LoadLastRunSettings();
            UpdateUI();
        }
        // 控件字段 - 使用组件容器来支持设计器
        private IContainer components = null;
        private Button btnCreateCharacter;
        private Button btnSwitchCharacter;
        private Button btnDeleteCharacter;
        private Label lblScene;
        private ComboBox cmbScene;
        private Label lblDifficulty;
        private ComboBox cmbDifficulty;
        private Button btnStartStop;
        private Label lblCurrentProfile;
        private Label lblTime;
        private Label lblStats;

        // MF记录功能相关字段
        private Models.CharacterProfile currentProfile = null;
        private Models.MFRecord currentRecord = null;
        private List<Models.FarmingScene> farmingScenes = new List<Models.FarmingScene>();
        public Models.CharacterProfile CurrentProfile => currentProfile;
        public Models.MFRecord CurrentRecord => currentRecord;

        // 当前选中的场景
        public string CurrentScene {
            get {
                if (cmbScene != null)
                    return cmbScene.Text;
                return string.Empty;
            }
        }

        // 当前选中的难度
        public Models.GameDifficulty CurrentDifficulty {
            get {
                return GetSelectedDifficulty();
            }
        }

        private void InitializeComponent() {
            btnCreateCharacter = new Button();
            btnSwitchCharacter = new Button();
            btnDeleteCharacter = new Button();
            lblScene = new Label();
            cmbScene = new ComboBox();
            lblDifficulty = new Label();
            cmbDifficulty = new ComboBox();
            btnStartStop = new Button();
            lblCurrentProfile = new Label();
            lblTime = new Label();
            lblStats = new Label();
            SuspendLayout();
            // 
            // btnCreateCharacter
            // 
            btnCreateCharacter.Location = new System.Drawing.Point(30, 30);
            btnCreateCharacter.Margin = new Padding(4);
            btnCreateCharacter.Name = "btnCreateCharacter";
            btnCreateCharacter.Size = new System.Drawing.Size(105, 40);
            btnCreateCharacter.TabIndex = 0;
            btnCreateCharacter.UseVisualStyleBackColor = true;
            btnCreateCharacter.Click += BtnCreateCharacter_Click;
            // 
            // btnSwitchCharacter
            // 
            btnSwitchCharacter.Location = new System.Drawing.Point(176, 30);
            btnSwitchCharacter.Margin = new Padding(4);
            btnSwitchCharacter.Name = "btnSwitchCharacter";
            btnSwitchCharacter.Size = new System.Drawing.Size(105, 40);
            btnSwitchCharacter.TabIndex = 1;
            btnSwitchCharacter.UseVisualStyleBackColor = true;
            btnSwitchCharacter.Click += BtnSwitchCharacter_Click;
            // 
            // btnDeleteCharacter
            // 
            btnDeleteCharacter.Enabled = false;
            btnDeleteCharacter.Location = new System.Drawing.Point(312, 30);
            btnDeleteCharacter.Margin = new Padding(4);
            btnDeleteCharacter.Name = "btnDeleteCharacter";
            btnDeleteCharacter.Size = new System.Drawing.Size(100, 40);
            btnDeleteCharacter.TabIndex = 2;
            btnDeleteCharacter.UseVisualStyleBackColor = true;
            btnDeleteCharacter.Click += BtnDeleteCharacter_Click;
            // 
            // lblScene
            // 
            lblScene.AutoSize = true;
            lblScene.Location = new System.Drawing.Point(30, 100);
            lblScene.Margin = new Padding(6, 0, 6, 0);
            lblScene.Name = "lblScene";
            lblScene.Size = new System.Drawing.Size(0, 28);
            lblScene.TabIndex = 3;
            // 
            // cmbScene
            // 
            cmbScene.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbScene.Location = new System.Drawing.Point(130, 100);
            cmbScene.Margin = new Padding(6);
            cmbScene.Name = "cmbScene";
            cmbScene.Size = new System.Drawing.Size(250, 36);
            cmbScene.TabIndex = 4;
            cmbScene.SelectedIndexChanged += CmbScene_SelectedIndexChanged;
            // 
            // lblDifficulty
            // 
            lblDifficulty.AutoSize = true;
            lblDifficulty.Location = new System.Drawing.Point(30, 160);
            lblDifficulty.Margin = new Padding(6, 0, 6, 0);
            lblDifficulty.Name = "lblDifficulty";
            lblDifficulty.Size = new System.Drawing.Size(0, 28);
            lblDifficulty.TabIndex = 5;
            // 
            // cmbDifficulty
            // 
            cmbDifficulty.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDifficulty.Location = new System.Drawing.Point(130, 160);
            cmbDifficulty.Margin = new Padding(6);
            cmbDifficulty.Name = "cmbDifficulty";
            cmbDifficulty.Size = new System.Drawing.Size(250, 36);
            cmbDifficulty.TabIndex = 6;
            cmbDifficulty.SelectedIndexChanged += CmbDifficulty_SelectedIndexChanged;
            // 
            // btnStartStop
            // 
            btnStartStop.Enabled = false;
            btnStartStop.Location = new System.Drawing.Point(30, 220);
            btnStartStop.Margin = new Padding(6);
            btnStartStop.Name = "btnStartStop";
            btnStartStop.Size = new System.Drawing.Size(130, 50);
            btnStartStop.TabIndex = 7;
            btnStartStop.UseVisualStyleBackColor = true;
            btnStartStop.Click += BtnStartStop_Click;
            // 
            // lblCurrentProfile
            // 
            lblCurrentProfile.AutoSize = true;
            lblCurrentProfile.Location = new System.Drawing.Point(30, 300);
            lblCurrentProfile.Margin = new Padding(6, 0, 6, 0);
            lblCurrentProfile.Name = "lblCurrentProfile";
            lblCurrentProfile.Size = new System.Drawing.Size(0, 28);
            lblCurrentProfile.TabIndex = 10;
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.Location = new System.Drawing.Point(30, 340);
            lblTime.Margin = new Padding(6, 0, 6, 0);
            lblTime.Name = "lblTime";
            lblTime.Size = new System.Drawing.Size(0, 28);
            lblTime.TabIndex = 11;
            // 
            // lblStats
            // 
            lblStats.AutoSize = true;
            lblStats.Location = new System.Drawing.Point(30, 380);
            lblStats.Margin = new Padding(6, 0, 6, 0);
            lblStats.Name = "lblStats";
            lblStats.Size = new System.Drawing.Size(0, 28);
            lblStats.TabIndex = 12;
            // 
            // ProfileManager
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnCreateCharacter);
            Controls.Add(btnSwitchCharacter);
            Controls.Add(btnDeleteCharacter);
            Controls.Add(lblScene);
            Controls.Add(cmbScene);
            Controls.Add(lblDifficulty);
            Controls.Add(cmbDifficulty);
            Controls.Add(btnStartStop);
            Controls.Add(lblCurrentProfile);
            Controls.Add(lblTime);
            Controls.Add(lblStats);
            Margin = new Padding(6);
            Name = "ProfileManager";
            Size = new System.Drawing.Size(542, 450);
            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        /// 初始化难度下拉框（从InitializeComponent中移出的代码）
        /// </summary>
        private void InitializeDifficultyComboBox() {
            if (cmbDifficulty != null) {
                cmbDifficulty.Items.Clear();
                foreach (Models.GameDifficulty difficulty in Enum.GetValues(typeof(Models.GameDifficulty)))
                    cmbDifficulty.Items.Add(SceneService.GetLocalizedDifficultyName(difficulty));

                if (cmbDifficulty.Items.Count > 0)
                    cmbDifficulty.SelectedIndex = 2; // 默认地狱难度
            }
        }

        private void LoadFarmingScenes() {
            // 先初始化难度下拉框
            InitializeDifficultyComboBox();

            farmingScenes = SceneService.LoadFarmingSpots();

            cmbScene?.Items.Clear();

            if (farmingScenes.Count == 0) {
                MessageBox.Show("场景列表为空，无法添加到下拉框", "场景加载警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else {
                foreach (var scene in farmingScenes) {
                    string displayName = SceneService.GetSceneDisplayName(scene);
                    cmbScene?.Items.Add(displayName);
                }

                if (cmbScene != null && cmbScene.Items.Count > 0) {
                    cmbScene.SelectedIndex = 0;
                }

                // 只输出到控制台，不再显示弹窗
                string summary = $"场景列表加载完成\n场景总数: {farmingScenes.Count}\n成功添加到下拉框: {(cmbScene != null ? cmbScene.Items.Count : 0)}";
                WriteDebugLog(summary);
            }
        }

        private void LoadLastRunSettings() {
            if (_profileService.CurrentProfile == null) {
                return;
            }

            var lastRunDifficulty = _profileService.CurrentProfile.LastRunDifficulty;
            var LastRunScene = _profileService.CurrentProfile.LastRunScene;

            // 加载上次使用的场景
            WriteDebugLog($"上次运行场景: {LastRunScene}， 加载的账户: {_profileService.CurrentProfile.Name}");
            if (cmbScene != null) {
                for (int i = 0; i < farmingScenes.Count; i++) {
                    WriteDebugLog($"匹配场景 {i}: {farmingScenes[i].EnUS}");
                    if (farmingScenes[i].EnUS == LastRunScene) {
                        cmbScene.SelectedIndex = i;
                        break;
                    }
                }
            }

            // 尝试加载上次使用的难度
            if (cmbDifficulty != null) {
                if (Enum.TryParse<DTwoMFTimerHelper.Models.GameDifficulty>(lastRunDifficulty.ToString(), out var difficulty)) {
                    // 根据难度值设置下拉框索引
                    switch (difficulty) {
                        case DTwoMFTimerHelper.Models.GameDifficulty.Normal:
                            cmbDifficulty.SelectedIndex = 0;
                            break;
                        case DTwoMFTimerHelper.Models.GameDifficulty.Nightmare:
                            cmbDifficulty.SelectedIndex = 1;
                            break;
                        case DTwoMFTimerHelper.Models.GameDifficulty.Hell:
                            cmbDifficulty.SelectedIndex = 2;
                            break;
                    }
                }
            }

            var settings = Services.SettingsManager.LoadSettings();
            // 尝试加载上次使用的角色档案
            if (!string.IsNullOrEmpty(settings.LastUsedProfile)) {
                var profile = Services.DataService.FindProfileByName(settings.LastUsedProfile);
                if (profile != null) {
                    currentProfile = profile;
                    if (btnDeleteCharacter != null)
                        btnDeleteCharacter.Enabled = true;
                    if (btnStartStop != null)
                        btnStartStop.Enabled = true;
                    // 更新界面显示
                    UpdateUI();
                }
            }
        }


        // 其他方法保持不变...
        private Models.GameDifficulty GetSelectedDifficulty() {
            if (cmbDifficulty?.SelectedIndex >= 0) {
                return SceneService.GetDifficultyByIndex(cmbDifficulty.SelectedIndex);
            }
            return Models.GameDifficulty.Hell;
        }

        private Models.FarmingScene GetSelectedScene() {
            if (cmbScene?.SelectedIndex >= 0 && cmbScene.SelectedIndex < farmingScenes.Count)
                return farmingScenes[cmbScene.SelectedIndex];
            return null;
        }

        private static void WriteDebugLog(string message) {
            LogManager.WriteDebugLog("ProfileManager", message);
        }

        /// <summary>
        /// 公共方法，供外部调用刷新UI
        /// </summary>
        public void RefreshUI() {
            if (this.InvokeRequired) {
                this.Invoke(new Action(UpdateUI));
            }
            else {
                UpdateUI();
            }
        }

        private void UpdateUI() {
            // 更新按钮文本
            if (btnCreateCharacter != null)
                btnCreateCharacter.Text = DTwoMFTimerHelper.Utils.LanguageManager.GetString("CreateCharacter");
            if (btnSwitchCharacter != null)
                btnSwitchCharacter.Text = DTwoMFTimerHelper.Utils.LanguageManager.GetString("SwitchCharacter");
            if (btnDeleteCharacter != null)
                btnDeleteCharacter.Text = DTwoMFTimerHelper.Utils.LanguageManager.GetString("DeleteCharacter");

            // 根据是否有未完成记录设置开始按钮文本
            if (btnStartStop != null) {
                // 添加日志记录开始检查过程
                WriteDebugLog("开始检查未完成记录");

                // 检查是否有未完成记录
                bool hasIncompleteRecord = false;
                if (currentProfile != null) {
                    WriteDebugLog($"当前角色: {currentProfile.Name}");

                    var selectedScene = GetSelectedScene();
                    if (selectedScene != null) {
                        var difficulty = GetSelectedDifficulty();
                        WriteDebugLog($"选中难度: {difficulty}");

                        // 获取场景的纯英文名称（与记录存储格式一致）
                        string sceneDisplayName = SceneService.GetSceneDisplayName(selectedScene);
                        WriteDebugLog($"场景显示名称: {sceneDisplayName}");

                        string pureSceneName = sceneDisplayName;
                        if (sceneDisplayName.StartsWith("ACT ") || sceneDisplayName.StartsWith("Act ") || sceneDisplayName.StartsWith("act ")) {
                            int colonIndex = sceneDisplayName.IndexOf(':');
                            if (colonIndex > 0) {
                                pureSceneName = sceneDisplayName.Substring(colonIndex + 1).Trim();
                                WriteDebugLog($"提取纯场景名称: {pureSceneName}");
                            }
                        }

                        string pureEnglishSceneName = DTwoMFTimerHelper.Utils.LanguageManager.GetPureEnglishSceneName(sceneDisplayName);
                        WriteDebugLog($"获取纯英文场景名称: {pureEnglishSceneName}");

                        // 记录当前配置文件中的记录数量
                        WriteDebugLog($"当前角色记录数量: {currentProfile.Records.Count}");

                        // 查找同场景、同难度、未完成的记录
                        hasIncompleteRecord = currentProfile.Records.Any(r =>
                            r.SceneName == pureEnglishSceneName &&
                            r.Difficulty == difficulty &&
                            !r.IsCompleted);

                        WriteDebugLog($"是否存在未完成记录: {hasIncompleteRecord}");

                        // 记录第一条匹配场景和难度的记录详细信息（用于调试）
                        var matchingRecord = currentProfile.Records.FirstOrDefault(r =>
                            r.SceneName == pureEnglishSceneName &&
                            r.Difficulty == difficulty);
                        if (matchingRecord != null) {
                            WriteDebugLog($"找到匹配记录 - 场景: {matchingRecord.SceneName}, 难度: {matchingRecord.Difficulty}, 完成状态: {matchingRecord.IsCompleted}");
                        }
                    }
                    else {
                        WriteDebugLog("未选中场景");
                    }
                }
                else {
                    WriteDebugLog("当前没有选择角色配置");
                }

                // 根据是否有未完成记录或计时器是否暂停设置按钮文本
                if (hasIncompleteRecord) {
                    btnStartStop.Text = LanguageManager.GetString("ContinueFarm");
                }
                else {
                    btnStartStop.Text = LanguageManager.GetString("StartTimer");
                }
            }
            if (lblScene != null)
                lblScene.Text = LanguageManager.GetString("SelectScene");
            if (lblDifficulty != null)
                lblDifficulty.Text = LanguageManager.GetString("DifficultyLabel");

            // 更新当前角色显示
            if (currentProfile != null) {
                string className = LanguageManager.GetLocalizedClassName(currentProfile.Class);
                if (lblCurrentProfile != null)
                    lblCurrentProfile.Text = LanguageManager.GetString("CurrentCharacter", currentProfile.Name, className);
            }
            else {
                if (lblCurrentProfile != null)
                    lblCurrentProfile.Text = LanguageManager.GetString("CurrentCharacterNotSelected");
            }

            // 隐藏时间和统计信息标签，去掉红色标注
            if (lblTime != null) {
                lblTime.Visible = false;
            }
            if (lblStats != null) {
                lblStats.Visible = false;
            }

            // 更新按钮状态
            if (btnDeleteCharacter != null)
                btnDeleteCharacter.Enabled = currentProfile != null;
            if (btnStartStop != null)
                btnStartStop.Enabled = currentProfile != null;
        }

        private void BtnCreateCharacter_Click(object sender, EventArgs e) {
            using var form = new CreateCharacterForm();
            if (form.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(form.CharacterName)) {
                // 确保cmbScene不为null
                if (cmbScene != null) {
                    cmbScene.SelectedIndex = 0; // 重置场景选择
                }

                string characterName = form.CharacterName;

                // 获取选中的职业并确保类型转换正确
                var selectedClass = form.GetSelectedClass();
                if (!selectedClass.HasValue) {
                    throw new InvalidOperationException("未选择有效的角色职业");
                }

                // 显式转换为Models命名空间下的CharacterClass
                Models.CharacterClass charClass = selectedClass.Value;

                // 创建新角色档案
                currentProfile = DataService.CreateNewProfile(characterName, charClass);

                // 验证创建结果
                if (currentProfile == null) {
                    throw new InvalidOperationException("创建角色失败，返回的配置文件为null");
                }

                WriteDebugLog($"角色创建成功: {currentProfile.Name}");

                // 更新上次使用的角色档案设置
                var settings = Services.SettingsManager.LoadSettings();
                settings.LastUsedProfile = currentProfile.Name;
                Services.SettingsManager.SaveSettings(settings);

                // 清除当前记录并更新UI
                currentRecord = null;
                UpdateUI();

                // 显示成功消息
                MessageBox.Show($"角色 '{characterName}' 创建成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnSwitchCharacter_Click(object sender, EventArgs e) {
            using var form = new SwitchCharacterForm();
            if (form.ShowDialog() == DialogResult.OK && form.SelectedProfile != null) {
                WriteDebugLog("开始切换角色...");

                var selectedProfile = form.SelectedProfile;

                // 验证角色数据
                if (selectedProfile == null || string.IsNullOrWhiteSpace(selectedProfile.Name)) {
                    throw new InvalidOperationException("无效的角色数据");
                }

                // 使用 ProfileService 的单例来统一管理角色切换
                bool switchResult = _profileService.SwitchCharacter(selectedProfile);

                if (switchResult) {
                    // 更新本地引用
                    currentProfile = selectedProfile;
                    currentRecord = null;

                    WriteDebugLog($"成功切换到角色: {currentProfile.Name}");

                    // 更新LastUsedProfile设置
                    var settings = SettingsManager.LoadSettings();
                    settings.LastUsedProfile = selectedProfile.Name;
                    SettingsManager.SaveSettings(settings);
                    WriteDebugLog($"更新LastUsedProfile为: {selectedProfile.Name}");
                    LoadLastRunSettings();
                    // 更新UI显示新角色信息
                    UpdateUI();

                    // 显示成功消息
                    MessageBox.Show($"已成功切换到角色 '{currentProfile.Name}'", "切换成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else {
                    throw new InvalidOperationException("角色切换失败");
                }
            }
        }

        private void BtnDeleteCharacter_Click(object sender, EventArgs e) {
            if (currentProfile == null)
                return;

            string confirmMsg = $"确定要删除角色: {currentProfile.Name}?";
            if (MessageBox.Show(confirmMsg, "删除角色", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                // 删除角色档案
                DataService.DeleteProfile(currentProfile);
                currentProfile = null;
                currentRecord = null;
                UpdateUI();
            }
        }

        private void BtnStartStop_Click(object sender, EventArgs e) {
            _mainServices.SetActiveTabPage(Models.TabPage.Timer);
            _timerService.HandleStartFarm();
        }

        // 场景选择变更事件处理
        private void CmbScene_SelectedIndexChanged(object sender, EventArgs e) {
            // 当场景改变时，更新UI和ProfileService
            if (cmbScene != null) {
                WriteDebugLog($"场景已变更为: {cmbScene.Text}");
                _profileService.CurrentScene = cmbScene.Text;
                // 更新按钮文本，检查是否有未完成记录
                UpdateUI();
            }
        }

        private void CmbDifficulty_SelectedIndexChanged(object sender, EventArgs e) {
            // 当难度改变时，更新ProfileService中的CurrentDifficulty
            if (currentProfile != null && cmbDifficulty != null && cmbDifficulty.SelectedIndex >= 0) {
                // 使用SelectedIndex而不是SelectedItem来获取难度值
                int selectedIndex = cmbDifficulty.SelectedIndex;
                string selectedDifficultyText = cmbDifficulty.SelectedItem?.ToString() ?? "未知";
                WriteDebugLog($"难度索引已变更为: {selectedIndex}，显示文本: {selectedDifficultyText}");

                // 使用SceneService中的GetDifficultyByIndex方法获取对应的GameDifficulty枚举值
                Models.GameDifficulty difficulty = Services.SceneService.GetDifficultyByIndex(selectedIndex);

                _profileService.CurrentDifficulty = difficulty;
                // 更新UI - TimerControl会通过事件监听自动更新
                UpdateUI();
            }
        }

        /// <summary>
        /// 加载上次使用的角色档案
        /// </summary>
        public void LoadLastUsedProfile() {
            WriteDebugLog("LoadLastUsedProfile 开始执行");
            var settings = SettingsManager.LoadSettings();
            string lastUsedProfileName = settings.LastUsedProfile;
            WriteDebugLog($"从配置文件加载设置: LastUsedProfile={lastUsedProfileName}");

            if (!string.IsNullOrWhiteSpace(lastUsedProfileName)) {
                WriteDebugLog($"尝试加载上次使用的角色档案: {lastUsedProfileName}");

                // 直接加载单个角色配置文件，而不是加载所有文件
                var profile = Services.DataService.LoadProfileByName(lastUsedProfileName);
                if (profile != null) {
                    currentProfile = profile;
                    currentRecord = null;
                    UpdateUI();
                    WriteDebugLog($"成功加载上次使用的角色档案: {lastUsedProfileName}, profile.Name={profile.Name}");
                }
                else {
                    WriteDebugLog($"未找到上次使用的角色档案: {lastUsedProfileName}");
                }
            }
            else {
                WriteDebugLog("没有保存的上次使用角色档案");
            }
        }

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}