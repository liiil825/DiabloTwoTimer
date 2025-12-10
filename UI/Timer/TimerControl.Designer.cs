using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Timer
{
    partial class TimerControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                UnsubscribeFromServiceEvents();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();

            // Top Bar
            this.tlpTop = new System.Windows.Forms.TableLayoutPanel();
            this.tlpTopLeft = new System.Windows.Forms.TableLayoutPanel();
            this.btnStatusIndicator = new System.Windows.Forms.Label();
            this.labelTime1 = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.pomodoroTime = new DiabloTwoMFTimer.UI.Pomodoro.PomodoroTimeDisplayLabel();

            // Main Time
            this.lblTimeDisplay = new DiabloTwoMFTimer.UI.Components.ThemedLabel();

            // Sub Controls
            this.statisticsControl = new DiabloTwoMFTimer.UI.Timer.StatisticsControl();
            this.historyControl = new DiabloTwoMFTimer.UI.Timer.HistoryControl();
            this.lootRecordsControl = new DiabloTwoMFTimer.UI.Timer.LootRecordsControl();

            // Bottom Bar
            this.tlpBottom = new System.Windows.Forms.TableLayoutPanel();
            this.characterSceneControl = new DiabloTwoMFTimer.UI.Timer.CharacterSceneControl();
            this.toggleLootButton = new DiabloTwoMFTimer.UI.Components.ThemedButton();

            this.tlpMain.SuspendLayout();
            this.tlpTop.SuspendLayout();
            this.tlpTopLeft.SuspendLayout();
            this.tlpBottom.SuspendLayout();
            this.SuspendLayout();

            this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;

            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

            // Rows Definition
            // 0: Top Bar
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, ScaleHelper.Scale(36)));
            // 1: Main Time - 【增大】为了容纳大字体
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            // 2: Stats 
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            // 3: History
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            // 4: Loot
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            // 5: Bottom
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

            this.tlpMain.Controls.Add(this.tlpTop, 0, 0);
            this.tlpMain.Controls.Add(this.lblTimeDisplay, 0, 1);
            this.tlpMain.Controls.Add(this.statisticsControl, 0, 2);
            this.tlpMain.Controls.Add(this.historyControl, 0, 3);
            this.tlpMain.Controls.Add(this.lootRecordsControl, 0, 4);
            this.tlpMain.Controls.Add(this.tlpBottom, 0, 5);

            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 6;
            this.tlpMain.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, Theme.UISizeConstants.ClientHeightWithoutLoot - Theme.UISizeConstants.TabItemHeight);
            this.tlpMain.TabIndex = 0;

            // 
            // tlpTop (1 Row, 2 Cols)
            // 

            this.tlpTop.ColumnCount = 2;
            // 【核心修改】左侧占满剩余空间 (100%)
            this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            // 【核心修改】右侧自适应 (AutoSize)，只占番茄钟需要的宽度
            this.tlpTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpTop.Controls.Add(this.tlpTopLeft, 0, 0); // 放入新的 TableLayout
            this.tlpTop.Controls.Add(this.pomodoroTime, 1, 0);
            this.tlpTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTop.Margin = new System.Windows.Forms.Padding(0);
            this.tlpTop.Name = "tlpTop";
            this.tlpTop.RowCount = 1;
            this.tlpTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            // 
            // tlpTopLeft (Indicator + Date) - 新增容器
            // 
            this.tlpTopLeft.ColumnCount = 2;
            this.tlpTopLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize)); // 适应红点大小
            this.tlpTopLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F)); // 适应文字
            this.tlpTopLeft.Controls.Add(this.btnStatusIndicator, 0, 0);
            this.tlpTopLeft.Controls.Add(this.labelTime1, 1, 0);
            this.tlpTopLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTopLeft.Location = new System.Drawing.Point(0, 0);
            this.tlpTopLeft.Margin = new System.Windows.Forms.Padding(0);
            this.tlpTopLeft.Name = "tlpTopLeft";
            this.tlpTopLeft.RowCount = 1;
            this.tlpTopLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            // Indicator
            this.btnStatusIndicator.AutoSize = false;
            this.btnStatusIndicator.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnStatusIndicator.BackColor = System.Drawing.Color.Red;
            this.btnStatusIndicator.Size = new System.Drawing.Size(ScaleHelper.Scale(8), ScaleHelper.Scale(8));
            this.btnStatusIndicator.Margin = new System.Windows.Forms.Padding(10, 0, 5, 0); // Top Margin 居中 (50高度的一半减去一半icon)

            // Date Time Label
            this.labelTime1.AutoSize = true;
            this.labelTime1.Dock = System.Windows.Forms.DockStyle.Fill;
            // this.labelTime1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelTime1.Font = DiabloTwoMFTimer.UI.Theme.AppTheme.SmallTitleFont; // 【加粗】
            this.labelTime1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft; // 【左对齐 + 垂直居中】
            this.labelTime1.ForeColor = System.Drawing.Color.LightGray;
            this.labelTime1.Text = "12:00";
            this.labelTime1.Margin = new System.Windows.Forms.Padding(0); // Top Margin 居中

            // 
            // pomodoroTime
            // 
            this.pomodoroTime.AutoSize = true;
            this.pomodoroTime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pomodoroTime.Font = DiabloTwoMFTimer.UI.Theme.AppTheme.SmallTitleFont; // 【加粗】
            this.pomodoroTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight; // 【右对齐 + 垂直居中】
            this.pomodoroTime.ForeColor = System.Drawing.Color.White;
            this.pomodoroTime.ShowMilliseconds = false;
            this.pomodoroTime.Margin = new System.Windows.Forms.Padding(0); // 右边距

            // 
            // lblTimeDisplay
            // 
            this.lblTimeDisplay.AutoSize = true;
            this.lblTimeDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTimeDisplay.Font = DiabloTwoMFTimer.UI.Theme.AppTheme.BigTimeFont; // 【超大字体】
            this.lblTimeDisplay.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.AccentColor;
            this.lblTimeDisplay.Text = "00:00:00.0";
            this.lblTimeDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // Sub Controls
            // 
            this.statisticsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lootRecordsControl.Dock = System.Windows.Forms.DockStyle.Fill;

            // 
            // tlpBottom
            // 
            this.tlpBottom.ColumnCount = 2;
            this.tlpBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpBottom.Controls.Add(this.characterSceneControl, 0, 0);
            this.tlpBottom.Controls.Add(this.toggleLootButton, 1, 0);
            this.tlpBottom.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpBottom.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.tlpBottom.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpBottom.Name = "tlpBottom";
            this.tlpBottom.RowCount = 1;
            this.tlpBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

            // Character Scene
            this.characterSceneControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.characterSceneControl.Margin = new System.Windows.Forms.Padding(5);

            // Toggle Button
            this.toggleLootButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.toggleLootButton.Size = new System.Drawing.Size(ScaleHelper.Scale(80), ScaleHelper.Scale(36));
            this.toggleLootButton.Text = "Show Loot";
            this.toggleLootButton.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.toggleLootButton.Click += new System.EventHandler(this.ToggleLootButton_Click);

            // 
            // TimerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpMain);
            this.Name = "TimerControl";
            this.Size = new System.Drawing.Size(500, 600);

            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.tlpTop.ResumeLayout(false);
            this.tlpTop.PerformLayout();
            this.tlpTopLeft.ResumeLayout(false);
            this.tlpBottom.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpTop;
        private System.Windows.Forms.TableLayoutPanel tlpTopLeft;
        private System.Windows.Forms.TableLayoutPanel tlpBottom;

        private System.Windows.Forms.Label btnStatusIndicator;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel labelTime1;
        private DiabloTwoMFTimer.UI.Pomodoro.PomodoroTimeDisplayLabel pomodoroTime;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblTimeDisplay;

        private DiabloTwoMFTimer.UI.Timer.StatisticsControl statisticsControl;
        private DiabloTwoMFTimer.UI.Timer.HistoryControl historyControl;
        private DiabloTwoMFTimer.UI.Timer.LootRecordsControl lootRecordsControl;
        private DiabloTwoMFTimer.UI.Timer.CharacterSceneControl characterSceneControl;
        private DiabloTwoMFTimer.UI.Components.ThemedButton toggleLootButton;
    }
}