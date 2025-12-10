namespace DiabloTwoMFTimer.UI.Profiles
{
    partial class SwitchCharacterForm
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
            this.lblCharacters = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.lstCharacters = new DiabloTwoMFTimer.UI.Components.ThemedListBox();

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

            this.tlpContent.Controls.Add(this.lblCharacters, 0, 0);
            this.tlpContent.Controls.Add(this.lstCharacters, 0, 1);

            this.tlpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpContent.RowCount = 2;
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            // ListBox 可以在这里占据固定高度或比例高度，建议给个最小高度
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 200F));

            // 
            // lblCharacters
            // 
            this.lblCharacters.AutoSize = true;
            this.lblCharacters.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.lblCharacters.Name = "lblCharacters";
            this.lblCharacters.Text = "Select Char:";

            // 
            // lstCharacters
            // 
            this.lstCharacters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstCharacters.FormattingEnabled = true;
            // ItemHeight 已经由基类自动计算，无需设置
            this.lstCharacters.Name = "lstCharacters";
            this.lstCharacters.TabIndex = 0;

            // 
            // SwitchCharacterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.MinimumSize = new System.Drawing.Size(400, 0);

            this.pnlContent.Controls.Add(this.tlpContent);
            this.Name = "SwitchCharacterForm";
            this.Text = "Switch Character";

            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.tlpContent.ResumeLayout(false);
            this.tlpContent.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TableLayoutPanel tlpContent;
        private DiabloTwoMFTimer.UI.Components.ThemedListBox lstCharacters;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblCharacters;
    }
}