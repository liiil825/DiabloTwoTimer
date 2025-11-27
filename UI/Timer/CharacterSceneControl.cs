using System;
using System.Drawing;
using System.Windows.Forms;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Timer
{
    public partial class CharacterSceneControl : UserControl
    {
        private IProfileService? _profileService;
        private bool _isInitialized = false; // 用于防止重复订阅

        public CharacterSceneControl()
        {
            InitializeComponent();
        }

        public void Initialize(IProfileService profileService)
        {
            if (_isInitialized || profileService == null)
                return;

            // 赋值服务
            _profileService = profileService;

            // 注册语言变更事件（LanguageManager 假设是静态的，设计器中可能也需要）
            Utils.LanguageManager.OnLanguageChanged += LanguageManager_OnLanguageChanged;

            // 注册ProfileService事件（只有在服务不为空时才订阅）
            _profileService.CurrentProfileChangedEvent += OnProfileChanged;
            _profileService.CurrentSceneChangedEvent += OnSceneChanged;
            _profileService.CurrentDifficultyChangedEvent += OnDifficultyChanged;

            _isInitialized = true;
            UpdateUI();
        }

        protected override void Dispose(bool disposing)
        {
            if (_profileService == null)
                return;

            if (disposing)
            { // 取消注册语言变更事件
                Utils.LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
                // 取消注册ProfileService事件
                _profileService.CurrentProfileChangedEvent -= OnProfileChanged;
                _profileService.CurrentSceneChangedEvent -= OnSceneChanged;
                _profileService.CurrentDifficultyChangedEvent -= OnDifficultyChanged;
            }
            base.Dispose(disposing);
        }

        private void OnProfileChanged(CharacterProfile? profile) => UpdateUI();

        private void OnSceneChanged(string scene) => UpdateUI();

        private void OnDifficultyChanged(GameDifficulty difficulty) => UpdateUI();

        private void LanguageManager_OnLanguageChanged(object? sender, EventArgs e) => UpdateUI();

        public void UpdateCharacterSceneInfo() => UpdateUI();

        /// <summary>
        /// 保存角色和场景设置
        /// </summary>
        /// <param name="character">角色名称</param>
        /// <param name="scene">场景名称</param>
        public void UpdateUI()
        {
            if (_profileService == null)
                return;

            // 更新角色显示 (直接使用变量，无需判空，因为 InitializeComponent 保证了它们存在)
            var profile = _profileService.CurrentProfile;
            if (profile == null)
            {
                this.lblCharacterDisplay.Text = "";
            }
            else
            {
                string characterClass = Utils.LanguageManager.GetLocalizedClassName(profile.Class);
                this.lblCharacterDisplay.Text = $"{profile.Name} ({characterClass})";
            }

            // 更新场景显示
            string currentScene = _profileService.CurrentScene;
            if (string.IsNullOrEmpty(currentScene))
            {
                this.lblSceneDisplay.Text = "";
            }
            else
            {
                string localizedSceneName = Utils.LanguageManager.GetString(currentScene);
                string localizedDifficultyName = _profileService.CurrentDifficultyLocalized;
                this.lblSceneDisplay.Text = $"{localizedDifficultyName} {localizedSceneName}";
            }
        }
    }
}
