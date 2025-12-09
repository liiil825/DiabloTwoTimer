namespace DiabloTwoMFTimer.UI.Timer
{
    partial class StatisticsControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                Utils.LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.lblRunCount = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.lblFastestTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.lblAverageTime = new DiabloTwoMFTimer.UI.Components.ThemedLabel();

            this.tlpMain.SuspendLayout();
            this.SuspendLayout();

            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.Controls.Add(this.lblRunCount, 0, 0);
            this.tlpMain.Controls.Add(this.lblFastestTime, 0, 1);
            this.tlpMain.Controls.Add(this.lblAverageTime, 0, 2);
            this.tlpMain.Name = "tlpMain";
            // 1. 修改主容器 tlpMain
            this.tlpMain.AutoSize = true; // 【开启】
            this.tlpMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpMain.TabIndex = 0;

            // 
            // lblRunCount
            // 
            this.lblRunCount.AutoSize = true;
            this.lblRunCount.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRunCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblRunCount.Font = DiabloTwoMFTimer.UI.Theme.AppTheme.SmallTitleFont;
            this.lblRunCount.Name = "lblRunCount";
            this.lblRunCount.TabIndex = 0;
            this.lblRunCount.Text = "--- Run count 0 ---";

            // 
            // lblFastestTime
            // 
            this.lblFastestTime.AutoSize = true;
            this.lblFastestTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblFastestTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFastestTime.Font = DiabloTwoMFTimer.UI.Theme.AppTheme.MainFont;
            this.lblFastestTime.Name = "lblFastestTime";
            this.lblFastestTime.TabIndex = 1;
            this.lblFastestTime.Text = "Fastest: --:--:--.-";

            // 
            // lblAverageTime
            // 
            this.lblAverageTime.AutoSize = true;
            this.lblAverageTime.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblAverageTime.Font = DiabloTwoMFTimer.UI.Theme.AppTheme.MainFont;
            this.lblAverageTime.Name = "lblAverageTime";
            this.lblAverageTime.TabIndex = 2;
            this.lblAverageTime.Text = "Average: --:--:--.-";
            this.lblAverageTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // StatisticsControl
            // 
            this.AutoSize = true; // 【开启】自身也随内容变化
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tlpMain);
            this.Name = "StatisticsControl";

            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblRunCount;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblFastestTime;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblAverageTime;
    }
}