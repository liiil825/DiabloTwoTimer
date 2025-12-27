using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.Utils;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Timer
{
    partial class LootHistoryForm
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.headerControl = new DiabloTwoMFTimer.UI.Components.ThemedWindowHeader();

            // 日期面板
            this.pnlCustomDate = new System.Windows.Forms.FlowLayoutPanel();
            this.dtpStart = new DiabloTwoMFTimer.UI.Components.ThemedDateTimePicker();
            this.lblSeparator = new DiabloTwoMFTimer.UI.Components.ThemedLabel(); // 新增分隔符
            this.dtpEnd = new DiabloTwoMFTimer.UI.Components.ThemedDateTimePicker();
            this.btnSearch = new DiabloTwoMFTimer.UI.Components.ThemedButton();

            // 表格容器
            this.pnlGridContainer = new System.Windows.Forms.Panel();
            this.gridLoot = new DiabloTwoMFTimer.UI.Components.ThemedDataGridView();

            // 底部按钮
            this.panelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new DiabloTwoMFTimer.UI.Components.ThemedModalButton();

            this.mainLayout.SuspendLayout();
            this.pnlCustomDate.SuspendLayout();
            this.pnlGridContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLoot)).BeginInit();
            this.panelButtons.SuspendLayout();
            this.SuspendLayout();

            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

            this.mainLayout.Controls.Add(this.headerControl, 0, 0);
            this.mainLayout.Controls.Add(this.pnlCustomDate, 0, 1);
            this.mainLayout.Controls.Add(this.pnlGridContainer, 0, 2);
            this.mainLayout.Controls.Add(this.panelButtons, 0, 3);

            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.RowCount = 4;

            // RowStyles 将在 ApplyScaledLayout 中设置
            // 关键：Row 1 将被设为 Absolute 固定高度
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

            this.mainLayout.TabIndex = 0;

            // 
            // headerControl
            // 
            this.headerControl.BackColor = System.Drawing.Color.Transparent;
            this.headerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerControl.Location = new System.Drawing.Point(0, 0);
            this.headerControl.Margin = new System.Windows.Forms.Padding(0);
            this.headerControl.Name = "headerControl";
            this.headerControl.Size = new System.Drawing.Size(800, 110);
            this.headerControl.TabIndex = 0;
            this.headerControl.Title = "LOOT HISTORY";

            // 
            // pnlCustomDate (FlowLayoutPanel)
            // 
            this.pnlCustomDate.AutoSize = true;
            this.pnlCustomDate.Anchor = System.Windows.Forms.AnchorStyles.None; // 居中
            this.pnlCustomDate.BackColor = System.Drawing.Color.Transparent;
            // 移除 lblFrom, lblTo，加入 lblSeparator
            this.pnlCustomDate.Controls.Add(this.dtpStart);
            this.pnlCustomDate.Controls.Add(this.lblSeparator);
            this.pnlCustomDate.Controls.Add(this.dtpEnd);
            this.pnlCustomDate.Controls.Add(this.btnSearch);
            this.pnlCustomDate.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.pnlCustomDate.Location = new System.Drawing.Point(100, 113);
            this.pnlCustomDate.Name = "pnlCustomDate";
            this.pnlCustomDate.Size = new System.Drawing.Size(600, 40);
            this.pnlCustomDate.TabIndex = 1;
            this.pnlCustomDate.WrapContents = false;

            // 
            // dtpStart
            // 
            this.dtpStart.Size = new System.Drawing.Size(160, 20);
            this.dtpStart.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);

            // 
            // lblSeparator (新增)
            // 
            this.lblSeparator.AutoSize = false; // 关闭自动大小，以便我们手动设置高度实现垂直居中
            this.lblSeparator.Text = "-";
            this.lblSeparator.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSeparator.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.lblSeparator.Size = new System.Drawing.Size(20, 20); // 宽度小一点，高度与 DTP 一致

            // 
            // dtpEnd
            // 
            this.dtpEnd.Size = new System.Drawing.Size(160, 20);
            this.dtpEnd.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);

            // 
            // btnSearch
            // 
            this.btnSearch.Text = "";
            this.btnSearch.Font = Theme.AppTheme.Fonts.SegoeIcon;
            this.btnSearch.Size = new System.Drawing.Size(80, 20);
            this.btnSearch.Click += new System.EventHandler(this.BtnSearch_Click);

            // 
            // pnlGridContainer
            // 
            this.pnlGridContainer.BackColor = System.Drawing.Color.Transparent;
            this.pnlGridContainer.Controls.Add(this.gridLoot);
            // 关键修改：取消 Dock=Fill，改为 Anchor=Top|Bottom (TLP内自动水平居中)
            // 具体的 Width 将在 Resize 事件中控制
            this.pnlGridContainer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            this.pnlGridContainer.Location = new System.Drawing.Point(50, 173);
            this.pnlGridContainer.Name = "pnlGridContainer";
            this.pnlGridContainer.Size = new System.Drawing.Size(700, 340);
            this.pnlGridContainer.TabIndex = 2;

            // 
            // gridLoot
            // 
            this.gridLoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridLoot.Name = "gridLoot";
            this.gridLoot.TabIndex = 0;

            // 
            // panelButtons
            // 
            this.panelButtons.AutoSize = true;
            this.panelButtons.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelButtons.BackColor = System.Drawing.Color.Transparent;
            this.panelButtons.Controls.Add(this.btnClose);
            this.panelButtons.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.panelButtons.Location = new System.Drawing.Point(340, 520);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(120, 61);
            this.panelButtons.TabIndex = 3;
            this.panelButtons.WrapContents = false;

            // 
            // btnClose
            // 
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(120, 40);
            this.btnClose.Padding = new Padding(ScaleHelper.Scale(5));
            this.btnClose.Margin = new Padding(ScaleHelper.Scale(10), 0, ScaleHelper.Scale(10), ScaleHelper.Scale(60));
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.SetThemeDanger();
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);

            // 
            // LootHistoryForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.mainLayout);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LootHistoryForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LootHistoryForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            this.mainLayout.ResumeLayout(false);
            this.mainLayout.PerformLayout();
            this.pnlCustomDate.ResumeLayout(false);
            this.pnlCustomDate.PerformLayout();
            this.pnlGridContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridLoot)).EndInit();
            this.panelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private ThemedWindowHeader headerControl;
        private System.Windows.Forms.FlowLayoutPanel pnlCustomDate;
        private System.Windows.Forms.Panel pnlGridContainer;
        private System.Windows.Forms.FlowLayoutPanel panelButtons;

        private ThemedModalButton btnClose;
        private ThemedDataGridView gridLoot;

        // 移除旧的 label, 新增 lblSeparator
        private ThemedDateTimePicker dtpStart;
        private ThemedLabel lblSeparator;
        private ThemedDateTimePicker dtpEnd;
        private ThemedButton btnSearch;
    }
}