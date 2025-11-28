#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Profiles;
partial class CreateCharacterForm
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

    #endregion

    private Label lblCharacterName;
    private TextBox txtCharacterName;
    private Label lblCharacterClass;
    private ComboBox cmbCharacterClass;
}