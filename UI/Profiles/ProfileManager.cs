using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Profiles
{
    public partial class ProfileManager : UserControl
    {
        // 控件字段 - 标记为可为null以修复CS8618警告
        private Button? btnCreateCharacter;
        private Button? btnSwitchCharacter;
        private Button? btnDeleteCharacter;
        private Label? lblScene;
        private ComboBox? cmbScene;
        private Label? lblDifficulty;
        private ComboBox? cmbDifficulty;
        private Button? btnStartStop;
        private Label? lblCurrentProfile;
        private Label? lblTime;
        private Label? lblStats;
        // MF记录功能相关字段
        private Models.CharacterProfile? currentProfile = null;
        private Models.MFRecord? currentRecord = null;
        private List<Models.FarmingScene> farmingScenes = [];
        public Models.CharacterProfile? CurrentProfile => currentProfile;
        public Models.MFRecord? CurrentRecord => currentRecord;
        
        // 当前选中的场景
        public string CurrentScene {
            get
            {
                if (cmbScene != null)
                    return cmbScene.Text;
                return string.Empty;
            }
        }

        // 当前选中的难度
        public Models.GameDifficulty CurrentDifficulty {
            get
            {
                return GetSelectedDifficulty();
            }
        }

        public ProfileManager()
        {
            InitializeComponent();
            LoadFarmingScenes();
            UpdateUI();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            
            // 角色管理按钮
            btnCreateCharacter = new Button
            {
                Location = new System.Drawing.Point(30, 30),
                Margin = new Padding(4),
                Name = "btnCreateCharacter",
                Size = new System.Drawing.Size(120, 40),
                TabIndex = 0,
                UseVisualStyleBackColor = true
            };
            btnCreateCharacter.Click += BtnCreateCharacter_Click;
            
            btnSwitchCharacter = new Button
            {
                Location = new System.Drawing.Point(190, 30),
                Margin = new Padding(4),
                Name = "btnSwitchCharacter",
                Size = new System.Drawing.Size(120, 40),
                TabIndex = 1,
                UseVisualStyleBackColor = true
            };
            btnSwitchCharacter.Click += BtnSwitchCharacter_Click;
            
            btnDeleteCharacter = new Button
            {
                Location = new System.Drawing.Point(350, 30),
                Margin = new Padding(4),
                Name = "btnDeleteCharacter",
                Size = new System.Drawing.Size(120, 40),
                TabIndex = 2,
                UseVisualStyleBackColor = true,
                Enabled = false
            };
            btnDeleteCharacter.Click += BtnDeleteCharacter_Click;
            
            // 场景和难度选择
            lblScene = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(30, 100),
                Margin = new Padding(6, 0, 6, 0),
                Name = "lblScene",
                Size = new System.Drawing.Size(80, 28),
                TabIndex = 3
            };
            
            cmbScene = new ComboBox
            {
                Location = new System.Drawing.Point(130, 100),
                Margin = new Padding(6),
                Name = "cmbScene",
                Size = new System.Drawing.Size(250, 34),
                TabIndex = 4,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbScene.SelectedIndexChanged += CmbScene_SelectedIndexChanged;
            
            lblDifficulty = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(30, 160),
                Margin = new Padding(6, 0, 6, 0),
                Name = "lblDifficulty",
                Size = new System.Drawing.Size(80, 28),
                TabIndex = 5
            };
            
            cmbDifficulty = new ComboBox
            {
                Location = new System.Drawing.Point(130, 160),
                Margin = new Padding(6),
                Name = "cmbDifficulty",
                Size = new System.Drawing.Size(200, 34),
                TabIndex = 6,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbDifficulty.SelectedIndexChanged += CmbDifficulty_SelectedIndexChanged;
            
            // 添加难度选项
            foreach (Models.GameDifficulty difficulty in Enum.GetValues(typeof(Models.GameDifficulty)))
                    cmbDifficulty.Items.Add(SceneService.GetLocalizedDifficultyName(difficulty));
            if (cmbDifficulty.Items.Count > 0)
                cmbDifficulty.SelectedIndex = 2; // 默认地狱难度
              
            // 计时控制按钮
            btnStartStop = new Button
            {
                Location = new System.Drawing.Point(30, 220),
                Margin = new Padding(6),
                Name = "btnStartStop",
                Size = new System.Drawing.Size(130, 50),
                TabIndex = 7,
                UseVisualStyleBackColor = true,
                Enabled = false
            };
            btnStartStop.Click += BtnStartStop_Click;
            
            // 状态显示标签
            lblCurrentProfile = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(30, 300),
                Margin = new Padding(6, 0, 6, 0),
                Name = "lblCurrentProfile",
                Size = new System.Drawing.Size(120, 28),
                TabIndex = 10
            };
            
            lblTime = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(30, 340),
                Margin = new Padding(6, 0, 6, 0),
                Name = "lblTime",
                Size = new System.Drawing.Size(120, 28),
                TabIndex = 11
            };
            
            lblStats = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(30, 380),
                Margin = new Padding(6, 0, 6, 0),
                Name = "lblStats",
                Size = new System.Drawing.Size(120, 28),
                TabIndex = 12
            };
            
            // ProfileManager
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.AddRange([
                btnCreateCharacter,
                btnSwitchCharacter,
                btnDeleteCharacter,
                lblScene,
                cmbScene,
                lblDifficulty,
                cmbDifficulty,
                btnStartStop,
                lblCurrentProfile,
                lblTime,
                lblStats
            ]);
            Margin = new Padding(6);
            Name = "ProfileManager";
            Size = new System.Drawing.Size(542, 450);
            ResumeLayout(false);
            PerformLayout();
        }
        private void LoadFarmingScenes()
        {
            farmingScenes = Services.SceneService.LoadFarmingSpots();
            
            cmbScene?.Items.Clear();
            
            if (farmingScenes.Count == 0)
            {
                MessageBox.Show("场景列表为空，无法添加到下拉框", "场景加载警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                foreach (var scene in farmingScenes)
                {
                    string displayName = SceneService.GetSceneDisplayName(scene);
                    cmbScene?.Items.Add(displayName);
                }
                
                if (cmbScene != null && cmbScene.Items.Count > 0)
                {
                    cmbScene.SelectedIndex = 0;
                }
                
                // 只输出到控制台，不再显示弹窗
                string summary = $"场景列表加载完成\n场景总数: {farmingScenes.Count}\n成功添加到下拉框: {(cmbScene != null ? cmbScene.Items.Count : 0)}";
                WriteDebugLog(summary);
            }
            
            // 尝试加载上次使用的场景、难度和角色档案
            try
            {
                var settings = Services.SettingsManager.LoadSettings();
                // 加载上次使用的场景
                if (!string.IsNullOrEmpty(settings.LastUsedScene) && cmbScene != null)
                {
                    for (int i = 0; i < cmbScene.Items.Count; i++)
                    {
                        var itemText = cmbScene.Items[i]?.ToString();
                        if (itemText != null && itemText == settings.LastUsedScene)
                        {
                            cmbScene.SelectedIndex = i;
                            break;
                        }
                    }
                }
                
                // 尝试加载上次使用的难度
                if (!string.IsNullOrEmpty(settings.LastUsedDifficulty) && cmbDifficulty != null)
                {
                    if (Enum.TryParse<DTwoMFTimerHelper.Models.GameDifficulty>(settings.LastUsedDifficulty, out var difficulty))
                    {
                        // 根据难度值设置下拉框索引
                        switch (difficulty)
                        {
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
                
                // 尝试加载上次使用的角色档案
                if (!string.IsNullOrEmpty(settings.LastUsedProfile))
                {
                    var profile = Services.DataService.FindProfileByName(settings.LastUsedProfile);
                    if (profile != null)
                    {
                        currentProfile = profile;
                        if (btnDeleteCharacter != null) btnDeleteCharacter.Enabled = true;
                        if (btnStartStop != null) btnStartStop.Enabled = true;
                        // 更新界面显示
                        UpdateUI();
                    }
                }
            }
            catch (Exception ex)
            {
                // 错误处理可以留空或添加更简洁的日志
                WriteDebugLog($"LoadFarmingScenes 错误: {ex.Message}");
            }
        }

        private Models.GameDifficulty GetSelectedDifficulty()
        {
            if (cmbDifficulty?.SelectedIndex >= 0)
            {
                return SceneService.GetDifficultyByIndex(cmbDifficulty.SelectedIndex);
            }
            return Models.GameDifficulty.Hell;
        }

        private Models.FarmingScene? GetSelectedScene()
        {
            if (cmbScene?.SelectedIndex >= 0 && cmbScene.SelectedIndex < farmingScenes.Count)
                return farmingScenes[cmbScene.SelectedIndex];
            return null;
        }

        private static void WriteDebugLog(string message)
        {
            LogManager.WriteDebugLog("ProfileManager", message);
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
        
        private void UpdateUI()
        {
            // 更新按钮文本
            if (btnCreateCharacter != null) btnCreateCharacter.Text = DTwoMFTimerHelper.Utils.LanguageManager.GetString("CreateCharacter");
            if (btnSwitchCharacter != null) btnSwitchCharacter.Text = DTwoMFTimerHelper.Utils.LanguageManager.GetString("SwitchCharacter");
            if (btnDeleteCharacter != null) btnDeleteCharacter.Text = DTwoMFTimerHelper.Utils.LanguageManager.GetString("DeleteCharacter");
            
            // 根据是否有未完成记录设置开始按钮文本
            if (btnStartStop != null)
            {
                // 添加日志记录开始检查过程
                WriteDebugLog("开始检查未完成记录");

                // 检查是否有未完成记录
                bool hasIncompleteRecord = false;
                if (currentProfile != null)
                {
                    WriteDebugLog($"当前角色: {currentProfile.Name}");
                    
                    var selectedScene = GetSelectedScene();
                    if (selectedScene != null)
                    {
                        var difficulty = GetSelectedDifficulty();
                        WriteDebugLog($"选中难度: {difficulty}");

                        // 获取场景的纯英文名称（与记录存储格式一致）
                        string sceneDisplayName = SceneService.GetSceneDisplayName(selectedScene);
                        WriteDebugLog($"场景显示名称: {sceneDisplayName}");

                        string pureSceneName = sceneDisplayName;
                        if (sceneDisplayName.StartsWith("ACT ") || sceneDisplayName.StartsWith("Act ") || sceneDisplayName.StartsWith("act "))
                        {
                            int colonIndex = sceneDisplayName.IndexOf(':');
                            if (colonIndex > 0)
                            {
                                pureSceneName = sceneDisplayName[(colonIndex + 1)..].Trim();
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
                        if (matchingRecord != null)
                        {
                            WriteDebugLog($"找到匹配记录 - 场景: {matchingRecord.SceneName}, 难度: {matchingRecord.Difficulty}, 完成状态: {matchingRecord.IsCompleted}");
                        }
                    }
                    else
                    {
                        WriteDebugLog("未选中场景");
                    }
                }
                else
                {
                    WriteDebugLog("当前没有选择角色配置");
                }
                
                // 根据是否有未完成记录设置按钮文本
                if (hasIncompleteRecord)
                {
                    btnStartStop.Text = LanguageManager.GetString("ContinueFarm");
                    WriteDebugLog("设置按钮文本为: ContinueFarm");
                }
                else
                {
                    btnStartStop.Text = LanguageManager.GetString("StartTimer");
                    WriteDebugLog("设置按钮文本为: StartTimer");
                }
            }
            if (lblScene != null) lblScene.Text = LanguageManager.GetString("SelectScene");
            if (lblDifficulty != null) lblDifficulty.Text = LanguageManager.GetString("DifficultyLabel");
            
            // 更新当前角色显示
            if (currentProfile != null)
            {
                string className = LanguageManager.GetLocalizedClassName(currentProfile.Class);
                if (lblCurrentProfile != null) lblCurrentProfile.Text = LanguageManager.GetString("CurrentCharacter", currentProfile.Name, className);
            }
            else
            {
                if (lblCurrentProfile != null) lblCurrentProfile.Text = LanguageManager.GetString("CurrentCharacterNotSelected");
            }
            
            // 隐藏时间和统计信息标签，去掉红色标注
            if (lblTime != null) 
            {
                lblTime.Visible = false;
            }
            if (lblStats != null) 
            {
                lblStats.Visible = false;
            }
            
            // 更新按钮状态
            if (btnDeleteCharacter != null) btnDeleteCharacter.Enabled = currentProfile != null;
            if (btnStartStop != null) btnStartStop.Enabled = currentProfile != null;
        }

        private void BtnCreateCharacter_Click(object? sender, EventArgs e)
        {
            using var form = new CreateCharacterForm();
            if (form.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(form.CharacterName))
            {
                try
                {
                    WriteDebugLog("开始创建新角色...");

                    // 确保cmbScene不为null
                    if (cmbScene != null)
                    {
                        cmbScene.SelectedIndex = 0; // 重置场景选择
                    }

                    string characterName = form.CharacterName;
                    WriteDebugLog($"角色名称: {characterName}");

                    // 获取选中的职业并确保类型转换正确
                    var selectedClass = form.GetSelectedClass();
                    if (!selectedClass.HasValue)
                    {
                        throw new InvalidOperationException("未选择有效的角色职业");
                    }

                    // 显式转换为Models命名空间下的CharacterClass
                    DTwoMFTimerHelper.Models.CharacterClass charClass = (DTwoMFTimerHelper.Models.CharacterClass)selectedClass.Value;
                    WriteDebugLog($"角色职业: {charClass}");

                    // 创建新角色档案
                    currentProfile = Services.DataService.CreateNewProfile(characterName, charClass);

                    // 验证创建结果
                    if (currentProfile == null)
                    {
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
                catch (Exception ex)
                {
                    WriteDebugLog($"创建角色失败: {ex.Message}");
                    WriteDebugLog($"异常堆栈: {ex.StackTrace}");
                    MessageBox.Show($"创建角色失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnSwitchCharacter_Click(object? sender, EventArgs e)
        {
            using var form = new SwitchCharacterForm();
            if (form.ShowDialog() == DialogResult.OK && form.SelectedProfile != null)
            {
                try
                {
                    WriteDebugLog("开始切换角色...");
                    
                    var selectedProfile = form.SelectedProfile;

                    // 验证角色数据
                    if (selectedProfile == null || string.IsNullOrWhiteSpace(selectedProfile.Name))
                    {
                        throw new InvalidOperationException("无效的角色数据");
                    }

                    // 使用 ProfileService 的单例来统一管理角色切换
                    bool switchResult = ProfileService.Instance.SwitchCharacter(selectedProfile);
                    
            if (switchResult)
            {
                // 更新本地引用
                currentProfile = selectedProfile;
                currentRecord = null;

                WriteDebugLog($"成功切换到角色: {currentProfile.Name}");

                // 更新UI显示新角色信息
                UpdateUI();

                // 同步更新TimerControl（确保这里不会再次触发角色切换）
                SyncTimerControl();

                // 显示成功消息
                MessageBox.Show($"已成功切换到角色 '{currentProfile.Name}'", "切换成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                throw new InvalidOperationException("角色切换失败");
            }
        }
        catch (Exception ex)
        {
            WriteDebugLog($"切换角色失败: {ex.Message}");
            WriteDebugLog($"异常堆栈: {ex.StackTrace}");
            MessageBox.Show($"切换角色失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

        private void BtnDeleteCharacter_Click(object? sender, EventArgs e)
        {
            if (currentProfile == null) return;
            
            string confirmMsg = $"确定要删除角色: {currentProfile.Name}?";
            if (MessageBox.Show(confirmMsg, "删除角色", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // 删除角色档案
                Services.DataService.DeleteProfile(currentProfile);
                currentProfile = null;
                currentRecord = null;
                UpdateUI();
            }
        }
        
        private void BtnStartStop_Click(object? sender, EventArgs e)
        {
            WriteDebugLog("计时功能已移除");
        }
        
        // 场景选择变更事件处理
        private void CmbScene_SelectedIndexChanged(object? sender, EventArgs e)
        {
            // 当场景改变时，更新UI和ProfileService
            if (cmbScene != null)
            {
                WriteDebugLog($"场景已变更为: {cmbScene.Text}");
                
                // 同步更新ProfileService中的CurrentScene
                if (Services.ProfileService.Instance != null)
                {
                    Services.ProfileService.Instance.CurrentScene = cmbScene.Text;
                    
                    // 同步更新TimerControl
                    SyncTimerControl();
                }
                
                // 更新按钮文本，检查是否有未完成记录
                UpdateUI();
            }
        }
        
        private void CmbDifficulty_SelectedIndexChanged(object? sender, EventArgs e)
        {            
            // 当难度改变时，更新ProfileService中的CurrentDifficulty
            if (currentProfile != null && cmbDifficulty != null && cmbDifficulty.SelectedIndex >= 0)
            {
                // 使用SelectedIndex而不是SelectedItem来获取难度值
                int selectedIndex = cmbDifficulty.SelectedIndex;
                string selectedDifficultyText = cmbDifficulty.SelectedItem?.ToString() ?? "未知";
                WriteDebugLog($"难度索引已变更为: {selectedIndex}，显示文本: {selectedDifficultyText}");
                
                // 使用SceneService中的GetDifficultyByIndex方法获取对应的GameDifficulty枚举值
                Models.GameDifficulty difficulty = Services.SceneService.GetDifficultyByIndex(selectedIndex);
                
                // 更新ProfileService中的CurrentDifficulty
                if (Services.ProfileService.Instance != null)
                {
                    Services.ProfileService.Instance.CurrentDifficulty = difficulty;
                    
                    // 同步更新TimerControl
                    SyncTimerControl();
                    
                    // 更新UI
                    UpdateUI();
                }
            }
        }
        
        /// <summary>
        /// 同步更新TimerControl的角色和场景信息
        /// </summary>
        private void SyncTimerControl()
        {            
            try
            {
                // 获取主窗口
                if (this.FindForm() is MainForm mainForm && mainForm.TimerControl != null)
                {
                    // 调用TimerControl的同步方法
                    mainForm.TimerControl.SyncWithProfileManager();
                    WriteDebugLog($"已同步TimerControl的角色和场景信息");
                }
            }
            catch (Exception ex)
            {                
                WriteDebugLog($"同步TimerControl失败: {ex.Message}");
                WriteDebugLog($"异常堆栈: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 加载上次使用的角色档案
        /// </summary>
        public void LoadLastUsedProfile()
        {
            WriteDebugLog("LoadLastUsedProfile 开始执行");
            try
            {                // 加载设置并获取上次使用的角色档案名称
                var settings = SettingsManager.LoadSettings();
                string lastUsedProfileName = settings.LastUsedProfile;
                WriteDebugLog($"从配置文件加载设置: LastUsedProfile={lastUsedProfileName}");

                if (!string.IsNullOrWhiteSpace(lastUsedProfileName))
                {
                    WriteDebugLog($"尝试加载上次使用的角色档案: {lastUsedProfileName}");

                    // 加载所有角色档案
                    var allProfiles = Services.DataService.LoadAllProfiles(false);
                    WriteDebugLog($"已加载所有角色档案，数量: {allProfiles.Count}");

                    // 查找上次使用的角色档案
                    var profile = allProfiles.FirstOrDefault(p => p.Name == lastUsedProfileName);
                    if (profile != null)
                    {                        
                        currentProfile = profile;
                        currentRecord = null;
                        UpdateUI();
                        WriteDebugLog($"成功加载上次使用的角色档案: {lastUsedProfileName}, profile.Name={profile.Name}");
                    }
                    else
                    {
                        WriteDebugLog($"未找到上次使用的角色档案: {lastUsedProfileName}");
                    }
                }
                else
                {
                    WriteDebugLog("没有保存的上次使用角色档案");
                }
            }
            catch (Exception ex)
            {
                WriteDebugLog($"加载上次使用角色档案失败: {ex.Message}, 堆栈: {ex.StackTrace}");
            }
        }
    }
}