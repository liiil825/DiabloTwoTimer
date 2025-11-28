using System;
using System.ComponentModel;
using System.Windows.Forms;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Common
{
    /// <summary>
    /// 通用弹窗基类
    /// </summary>
    public class BaseForm : Form
    {
        // 将控件公开给继承类，但建议使用 protected
        protected Button btnConfirm = null!;
        protected Button btnCancel = null!;
        private readonly IContainer components = null!;

        // 属性
        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text for the Confirm button")]
        public string ConfirmButtonText { get; set; } = "确认";

        [Browsable(true)]
        [Category("Appearance")]
        [Description("Text for the Cancel button")]
        public string CancelButtonText { get; set; } = "取消";

        public BaseForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 标准的 WinForm 初始化方法
        /// </summary>
        private void InitializeComponent()
        {
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // btnConfirm
            //
            this.btnConfirm.Anchor = (
                (System.Windows.Forms.AnchorStyles)(
                    (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)
                )
            );
            this.btnConfirm.Location = new System.Drawing.Point(197, 260); // 默认位置，子类可覆盖
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(80, 30);
            this.btnConfirm.TabIndex = 100;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.BtnConfirm_Click);
            //
            // btnCancel
            //
            this.btnCancel.Anchor = (
                (System.Windows.Forms.AnchorStyles)(
                    (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)
                )
            );
            this.btnCancel.Location = new System.Drawing.Point(292, 260); // 默认位置，子类可覆盖
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 101;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            //
            // BaseForm
            //
            this.ClientSize = new System.Drawing.Size(400, 300);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Name = "BaseForm";
            this.ResumeLayout(false);
        }

        // 这里的逻辑移到 OnLoad 比较安全，确保 Handle 已创建
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // 仅在运行时更新文本，避免设计器因为找不到 LanguageManager 报错
            if (!this.DesignMode)
            {
                UpdateUI();
            }
        }

        protected virtual void UpdateUI()
        {
            // 使用 ?. 操作符防止空引用，虽然在 InitializeComponent 后一般不会为空
            if (btnConfirm != null)
                btnConfirm.Text = LanguageManager.GetString("Confirm") ?? ConfirmButtonText;

            if (btnCancel != null)
                btnCancel.Text = LanguageManager.GetString("Cancel") ?? CancelButtonText;
        }

        protected virtual void BtnConfirm_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        protected virtual void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                BtnCancel_Click(null, EventArgs.Empty);
                return true;
            }
            if (keyData == Keys.Enter)
            {
                // 如果焦点在按钮上，让按钮自己处理，否则默认触发确认
                if (!btnConfirm!.Focused && !btnCancel!.Focused)
                {
                    BtnConfirm_Click(null, EventArgs.Empty);
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // 标准的 Dispose 模式
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
