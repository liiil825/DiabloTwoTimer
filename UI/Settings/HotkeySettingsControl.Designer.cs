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

        // 初始化控件
        this.lblStatus = new DiabloTwoMFTimer.UI.Components.ThemedLabel();

        this.lblLeaderKey = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.txtLeaderKey = new DiabloTwoMFTimer.UI.Components.ThemedTextBox();

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

        // 
        // HotkeySettingsControl (自身属性)
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.AutoScroll = true;
        this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.Controls.Add(this.tlpMain);
        this.Name = "HotkeySettingsControl";
        // 【关键】启用自动大小，防止截断
        // this.AutoSize = true;
        // this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        // this.Dock = System.Windows.Forms.DockStyle.Top;
        this.Dock = System.Windows.Forms.DockStyle.Fill;


        // 
        // tlpMain (主布局容器)
        // 
        this.tlpMain.AutoSize = true;
        this.tlpMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpMain.ColumnCount = 1;
        this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpMain.Controls.Add(this.grpHotkeys, 0, 0);
        this.tlpMain.Dock = System.Windows.Forms.DockStyle.Top;
        this.tlpMain.Location = new System.Drawing.Point(0, 0);
        this.tlpMain.Name = "tlpMain";
        this.tlpMain.Padding = new System.Windows.Forms.Padding(10);
        this.tlpMain.RowCount = 1;
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpMain.TabIndex = 0;

        // 
        // grpHotkeys (分组框)
        // 
        this.grpHotkeys.AutoSize = true;
        this.grpHotkeys.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.grpHotkeys.Controls.Add(this.tlpHotkeys);
        this.grpHotkeys.Dock = System.Windows.Forms.DockStyle.Fill;
        this.grpHotkeys.Location = new System.Drawing.Point(13, 13);
        this.grpHotkeys.Name = "grpHotkeys";
        this.grpHotkeys.Padding = new System.Windows.Forms.Padding(3, 20, 3, 3);
        this.grpHotkeys.TabIndex = 0;
        this.grpHotkeys.TabStop = false;
        this.grpHotkeys.Text = "快捷键设置";

        // 
        // tlpHotkeys (内部表格布局)
        // 
        this.tlpHotkeys.AutoSize = true;
        this.tlpHotkeys.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.tlpHotkeys.ColumnCount = 2;
        this.tlpHotkeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpHotkeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpHotkeys.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpHotkeys.Location = new System.Drawing.Point(3, 20); // 避开 GroupBox 标题
        this.tlpHotkeys.Name = "tlpHotkeys";
        this.tlpHotkeys.RowCount = 6;

        // 设置所有行为 AutoSize，确保内容撑开高度
        for (int i = 0; i < 6; i++)
        {
            this.tlpHotkeys.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        }

        // --- 布局添加顺序 ---

        // Row 0: 状态提示 (跨两列)
        this.tlpHotkeys.Controls.Add(this.lblStatus, 0, 0);
        this.tlpHotkeys.SetColumnSpan(this.lblStatus, 2);

        // Row 1: Leader Key
        this.tlpHotkeys.Controls.Add(this.lblLeaderKey, 0, 1);
        this.tlpHotkeys.Controls.Add(this.txtLeaderKey, 1, 1);

        // Row 2: Start/Next
        this.tlpHotkeys.Controls.Add(this.lblStartNext, 0, 2);
        this.tlpHotkeys.Controls.Add(this.txtStartNext, 1, 2);

        // Row 3: Pause
        this.tlpHotkeys.Controls.Add(this.lblPause, 0, 3);
        this.tlpHotkeys.Controls.Add(this.txtPause, 1, 3);

        // Row 4: Delete
        this.tlpHotkeys.Controls.Add(this.lblDeleteHistory, 0, 4);
        this.tlpHotkeys.Controls.Add(this.txtDeleteHistory, 1, 4);

        // Row 5: Record
        this.tlpHotkeys.Controls.Add(this.lblRecordLoot, 0, 5);
        this.tlpHotkeys.Controls.Add(this.txtRecordLoot, 1, 5);

        // 
        // 控件属性设置
        // 

        // lblStatus
        this.lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.lblStatus.AutoSize = true;
        this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        this.lblStatus.ForeColor = DiabloTwoMFTimer.UI.Theme.AppTheme.Colors.Primary;
        this.lblStatus.Margin = new System.Windows.Forms.Padding(5, 5, 5, 10); // 底部多留点空隙
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Text = ""; // 默认留空，运行时填充

        // 辅助方法设置 Label 和 TextBox
        ConfigureLabel(lblLeaderKey, "Leader Key:");
        ConfigureLabel(lblStartNext, "开始/下一局:");
        ConfigureLabel(lblPause, "暂停/恢复:");
        ConfigureLabel(lblDeleteHistory, "删除最后记录:");
        ConfigureLabel(lblRecordLoot, "记录掉落:");

        ConfigureTextBox(txtLeaderKey, "Leader");
        ConfigureTextBox(txtStartNext, "StartNext");
        ConfigureTextBox(txtPause, "Pause");
        ConfigureTextBox(txtDeleteHistory, "Delete");
        ConfigureTextBox(txtRecordLoot, "Record");

        this.tlpMain.ResumeLayout(false);
        this.tlpMain.PerformLayout();
        this.grpHotkeys.ResumeLayout(false);
        this.grpHotkeys.PerformLayout();
        this.tlpHotkeys.ResumeLayout(false);
        this.tlpHotkeys.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void ConfigureLabel(System.Windows.Forms.Label lbl, string text)
    {
        lbl.Anchor = System.Windows.Forms.AnchorStyles.Right;
        lbl.AutoSize = true;
        lbl.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
        lbl.Text = text;
    }

    private void ConfigureTextBox(System.Windows.Forms.TextBox txt, string tag)
    {
        txt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
        txt.Cursor = System.Windows.Forms.Cursors.Hand;
        txt.Margin = new System.Windows.Forms.Padding(5);
        txt.ReadOnly = true;
        txt.Tag = tag;
        txt.TabStop = false;
        txt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnHotkeyInput);
        txt.Enter += new System.EventHandler(this.OnTextBoxEnter);
        txt.Leave += new System.EventHandler(this.OnTextBoxLeave);
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tlpMain;
    private System.Windows.Forms.TableLayoutPanel tlpHotkeys;
    private DiabloTwoMFTimer.UI.Components.ThemedGroupBox grpHotkeys;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblStatus;

    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblLeaderKey;
    private DiabloTwoMFTimer.UI.Components.ThemedTextBox txtLeaderKey;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblStartNext;
    private DiabloTwoMFTimer.UI.Components.ThemedTextBox txtStartNext;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblPause;
    private DiabloTwoMFTimer.UI.Components.ThemedTextBox txtPause;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblDeleteHistory;
    private DiabloTwoMFTimer.UI.Components.ThemedTextBox txtDeleteHistory;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblRecordLoot;
    private DiabloTwoMFTimer.UI.Components.ThemedTextBox txtRecordLoot;
}