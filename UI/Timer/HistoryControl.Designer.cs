#nullable disable

using System;

namespace DiabloTwoMFTimer.UI.Timer
{
    partial class HistoryControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                Utils.LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
                if (_historyService != null)
                    _historyService.HistoryDataChanged -= OnHistoryDataChanged;

                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle cellStyleCenter = new System.Windows.Forms.DataGridViewCellStyle();
            // 【关键】当网格被点击或获得焦点时，触发交互事件
            this.gridRunHistory = new DiabloTwoMFTimer.UI.Components.ThemedDataGridView();
            this.gridRunHistory.Enter += (s, e) => InteractionOccurred?.Invoke(this, EventArgs.Empty);
            this.gridRunHistory.Click += (s, e) => InteractionOccurred?.Invoke(this, EventArgs.Empty);

            this.gridRunHistory.CellValueNeeded += GridRunHistory_CellValueNeeded;
            this.colIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();

            ((System.ComponentModel.ISupportInitialize)(this.gridRunHistory)).BeginInit();
            this.SuspendLayout();

            // 
            // gridRunHistory
            // 
            this.gridRunHistory.BackgroundColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
            this.gridRunHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridRunHistory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIndex,
            this.colTime});
            this.gridRunHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridRunHistory.Location = new System.Drawing.Point(0, 0);
            this.gridRunHistory.Name = "gridRunHistory";
            this.gridRunHistory.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, 90);
            this.gridRunHistory.TabIndex = 0;

            // 
            // colIndex (#)
            // 
            this.colIndex.HeaderText = "#";
            this.colIndex.MinimumWidth = 6;
            this.colIndex.Name = "colIndex";
            this.colIndex.ReadOnly = true;
            // 【关键】固定宽度，不做自动缩放，或者设为 None
            this.colIndex.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colIndex.Width = DiabloTwoMFTimer.Utils.ScaleHelper.Scale(50); // 缩放后的固定宽
            cellStyleCenter.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colIndex.DefaultCellStyle = cellStyleCenter;

            // 
            // colTime (Time)
            // 
            this.colTime.HeaderText = "Time";
            this.colTime.MinimumWidth = 6;
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            // 【关键】Fill 填充剩余空间
            this.colTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;

            // 
            // HistoryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridRunHistory);
            this.Name = "HistoryControl";
            this.Size = new System.Drawing.Size(290, 90);
            ((System.ComponentModel.ISupportInitialize)(this.gridRunHistory)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DiabloTwoMFTimer.UI.Components.ThemedDataGridView gridRunHistory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
    }
}