namespace DiabloTwoMFTimer.UI.Settings;

partial class GeneralSettingsControl
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
        this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
        this.groupBoxPosition = new DiabloTwoMFTimer.UI.Components.ThemedGroupBox();
        this.tlpPosition = new System.Windows.Forms.TableLayoutPanel();
        this.radioTopLeft = new DiabloTwoMFTimer.UI.Components.ThemedRadioButton();
        this.radioTopCenter = new DiabloTwoMFTimer.UI.Components.ThemedRadioButton();
        this.radioTopRight = new DiabloTwoMFTimer.UI.Components.ThemedRadioButton();
        this.radioBottomLeft = new DiabloTwoMFTimer.UI.Components.ThemedRadioButton();
        this.radioBottomCenter = new DiabloTwoMFTimer.UI.Components.ThemedRadioButton();
        this.radioBottomRight = new DiabloTwoMFTimer.UI.Components.ThemedRadioButton();

        this.groupBoxLanguage = new DiabloTwoMFTimer.UI.Components.ThemedGroupBox();
        this.tlpLanguage = new System.Windows.Forms.TableLayoutPanel();
        this.chineseRadioButton = new DiabloTwoMFTimer.UI.Components.ThemedRadioButton();
        this.englishRadioButton = new DiabloTwoMFTimer.UI.Components.ThemedRadioButton();

        this.alwaysOnTopCheckBox = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();

        this.grpOpacity = new DiabloTwoMFTimer.UI.Components.ThemedGroupBox();
        this.cmbOpacity = new DiabloTwoMFTimer.UI.Components.ThemedComboBox();

        this.grpUiScale = new DiabloTwoMFTimer.UI.Components.ThemedGroupBox();
        this.cmbUiScale = new DiabloTwoMFTimer.UI.Components.ThemedComboBox();

        this.tlpMain.SuspendLayout();
        this.groupBoxPosition.SuspendLayout();
        this.tlpPosition.SuspendLayout();
        this.groupBoxLanguage.SuspendLayout();
        this.tlpLanguage.SuspendLayout();
        this.grpUiScale.SuspendLayout();
        this.SuspendLayout();

        // 
        // tlpMain
        // 
        this.tlpMain.AutoSize = true;
        this.tlpMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpMain.ColumnCount = 1;
        this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

        // --- 顺序调整 ---
        this.tlpMain.Controls.Add(this.groupBoxPosition, 0, 0);
        this.tlpMain.Controls.Add(this.groupBoxLanguage, 0, 1);
        this.tlpMain.Controls.Add(this.grpOpacity, 0, 2);
        this.tlpMain.Controls.Add(this.grpUiScale, 0, 3);
        this.tlpMain.Controls.Add(this.alwaysOnTopCheckBox, 0, 4);

        this.tlpMain.Dock = System.Windows.Forms.DockStyle.Top;
        this.tlpMain.Location = new System.Drawing.Point(0, 0);
        this.tlpMain.Name = "tlpMain";
        // 【修改点 1】这里改为 10，与 Hotkey/Timer 页面保持一致
        this.tlpMain.Padding = new System.Windows.Forms.Padding(10);
        this.tlpMain.RowCount = 5;
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpMain.TabIndex = 0;

        // 
        // groupBoxPosition
        // 
        this.groupBoxPosition.AutoSize = true;
        this.groupBoxPosition.Controls.Add(this.tlpPosition);
        this.groupBoxPosition.Dock = System.Windows.Forms.DockStyle.Fill;
        // Padding 10 之后，这里的 margin 可以稍微小一点，或者保持一致
        this.groupBoxPosition.Location = new System.Drawing.Point(13, 13);
        this.groupBoxPosition.Name = "groupBoxPosition";
        this.groupBoxPosition.Padding = new System.Windows.Forms.Padding(3, 20, 3, 3);
        this.groupBoxPosition.Size = new System.Drawing.Size(344, 100);
        this.groupBoxPosition.TabIndex = 0;
        this.groupBoxPosition.TabStop = false;
        this.groupBoxPosition.Text = "窗口位置";

        // 
        // tlpPosition
        // 
        this.tlpPosition.AutoSize = true;
        this.tlpPosition.ColumnCount = 3;
        this.tlpPosition.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
        this.tlpPosition.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
        this.tlpPosition.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
        this.tlpPosition.Controls.Add(this.radioTopLeft, 0, 0);
        this.tlpPosition.Controls.Add(this.radioTopCenter, 1, 0);
        this.tlpPosition.Controls.Add(this.radioTopRight, 2, 0);
        this.tlpPosition.Controls.Add(this.radioBottomLeft, 0, 1);
        this.tlpPosition.Controls.Add(this.radioBottomCenter, 1, 1);
        this.tlpPosition.Controls.Add(this.radioBottomRight, 2, 1);
        this.tlpPosition.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpPosition.Location = new System.Drawing.Point(3, 20);
        this.tlpPosition.Name = "tlpPosition";
        this.tlpPosition.RowCount = 2;
        this.tlpPosition.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpPosition.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpPosition.Size = new System.Drawing.Size(338, 77);
        this.tlpPosition.TabIndex = 0;

        void SetRadio(System.Windows.Forms.RadioButton rb, string text)
        {
            rb.AutoSize = true;
            rb.Dock = System.Windows.Forms.DockStyle.Fill;
            rb.Margin = new System.Windows.Forms.Padding(5);
            rb.Text = text;
        }
        SetRadio(radioTopLeft, "左上"); radioTopLeft.Checked = true;
        SetRadio(radioTopCenter, "上中");
        SetRadio(radioTopRight, "右上");
        SetRadio(radioBottomLeft, "左下");
        SetRadio(radioBottomCenter, "下中");
        SetRadio(radioBottomRight, "右下");

        // 
        // grpOpacity
        // 
        this.grpOpacity.AutoSize = true;
        this.grpOpacity.Controls.Add(this.cmbOpacity);
        this.grpOpacity.Dock = System.Windows.Forms.DockStyle.Fill;
        this.grpOpacity.Location = new System.Drawing.Point(3, 175);
        this.grpOpacity.Name = "grpOpacity";
        this.grpOpacity.Padding = new System.Windows.Forms.Padding(3, 20, 3, 3);
        this.grpOpacity.TabIndex = 2;
        this.grpOpacity.TabStop = false;
        this.grpOpacity.Text = "窗口透明度";

        // 
        // cmbOpacity
        // 
        this.cmbOpacity.Dock = System.Windows.Forms.DockStyle.Top;
        this.cmbOpacity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbOpacity.Location = new System.Drawing.Point(3, 20);
        this.cmbOpacity.Name = "cmbOpacity";
        this.cmbOpacity.TabIndex = 0;

        // 
        // groupBoxLanguage
        // 
        this.groupBoxLanguage.AutoSize = true;
        this.groupBoxLanguage.Controls.Add(this.tlpLanguage);
        this.groupBoxLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
        this.groupBoxLanguage.Location = new System.Drawing.Point(3, 109);
        this.groupBoxLanguage.Name = "groupBoxLanguage";
        this.groupBoxLanguage.Padding = new System.Windows.Forms.Padding(3, 20, 3, 3);
        this.groupBoxLanguage.TabIndex = 1;
        this.groupBoxLanguage.TabStop = false;
        this.groupBoxLanguage.Text = "语言";

        // 
        // tlpLanguage
        // 
        this.tlpLanguage.AutoSize = true;
        this.tlpLanguage.ColumnCount = 2;
        this.tlpLanguage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.tlpLanguage.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.tlpLanguage.Controls.Add(this.chineseRadioButton, 0, 0);
        this.tlpLanguage.Controls.Add(this.englishRadioButton, 1, 0);
        this.tlpLanguage.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpLanguage.Location = new System.Drawing.Point(3, 20);
        this.tlpLanguage.Name = "tlpLanguage";
        this.tlpLanguage.RowCount = 1;
        this.tlpLanguage.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpLanguage.TabIndex = 0;

        SetRadio(chineseRadioButton, "Chinese"); chineseRadioButton.Checked = true;
        SetRadio(englishRadioButton, "English");

        // 
        // grpUiScale
        // 
        this.grpUiScale.AutoSize = true;
        this.grpUiScale.Controls.Add(this.cmbUiScale);
        this.grpUiScale.Dock = System.Windows.Forms.DockStyle.Fill;
        this.grpUiScale.Location = new System.Drawing.Point(3, 175);
        this.grpUiScale.Name = "grpUiScale";
        this.grpUiScale.Padding = new System.Windows.Forms.Padding(3, 20, 3, 3);
        this.grpUiScale.TabIndex = 2;
        this.grpUiScale.TabStop = false;
        this.grpUiScale.Text = "界面大小";

        // 
        // cmbUiScale
        // 
        this.cmbUiScale.Dock = System.Windows.Forms.DockStyle.Top;
        this.cmbUiScale.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbUiScale.Location = new System.Drawing.Point(3, 20);
        this.cmbUiScale.Name = "cmbUiScale";
        this.cmbUiScale.TabIndex = 0;

        // 
        // alwaysOnTopCheckBox
        // 
        this.alwaysOnTopCheckBox.AutoSize = true;
        this.alwaysOnTopCheckBox.Checked = true;
        this.alwaysOnTopCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
        this.alwaysOnTopCheckBox.Dock = System.Windows.Forms.DockStyle.Fill;
        this.alwaysOnTopCheckBox.Location = new System.Drawing.Point(13, 241);
        this.alwaysOnTopCheckBox.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
        this.alwaysOnTopCheckBox.Name = "alwaysOnTopCheckBox";
        this.alwaysOnTopCheckBox.TabIndex = 3;
        this.alwaysOnTopCheckBox.Text = "总在最前";

        // 
        // GeneralSettingsControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.AutoScroll = true;
        this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.Controls.Add(this.tlpMain);
        this.Name = "GeneralSettingsControl";

        this.tlpMain.ResumeLayout(false);
        this.tlpMain.PerformLayout();
        this.groupBoxPosition.ResumeLayout(false);
        this.groupBoxPosition.PerformLayout();
        this.tlpPosition.ResumeLayout(false);
        this.tlpPosition.PerformLayout();
        this.groupBoxLanguage.ResumeLayout(false);
        this.groupBoxLanguage.PerformLayout();
        this.tlpLanguage.ResumeLayout(false);
        this.tlpLanguage.PerformLayout();
        this.grpUiScale.ResumeLayout(false);
        this.grpUiScale.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tlpMain;
    private System.Windows.Forms.TableLayoutPanel tlpPosition;
    private System.Windows.Forms.TableLayoutPanel tlpLanguage;
    private DiabloTwoMFTimer.UI.Components.ThemedGroupBox groupBoxPosition;
    private DiabloTwoMFTimer.UI.Components.ThemedGroupBox groupBoxLanguage;
    private DiabloTwoMFTimer.UI.Components.ThemedRadioButton radioTopLeft;
    private DiabloTwoMFTimer.UI.Components.ThemedRadioButton radioTopCenter;
    private DiabloTwoMFTimer.UI.Components.ThemedRadioButton radioTopRight;
    private DiabloTwoMFTimer.UI.Components.ThemedRadioButton radioBottomLeft;
    private DiabloTwoMFTimer.UI.Components.ThemedRadioButton radioBottomCenter;
    private DiabloTwoMFTimer.UI.Components.ThemedRadioButton radioBottomRight;
    private DiabloTwoMFTimer.UI.Components.ThemedRadioButton chineseRadioButton;
    private DiabloTwoMFTimer.UI.Components.ThemedRadioButton englishRadioButton;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox alwaysOnTopCheckBox;

    private DiabloTwoMFTimer.UI.Components.ThemedGroupBox grpUiScale;
    private DiabloTwoMFTimer.UI.Components.ThemedComboBox cmbUiScale;
    private DiabloTwoMFTimer.UI.Components.ThemedGroupBox grpOpacity;
    private DiabloTwoMFTimer.UI.Components.ThemedComboBox cmbOpacity;
}