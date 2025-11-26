using System;
using System.Windows.Forms;
using System.ComponentModel;
using DTwoMFTimerHelper.Models;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.UI.Common;

namespace DTwoMFTimerHelper.UI.Profiles {
    public class CreateCharacterForm : BaseForm {
        private Label lblCharacterName = null!;
        private TextBox txtCharacterName = null!;
        private Label lblCharacterClass = null!;
        private ComboBox cmbCharacterClass = null!;

        // 必需的设计器变量
        private readonly IContainer components = null!;

        public string? CharacterName => txtCharacterName?.Text.Trim();

        public CreateCharacterForm() {
            // 构造函数只负责初始化组件，不做业务逻辑
            InitializeComponent();
        }

        // 将业务逻辑移至 OnLoad，防止设计器打开时崩溃
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e); // 调用基类 OnLoad (触发 BaseForm 的 UpdateUI)

            if (!this.DesignMode) // 再次确保设计模式下不运行
            {
                SetupCharacterClasses();
                UpdateUI();
            }
        }

        private void InitializeComponent() {
            this.lblCharacterName = new System.Windows.Forms.Label();
            this.txtCharacterName = new System.Windows.Forms.TextBox();
            this.lblCharacterClass = new System.Windows.Forms.Label();
            this.cmbCharacterClass = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();

            // 
            // lblCharacterName
            // 
            this.lblCharacterName.AutoSize = true;
            this.lblCharacterName.Location = new System.Drawing.Point(50, 33);
            this.lblCharacterName.Name = "lblCharacterName";
            this.lblCharacterName.Size = new System.Drawing.Size(60, 15); // 建议由 AutoSize 控制
            this.lblCharacterName.TabIndex = 0;
            this.lblCharacterName.Text = "Name:";

            // 
            // txtCharacterName
            // 
            this.txtCharacterName.Location = new System.Drawing.Point(150, 30);
            this.txtCharacterName.Name = "txtCharacterName";
            this.txtCharacterName.Size = new System.Drawing.Size(180, 25);
            this.txtCharacterName.TabIndex = 1;

            // 
            // lblCharacterClass
            // 
            this.lblCharacterClass.AutoSize = true;
            this.lblCharacterClass.Location = new System.Drawing.Point(50, 73);
            this.lblCharacterClass.Name = "lblCharacterClass";
            this.lblCharacterClass.Size = new System.Drawing.Size(60, 15);
            this.lblCharacterClass.TabIndex = 2;
            this.lblCharacterClass.Text = "Class:";

            // 
            // cmbCharacterClass
            // 
            this.cmbCharacterClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCharacterClass.FormattingEnabled = true;
            this.cmbCharacterClass.Location = new System.Drawing.Point(150, 70);
            this.cmbCharacterClass.Name = "cmbCharacterClass";
            this.cmbCharacterClass.Size = new System.Drawing.Size(180, 23);
            this.cmbCharacterClass.TabIndex = 3;

            // 
            // btnConfirm (调整继承自基类的按钮位置)
            // 
            this.btnConfirm.Location = new System.Drawing.Point(120, 230);
            this.btnConfirm.TabIndex = 4;

            // 
            // btnCancel (调整继承自基类的按钮位置)
            // 
            this.btnCancel.Location = new System.Drawing.Point(250, 230);
            this.btnCancel.TabIndex = 5;

            // 
            // CreateCharacterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 300);
            this.Controls.Add(this.cmbCharacterClass);
            this.Controls.Add(this.lblCharacterClass);
            this.Controls.Add(this.txtCharacterName);
            this.Controls.Add(this.lblCharacterName);
            this.Name = "CreateCharacterForm";
            this.Text = "创建角色档案";

            // 必须调用 ResumeLayout
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void SetupCharacterClasses() {
            // 健壮性检查
            if (cmbCharacterClass == null) return;

            cmbCharacterClass.Items.Clear();
            foreach (CharacterClass charClass in Enum.GetValues(typeof(CharacterClass))) {
                // 此时还未加载本地化文本，先存入 Enum 值，DisplayMember 会处理显示，或者在 UpdateUI 统一处理
                // 既然你在 UpdateUI 里会清除重加，这里其实可以简化，只做默认初始化
                cmbCharacterClass.Items.Add(charClass);
            }
            if (cmbCharacterClass.Items.Count > 0)
                cmbCharacterClass.SelectedIndex = 0;
        }

        protected override void UpdateUI() {
            // 这里的 base.UpdateUI() 会调用 BaseForm 的逻辑，更新确认/取消按钮
            base.UpdateUI();

            this.Text = LanguageManager.GetString("CreateCharacter") ?? "创建";
            lblCharacterName!.Text = LanguageManager.GetString("CharacterName") ?? "角色名称:";
            lblCharacterClass!.Text = LanguageManager.GetString("CharacterClass") ?? "职业:";

            // 重新填充下拉框以应用本地化
            if (cmbCharacterClass != null) {
                // 记录当前选中的索引，避免刷新后丢失选择
                int oldIndex = cmbCharacterClass.SelectedIndex;
                // 如果之前没选中（比如刚初始化），默认为0
                if (oldIndex < 0) oldIndex = 0;

                cmbCharacterClass.Items.Clear();

                foreach (CharacterClass charClass in Enum.GetValues(typeof(CharacterClass))) {
                    string displayName = LanguageManager.GetLocalizedClassName(charClass);
                    // 这里可以直接存对象，利用 Tag 或者后续反查，
                    // 但为了配合你的 GetCharacterClassFromLocalizedName 逻辑，这里存字符串
                    cmbCharacterClass.Items.Add(displayName);
                }

                if (cmbCharacterClass.Items.Count > 0) {
                    // 确保索引不越界
                    cmbCharacterClass.SelectedIndex = Math.Min(oldIndex, cmbCharacterClass.Items.Count - 1);
                }
            }
        }

        protected override void BtnConfirm_Click(object? sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(CharacterName)) {
                Utils.Toast.Info(LanguageManager.GetString("EnterCharacterName") ?? "请输入角色名称");
                txtCharacterName!.Focus(); // 聚焦到输入框
                return;
            }

            if (DataHelper.FindProfileByName(CharacterName) != null) {
                Utils.Toast.Warning(LanguageManager.GetString("CharacterExists") ?? "该角色名称已存在");
                txtCharacterName!.SelectAll();
                txtCharacterName!.Focus();
                return;
            }

            base.BtnConfirm_Click(sender, e);
        }

        public CharacterClass? GetSelectedClass() {
            if (cmbCharacterClass?.SelectedItem != null) {
                string localizedName = cmbCharacterClass.SelectedItem.ToString()!;
                return GetCharacterClassFromLocalizedName(localizedName);
            }
            return null;
        }

        private static CharacterClass GetCharacterClassFromLocalizedName(string localizedName) {
            foreach (CharacterClass charClass in Enum.GetValues(typeof(CharacterClass))) {
                if (LanguageManager.GetLocalizedClassName(charClass).Equals(localizedName, StringComparison.OrdinalIgnoreCase))
                    return charClass;
            }
            return CharacterClass.Barbarian;
        }

        // 记得实现 Dispose 释放资源
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}