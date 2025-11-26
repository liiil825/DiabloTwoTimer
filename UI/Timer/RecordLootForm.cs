using System;
using System.ComponentModel;
using System.Windows.Forms;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.UI.Common;

namespace DTwoMFTimerHelper.UI.Timer {
    public partial class RecordLootForm : BaseForm {
        private TextBox txtLootName = null!;
        private Label label1 = null!;
        private CheckBox chkPreviousRun = null!;
        private readonly IContainer components = null!;

        private readonly IProfileService? _profileService;
        private readonly ITimerHistoryService? _timerHistoryService;

        public event EventHandler? LootRecordSaved;

        // 1. 无参构造函数 (VS设计器专用)
        public RecordLootForm() {
            InitializeComponent();
        }

        // 2. 依赖注入构造函数 (运行时专用)
        public RecordLootForm(IProfileService? profileService, ITimerHistoryService? timerHistoryService) : this() {
            _profileService = profileService;
            _timerHistoryService = timerHistoryService;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            if (!this.DesignMode) {
                UpdateUI();
            }
        }

        // 添加 OnShown 覆盖代替构造函数里的事件绑定
        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
            if (!this.DesignMode) {
                this.Activate();
                txtLootName.Focus();
                txtLootName.SelectAll();
            }
        }

        private void InitializeComponent() {
            this.txtLootName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkPreviousRun = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtLootName
            // 
            this.txtLootName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLootName.Location = new System.Drawing.Point(12, 29);
            this.txtLootName.Name = "txtLootName";
            this.txtLootName.Size = new System.Drawing.Size(356, 25);
            this.txtLootName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Loot Name";
            // 
            // chkPreviousRun
            // 
            this.chkPreviousRun.AutoSize = true;
            this.chkPreviousRun.Location = new System.Drawing.Point(12, 67);
            this.chkPreviousRun.Name = "chkPreviousRun";
            this.chkPreviousRun.Size = new System.Drawing.Size(117, 19);
            this.chkPreviousRun.TabIndex = 2;
            this.chkPreviousRun.Text = "Previous Run";
            this.chkPreviousRun.UseVisualStyleBackColor = true;
            // 
            // btnConfirm (Inherited)
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.Location = new System.Drawing.Point(212, 92);
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.Text = "保存";
            // 
            // btnCancel (Inherited)
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(293, 92);
            this.btnCancel.Size = new System.Drawing.Size(75, 23);

            // 
            // RecordLootForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 127);
            this.Controls.Add(this.chkPreviousRun);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLootName);
            this.Name = "RecordLootForm";
            this.Text = "记录掉落";

            this.Controls.SetChildIndex(this.btnConfirm, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.txtLootName, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.chkPreviousRun, 0);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        protected override void UpdateUI() {
            base.UpdateUI();
            this.Text = LanguageManager.GetString("RecordLoot") ?? "记录掉落";
            txtLootName.PlaceholderText = LanguageManager.GetString("EnterLootName") ?? "输入掉落名称";
            label1.Text = LanguageManager.GetString("LootNameLabel") ?? "掉落名称:";
            chkPreviousRun.Text = LanguageManager.GetString("UsePreviousRun") ?? "使用上一次符文掉落";
            if (btnConfirm != null) btnConfirm.Text = LanguageManager.GetString("Save") ?? "保存";
        }

        protected override void BtnConfirm_Click(object? sender, EventArgs e) {
            SaveLootRecord();
        }

        private void SaveLootRecord() {
            if (string.IsNullOrWhiteSpace(txtLootName.Text)) {
                Utils.Toast.Warning(LanguageManager.GetString("EnterLootNameMessage") ?? "请输入掉落名称");
                return;
            }

            // 安全检查：如果服务为空（例如从设计器运行或未正确注入），则直接返回
            if (_timerHistoryService == null || _profileService == null) {
                return;
            }

            int runCount = _timerHistoryService.RunCount;
            if (chkPreviousRun.Checked) {
                runCount = Math.Max(0, runCount - 1);
            }

            string sceneName = _profileService.CurrentScene ?? "";
            string englishSceneName = SceneHelper.GetEnglishSceneName(sceneName);

            var lootRecord = new LootRecord {
                Name = txtLootName.Text.Trim(),
                SceneName = englishSceneName,
                RunCount = runCount,
                DropTime = DateTime.Now
            };

            var currentProfile = _profileService.CurrentProfile;
            if (currentProfile != null) {
                currentProfile.LootRecords.Add(lootRecord);
                DataHelper.SaveProfile(currentProfile);
            }

            Utils.Toast.Success(LanguageManager.GetString("LootRecordSavedSuccessfully") ?? "掉落记录保存成功");
            OnLootRecordSaved(EventArgs.Empty);

            DialogResult = DialogResult.OK;
            Close();
        }

        protected virtual void OnLootRecordSaved(EventArgs e) {
            LootRecordSaved?.Invoke(this, e);
        }

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}