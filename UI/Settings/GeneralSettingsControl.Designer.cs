#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Settings;
partial class GeneralSettingsControl
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

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        groupBoxPosition = new GroupBox();
        radioTopLeft = new RadioButton();
        radioTopCenter = new RadioButton();
        radioTopRight = new RadioButton();
        radioBottomLeft = new RadioButton();
        radioBottomCenter = new RadioButton();
        radioBottomRight = new RadioButton();
        groupBoxLanguage = new GroupBox();
        chineseRadioButton = new RadioButton();
        englishRadioButton = new RadioButton();
        alwaysOnTopCheckBox = new CheckBox();
        alwaysOnTopLabel = new Label();
        groupBoxPosition.SuspendLayout();
        groupBoxLanguage.SuspendLayout();
        SuspendLayout();
        //
        // groupBoxPosition
        //
        groupBoxPosition.Controls.Add(radioTopLeft);
        groupBoxPosition.Controls.Add(radioTopCenter);
        groupBoxPosition.Controls.Add(radioTopRight);
        groupBoxPosition.Controls.Add(radioBottomLeft);
        groupBoxPosition.Controls.Add(radioBottomCenter);
        groupBoxPosition.Controls.Add(radioBottomRight);
        groupBoxPosition.Location = new Point(8, 8);
        groupBoxPosition.Name = "groupBoxPosition";
        groupBoxPosition.Size = new Size(300, 110);
        groupBoxPosition.TabIndex = 0;
        groupBoxPosition.TabStop = false;
        groupBoxPosition.Text = "窗口位置";
        //
        // radioTopLeft
        //
        radioTopLeft.AutoSize = true;
        radioTopLeft.Checked = true;
        radioTopLeft.Location = new Point(15, 25);
        radioTopLeft.Name = "radioTopLeft";
        radioTopLeft.Size = new Size(79, 32);
        radioTopLeft.TabIndex = 0;
        radioTopLeft.TabStop = true;
        radioTopLeft.Text = "左上";
        //
        // radioTopCenter
        //
        radioTopCenter.AutoSize = true;
        radioTopCenter.Location = new Point(110, 25);
        radioTopCenter.Name = "radioTopCenter";
        radioTopCenter.Size = new Size(79, 32);
        radioTopCenter.TabIndex = 1;
        radioTopCenter.Text = "上中";
        //
        // radioTopRight
        //
        radioTopRight.AutoSize = true;
        radioTopRight.Location = new Point(205, 25);
        radioTopRight.Name = "radioTopRight";
        radioTopRight.Size = new Size(79, 32);
        radioTopRight.TabIndex = 2;
        radioTopRight.Text = "右上";
        //
        // radioBottomLeft
        //
        radioBottomLeft.AutoSize = true;
        radioBottomLeft.Location = new Point(15, 60);
        radioBottomLeft.Name = "radioBottomLeft";
        radioBottomLeft.Size = new Size(79, 32);
        radioBottomLeft.TabIndex = 3;
        radioBottomLeft.Text = "左下";
        //
        // radioBottomCenter
        //
        radioBottomCenter.AutoSize = true;
        radioBottomCenter.Location = new Point(110, 60);
        radioBottomCenter.Name = "radioBottomCenter";
        radioBottomCenter.Size = new Size(79, 32);
        radioBottomCenter.TabIndex = 4;
        radioBottomCenter.Text = "下中";
        //
        // radioBottomRight
        //
        radioBottomRight.AutoSize = true;
        radioBottomRight.Location = new Point(205, 60);
        radioBottomRight.Name = "radioBottomRight";
        radioBottomRight.Size = new Size(79, 32);
        radioBottomRight.TabIndex = 5;
        radioBottomRight.Text = "右下";
        //
        // groupBoxLanguage
        //
        groupBoxLanguage.Controls.Add(chineseRadioButton);
        groupBoxLanguage.Controls.Add(englishRadioButton);
        groupBoxLanguage.Location = new Point(8, 125);
        groupBoxLanguage.Name = "groupBoxLanguage";
        groupBoxLanguage.Size = new Size(300, 70);
        groupBoxLanguage.TabIndex = 1;
        groupBoxLanguage.TabStop = false;
        groupBoxLanguage.Text = "语言";
        //
        // chineseRadioButton
        //
        chineseRadioButton.AutoSize = true;
        chineseRadioButton.Checked = true;
        chineseRadioButton.Location = new Point(15, 30);
        chineseRadioButton.Name = "chineseRadioButton";
        chineseRadioButton.Size = new Size(117, 32);
        chineseRadioButton.TabIndex = 0;
        chineseRadioButton.TabStop = true;
        chineseRadioButton.Text = "Chinese";
        //
        // englishRadioButton
        //
        englishRadioButton.AutoSize = true;
        englishRadioButton.Location = new Point(155, 30);
        englishRadioButton.Name = "englishRadioButton";
        englishRadioButton.Size = new Size(110, 32);
        englishRadioButton.TabIndex = 1;
        englishRadioButton.Text = "English";
        //
        // alwaysOnTopCheckBox
        //
        alwaysOnTopCheckBox.AutoSize = true;
        alwaysOnTopCheckBox.Checked = true;
        alwaysOnTopCheckBox.CheckState = CheckState.Checked;
        alwaysOnTopCheckBox.Location = new Point(23, 211);
        alwaysOnTopCheckBox.Name = "alwaysOnTopCheckBox";
        alwaysOnTopCheckBox.Size = new Size(22, 21);
        alwaysOnTopCheckBox.TabIndex = 3;
        //
        // alwaysOnTopLabel
        //
        alwaysOnTopLabel.AutoSize = true;
        alwaysOnTopLabel.Location = new Point(51, 206);
        alwaysOnTopLabel.Name = "alwaysOnTopLabel";
        alwaysOnTopLabel.Size = new Size(96, 28);
        alwaysOnTopLabel.TabIndex = 2;
        alwaysOnTopLabel.Text = "总在最前";
        //
        // GeneralSettingsControl
        //
        AutoScroll = true;
        Controls.Add(groupBoxPosition);
        Controls.Add(groupBoxLanguage);
        Controls.Add(alwaysOnTopLabel);
        Controls.Add(alwaysOnTopCheckBox);
        Name = "GeneralSettingsControl";
        Size = new Size(320, 280);
        groupBoxPosition.ResumeLayout(false);
        groupBoxPosition.PerformLayout();
        groupBoxLanguage.ResumeLayout(false);
        groupBoxLanguage.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private GroupBox groupBoxPosition;
    private GroupBox groupBoxLanguage;
    private RadioButton radioTopLeft;
    private RadioButton radioTopCenter;
    private RadioButton radioTopRight;
    private RadioButton radioBottomLeft;
    private RadioButton radioBottomCenter;
    private RadioButton radioBottomRight;
    private RadioButton chineseRadioButton;
    private RadioButton englishRadioButton;
    private CheckBox alwaysOnTopCheckBox;
    private Label alwaysOnTopLabel;
}