using System;

namespace DiabloTwoMFTimer.UI.Timer
{
    partial class LootRecordsControl
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
            System.Windows.Forms.DataGridViewCellStyle cellStyleCenter = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle cellStyleRight = new System.Windows.Forms.DataGridViewCellStyle();

            this.gridLoot = new DiabloTwoMFTimer.UI.Components.ThemedDataGridView();
            this.gridLoot.Click += (s, e) => InteractionOccurred?.Invoke(this, EventArgs.Empty);
            this.gridLoot.Enter += (s, e) => InteractionOccurred?.Invoke(this, EventArgs.Empty);

            this.colRun = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();

            ((System.ComponentModel.ISupportInitialize)(this.gridLoot)).BeginInit();
            this.SuspendLayout();

            // 
            // gridLoot
            // 
            this.gridLoot.BackgroundColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
            this.gridLoot.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridLoot.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRun,
            this.colName,
            this.colTime});
            this.gridLoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLoot.Location = new System.Drawing.Point(0, 0);
            this.gridLoot.Name = "gridLoot";
            this.gridLoot.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, 150);
            this.gridLoot.TabIndex = 0;
            this.gridLoot.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.GridLoot_CellValueNeeded);

            // 
            // colRun (#)
            // 
            this.colRun.HeaderText = "#";
            this.colRun.Name = "colRun";
            this.colRun.ReadOnly = true;
            // 固定小宽度
            this.colRun.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colRun.Width = DiabloTwoMFTimer.Utils.ScaleHelper.Scale(40);
            cellStyleCenter.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colRun.DefaultCellStyle = cellStyleCenter;

            // 
            // colName (Item)
            // 
            this.colName.HeaderText = "Item";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 填充剩余空间
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;

            // 
            // colTime (Time)
            // 
            this.colTime.HeaderText = "Time";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            // 固定宽度显示时间，避免挤压
            this.colTime.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colTime.Width = DiabloTwoMFTimer.Utils.ScaleHelper.Scale(110);
            cellStyleRight.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colTime.DefaultCellStyle = cellStyleRight;

            // 
            // LootRecordsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridLoot);
            this.Name = "LootRecordsControl";
            this.Size = new System.Drawing.Size(Theme.UISizeConstants.ClientWidth, 150);
            ((System.ComponentModel.ISupportInitialize)(this.gridLoot)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private DiabloTwoMFTimer.UI.Components.ThemedDataGridView gridLoot;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRun;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
    }
}