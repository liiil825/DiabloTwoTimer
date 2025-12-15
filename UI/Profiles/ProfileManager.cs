using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Profiles;

public partial class ProfileManager : UserControl
{
    private readonly ITimerService _timerService;
    private readonly IProfileService _profileService;
    private readonly IAppSettings _appSettings;
    private readonly IMainService _mainService;
    private readonly IPomodoroTimerService _pomodoroTimerService;
    private readonly IStatisticsService _statisticsService;
    private readonly ISceneService _sceneService;

    private readonly IMessenger _messenger;

    public ProfileManager(
        IProfileService profileService,
        IAppSettings appSettings,
        ITimerService timerService,
        IPomodoroTimerService pomodoroTimerService,
        IMainService mainService,
        IStatisticsService statisticsService,
        ISceneService sceneService,
        IMessenger messenger
    )
    {
        _profileService = profileService;
        _timerService = timerService;
        _appSettings = appSettings;
        _mainService = mainService;
        _statisticsService = statisticsService;
        _pomodoroTimerService = pomodoroTimerService;
        _sceneService = sceneService;
        _messenger = messenger;

        InitializeComponent();
        // 注册语言变更事件
        LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;
        // 订阅ProfileService事件
        _profileService.CurrentSceneChangedEvent += OnCurrentSceneChanged;
        LoadFarmingScenes();
        LoadLastRunSettings();
        UpdateUI();

        // 订阅消息
        _messenger.Subscribe<CreateCharacterMessage>(OnCreateCharacterRequested);
        _messenger.Subscribe<SwitchCharacterMessage>(OnSwitchCharacterRequested);
        _messenger.Subscribe<ExportCharacterMessage>(OnExportCharacterRequested);
    }

    // MF记录功能相关字段
    private Models.CharacterProfile? currentProfile = null;
    private Models.MFRecord? currentRecord = null;
    private List<Models.FarmingScene> farmingScenes = new();
    public Models.CharacterProfile? CurrentProfile => currentProfile;
    public Models.MFRecord? CurrentRecord => currentRecord;

    // 当前选中的场景
    public string CurrentScene
    {
        get
        {
            if (cmbScene != null)
                return cmbScene.Text;
            return string.Empty;
        }
    }

    // 当前选中的难度
    public Models.GameDifficulty CurrentDifficulty
    {
        get { return GetSelectedDifficulty(); }
    }

    /// <summary>
    /// 初始化难度下拉框（从InitializeComponent中移出的代码）
    /// </summary>
    private void InitializeDifficultyComboBox()
    {
        if (cmbDifficulty != null)
        {
            cmbDifficulty.Items.Clear();
            foreach (Models.GameDifficulty difficulty in Enum.GetValues(typeof(Models.GameDifficulty)))
                cmbDifficulty.Items.Add(_sceneService.GetLocalizedDifficultyName(difficulty));

            if (cmbDifficulty.Items.Count > 0)
                cmbDifficulty.SelectedIndex = 2; // 默认地狱难度
        }
    }

