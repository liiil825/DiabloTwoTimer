namespace DiabloTwoMFTimer.UI.Timer
{
    partial class RecordLootForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tlpContent = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.txtLootName = new DiabloTwoMFTimer.UI.Components.ThemedTextBox();
            this.chkPreviousRun = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();

            this.pnlContent.SuspendLayout();
            this.tlpContent.SuspendLayout();
            this.SuspendLayout();

            // 
            // tlpContent
            // 
            this.tlpContent.AutoSize = true;
            this.tlpContent.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpContent.ColumnCount = 1;
            this.tlpContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

            this.tlpContent.Controls.Add(this.label1, 0, 0);
            this.tlpContent.Controls.Add(this.txtLootName, 0, 1);
            this.tlpContent.Controls.Add(this.chkPreviousRun, 0, 2);

            this.tlpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpContent.RowCount = 3;
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));

            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.label1.Name = "label1";
            this.label1.Text = "Loot Name";

            // 
            // txtLootName
            // 
            this.txtLootName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLootName.Margin = new System.Windows.Forms.Padding(0, 0, 0, 15);
            this.txtLootName.Name = "txtLootName";
            this.txtLootName.TabIndex = 0;

            // 
            // chkPreviousRun
            // 
            this.chkPreviousRun.AutoSize = true;
            this.chkPreviousRun.Margin = new System.Windows.Forms.Padding(0);
            this.chkPreviousRun.Name = "chkPreviousRun";
            this.chkPreviousRun.Text = "Previous Run";
            this.chkPreviousRun.TabIndex = 1;

            // 
            // RecordLootForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.MinimumSize = new System.Drawing.Size(400, 0);

            this.pnlContent.Controls.Add(this.tlpContent);
            this.Name = "RecordLootForm";
            this.Text = "Record Loot";

            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.tlpContent.ResumeLayout(false);
            this.tlpContent.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TableLayoutPanel tlpContent;
        private System.Windows.Forms.TextBox txtLootName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkPreviousRun;
    }
}