namespace DiabloTwoMFTimer.UI.Settings;

partial class HotkeySettingsControl
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
        this.grpHotkeys = new DiabloTwoMFTimer.UI.Components.ThemedGroupBox();
        this.tlpHotkeys = new System.Windows.Forms.TableLayoutPanel();

        this.lblStartNext = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.txtStartNext = new DiabloTwoMFTimer.UI.Components.ThemedTextBox();
        this.lblPause = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.txtPause = new DiabloTwoMFTimer.UI.Components.ThemedTextBox();
        this.lblDeleteHistory = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.txtDeleteHistory = new DiabloTwoMFTimer.UI.Components.ThemedTextBox();
        this.lblRecordLoot = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.txtRecordLoot = new DiabloTwoMFTimer.UI.Components.ThemedTextBox();

        this.tlpMain.SuspendLayout();
        this.grpHotkeys.SuspendLayout();
        this.tlpHotkeys.SuspendLayout();
        this.SuspendLayout();

        this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;

        // 
        // tlpMain
        // 
        this.tlpMain.ColumnCount = 1;
        this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpMain.Controls.Add(this.grpHotkeys, 0, 0);
        // this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpMain.Dock = System.Windows.Forms.DockStyle.Top; // 改为 Top
        this.tlpMain.AutoSize = true; // 开启自动大小
        this.tlpMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpMain.Location = new System.Drawing.Point(0, 0);
        this.tlpMain.Name = "tlpMain";
        this.tlpMain.Padding = new System.Windows.Forms.Padding(10);
        this.tlpMain.RowCount = 2;
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpMain.Size = new System.Drawing.Size(350, 250);
        this.tlpMain.TabIndex = 0;

        // 
        // grpHotkeys
        // 
        this.grpHotkeys.AutoSize = true;
        this.grpHotkeys.Controls.Add(this.tlpHotkeys);
        this.grpHotkeys.Dock = System.Windows.Forms.DockStyle.Fill;
        this.grpHotkeys.Location = new System.Drawing.Point(13, 13);
        this.grpHotkeys.Name = "grpHotkeys";
        this.grpHotkeys.Padding = new System.Windows.Forms.Padding(3, 20, 3, 3);
        this.grpHotkeys.Size = new System.Drawing.Size(324, 200);
        this.grpHotkeys.TabIndex = 0;
        this.grpHotkeys.TabStop = false;
        this.grpHotkeys.Text = "快捷键设置";

        // 
        // tlpHotkeys (2 Cols, 4 Rows)
        // 
        this.tlpHotkeys.AutoSize = true;
        this.tlpHotkeys.ColumnCount = 2;
        this.tlpHotkeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpHotkeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

        // 添加控件
        this.tlpHotkeys.Controls.Add(this.lblStartNext, 0, 0);
        this.tlpHotkeys.Controls.Add(this.txtStartNext, 1, 0);
        this.tlpHotkeys.Controls.Add(this.lblPause, 0, 1);
        this.tlpHotkeys.Controls.Add(this.txtPause, 1, 1);
        this.tlpHotkeys.Controls.Add(this.lblDeleteHistory, 0, 2);
        this.tlpHotkeys.Controls.Add(this.txtDeleteHistory, 1, 2);
        this.tlpHotkeys.Controls.Add(this.lblRecordLoot, 0, 3);
        this.tlpHotkeys.Controls.Add(this.txtRecordLoot, 1, 3);

        this.tlpHotkeys.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpHotkeys.Location = new System.Drawing.Point(3, 20);
        this.tlpHotkeys.Name = "tlpHotkeys";
        this.tlpHotkeys.RowCount = 4;
        this.tlpHotkeys.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpHotkeys.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpHotkeys.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpHotkeys.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpHotkeys.Size = new System.Drawing.Size(318, 177);
        this.tlpHotkeys.TabIndex = 0;

        // Labels
        void SetLabel(System.Windows.Forms.Label lbl, string text)
        {
            lbl.Anchor = System.Windows.Forms.AnchorStyles.Right; // 右对齐
            lbl.AutoSize = true;
            lbl.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            lbl.Text = text;
        }
        SetLabel(lblStartNext, "开始/下一局:");
        SetLabel(lblPause, "暂停/恢复:");
        SetLabel(lblDeleteHistory, "删除最后记录:");
        SetLabel(lblRecordLoot, "记录掉落:");

        // TextBoxes
        void SetTextBox(System.Windows.Forms.TextBox txt, string tag)
        {
            txt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))); // 水平填充
            txt.Cursor = System.Windows.Forms.Cursors.Hand;
            txt.Margin = new System.Windows.Forms.Padding(5);
            txt.ReadOnly = true;
            txt.Tag = tag;
            // 事件在 .cs 文件中绑定
            txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnHotkeyInput);
            txt.Enter += new System.EventHandler(this.OnTextBoxEnter);
            txt.Leave += new System.EventHandler(this.OnTextBoxLeave);
        }
        SetTextBox(txtStartNext, "StartNext");
        SetTextBox(txtPause, "Pause");
        SetTextBox(txtDeleteHistory, "Delete");
        SetTextBox(txtRecordLoot, "Record");

        // 
        // HotkeySettingsControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.AutoScroll = true;
        this.Controls.Add(this.tlpMain);
        this.Name = "HotkeySettingsControl";
        this.Size = new System.Drawing.Size(350, 250);

        this.tlpMain.ResumeLayout(false);
        this.tlpMain.PerformLayout();
        this.grpHotkeys.ResumeLayout(false);
        this.grpHotkeys.PerformLayout();
        this.tlpHotkeys.ResumeLayout(false);
        this.tlpHotkeys.PerformLayout();
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tlpMain;
    private System.Windows.Forms.TableLayoutPanel tlpHotkeys;
    private DiabloTwoMFTimer.UI.Components.ThemedGroupBox grpHotkeys;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblStartNext;
    private DiabloTwoMFTimer.UI.Components.ThemedTextBox txtStartNext;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblPause;
    private DiabloTwoMFTimer.UI.Components.ThemedTextBox txtPause;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblDeleteHistory;
    private DiabloTwoMFTimer.UI.Components.ThemedTextBox txtDeleteHistory;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblRecordLoot;
    private DiabloTwoMFTimer.UI.Components.ThemedTextBox txtRecordLoot;
}