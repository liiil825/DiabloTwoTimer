namespace DiabloTwoMFTimer.UI.Settings;

partial class AudioSettingsControl
{
    private System.ComponentModel.IContainer components = null;

    // 核心控件
    private DiabloTwoMFTimer.UI.Components.ThemedGroupBox grpAudioSettings;
    private System.Windows.Forms.TableLayoutPanel tlpMain;
    private DiabloTwoMFTimer.UI.Components.ThemedCheckBox chkEnableAudio;
    private System.Windows.Forms.TableLayoutPanel tlpVolume;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblVolumeTitle;
    private System.Windows.Forms.TrackBar trackVolume;
    private DiabloTwoMFTimer.UI.Components.ThemedLabel lblVolumeValue;
    private System.Windows.Forms.TableLayoutPanel tblAudioRows;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.grpAudioSettings = new DiabloTwoMFTimer.UI.Components.ThemedGroupBox();
        this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
        this.chkEnableAudio = new DiabloTwoMFTimer.UI.Components.ThemedCheckBox();
        this.tlpVolume = new System.Windows.Forms.TableLayoutPanel();
        this.lblVolumeTitle = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.trackVolume = new System.Windows.Forms.TrackBar();
        this.lblVolumeValue = new DiabloTwoMFTimer.UI.Components.ThemedLabel();
        this.tblAudioRows = new System.Windows.Forms.TableLayoutPanel();

        ((System.ComponentModel.ISupportInitialize)(this.trackVolume)).BeginInit();
        this.grpAudioSettings.SuspendLayout();
        this.tlpMain.SuspendLayout();
        this.tlpVolume.SuspendLayout();
        this.SuspendLayout();

        // 
        // grpAudioSettings
        // 
        this.grpAudioSettings.AutoSize = true;
        this.grpAudioSettings.Controls.Add(this.tlpMain);
        this.grpAudioSettings.Dock = System.Windows.Forms.DockStyle.Top;
        this.grpAudioSettings.Location = new System.Drawing.Point(0, 0);
        this.grpAudioSettings.Name = "grpAudioSettings";
        this.grpAudioSettings.Padding = new System.Windows.Forms.Padding(3, 20, 3, 3);
        this.grpAudioSettings.TabIndex = 0;
        this.grpAudioSettings.TabStop = false;
        this.grpAudioSettings.Text = "声音设置";

        // 
        // tlpMain
        // 
        this.tlpMain.ColumnCount = 1;
        this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tlpMain.Controls.Add(this.chkEnableAudio, 0, 0);
        this.tlpMain.Controls.Add(this.tlpVolume, 0, 1);
        this.tlpMain.Controls.Add(this.tblAudioRows, 0, 2);
        this.tlpMain.Dock = System.Windows.Forms.DockStyle.Top;
        this.tlpMain.Location = new System.Drawing.Point(3, 20);
        this.tlpMain.Name = "tlpMain";
        this.tlpMain.RowCount = 3;
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpMain.Size = new System.Drawing.Size(644, 400);
        this.tlpMain.TabIndex = 0;

        // 
        // chkEnableAudio
        // 
        this.chkEnableAudio.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.chkEnableAudio.AutoSize = true;
        this.chkEnableAudio.Location = new System.Drawing.Point(10, 10);
        this.chkEnableAudio.Margin = new System.Windows.Forms.Padding(10, 10, 10, 10);
        this.chkEnableAudio.Name = "chkEnableAudio";
        this.chkEnableAudio.Size = new System.Drawing.Size(120, 24);
        this.chkEnableAudio.TabIndex = 0;
        this.chkEnableAudio.Text = "Enable Audio";

        // 
        // tlpVolume
        // 
        this.tlpVolume.AutoSize = true;
        this.tlpVolume.ColumnCount = 3;
        this.tlpVolume.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
        this.tlpVolume.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
        this.tlpVolume.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
        this.tlpVolume.Controls.Add(this.lblVolumeTitle, 0, 0);
        this.tlpVolume.Controls.Add(this.trackVolume, 1, 0);
        this.tlpVolume.Controls.Add(this.lblVolumeValue, 2, 0);
        this.tlpVolume.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tlpVolume.Location = new System.Drawing.Point(3, 44);
        this.tlpVolume.Name = "tlpVolume";
        this.tlpVolume.RowCount = 1;
        this.tlpVolume.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tlpVolume.Size = new System.Drawing.Size(270, 50);
        this.tlpVolume.TabIndex = 1;

        // 
        // lblVolumeTitle
        // 
        this.lblVolumeTitle.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        this.lblVolumeTitle.AutoSize = true;
        this.lblVolumeTitle.Location = new System.Drawing.Point(3, 15);
        this.lblVolumeTitle.Name = "lblVolumeTitle";
        this.lblVolumeTitle.Size = new System.Drawing.Size(54, 20);
        this.lblVolumeTitle.TabIndex = 0;
        this.lblVolumeTitle.Text = "Volume";

        // 
        // trackVolume
        // 
        this.trackVolume.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        this.trackVolume.Location = new System.Drawing.Point(63, 10);
        this.trackVolume.Maximum = 100;
        this.trackVolume.Name = "trackVolume";
        this.trackVolume.Size = new System.Drawing.Size(522, 30);
        this.trackVolume.TabIndex = 1;
        this.trackVolume.TickFrequency = 10;
        this.trackVolume.TickStyle = System.Windows.Forms.TickStyle.None;

        // 
        // lblVolumeValue
        // 
        this.lblVolumeValue.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        this.lblVolumeValue.AutoSize = true;
        this.lblVolumeValue.Location = new System.Drawing.Point(591, 15);
        this.lblVolumeValue.Name = "lblVolumeValue";
        this.lblVolumeValue.Size = new System.Drawing.Size(44, 20);
        this.lblVolumeValue.TabIndex = 2;
        this.lblVolumeValue.Text = "100%";

        // 
        // tblAudioRows (使用表格布局管理4行配置)
        // 
        this.tblAudioRows.ColumnCount = 3;
        this.tblAudioRows.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize)); // Label
        this.tblAudioRows.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize)); // Sound File Name
        this.tblAudioRows.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize)); // Button
        this.tblAudioRows.Dock = System.Windows.Forms.DockStyle.Top;
        this.tblAudioRows.Location = new System.Drawing.Point(3, 94);
        this.tblAudioRows.Name = "tblAudioRows";
        this.tblAudioRows.RowCount = 5; // 预留
        this.tblAudioRows.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
        this.tblAudioRows.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
        this.tblAudioRows.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
        this.tblAudioRows.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
        this.tblAudioRows.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
        this.tblAudioRows.AutoSize = true;
        this.tblAudioRows.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);

        // 
        // AudioSettingsControl
        // 
        this.AutoScroll = true;
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = DiabloTwoMFTimer.UI.Theme.AppTheme.BackColor;
        this.Controls.Add(this.grpAudioSettings);
        this.Name = "AudioSettingsControl";
        this.Size = new System.Drawing.Size(650, 450);

        ((System.ComponentModel.ISupportInitialize)(this.trackVolume)).EndInit();
        this.grpAudioSettings.ResumeLayout(false);
        this.tlpMain.ResumeLayout(false);
        this.tlpMain.PerformLayout();
        this.tlpVolume.ResumeLayout(false);
        this.tlpVolume.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}