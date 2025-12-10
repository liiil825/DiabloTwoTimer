namespace DiabloTwoMFTimer.UI.Profiles
{
    partial class ProfileManager
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();

            this.tlpTopButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnCreateCharacter = new DiabloTwoMFTimer.UI.Components.ThemedButton();
            this.btnSwitchCharacter = new DiabloTwoMFTimer.UI.Components.ThemedButton();
            this.btnDeleteCharacter = new DiabloTwoMFTimer.UI.Components.ThemedButton();

            this.tlpScene = new System.Windows.Forms.TableLayoutPanel();
            this.lblScene = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.cmbScene = new DiabloTwoMFTimer.UI.Components.ThemedComboBox();

            this.tlpDifficulty = new System.Windows.Forms.TableLayoutPanel();
            this.lblDifficulty = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.cmbDifficulty = new DiabloTwoMFTimer.UI.Components.ThemedComboBox();

            // 第一行操作栏：开始 + 导出
            this.tlpActionButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnStartFarm = new DiabloTwoMFTimer.UI.Components.ThemedButton();
            this.btnExport = new DiabloTwoMFTimer.UI.Components.ThemedButton(); // 新增

            // 第二行操作栏：掉落 + 统计 (新增容器)
            this.tlpStatButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnShowLootHistory = new DiabloTwoMFTimer.UI.Components.ThemedButton();
            this.btnShowStats = new DiabloTwoMFTimer.UI.Components.ThemedButton();

            this.lblCurrentProfile = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.lblTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.lblStats = new DiabloTwoMFTimer.UI.Components.ThemedLabel();

            this.tlpMain.SuspendLayout();
            this.tlpTopButtons.SuspendLayout();
            this.tlpScene.SuspendLayout();
            this.tlpDifficulty.SuspendLayout();
            this.tlpActionButtons.SuspendLayout();
            this.tlpStatButtons.SuspendLayout(); // 挂起新容器
            this.SuspendLayout();

            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

            // --- 调整行定义 ---
            // Row 0: Top Buttons
            // Row 1: Scene
            // Row 2: Difficulty
            // Row 3: Action Buttons (Start + Export)
            // Row 4: Stat Buttons (Loot + Stats) [新增]
            // Row 5: Current Profile
            // ...
            this.tlpMain.Controls.Add(this.tlpTopButtons, 0, 0);
            this.tlpMain.Controls.Add(this.tlpScene, 0, 1);
            this.tlpMain.Controls.Add(this.tlpDifficulty, 0, 2);
            this.tlpMain.Controls.Add(this.tlpActionButtons, 0, 3);
            this.tlpMain.Controls.Add(this.tlpStatButtons, 0, 4); // 新增行
            this.tlpMain.Controls.Add(this.lblCurrentProfile, 0, 5);
            this.tlpMain.Controls.Add(this.lblTime, 0, 6);
            this.tlpMain.Controls.Add(this.lblStats, 0, 7);

            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.Padding = new System.Windows.Forms.Padding(20);

            // 更新 RowCount 和 Styles
            this.tlpMain.RowCount = 9;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Top
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Scene
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Diff
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Action 1
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize)); // Action 2 (Stat)
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

            this.tlpMain.TabIndex = 0;

            // ... (TopButtons, Scene, Difficulty 代码保持不变，省略) ...

            // -----------------------------------------------------------
            // 顶部按钮组 (tlpTopButtons) - 保持原有逻辑
            // -----------------------------------------------------------
            this.tlpTopButtons.AutoSize = true;
            this.tlpTopButtons.ColumnCount = 3;
            this.tlpTopButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpTopButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpTopButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tlpTopButtons.Controls.Add(this.btnCreateCharacter, 0, 0);
            this.tlpTopButtons.Controls.Add(this.btnSwitchCharacter, 1, 0);
            this.tlpTopButtons.Controls.Add(this.btnDeleteCharacter, 2, 0);
            this.tlpTopButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTopButtons.Margin = new System.Windows.Forms.Padding(3, 3, 3, 15);
            this.tlpTopButtons.RowCount = 1;
            this.tlpTopButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpTopButtons.Size = new System.Drawing.Size(496, 48);

            // 统一间距设置
            System.Windows.Forms.Padding commonMargin = new System.Windows.Forms.Padding(5, 5, 5, 10);

            this.btnCreateCharacter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnCreateCharacter.Margin = commonMargin;
            this.btnCreateCharacter.Click += BtnCreateCharacter_Click;

            this.btnSwitchCharacter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSwitchCharacter.Margin = commonMargin;
            this.btnSwitchCharacter.Click += BtnSwitchCharacter_Click;

            this.btnDeleteCharacter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDeleteCharacter.Margin = commonMargin;
            this.btnDeleteCharacter.Click += BtnDeleteCharacter_Click;

            // 
            // tlpScene
            // 
            this.tlpScene.AutoSize = true;
            this.tlpScene.ColumnCount = 2;
            // 【修改点 1】第一列改为 AutoSize (文字自适应)，第二列保持 100% (占满剩余)
            this.tlpScene.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpScene.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpScene.Controls.Add(this.lblScene, 0, 0);
            this.tlpScene.Controls.Add(this.cmbScene, 1, 0);
            this.tlpScene.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpScene.Location = new System.Drawing.Point(23, 89);
            this.tlpScene.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.tlpScene.Name = "tlpScene";
            this.tlpScene.RowCount = 1;
            this.tlpScene.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpScene.Size = new System.Drawing.Size(496, 41);
            this.tlpScene.TabIndex = 1;

            // 【修改点 2】调整 Label 的属性实现垂直居中和右侧间距
            // 原来是 AnchorStyles.Right，现在改为 Left 以跟随文字长度
            this.lblScene.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblScene.AutoSize = true;
            // Top=7 是为了让文字在 35px 高的 ComboBox 旁垂直居中 ((35-文字高)/2 约等于 7)
            // Right=10 是为了给文字和输入框之间留出空隙
            this.lblScene.Margin = new System.Windows.Forms.Padding(0, 7, 10, 0);
            this.lblScene.Text = "Scene:";

            // ComboBox 保持不变
            this.cmbScene.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbScene.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScene.Height = 35;

            // 
            // tlpDifficulty
            // 
            this.tlpDifficulty.AutoSize = true;
            this.tlpDifficulty.ColumnCount = 2;
            // 【修改点 3】同上，第一列 AutoSize
            this.tlpDifficulty.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpDifficulty.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDifficulty.Controls.Add(this.lblDifficulty, 0, 0);
            this.tlpDifficulty.Controls.Add(this.cmbDifficulty, 1, 0);
            this.tlpDifficulty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpDifficulty.Location = new System.Drawing.Point(23, 143);
            this.tlpDifficulty.Margin = new System.Windows.Forms.Padding(3, 3, 3, 20);
            this.tlpDifficulty.Name = "tlpDifficulty";
            this.tlpDifficulty.RowCount = 1;
            this.tlpDifficulty.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpDifficulty.Size = new System.Drawing.Size(496, 41);
            this.tlpDifficulty.TabIndex = 2;

            // 【修改点 4】调整 Label 属性
            this.lblDifficulty.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDifficulty.AutoSize = true;
            this.lblDifficulty.Margin = new System.Windows.Forms.Padding(0, 7, 10, 0); // 同样的垂直居中和间距
            this.lblDifficulty.Text = "Diff:";

            // ComboBox 保持不变
            this.cmbDifficulty.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbDifficulty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDifficulty.Height = 35;

            // -----------------------------------------------------------
            // 3. 第一行操作按钮 (Start + Export)
            // -----------------------------------------------------------
            this.tlpActionButtons.AutoSize = true;
            this.tlpActionButtons.ColumnCount = 2;
            this.tlpActionButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpActionButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));

            this.tlpActionButtons.Controls.Add(this.btnStartFarm, 0, 0);
            this.tlpActionButtons.Controls.Add(this.btnExport, 1, 0);

            this.tlpActionButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpActionButtons.Location = new System.Drawing.Point(23, 207); // 自动布局下Location仅供参考
            this.tlpActionButtons.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10); // 底部留白给下一行
            this.tlpActionButtons.Name = "tlpActionButtons";
            this.tlpActionButtons.RowCount = 1;
            this.tlpActionButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpActionButtons.Size = new System.Drawing.Size(496, 54);
            this.tlpActionButtons.TabIndex = 3;

            // Start Farm
            this.btnStartFarm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStartFarm.Margin = commonMargin;
            this.btnStartFarm.Click += BtnStartFarm_Click;

            // Export (New)
            this.btnExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExport.Margin = commonMargin;
            this.btnExport.Text = "Export"; // 初始文本
            this.btnExport.Click += BtnExport_Click;

            // -----------------------------------------------------------
            // 4. 第二行操作按钮 (Loot + Stats) - 新增容器
            // -----------------------------------------------------------
            this.tlpStatButtons.AutoSize = true;
            this.tlpStatButtons.ColumnCount = 2;
            this.tlpStatButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpStatButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));

            this.tlpStatButtons.Controls.Add(this.btnShowLootHistory, 0, 0);
            this.tlpStatButtons.Controls.Add(this.btnShowStats, 1, 0);

            this.tlpStatButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpStatButtons.Margin = new System.Windows.Forms.Padding(3, 3, 3, 20); // 底部留白
            this.tlpStatButtons.Name = "tlpStatButtons";
            this.tlpStatButtons.RowCount = 1;
            this.tlpStatButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpStatButtons.TabIndex = 4;

            // Loot History
            this.btnShowLootHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnShowLootHistory.Height = 50;
            this.btnShowLootHistory.Margin = commonMargin;
            this.btnShowLootHistory.Click += BtnShowLootHistory_Click;

            // Stats
            this.btnShowStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnShowStats.Height = 50;
            this.btnShowStats.Margin = commonMargin;
            this.btnShowStats.Click += BtnShowStats_Click;

            // 
            // Labels
            // 
            this.lblCurrentProfile.AutoSize = true;
            this.lblCurrentProfile.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCurrentProfile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblTime.AutoSize = true;
            this.lblTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.lblStats.AutoSize = true;
            this.lblStats.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblStats.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // ProfileManager
            // 
            this.AutoSize = true; // 【开启】自身也随内容变化
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(32, 32, 32);
            this.Controls.Add(this.tlpMain);
            this.Name = "ProfileManager";

            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.tlpTopButtons.ResumeLayout(false);
            this.tlpScene.ResumeLayout(false);
            this.tlpScene.PerformLayout();
            this.tlpDifficulty.ResumeLayout(false);
            this.tlpDifficulty.PerformLayout();
            this.tlpActionButtons.ResumeLayout(false);
            this.tlpStatButtons.ResumeLayout(false); // 恢复
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpTopButtons;
        private System.Windows.Forms.TableLayoutPanel tlpScene;
        private System.Windows.Forms.TableLayoutPanel tlpDifficulty;
        private System.Windows.Forms.TableLayoutPanel tlpActionButtons;
        private System.Windows.Forms.TableLayoutPanel tlpStatButtons; // 新增

        private DiabloTwoMFTimer.UI.Components.ThemedButton btnCreateCharacter;
        private DiabloTwoMFTimer.UI.Components.ThemedButton btnSwitchCharacter;
        private DiabloTwoMFTimer.UI.Components.ThemedButton btnDeleteCharacter;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblScene;
        private DiabloTwoMFTimer.UI.Components.ThemedComboBox cmbScene;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblDifficulty;
        private DiabloTwoMFTimer.UI.Components.ThemedComboBox cmbDifficulty;

        private DiabloTwoMFTimer.UI.Components.ThemedButton btnStartFarm;
        private DiabloTwoMFTimer.UI.Components.ThemedButton btnExport; // 新增
        private DiabloTwoMFTimer.UI.Components.ThemedButton btnShowLootHistory;
        private DiabloTwoMFTimer.UI.Components.ThemedButton btnShowStats;

        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblCurrentProfile;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblTime;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblStats;
    }
}