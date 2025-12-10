namespace DiabloTwoMFTimer.UI.Profiles
{
    partial class CreateCharacterForm
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
            this.lblCharacterName = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.txtCharacterName = new DiabloTwoMFTimer.UI.Components.ThemedTextBox();
            this.lblCharacterClass = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
            this.cmbCharacterClass = new DiabloTwoMFTimer.UI.Components.ThemedComboBox();

            this.pnlContent.SuspendLayout(); // 挂起基类容器
            this.tlpContent.SuspendLayout();
            this.SuspendLayout();

            // 
            // tlpContent
            // 
            this.tlpContent.AutoSize = true;
            this.tlpContent.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpContent.ColumnCount = 2;
            // 第一列：Label (AutoSize)
            this.tlpContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            // 第二列：Input (Percent 100%) - 占满剩余
            this.tlpContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

            this.tlpContent.Controls.Add(this.lblCharacterName, 0, 0);
            this.tlpContent.Controls.Add(this.txtCharacterName, 1, 0);
            this.tlpContent.Controls.Add(this.lblCharacterClass, 0, 1);
            this.tlpContent.Controls.Add(this.cmbCharacterClass, 1, 1);

            this.tlpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpContent.RowCount = 2;
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tlpContent.Padding = new System.Windows.Forms.Padding(0);

            // 
            // lblCharacterName
            // 
            this.lblCharacterName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCharacterName.AutoSize = true;
            this.lblCharacterName.Margin = new System.Windows.Forms.Padding(0, 10, 10, 10);
            this.lblCharacterName.Name = "lblCharacterName";
            this.lblCharacterName.Text = "Name:";

            // 
            // txtCharacterName
            // 
            this.txtCharacterName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCharacterName.Margin = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.txtCharacterName.Name = "txtCharacterName";
            this.txtCharacterName.TabIndex = 0;

            // 
            // lblCharacterClass
            // 
            this.lblCharacterClass.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCharacterClass.AutoSize = true;
            this.lblCharacterClass.Margin = new System.Windows.Forms.Padding(0, 10, 10, 10);
            this.lblCharacterClass.Name = "lblCharacterClass";
            this.lblCharacterClass.Text = "Class:";

            // 
            // cmbCharacterClass
            // 
            this.cmbCharacterClass.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbCharacterClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCharacterClass.Margin = new System.Windows.Forms.Padding(0, 10, 0, 10);
            this.cmbCharacterClass.Name = "cmbCharacterClass";
            this.cmbCharacterClass.TabIndex = 1;

            // 
            // CreateCharacterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            // 只需要设定一个最小宽度，高度自动
            this.MinimumSize = new System.Drawing.Size(400, 0);

            // 将布局添加到基类的 Content Panel
            this.pnlContent.Controls.Add(this.tlpContent);

            this.Name = "CreateCharacterForm";
            this.Text = "Create Character";

            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.tlpContent.ResumeLayout(false);
            this.tlpContent.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TableLayoutPanel tlpContent;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblCharacterName;
        private DiabloTwoMFTimer.UI.Components.ThemedTextBox txtCharacterName;
        private DiabloTwoMFTimer.UI.Components.ThemedLabel lblCharacterClass;
        private DiabloTwoMFTimer.UI.Components.ThemedComboBox cmbCharacterClass;
    }
}