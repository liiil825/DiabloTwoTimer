#nullable disable

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DTwoMFTimerHelper.UI.Timer
{
    partial class CharacterSceneControl
    {

        private Label lblCharacterDisplay;
        private Label lblSceneDisplay;

        private void InitializeComponent()
        {
            this.lblCharacterDisplay = new System.Windows.Forms.Label();
            this.lblSceneDisplay = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // lblCharacterDisplay
            //
            this.lblCharacterDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblCharacterDisplay.Font = new System.Drawing.Font(
                "Microsoft YaHei UI",
                12F,
                System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point,
                ((byte)(134))
            );
            this.lblCharacterDisplay.Location = new System.Drawing.Point(0, 0);
            this.lblCharacterDisplay.Name = "lblCharacterDisplay";
            this.lblCharacterDisplay.Size = new System.Drawing.Size(290, 25);
            this.lblCharacterDisplay.TabIndex = 0;
            this.lblCharacterDisplay.Text = "初始化角色";
            this.lblCharacterDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblSceneDisplay
            //
            this.lblSceneDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblSceneDisplay.Font = new System.Drawing.Font(
                "Microsoft YaHei UI",
                12F,
                System.Drawing.FontStyle.Regular,
                System.Drawing.GraphicsUnit.Point,
                ((byte)(134))
            );
            this.lblSceneDisplay.Location = new System.Drawing.Point(0, 25);
            this.lblSceneDisplay.Name = "lblSceneDisplay";
            this.lblSceneDisplay.Size = new System.Drawing.Size(290, 25);
            this.lblSceneDisplay.TabIndex = 1;
            this.lblSceneDisplay.Text = "初始化场景";
            this.lblSceneDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // CharacterSceneControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            // 关键修改：使用 Controls.Add 而不是 Parent = this
            this.Controls.Add(this.lblSceneDisplay);
            this.Controls.Add(this.lblCharacterDisplay);
            this.Name = "CharacterSceneControl";
            this.Size = new System.Drawing.Size(290, 50);
            this.ResumeLayout(false);
        }


    }
}