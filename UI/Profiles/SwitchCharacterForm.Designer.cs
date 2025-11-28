#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Profiles;
partial class SwitchCharacterForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
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

    #endregion

    private ListBox lstCharacters;
    private Label lblCharacters;
}