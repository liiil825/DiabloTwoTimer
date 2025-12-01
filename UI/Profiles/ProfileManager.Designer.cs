#nullable disable

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Components;

namespace DiabloTwoMFTimer.UI.Profiles
{
    partial class ProfileManager
    {

        private void InitializeComponent()
        {
            btnCreateCharacter = new ThemedButton();
            btnSwitchCharacter = new ThemedButton();
            btnDeleteCharacter = new ThemedButton();
            lblScene = new ThemedLabel();
            cmbScene = new ThemedComboBox();
            lblDifficulty = new ThemedLabel();
            cmbDifficulty = new ThemedComboBox();
            btnStartFarm = new ThemedButton();
            btnShowStats = new ThemedButton();
            lblCurrentProfile = new ThemedLabel();
            lblTime = new ThemedLabel();
            lblStats = new ThemedLabel();
            SuspendLayout();
            // 
            // btnCreateCharacter
            // 
            btnCreateCharacter.BackColor = Color.Transparent;
            btnCreateCharacter.BorderRadius = 8;
            btnCreateCharacter.FlatStyle = FlatStyle.Flat;
            btnCreateCharacter.Font = new Font("微软雅黑", 10F);
            btnCreateCharacter.Location = new Point(30, 30);
            btnCreateCharacter.Margin = new Padding(4);
            btnCreateCharacter.Name = "btnCreateCharacter";
            btnCreateCharacter.Size = new Size(100, 40);
            btnCreateCharacter.TabIndex = 0;
            btnCreateCharacter.UseVisualStyleBackColor = true;
            btnCreateCharacter.Click += BtnCreateCharacter_Click;
            // 
            // btnSwitchCharacter
            // 
            btnSwitchCharacter.BackColor = Color.Transparent;
            btnSwitchCharacter.BorderRadius = 8;
            btnSwitchCharacter.FlatStyle = FlatStyle.Flat;
            btnSwitchCharacter.Font = new Font("微软雅黑", 10F);
            btnSwitchCharacter.Location = new Point(181, 30);
            btnSwitchCharacter.Margin = new Padding(4);
            btnSwitchCharacter.Name = "btnSwitchCharacter";
            btnSwitchCharacter.Size = new Size(100, 40);
            btnSwitchCharacter.TabIndex = 1;
            btnSwitchCharacter.UseVisualStyleBackColor = true;
            btnSwitchCharacter.Click += BtnSwitchCharacter_Click;
            // 
            // btnDeleteCharacter
            // 
            btnDeleteCharacter.BackColor = Color.Transparent;
            btnDeleteCharacter.BorderRadius = 8;
            btnDeleteCharacter.Enabled = false;
            btnDeleteCharacter.FlatStyle = FlatStyle.Flat;
            btnDeleteCharacter.Font = new Font("微软雅黑", 10F);
            btnDeleteCharacter.Location = new Point(324, 30);
            btnDeleteCharacter.Margin = new Padding(4);
            btnDeleteCharacter.Name = "btnDeleteCharacter";
            btnDeleteCharacter.Size = new Size(100, 40);
            btnDeleteCharacter.TabIndex = 2;
            btnDeleteCharacter.UseVisualStyleBackColor = true;
            btnDeleteCharacter.Click += BtnDeleteCharacter_Click;
            // 
            // lblScene
            // 
            lblScene.AutoSize = true;
            lblScene.BackColor = Color.Transparent;
            lblScene.Font = new Font("微软雅黑", 10F);
            lblScene.ForeColor = Color.FromArgb(240, 240, 240);
            lblScene.IsTitle = false;
            lblScene.Location = new Point(30, 100);
            lblScene.Margin = new Padding(6, 0, 6, 0);
            lblScene.Name = "lblScene";
            lblScene.Size = new Size(0, 31);
            lblScene.TabIndex = 3;
            // 
            // cmbScene
            // 
            cmbScene.BackColor = Color.FromArgb(45, 45, 48);
            cmbScene.DrawMode = DrawMode.OwnerDrawFixed;
            cmbScene.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbScene.FlatStyle = FlatStyle.Flat;
            cmbScene.Font = new Font("微软雅黑", 10F);
            cmbScene.ForeColor = Color.FromArgb(240, 240, 240);
            cmbScene.Location = new Point(130, 100);
            cmbScene.Margin = new Padding(6);
            cmbScene.Name = "cmbScene";
            cmbScene.Size = new Size(294, 39);
            cmbScene.TabIndex = 4;
            cmbScene.SelectedIndexChanged += CmbScene_SelectedIndexChanged;
            // 
            // lblDifficulty
            // 
            lblDifficulty.AutoSize = true;
            lblDifficulty.BackColor = Color.Transparent;
            lblDifficulty.Font = new Font("微软雅黑", 10F);
            lblDifficulty.ForeColor = Color.FromArgb(240, 240, 240);
            lblDifficulty.IsTitle = false;
            lblDifficulty.Location = new Point(30, 160);
            lblDifficulty.Margin = new Padding(6, 0, 6, 0);
            lblDifficulty.Name = "lblDifficulty";
            lblDifficulty.Size = new Size(0, 31);
            lblDifficulty.TabIndex = 5;
            // 
            // cmbDifficulty
            // 
            cmbDifficulty.BackColor = Color.FromArgb(45, 45, 48);
            cmbDifficulty.DrawMode = DrawMode.OwnerDrawFixed;
            cmbDifficulty.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDifficulty.FlatStyle = FlatStyle.Flat;
            cmbDifficulty.Font = new Font("微软雅黑", 10F);
            cmbDifficulty.ForeColor = Color.FromArgb(240, 240, 240);
            cmbDifficulty.Location = new Point(130, 160);
            cmbDifficulty.Margin = new Padding(6);
            cmbDifficulty.Name = "cmbDifficulty";
            cmbDifficulty.Size = new Size(294, 39);
            cmbDifficulty.TabIndex = 6;
            cmbDifficulty.SelectedIndexChanged += CmbDifficulty_SelectedIndexChanged;
            // 
            // btnStartFarm
            // 
            btnStartFarm.BackColor = Color.Transparent;
            btnStartFarm.BorderRadius = 8;
            btnStartFarm.Enabled = false;
            btnStartFarm.FlatStyle = FlatStyle.Flat;
            btnStartFarm.Font = new Font("微软雅黑", 10F);
            btnStartFarm.Location = new Point(30, 220);
            btnStartFarm.Margin = new Padding(6);
            btnStartFarm.Name = "btnStartFarm";
            btnStartFarm.Size = new Size(130, 50);
            btnStartFarm.TabIndex = 7;
            btnStartFarm.UseVisualStyleBackColor = true;
            btnStartFarm.Click += BtnStartFarm_Click;
            // 
            // btnShowStats
            // 
            btnShowStats.BackColor = Color.Transparent;
            btnShowStats.BorderRadius = 8;
            btnShowStats.FlatStyle = FlatStyle.Flat;
            btnShowStats.Font = new Font("微软雅黑", 10F);
            btnShowStats.Location = new Point(294, 220);
            btnShowStats.Margin = new Padding(6);
            btnShowStats.Name = "btnShowStats";
            btnShowStats.Size = new Size(130, 50);
            btnShowStats.TabIndex = 8;
            btnShowStats.Text = "全屏统计";
            btnShowStats.UseVisualStyleBackColor = false;
            btnShowStats.Click += BtnShowStats_Click;
            // 
            // lblCurrentProfile
            // 
            lblCurrentProfile.AutoSize = true;
            lblCurrentProfile.BackColor = Color.Transparent;
            lblCurrentProfile.Font = new Font("微软雅黑", 10F);
            lblCurrentProfile.ForeColor = Color.FromArgb(240, 240, 240);
            lblCurrentProfile.IsTitle = false;
            lblCurrentProfile.Location = new Point(30, 300);
            lblCurrentProfile.Margin = new Padding(6, 0, 6, 0);
            lblCurrentProfile.Name = "lblCurrentProfile";
            lblCurrentProfile.Size = new Size(0, 31);
            lblCurrentProfile.TabIndex = 10;
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.BackColor = Color.Transparent;
            lblTime.Font = new Font("微软雅黑", 10F);
            lblTime.ForeColor = Color.FromArgb(240, 240, 240);
            lblTime.IsTitle = false;
            lblTime.Location = new Point(30, 340);
            lblTime.Margin = new Padding(6, 0, 6, 0);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(0, 31);
            lblTime.TabIndex = 11;
            // 
            // lblStats
            // 
            lblStats.AutoSize = true;
            lblStats.BackColor = Color.Transparent;
            lblStats.Font = new Font("微软雅黑", 10F);
            lblStats.ForeColor = Color.FromArgb(240, 240, 240);
            lblStats.IsTitle = false;
            lblStats.Location = new Point(30, 380);
            lblStats.Margin = new Padding(6, 0, 6, 0);
            lblStats.Name = "lblStats";
            lblStats.Size = new Size(0, 31);
            lblStats.TabIndex = 12;
            // 
            // ProfileManager
            // 
            AutoScaleDimensions = new SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(32, 32, 32);
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
            Size = new Size(542, 450);
            ResumeLayout(false);
            PerformLayout();
        }
        private ThemedButton btnCreateCharacter;
        private ThemedButton btnSwitchCharacter;
        private ThemedButton btnDeleteCharacter;
        private ThemedLabel lblScene;
        private ThemedComboBox cmbScene;
        private ThemedLabel lblDifficulty;
        private ThemedComboBox cmbDifficulty;
        private ThemedButton btnStartFarm;
        private ThemedButton btnShowStats;
        private ThemedLabel lblCurrentProfile;
        private ThemedLabel lblTime;
        private ThemedLabel lblStats;
    }
}