using System;
using System.ComponentModel;
using System.Windows.Forms;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.UI.Common;

namespace DTwoMFTimerHelper.UI.Profiles {
    public class SwitchCharacterForm : BaseForm {
        // 使用 null! 消除 CS8618，因为 InitializeComponent 会初始化它们
        private ListBox lstCharacters = null!;
        private Label lblCharacters = null!;

        // 必需的设计器变量
        private readonly IContainer components = null!;

        public Models.CharacterProfile? SelectedProfile { get; private set; }

        public SwitchCharacterForm() {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            if (!this.DesignMode) {
                UpdateUI();
                LoadProfiles();
            }
        }

        private void InitializeComponent() {
            this.lblCharacters = new System.Windows.Forms.Label();
            this.lstCharacters = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lblCharacters
            // 
            this.lblCharacters.AutoSize = true;
            this.lblCharacters.Location = new System.Drawing.Point(30, 20);
            this.lblCharacters.Name = "lblCharacters";
            this.lblCharacters.Size = new System.Drawing.Size(90, 15);
            this.lblCharacters.TabIndex = 0;
            this.lblCharacters.Text = "Select Char:";
            // 
            // lstCharacters
            // 
            this.lstCharacters.FormattingEnabled = true;
            this.lstCharacters.ItemHeight = 15;
            this.lstCharacters.Location = new System.Drawing.Point(30, 50);
            this.lstCharacters.Name = "lstCharacters";
            this.lstCharacters.Size = new System.Drawing.Size(320, 154);
            this.lstCharacters.TabIndex = 1;
            // 
            // btnConfirm (Inherited)
            // 
            this.btnConfirm.Location = new System.Drawing.Point(120, 230);
            this.btnConfirm.Text = "选择"; // 默认文本
            // 
            // btnCancel (Inherited)
            // 
            this.btnCancel.Location = new System.Drawing.Point(250, 230);
            // 
            // SwitchCharacterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 350);
            this.Controls.Add(this.lstCharacters);
            this.Controls.Add(this.lblCharacters);
            this.Name = "SwitchCharacterForm";
            this.Text = "切换角色档案";

            // 重要：必须保留对基类控件的引用
            this.Controls.SetChildIndex(this.btnConfirm, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.lblCharacters, 0);
            this.Controls.SetChildIndex(this.lstCharacters, 0);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        protected override void UpdateUI() {
            base.UpdateUI();
            this.Text = LanguageManager.GetString("SwitchCharacter") ?? "切换角色档案";

            if (lblCharacters != null)
                lblCharacters.Text = LanguageManager.GetString("SelectCharacter") ?? "选择角色:";

            if (btnConfirm != null)
                btnConfirm.Text = LanguageManager.GetString("Select") ?? "选择";
        }

        private void LoadProfiles() {
            try {
                LogManager.WriteDebugLog("SwitchCharacterForm", "[详细调试] 开始加载角色档案名称...");
                var profileNames = DataHelper.GetProfileNames();

                lstCharacters.Items.Clear();

                foreach (var profileName in profileNames) {
                    var profileItem = new ProfileItem(profileName);
                    lstCharacters.Items.Add(profileItem);
                }

                if (lstCharacters.Items.Count > 0) {
                    lstCharacters.SelectedIndex = 0;
                }

                UpdateEmptyListMessage();
            }
            catch (Exception ex) {
                LogManager.WriteDebugLog("SwitchCharacterForm", $"加载角色档案异常: {ex.Message}");
                MessageBox.Show($"{LanguageManager.GetString("ErrorLoadingProfiles") ?? "加载角色档案失败"}: {ex.Message}",
                    LanguageManager.GetString("Error") ?? "错误");
            }
        }

        private void UpdateEmptyListMessage() {
            if (lstCharacters.Items.Count == 0) {
                string emptyMessage = LanguageManager.GetString("NoCharactersFound") ?? "没有找到角色档案";
                lstCharacters.Items.Add(emptyMessage);
                btnConfirm.Enabled = false;
            }
            else {
                // 如果之前禁用了，且现在不是显示空消息的状态，则启用
                // 简单的逻辑：只要选中的不是字符串类型的提示消息，就启用
                btnConfirm.Enabled = !(lstCharacters.Items.Count == 1 && lstCharacters.Items[0] is string);
            }
        }

        // 注意：复用基类的 Confirm 点击事件，不需要重新 +=，只需要重写逻辑
        protected override void BtnConfirm_Click(object? sender, EventArgs e) {
            if (lstCharacters.SelectedItem is ProfileItem profileItem) {
                var selectedProfile = DataHelper.LoadProfileByName(profileItem.ProfileName);

                if (selectedProfile != null && !string.IsNullOrEmpty(selectedProfile.Name)) {
                    SelectedProfile = selectedProfile;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else {
                    Utils.Toast.Warning(LanguageManager.GetString("InvalidCharacter") ?? "选中的角色数据无效");
                }
            }
            else if (lstCharacters.Items.Count > 0 && lstCharacters.Items[0] is string) {
                Utils.Toast.Warning(LanguageManager.GetString("NoCharactersAvailable") ?? "没有可用的角色档案，请先创建角色。");
            }
            else {
                Utils.Toast.Warning(LanguageManager.GetString("SelectCharacterFirst") ?? "请先选择一个角色");
            }
        }

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // 内部类优化
        private class ProfileItem {
            public string ProfileName { get; }
            public string DisplayName { get; }

            public ProfileItem(string profileName) {
                ProfileName = profileName;
                DisplayName = profileName; // 默认显示

                // 尝试加载概要信息用于显示（可选优化：避免完全加载Profile）
                // 这里为了保持原始逻辑，如果需要完整信息可以在这里处理
            }

            public override string ToString() {
                // ListBox 默认调用 ToString()
                return DisplayName;
            }
        }
    }
}