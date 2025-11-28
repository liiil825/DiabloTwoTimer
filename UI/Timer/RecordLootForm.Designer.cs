#nullable disable

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Timer
{
    partial class RecordLootForm
    {

        private TextBox txtLootName;
        private Label label1;
        private CheckBox chkPreviousRun;

        private void InitializeComponent()
        {
            this.txtLootName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkPreviousRun = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            //
            // txtLootName
            //
            this.txtLootName.Anchor = (
                (System.Windows.Forms.AnchorStyles)(
                    (
                        (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right
                    )
                )
            );
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
            this.btnConfirm.Anchor = (
                (System.Windows.Forms.AnchorStyles)(
                    (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)
                )
            );
            this.btnConfirm.Location = new System.Drawing.Point(212, 92);
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.Text = "保存";
            //
            // btnCancel (Inherited)
            //
            this.btnCancel.Anchor = (
                (System.Windows.Forms.AnchorStyles)(
                    (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)
                )
            );
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


    }
}