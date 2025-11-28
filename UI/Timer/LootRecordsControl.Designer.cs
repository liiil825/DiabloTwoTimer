#nullable disable

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Timer
{
    partial class LootRecordsControl
    {

        private DataGridView gridLoot;

        private void InitializeComponent()
        {
            this.gridLoot = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridLoot)).BeginInit();
            this.SuspendLayout();

            this.gridLoot.Dock = DockStyle.Fill;
            this.gridLoot.BackgroundColor = SystemColors.Window;
            this.gridLoot.BorderStyle = BorderStyle.None;
            this.gridLoot.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.gridLoot.ColumnHeadersVisible = true;
            this.gridLoot.RowHeadersVisible = false;
            this.gridLoot.AllowUserToAddRows = false;
            this.gridLoot.AllowUserToDeleteRows = false;
            this.gridLoot.AllowUserToResizeRows = false;
            this.gridLoot.ReadOnly = true;
            this.gridLoot.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.gridLoot.MultiSelect = false;
            this.gridLoot.VirtualMode = true;

            this.gridLoot.CellValueNeeded += GridLoot_CellValueNeeded;
            // 交互事件
            this.gridLoot.Click += (s, e) => InteractionOccurred?.Invoke(this, EventArgs.Empty);
            this.gridLoot.Enter += (s, e) => InteractionOccurred?.Invoke(this, EventArgs.Empty);

            // 【优化】不需要场景列
            DataGridViewTextBoxColumn colIndex = new DataGridViewTextBoxColumn();
            colIndex.HeaderText = "#";
            colIndex.Width = 40;
            colIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
            colName.HeaderText = "Item";
            colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            DataGridViewTextBoxColumn colTime = new DataGridViewTextBoxColumn();
            colTime.HeaderText = "Time";
            colTime.Width = 120;
            colTime.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            this.gridLoot.Columns.AddRange(new DataGridViewColumn[] { colIndex, colName, colTime });

            this.AutoScaleDimensions = new SizeF(9F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.gridLoot);
            this.Name = "LootRecordsControl";
            this.Size = new Size(605, 150);

            ((System.ComponentModel.ISupportInitialize)(this.gridLoot)).EndInit();
            this.ResumeLayout(false);
        }


    }
}