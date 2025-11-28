#nullable disable

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Profiles
{
    partial class ProfileManager
    {

        private Button btnCreateCharacter;
        private Button btnSwitchCharacter;
        private Button btnDeleteCharacter;
        private Label lblScene;
        private ComboBox cmbScene;
        private Label lblDifficulty;
        private ComboBox cmbDifficulty;
        private Button btnStartFarm;
        private Button btnShowStats;
        private Label lblCurrentProfile;
        private Label lblTime;
        private Label lblStats;

        private void InitializeComponent()
        {
            btnCreateCharacter = new Button();
            btnSwitchCharacter = new Button();
            btnDeleteCharacter = new Button();
            lblScene = new Label();
            cmbScene = new ComboBox();
            lblDifficulty = new Label();
            cmbDifficulty = new ComboBox();
            btnStartFarm = new Button();
            btnShowStats = new Button();
            lblCurrentProfile = new Label();
            lblTime = new Label();
            lblStats = new Label();
            SuspendLayout();
            //
            // btnCreateCharacter
            //
            btnCreateCharacter.Location = new System.Drawing.Point(30, 30);
            btnCreateCharacter.Margin = new Padding(4);
            btnCreateCharacter.Name = "btnCreateCharacter";
            btnCreateCharacter.Size = new System.Drawing.Size(105, 40);
            btnCreateCharacter.TabIndex = 0;
            btnCreateCharacter.UseVisualStyleBackColor = true;
            btnCreateCharacter.Click += BtnCreateCharacter_Click;
            //
            // btnSwitchCharacter
            //
            btnSwitchCharacter.Location = new System.Drawing.Point(176, 30);
            btnSwitchCharacter.Margin = new Padding(4);
            btnSwitchCharacter.Name = "btnSwitchCharacter";
            btnSwitchCharacter.Size = new System.Drawing.Size(105, 40);
            btnSwitchCharacter.TabIndex = 1;
            btnSwitchCharacter.UseVisualStyleBackColor = true;
            btnSwitchCharacter.Click += BtnSwitchCharacter_Click;
            //
            // btnDeleteCharacter
            //
            btnDeleteCharacter.Enabled = false;
            btnDeleteCharacter.Location = new System.Drawing.Point(312, 30);
            btnDeleteCharacter.Margin = new Padding(4);
            btnDeleteCharacter.Name = "btnDeleteCharacter";
            btnDeleteCharacter.Size = new System.Drawing.Size(100, 40);
            btnDeleteCharacter.TabIndex = 2;
            btnDeleteCharacter.UseVisualStyleBackColor = true;
            btnDeleteCharacter.Click += BtnDeleteCharacter_Click;
            //
            // lblScene
            //
            lblScene.AutoSize = true;
            lblScene.Location = new System.Drawing.Point(30, 100);
            lblScene.Margin = new Padding(6, 0, 6, 0);
            lblScene.Name = "lblScene";
            lblScene.Size = new System.Drawing.Size(0, 28);
            lblScene.TabIndex = 3;
            //
            // cmbScene
            //
            cmbScene.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbScene.Location = new System.Drawing.Point(130, 100);
            cmbScene.Margin = new Padding(6);
            cmbScene.Name = "cmbScene";
            cmbScene.Size = new System.Drawing.Size(250, 36);
            cmbScene.TabIndex = 4;
            cmbScene.SelectedIndexChanged += CmbScene_SelectedIndexChanged;
            //
            // lblDifficulty
            //
            lblDifficulty.AutoSize = true;
            lblDifficulty.Location = new System.Drawing.Point(30, 160);
            lblDifficulty.Margin = new Padding(6, 0, 6, 0);
            lblDifficulty.Name = "lblDifficulty";
            lblDifficulty.Size = new System.Drawing.Size(0, 28);
            lblDifficulty.TabIndex = 5;
            //
            // cmbDifficulty
            //
            cmbDifficulty.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDifficulty.Location = new System.Drawing.Point(130, 160);
            cmbDifficulty.Margin = new Padding(6);
            cmbDifficulty.Name = "cmbDifficulty";
            cmbDifficulty.Size = new System.Drawing.Size(250, 36);
            cmbDifficulty.TabIndex = 6;
            cmbDifficulty.SelectedIndexChanged += CmbDifficulty_SelectedIndexChanged;
            //
            // btnStartFarm
            //
            btnStartFarm.Enabled = false;
            btnStartFarm.Location = new System.Drawing.Point(30, 220);
            btnStartFarm.Margin = new Padding(6);
            btnStartFarm.Name = "btnStartFarm";
            btnStartFarm.Size = new System.Drawing.Size(130, 50);
            btnStartFarm.TabIndex = 7;
            btnStartFarm.UseVisualStyleBackColor = true;
            btnStartFarm.Click += BtnStartFarm_Click;
            //
            // btnShowStats
            //
            btnShowStats.Location = new System.Drawing.Point(200, 220);
            btnShowStats.Margin = new Padding(6);
            btnShowStats.Name = "btnShowStats";
            btnShowStats.Size = new System.Drawing.Size(130, 50);
            btnShowStats.TabIndex = 8;
            btnShowStats.Text = "全屏统计";
            btnShowStats.Click += BtnShowStats_Click;
            //
            // lblCurrentProfile
            //
            lblCurrentProfile.AutoSize = true;
            lblCurrentProfile.Location = new System.Drawing.Point(30, 300);
            lblCurrentProfile.Margin = new Padding(6, 0, 6, 0);
            lblCurrentProfile.Name = "lblCurrentProfile";
            lblCurrentProfile.Size = new System.Drawing.Size(0, 28);
            lblCurrentProfile.TabIndex = 10;
            //
            // lblTime
            //
            lblTime.AutoSize = true;
            lblTime.Location = new System.Drawing.Point(30, 340);
            lblTime.Margin = new Padding(6, 0, 6, 0);
            lblTime.Name = "lblTime";
            lblTime.Size = new System.Drawing.Size(0, 28);
            lblTime.TabIndex = 11;
            //
            // lblStats
            //
            lblStats.AutoSize = true;
            lblStats.Location = new System.Drawing.Point(30, 380);
            lblStats.Margin = new Padding(6, 0, 6, 0);
            lblStats.Name = "lblStats";
            lblStats.Size = new System.Drawing.Size(0, 28);
            lblStats.TabIndex = 12;
            //
            // ProfileManager
            //
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnShowStats);
            Controls.Add(btnCreateCharacter);
            Controls.Add(btnSwitchCharacter);
            Controls.Add(btnDeleteCharacter);
            Controls.Add(lblScene);
            Controls.Add(cmbScene);
            Controls.Add(lblDifficulty);
            Controls.Add(cmbDifficulty);
            Controls.Add(btnStartFarm);
            Controls.Add(lblCurrentProfile);
            Controls.Add(lblTime);
            Controls.Add(lblStats);
            Margin = new Padding(6);
            Name = "ProfileManager";
            Size = new System.Drawing.Size(542, 450);
            ResumeLayout(false);
            PerformLayout();
        }


    }
}