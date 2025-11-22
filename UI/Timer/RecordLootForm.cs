using System;
using System.Windows.Forms;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.Utils;

namespace DTwoMFTimerHelper.UI.Timer {
    public partial class RecordLootForm : Form {
        private readonly IProfileService? _profileService;
        private readonly ITimerHistoryService? _timerHistoryService;

        public RecordLootForm(IProfileService? profileService, ITimerHistoryService? timerHistoryService) {
            InitializeComponent();

            if (profileService == null || timerHistoryService == null) {
                return;
            }

            _profileService = profileService;
            _timerHistoryService = timerHistoryService;

            // 设置默认焦点到装备名称输入框
            txtLootName.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(txtLootName.Text)) {
                MessageBox.Show(LanguageManager.GetString("EnterLootNameMessage"),
                LanguageManager.GetString("NotificationTitle"),
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 创建新的掉落记录
            int runCount = _timerHistoryService.RunCount;
            if (chkPreviousRun.Checked) {
                runCount = Math.Max(0, runCount - 1);
            }

            string sceneName = _profileService.CurrentScene ?? "";

            var lootRecord = new LootRecord {
                Name = txtLootName.Text.Trim(),
                SceneName = sceneName,
                RunCount = runCount,
                DropTime = DateTime.Now
            };

            // 将掉落记录添加到当前角色档案中
            var currentProfile = _profileService.CurrentProfile;
            if (currentProfile != null) {
                currentProfile.LootRecords.Add(lootRecord);
                Services.DataService.SaveProfile(currentProfile); // 保存修改
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void InitializeComponent() {
            this.txtLootName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkPreviousRun = new System.Windows.Forms.CheckBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtLootName
            // 
            this.txtLootName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLootName.Location = new System.Drawing.Point(12, 29);
            this.txtLootName.Name = "txtLootName";
            this.txtLootName.Size = new System.Drawing.Size(356, 23);
            this.txtLootName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = LanguageManager.GetString("LootNameLabel");
            // 
            // chkPreviousRun
            // 
            this.chkPreviousRun.AutoSize = true;
            this.chkPreviousRun.Location = new System.Drawing.Point(12, 67);
            this.chkPreviousRun.Name = "chkPreviousRun";
            this.chkPreviousRun.Size = new System.Drawing.Size(265, 19);
            this.chkPreviousRun.TabIndex = 2;
            this.chkPreviousRun.Text = LanguageManager.GetString("PreviousRunCheckbox");
            this.chkPreviousRun.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(212, 92);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = LanguageManager.GetString("SaveButton");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(293, 92);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = LanguageManager.GetString("CancelButton");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // RecordLootForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 127);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.chkPreviousRun);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLootName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecordLootForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = LanguageManager.GetString("RecordLootFormTitle");
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.TextBox? txtLootName;
        private System.Windows.Forms.Label? label1;
        private System.Windows.Forms.CheckBox? chkPreviousRun;
        private System.Windows.Forms.Button? btnSave;
        private System.Windows.Forms.Button? btnCancel;
    }
}