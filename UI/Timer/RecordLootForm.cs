using System;
using System.ComponentModel;
using System.Windows.Forms;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.UI.Common;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Timer
{
    public partial class RecordLootForm : BaseForm
    {
        private readonly IProfileService _profileService = null!;
        private readonly ITimerHistoryService _timerHistoryService = null!;

        public event EventHandler? LootRecordSaved;

        // 1. 无参构造函数 (VS设计器专用)
        public RecordLootForm()
        {
            InitializeComponent();
        }

        // 2. 依赖注入构造函数 (运行时专用)
        public RecordLootForm(IProfileService profileService, ITimerHistoryService timerHistoryService)
            : this()
        {
            _profileService = profileService;
            _timerHistoryService = timerHistoryService;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!this.DesignMode)
            {
                UpdateUI();
            }
        }

        // 添加 OnShown 覆盖代替构造函数里的事件绑定
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (!this.DesignMode)
            {
                this.Activate();
                txtLootName.Focus();
                txtLootName.SelectAll();
            }
        }

        protected override void UpdateUI()
        {
            base.UpdateUI();
            this.Text = LanguageManager.GetString("RecordLoot") ?? "记录掉落";
            txtLootName.PlaceholderText = LanguageManager.GetString("EnterLootName") ?? "输入掉落名称";
            label1.Text = LanguageManager.GetString("LootNameLabel") ?? "掉落名称:";
            chkPreviousRun.Text = LanguageManager.GetString("UsePreviousRun") ?? "使用上一次符文掉落";
            if (btnConfirm != null)
                btnConfirm.Text = LanguageManager.GetString("Save") ?? "保存";
        }

        protected override void BtnConfirm_Click(object? sender, EventArgs e)
        {
            SaveLootRecord();
        }

        private void SaveLootRecord()
        {
            if (string.IsNullOrWhiteSpace(txtLootName.Text))
            {
                Utils.Toast.Warning(LanguageManager.GetString("EnterLootNameMessage") ?? "请输入掉落名称");
                return;
            }

            int runCount = _timerHistoryService.RunCount + 1;
            if (chkPreviousRun.Checked)
            {
                runCount = Math.Max(0, runCount - 1);
            }

            string sceneName = _profileService.CurrentScene ?? "";
            string englishSceneName = SceneHelper.GetEnglishSceneName(sceneName);

            var lootRecord = new LootRecord
            {
                Name = txtLootName.Text.Trim(),
                SceneName = englishSceneName,
                RunCount = runCount,
                DropTime = DateTime.Now,
            };

            var currentProfile = _profileService.CurrentProfile;
            if (currentProfile != null)
            {
                currentProfile.LootRecords.Add(lootRecord);
                DataHelper.SaveProfile(currentProfile);
            }

            Utils.Toast.Success(LanguageManager.GetString("LootRecordSavedSuccessfully") ?? "掉落记录保存成功");
            OnLootRecordSaved(EventArgs.Empty);

            DialogResult = DialogResult.OK;
            Close();
        }

        protected virtual void OnLootRecordSaved(EventArgs e)
        {
            LootRecordSaved?.Invoke(this, e);
        }


    }
}