    private void LoadFarmingScenes()
    {
        // 先初始化难度下拉框
        InitializeDifficultyComboBox();

        // 保存当前选中的场景英文名称
        string currentSceneName = string.Empty;
        if (cmbScene != null && cmbScene.SelectedIndex >= 0 && farmingScenes.Count > 0)
        {
            // 获取当前选中场景的英文名称，以便重新加载后能找到对应的场景
            var selectedScene = farmingScenes[cmbScene.SelectedIndex];
            currentSceneName = selectedScene.EnUS;
        }

        farmingScenes = _sceneService.FarmingScenes;

        cmbScene?.Items.Clear();

        if (farmingScenes.Count == 0)
        {
            MessageBox.Show(
                "场景列表为空，无法添加到下拉框",
                "场景加载警告",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
        else
        {
            int selectedIndex = 0;
            for (int i = 0; i < farmingScenes.Count; i++)
            {
                var scene = farmingScenes[i];
                // LogManager.WriteDebugLog($"添加场景到下拉框: {scene}");
                string displayName = _sceneService.GetSceneDisplayName(scene);
                cmbScene?.Items.Add(displayName);

                // 如果当前场景是之前选中的场景，保存索引
                if (!string.IsNullOrEmpty(currentSceneName) && scene.EnUS == currentSceneName)
                {
                    selectedIndex = i;
                }
            }

            if (cmbScene != null && cmbScene.Items.Count > 0)
            {
                cmbScene.SelectedIndex = selectedIndex;
            }

            // 只输出到控制台，不再显示弹窗
            string summary =
                $"场景列表加载完成\n场景总数: {farmingScenes.Count}\n成功添加到下拉框: {(cmbScene != null ? cmbScene.Items.Count : 0)}";
            WriteDebugLog(summary);
        }
    }

    private void LoadLastRunSettings()
    {
        if (_profileService.CurrentProfile == null)
        {
            return;
        }

        var lastRunDifficulty = _profileService.CurrentProfile.LastRunDifficulty;
        var LastRunScene = _profileService.CurrentProfile.LastRunScene;

        // 加载上次使用的场景
        WriteDebugLog($"上次运行场景: {LastRunScene}， 加载的账户: {_profileService.CurrentProfile.Name}");
        if (cmbScene != null)
        {
            for (int i = 0; i < farmingScenes.Count; i++)
            {
                // WriteDebugLog($"匹配场景 {i}: {farmingScenes[i].EnUS}");
                if (farmingScenes[i].EnUS == LastRunScene)
                {
                    cmbScene.SelectedIndex = i;
                    break;
                }
            }
        }

        // 尝试加载上次使用的难度
        if (cmbDifficulty != null)
        {
            if (Enum.TryParse<DiabloTwoMFTimer.Models.GameDifficulty>(lastRunDifficulty.ToString(), out var difficulty))
            {
                // 根据难度值设置下拉框索引
                switch (difficulty)
                {
                    case DiabloTwoMFTimer.Models.GameDifficulty.Normal:
                        cmbDifficulty.SelectedIndex = 0;
                        break;
                    case DiabloTwoMFTimer.Models.GameDifficulty.Nightmare:
                        cmbDifficulty.SelectedIndex = 1;
                        break;
                    case DiabloTwoMFTimer.Models.GameDifficulty.Hell:
                        cmbDifficulty.SelectedIndex = 2;
                        break;
                }
            }
        }

        // 尝试加载上次使用的角色档案
        if (!string.IsNullOrEmpty(_appSettings.LastUsedProfile))
        {
            var profile = _profileService.FindProfileByName(_appSettings.LastUsedProfile);
            if (profile != null)
            {
                currentProfile = profile;
                if (btnDeleteCharacter != null)
                    btnDeleteCharacter.Enabled = true;
                if (btnStartFarm != null)
                    btnStartFarm.Enabled = true;
                // 更新界面显示
                UpdateUI();
            }
        }
    }

    // 其他方法保持不变...
    private Models.GameDifficulty GetSelectedDifficulty()
    {
        if (cmbDifficulty?.SelectedIndex >= 0)
        {
            return _sceneService.GetDifficultyByIndex(cmbDifficulty.SelectedIndex);
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
        this.SafeInvoke(() =>
        {
            UpdateUI();
        });
    }

    private void UpdateUI()
    {
        // 更新按钮文本
        btnCreateCharacter!.Text = DiabloTwoMFTimer.Utils.LanguageManager.GetString("CreateCharacter");
        btnSwitchCharacter!.Text = DiabloTwoMFTimer.Utils.LanguageManager.GetString("SwitchCharacter");
        btnDeleteCharacter!.Text = DiabloTwoMFTimer.Utils.LanguageManager.GetString("DeleteCharacter");
        btnShowStats!.Text = DiabloTwoMFTimer.Utils.LanguageManager.GetString("Statistics");
        btnShowLootHistory!.Text = DiabloTwoMFTimer.Utils.LanguageManager.GetString("LootHistory");
        btnExport!.Text = DiabloTwoMFTimer.Utils.LanguageManager.GetString("Export", "导出");

        // 根据是否有未完成记录设置开始按钮文本
        if (btnStartFarm != null)
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
                    string sceneDisplayName = _sceneService.GetSceneDisplayName(selectedScene);
                    WriteDebugLog($"场景显示名称: {sceneDisplayName}");

                    string pureEnglishSceneName = _sceneService.GetEnglishSceneName(sceneDisplayName);
                    WriteDebugLog($"获取纯英文场景名称: {pureEnglishSceneName}");

                    // 查找同场景、同难度、未完成的记录
                    hasIncompleteRecord = currentProfile.Records.Any(r =>
                        r.SceneName == pureEnglishSceneName && r.Difficulty == difficulty && !r.IsCompleted
                    );

                    WriteDebugLog($"是否存在未完成记录: {hasIncompleteRecord}");

                    // 记录第一条匹配场景和难度的记录详细信息（用于调试）
                    var matchingRecord = currentProfile.Records.FirstOrDefault(r =>
                        r.SceneName == pureEnglishSceneName && r.Difficulty == difficulty
                    );
                    if (matchingRecord != null)
                    {
                        WriteDebugLog(
                            $"找到匹配记录 - 场景: {matchingRecord.SceneName}, 难度: {matchingRecord.Difficulty}, 完成状态: {matchingRecord.IsCompleted}"
                        );
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

            // 根据是否有未完成记录或计时器是否暂停设置按钮文本
            if (hasIncompleteRecord)
            {
                btnStartFarm.Text = LanguageManager.GetString("ContinueFarm");
            }
            else
            {
                btnStartFarm.Text = LanguageManager.GetString("StartTimer");
            }
        }
        lblScene!.Text = LanguageManager.GetString("SelectScene");
        lblDifficulty!.Text = LanguageManager.GetString("DifficultyLabel");

        // 更新当前角色显示
        if (currentProfile != null)
        {
            string className = LanguageManager.GetLocalizedClassName(currentProfile.Class);
            if (lblCurrentProfile != null)
                lblCurrentProfile.Text = LanguageManager.GetString("CurrentCharacter", currentProfile.Name, className);
        }
        else
        {
            if (lblCurrentProfile != null)
                lblCurrentProfile.Text = LanguageManager.GetString("CurrentCharacterNotSelected");
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
        if (btnDeleteCharacter != null)
            btnDeleteCharacter.Enabled = currentProfile != null;
        if (btnStartFarm != null)
            btnStartFarm.Enabled = currentProfile != null;
    }

    private void BtnCreateCharacter_Click(object? sender, EventArgs e)
    {
        using var form = new CreateCharacterForm(_profileService);
        if (form.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(form.CharacterName))
        {
            // 确保cmbScene不为null
            if (cmbScene != null)
            {
                cmbScene.SelectedIndex = 0; // 重置场景选择
            }

            string characterName = form.CharacterName;

            // 获取选中的职业并确保类型转换正确
            var selectedClass = form.GetSelectedClass();
            if (!selectedClass.HasValue)
            {
                throw new InvalidOperationException("未选择有效的角色职业");
            }

            // 显式转换为Models命名空间下的CharacterClass
            Models.CharacterClass charClass = selectedClass.Value;

            // 使用ProfileService创建新角色档案
            currentProfile = _profileService.CreateCharacter(characterName, charClass);

            // 验证创建结果
            if (currentProfile == null)
            {
                throw new InvalidOperationException("创建角色失败，返回的配置文件为null");
            }

            WriteDebugLog($"角色创建成功: {currentProfile.Name}");

            // 清除当前记录并更新UI
            currentRecord = null;
            UpdateUI();

            // 显示成功提示
            Utils.Toast.Success($"角色 '{characterName}' 创建成功！");
        }
    }

    private void BtnSwitchCharacter_Click(object? sender, EventArgs e)
    {
        using var form = new SwitchCharacterForm(_profileService);
        if (form.ShowDialog() == DialogResult.OK && form.SelectedProfile != null)
        {
            WriteDebugLog("开始切换角色...");

            var selectedProfile = form.SelectedProfile;

            // 验证角色数据
            if (selectedProfile == null || string.IsNullOrWhiteSpace(selectedProfile.Name))
            {
                throw new InvalidOperationException("无效的角色数据");
            }

            // 使用 ProfileService 的单例来统一管理角色切换
            bool switchResult = _profileService.SwitchCharacter(selectedProfile);

            if (switchResult)
            {
                // 更新本地引用
                currentProfile = selectedProfile;
                currentRecord = null;

                WriteDebugLog($"成功切换到角色: {currentProfile.Name}");

                // 更新LastUsedProfile设置
                _appSettings.LastUsedProfile = selectedProfile.Name;
                _appSettings.Save();
                WriteDebugLog($"更新LastUsedProfile为: {selectedProfile.Name}");
                LoadLastRunSettings();
                // 更新UI显示新角色信息
                UpdateUI();

                // 显示成功提示
                Utils.Toast.Success($"已成功切换到角色 '{currentProfile.Name}'");
            }
            else
            {
                throw new InvalidOperationException("角色切换失败");
            }
        }
    }

    private void BtnDeleteCharacter_Click(object? sender, EventArgs e)
    {
        if (currentProfile == null)
            return;

        string confirmMsg = $"确定要删除角色: {currentProfile.Name}?";
        if (
            DiabloTwoMFTimer.UI.Components.ThemedMessageBox.Show(
                confirmMsg,
                LanguageManager.GetString("DeleteCharacter") ?? "删除角色",
                MessageBoxButtons.YesNo
            ) == DialogResult.Yes
        )
        {
            // 使用ProfileService删除角色档案
            bool deleteResult = _profileService.DeleteCharacter(currentProfile);
            if (deleteResult)
            {
                currentProfile = null;
                currentRecord = null;
                UpdateUI();
                // 显示成功提示
                Utils.Toast.Success($"已成功删除角色");
            }
        }
    }

    private void BtnStartFarm_Click(object? sender, EventArgs e)
    {
        // 调用 Service 请求切换 Tab，Service 会触发事件，MainForm 再响应切换
        _mainService.SetActiveTabPage(Models.TabPage.Timer);
        _timerService.HandleStartFarm();
    }

    private void BtnShowLootHistory_Click(object? sender, EventArgs e)
    {
        // 实例化全屏历史记录窗体
        var historyForm = new DiabloTwoMFTimer.UI.Timer.LootHistoryForm(
            _profileService,
            _sceneService,
            _statisticsService
        );

        historyForm.ShowDialog(); // 模态显示
    }

    private void BtnShowStats_Click(object? sender, EventArgs e)
    {
        // 创建 BreakForm，使用 StatisticsView 模式
        // 注意：BreakType 在这里不重要，传默认值即可
        var statsForm = new DiabloTwoMFTimer.UI.Pomodoro.BreakForm(
            _pomodoroTimerService,
            _appSettings,
            _profileService,
            _statisticsService,
            Models.BreakFormMode.StatisticsView // <--- 关键：指定为查看模式
        );

        statsForm.Show(this.FindForm()); // 使用 Show() 允许非模态，或者 ShowDialog() 模态显示
    }

    // 场景选择变更事件处理
    private void CmbScene_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // 当场景改变时，更新UI和ProfileService
        if (cmbScene != null)
        {
            WriteDebugLog($"场景已变更为: {cmbScene.Text}");
            _profileService.CurrentScene = cmbScene.Text;
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
            Models.GameDifficulty difficulty = _sceneService.GetDifficultyByIndex(selectedIndex);

            _profileService.CurrentDifficulty = difficulty;
            // 更新UI - TimerControl会通过事件监听自动更新
            UpdateUI();
        }
    }

    private void BtnExport_Click(object? sender, EventArgs e)
    {
        try
        {
            // 构造档案文件夹路径 (与 YamlProfileRepository 中的路径一致)
            string profilesPath = Utils.FolderManager.ProfilesPath;

            // 确保目录存在
            if (!Directory.Exists(profilesPath))
            {
                Directory.CreateDirectory(profilesPath);
            }

            // 打开文件夹
            Process.Start(
                new ProcessStartInfo
                {
                    FileName = profilesPath,
                    UseShellExecute = true,
                    Verb = "open",
                }
            );
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("ProfileManager", "打开档案文件夹失败", ex);
            Utils.Toast.Error($"无法打开文件夹: {ex.Message}");
        }
    }

    /// <summary>
    /// 加载上次使用的角色档案
    /// </summary>
    public void LoadLastUsedProfile()
    {
        WriteDebugLog("LoadLastUsedProfile 开始执行");
        string lastUsedProfileName = _appSettings.LastUsedProfile;
        WriteDebugLog($"从配置文件加载设置: LastUsedProfile={lastUsedProfileName}");

        if (!string.IsNullOrWhiteSpace(lastUsedProfileName))
        {
            WriteDebugLog($"尝试加载上次使用的角色档案: {lastUsedProfileName}");

            // 直接加载单个角色配置文件，而不是加载所有文件
            var profile = _profileService.FindProfileByName(lastUsedProfileName);
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

    /// <summary>
    /// 语言变更事件处理方法
    /// </summary>
    private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e)
    {
        // 重新初始化难度下拉框，以更新语言
        InitializeDifficultyComboBox();
        // 重新加载场景以更新语言
        LoadFarmingScenes();
        // 更新UI以反映语言变化
        UpdateUI();
    }

    // 处理创建角色请求
    private void OnCreateCharacterRequested(CreateCharacterMessage message)
    {
        this.SafeInvoke(() =>
        {
            BtnCreateCharacter_Click(null, EventArgs.Empty);
        });
    }

    // 处理切换角色请求
    private void OnSwitchCharacterRequested(SwitchCharacterMessage message)
    {
        this.SafeInvoke(() =>
        {
            BtnSwitchCharacter_Click(null, EventArgs.Empty);
        });
    }

    // 处理导出角色请求
    private void OnExportCharacterRequested(ExportCharacterMessage message)
    {
        this.SafeInvoke(() =>
        {
            BtnExport_Click(null, EventArgs.Empty);
        });
    }

    // 处理当前场景变更事件
    private void OnCurrentSceneChanged(string newSceneName)
    {
        this.SafeInvoke(() =>
        {
            WriteDebugLog($"收到场景变更事件: {newSceneName}");

            // 查找新场景在下拉框中的索引
            int index = -1;
            for (int i = 0; i < farmingScenes.Count; i++)
            {
                if (farmingScenes[i].EnUS == newSceneName)
                {
                    index = i;
                    break;
                }
            }

            // 更新下拉框选中项
            if (cmbScene != null && index >= 0 && index < cmbScene.Items.Count)
            {
                cmbScene.SelectedIndex = index;
                WriteDebugLog($"已更新场景下拉框选中项: {cmbScene.Text}");
            }

            // 更新UI
            UpdateUI();
        });
    }
}
