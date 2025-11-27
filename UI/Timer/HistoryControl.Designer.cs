#nullable disable

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DTwoMFTimerHelper.UI.Timer
{
    partial class HistoryControl
    {

        private DataGridView gridRunHistory;

        private void InitializeComponent()
        {
            this.gridRunHistory = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridRunHistory)).BeginInit();
            this.SuspendLayout();

            // Grid 配置 (保持之前的配置)
            this.gridRunHistory.Dock = DockStyle.Fill;
            this.gridRunHistory.BackgroundColor = SystemColors.Window;
            this.gridRunHistory.BorderStyle = BorderStyle.None;
            this.gridRunHistory.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.gridRunHistory.ColumnHeadersVisible = true;
            this.gridRunHistory.RowHeadersVisible = false;
            this.gridRunHistory.AllowUserToAddRows = false;
            this.gridRunHistory.AllowUserToDeleteRows = false;
            this.gridRunHistory.AllowUserToResizeRows = false;
            this.gridRunHistory.ReadOnly = true;
            this.gridRunHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.gridRunHistory.MultiSelect = false;
            this.gridRunHistory.VirtualMode = true;

            // 【关键】当网格被点击或获得焦点时，触发交互事件
            this.gridRunHistory.Enter += (s, e) => InteractionOccurred?.Invoke(this, EventArgs.Empty);
            this.gridRunHistory.Click += (s, e) => InteractionOccurred?.Invoke(this, EventArgs.Empty);

            this.gridRunHistory.CellValueNeeded += GridRunHistory_CellValueNeeded;

            // ... 列配置保持不变 ...
            DataGridViewTextBoxColumn colIndex = new DataGridViewTextBoxColumn();
            colIndex.HeaderText = "#";
            colIndex.Width = 50;
            colIndex.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridViewTextBoxColumn colTime = new DataGridViewTextBoxColumn();
            colTime.HeaderText = "Time";
            colTime.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.gridRunHistory.Columns.AddRange(new DataGridViewColumn[] { colIndex, colTime });

            this.AutoScaleDimensions = new SizeF(9F, 20F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.gridRunHistory);
            this.Name = "HistoryControl";
            this.Size = new Size(290, 90);

            ((System.ComponentModel.ISupportInitialize)(this.gridRunHistory)).EndInit();
            this.ResumeLayout(false);
        }


    }
}