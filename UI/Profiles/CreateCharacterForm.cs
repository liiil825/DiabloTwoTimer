using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.UI.Form;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Profiles;

public partial class CreateCharacterForm : BaseForm
{
    private readonly IProfileService _profileService;
    private readonly ISceneService _sceneService;

    public string? CharacterName => txtCharacterName?.Text.Trim();

    public CreateCharacterForm(IProfileService profileService, ISceneService sceneService)
    {
        _profileService = profileService;
        _sceneService = sceneService;

        InitializeComponent();
        // 控件已在 Designer 中初始化，无需 InitializeManualControls
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!this.DesignMode)
        {
            SetupCharacterClasses();
            SetupDifficulty();
            SetupScenes();
            UpdateUI();
        }
    }

    private void SetupCharacterClasses()
    {
        if (cmbCharacterClass == null) return;

        cmbCharacterClass.Items.Clear();
        foreach (CharacterClass charClass in Enum.GetValues(typeof(CharacterClass)))
        {
            cmbCharacterClass.Items.Add(charClass);
        }
        if (cmbCharacterClass.Items.Count > 0)
            cmbCharacterClass.SelectedIndex = 0;
    }

    private void SetupDifficulty()
    {
        if (cmbDifficulty == null) return;

        cmbDifficulty.Items.Clear();
        foreach (GameDifficulty diff in Enum.GetValues(typeof(GameDifficulty)))
        {
            // 暂时存 Enum，UpdateUI 会本地化
            cmbDifficulty.Items.Add(diff);
        }
        // 默认地狱难度 (Hell)
        cmbDifficulty.SelectedItem = GameDifficulty.Hell;
    }

    private void SetupScenes()
    {
        if (cmbScene == null) return;

        cmbScene.Items.Clear();
        var scenes = _sceneService.FarmingScenes;

        // 填充场景显示名称
        foreach (var scene in scenes)
        {
            cmbScene.Items.Add(_sceneService.GetSceneDisplayName(scene));
        }

        if (cmbScene.Items.Count > 0)
            cmbScene.SelectedIndex = 0;
    }

    protected override void UpdateUI()
    {
        base.UpdateUI();

        this.Text = LanguageManager.GetString("CreateCharacter") ?? "Create Character";
        if (lblCharacterName != null) lblCharacterName.Text = LanguageManager.GetString("CharacterName") ?? "Name";
        if (lblCharacterClass != null) lblCharacterClass.Text = LanguageManager.GetString("CharacterClass") ?? "Class";

        // 使用本地化字符串更新新增的 Label
        if (lblDifficulty != null) lblDifficulty.Text = LanguageManager.GetString("DifficultyLabel") ?? "Difficulty";
        if (lblScene != null) lblScene.Text = LanguageManager.GetString("SelectScene") ?? "Scene";

        UpdateComboBoxes();
    }

    private void UpdateComboBoxes()
    {
        // 更新职业下拉框 (本地化)
        if (cmbCharacterClass != null)
        {
            int oldIndex = cmbCharacterClass.SelectedIndex;
            if (oldIndex < 0) oldIndex = 0;
            cmbCharacterClass.Items.Clear();
            foreach (CharacterClass charClass in Enum.GetValues(typeof(CharacterClass)))
            {
                cmbCharacterClass.Items.Add(LanguageManager.GetLocalizedClassName(charClass));
            }
            if (cmbCharacterClass.Items.Count > 0)
                cmbCharacterClass.SelectedIndex = Math.Min(oldIndex, cmbCharacterClass.Items.Count - 1);
        }

        // 更新难度下拉框 (本地化)
        if (cmbDifficulty != null)
        {
            int oldIndex = cmbDifficulty.SelectedIndex;
            cmbDifficulty.Items.Clear();
            foreach (GameDifficulty diff in Enum.GetValues(typeof(GameDifficulty)))
            {
                cmbDifficulty.Items.Add(_sceneService.GetLocalizedDifficultyName(diff));
            }

            if (oldIndex >= 0 && oldIndex < cmbDifficulty.Items.Count)
                cmbDifficulty.SelectedIndex = oldIndex;
            else
                cmbDifficulty.SelectedIndex = 2; // 默认 Hell
        }

        // 更新场景下拉框
        if (cmbScene != null)
        {
            int oldIndex = cmbScene.SelectedIndex;
            cmbScene.Items.Clear();
            foreach (var scene in _sceneService.FarmingScenes)
            {
                cmbScene.Items.Add(_sceneService.GetSceneDisplayName(scene));
            }
            if (oldIndex >= 0 && oldIndex < cmbScene.Items.Count)
                cmbScene.SelectedIndex = oldIndex;
        }
    }

    protected override void BtnConfirm_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CharacterName))
        {
            Utils.Toast.Info(LanguageManager.GetString("EnterCharacterName") ?? "Please enter character name");
            txtCharacterName!.Focus();
            return;
        }

        if (_profileService.FindProfileByName(CharacterName) != null)
        {
            Utils.Toast.Warning(LanguageManager.GetString("CharacterExists") ?? "Character name already exists");
            txtCharacterName!.SelectAll();
            txtCharacterName!.Focus();
            return;
        }

        // 验证职业
        var selectedClass = GetSelectedClass();
        if (selectedClass == null) return;

        // 1. 创建角色
        var profile = _profileService.CreateCharacter(CharacterName, selectedClass.Value);

        if (profile != null)
        {
            // 2. 设置初始场景和难度
            if (cmbScene != null && cmbScene.SelectedIndex >= 0)
            {
                // 获取选中的场景对象
                if (cmbScene.SelectedIndex < _sceneService.FarmingScenes.Count)
                {
                    var scene = _sceneService.FarmingScenes[cmbScene.SelectedIndex];
                    profile.LastRunScene = scene.EnUS;
                    _profileService.CurrentScene = scene.EnUS;
                }
            }

            if (cmbDifficulty != null && cmbDifficulty.SelectedIndex >= 0)
            {
                profile.LastRunDifficulty = _sceneService.GetDifficultyByIndex(cmbDifficulty.SelectedIndex);
                _profileService.CurrentDifficulty = profile.LastRunDifficulty;
            }

            // 3. 立即切换到该角色并保存
            _profileService.SwitchCharacter(profile);
            _profileService.SaveCurrentProfile();
        }

        base.BtnConfirm_Click(sender, e);
    }

    public CharacterClass? GetSelectedClass()
    {
        if (cmbCharacterClass?.SelectedItem != null)
        {
            if (cmbCharacterClass.SelectedIndex >= 0)
            {
                var values = Enum.GetValues(typeof(CharacterClass));
                if (cmbCharacterClass.SelectedIndex < values.Length)
                {
                    return (CharacterClass?)values.GetValue(cmbCharacterClass.SelectedIndex);
                }
            }
        }
        return null;
    }
}