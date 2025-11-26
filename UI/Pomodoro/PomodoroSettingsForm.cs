using System;
using System.ComponentModel;
using System.Windows.Forms;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.UI.Common;

namespace DTwoMFTimerHelper.UI.Pomodoro {
    public class PomodoroSettingsForm : BaseForm {
        // 控件使用 null! 初始化
        private Label lblWorkTime = null!;
        private Label lblShortBreakTime = null!;
        private Label lblLongBreakTime = null!;
        private Label lblWorkTimeSec = null!;
        private Label lblShortBreakTimeSec = null!;
        private Label lblLongBreakTimeSec = null!;
        private NumericUpDown nudWorkTimeMin = null!;
        private NumericUpDown nudShortBreakTimeMin = null!;
        private NumericUpDown nudLongBreakTimeMin = null!;
        private NumericUpDown nudWorkTimeSec = null!;
        private NumericUpDown nudShortBreakTimeSec = null!;
        private NumericUpDown nudLongBreakTimeSec = null!;
        private readonly IContainer components = null!;

        // 属性
        public int WorkTimeMinutes { get; private set; }
        public int WorkTimeSeconds { get; private set; }
        public int ShortBreakMinutes { get; private set; }
        public int ShortBreakSeconds { get; private set; }
        public int LongBreakMinutes { get; private set; }
        public int LongBreakSeconds { get; private set; }

        // 1. 必需的无参构造函数，供设计器使用
        public PomodoroSettingsForm() {
            InitializeComponent();
        }

        // 2. 带参构造函数
        public PomodoroSettingsForm(int workTime, int shortBreakTime, int longBreakTime) : this() {
            WorkTimeMinutes = workTime;
            ShortBreakMinutes = shortBreakTime;
            LongBreakMinutes = longBreakTime;
        }

        // 3. 全参数构造函数
        public PomodoroSettingsForm(int workTimeMinutes, int workTimeSeconds, int shortBreakMinutes, int shortBreakSeconds, int longBreakMinutes, int longBreakSeconds) : this() {
            WorkTimeMinutes = workTimeMinutes;
            WorkTimeSeconds = workTimeSeconds;
            ShortBreakMinutes = shortBreakMinutes;
            ShortBreakSeconds = shortBreakSeconds;
            LongBreakMinutes = longBreakMinutes;
            LongBreakSeconds = longBreakSeconds;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            if (!this.DesignMode) {
                // 如果所有值都是0（通常是无参构造进入），尝试加载配置
                if (WorkTimeMinutes == 0 && ShortBreakMinutes == 0 && LongBreakMinutes == 0) {
                    LoadSettings();
                }
                UpdateUI();
            }
        }

        private void LoadSettings() {
            try {
                var settings = SettingsManager.LoadSettings();
                WorkTimeMinutes = settings.WorkTimeMinutes; // 假设设置里有Min
                WorkTimeSeconds = settings.WorkTimeSeconds;
                ShortBreakMinutes = settings.ShortBreakMinutes;
                ShortBreakSeconds = settings.ShortBreakSeconds;
                LongBreakMinutes = settings.LongBreakMinutes;
                LongBreakSeconds = settings.LongBreakSeconds;
            }
            catch { /* Ignore */ }
        }

        private void InitializeComponent() {
            lblWorkTime = new Label();
            lblShortBreakTime = new Label();
            lblLongBreakTime = new Label();
            lblWorkTimeSec = new Label();
            lblShortBreakTimeSec = new Label();
            lblLongBreakTimeSec = new Label();
            nudWorkTimeMin = new NumericUpDown();
            nudShortBreakTimeMin = new NumericUpDown();
            nudLongBreakTimeMin = new NumericUpDown();
            nudWorkTimeSec = new NumericUpDown();
            nudShortBreakTimeSec = new NumericUpDown();
            nudLongBreakTimeSec = new NumericUpDown();
            ((ISupportInitialize)nudWorkTimeMin).BeginInit();
            ((ISupportInitialize)nudShortBreakTimeMin).BeginInit();
            ((ISupportInitialize)nudLongBreakTimeMin).BeginInit();
            ((ISupportInitialize)nudWorkTimeSec).BeginInit();
            ((ISupportInitialize)nudShortBreakTimeSec).BeginInit();
            ((ISupportInitialize)nudLongBreakTimeSec).BeginInit();
            SuspendLayout();
            // 
            // btnConfirm
            // 
            btnConfirm.Location = new System.Drawing.Point(239, 330);
            btnConfirm.Margin = new Padding(5, 6, 5, 6);
            btnConfirm.Size = new System.Drawing.Size(130, 56);
            btnConfirm.Text = "保存";
            // 
            // btnCancel
            // 
            btnCancel.Location = new System.Drawing.Point(444, 330);
            btnCancel.Margin = new Padding(5, 6, 5, 6);
            btnCancel.Size = new System.Drawing.Size(130, 56);
            // 
            // lblWorkTime
            // 
            lblWorkTime.Location = new System.Drawing.Point(67, 52);
            lblWorkTime.Margin = new Padding(5, 0, 5, 0);
            lblWorkTime.Name = "lblWorkTime";
            lblWorkTime.Size = new System.Drawing.Size(211, 67);
            lblWorkTime.TabIndex = 0;
            lblWorkTime.Text = "Work Time (min):";
            // 
            // lblShortBreakTime
            // 
            lblShortBreakTime.Location = new System.Drawing.Point(67, 127);
            lblShortBreakTime.Margin = new Padding(5, 0, 5, 0);
            lblShortBreakTime.Name = "lblShortBreakTime";
            lblShortBreakTime.Size = new System.Drawing.Size(211, 67);
            lblShortBreakTime.TabIndex = 4;
            // 
            // lblLongBreakTime
            // 
            lblLongBreakTime.Location = new System.Drawing.Point(67, 194);
            lblLongBreakTime.Margin = new Padding(5, 0, 5, 0);
            lblLongBreakTime.Name = "lblLongBreakTime";
            lblLongBreakTime.Size = new System.Drawing.Size(211, 75);
            lblLongBreakTime.TabIndex = 8;
            // 
            // lblWorkTimeSec
            // 
            lblWorkTimeSec.Location = new System.Drawing.Point(411, 50);
            lblWorkTimeSec.Margin = new Padding(5, 0, 5, 0);
            lblWorkTimeSec.Name = "lblWorkTimeSec";
            lblWorkTimeSec.Size = new System.Drawing.Size(76, 60);
            lblWorkTimeSec.TabIndex = 2;
            lblWorkTimeSec.Text = "sec:";
            // 
            // lblShortBreakTimeSec
            // 
            lblShortBreakTimeSec.Location = new System.Drawing.Point(411, 131);
            lblShortBreakTimeSec.Margin = new Padding(5, 0, 5, 0);
            lblShortBreakTimeSec.Name = "lblShortBreakTimeSec";
            lblShortBreakTimeSec.Size = new System.Drawing.Size(76, 43);
            lblShortBreakTimeSec.TabIndex = 6;
            // 
            // lblLongBreakTimeSec
            // 
            lblLongBreakTimeSec.Location = new System.Drawing.Point(411, 205);
            lblLongBreakTimeSec.Margin = new Padding(5, 0, 5, 0);
            lblLongBreakTimeSec.Name = "lblLongBreakTimeSec";
            lblLongBreakTimeSec.Size = new System.Drawing.Size(76, 60);
            lblLongBreakTimeSec.TabIndex = 10;
            // 
            // nudWorkTimeMin
            // 
            nudWorkTimeMin.Location = new System.Drawing.Point(287, 50);
            nudWorkTimeMin.Margin = new Padding(5, 6, 5, 6);
            nudWorkTimeMin.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            nudWorkTimeMin.Name = "nudWorkTimeMin";
            nudWorkTimeMin.Size = new System.Drawing.Size(114, 34);
            nudWorkTimeMin.TabIndex = 1;
            // 
            // nudShortBreakTimeMin
            // 
            nudShortBreakTimeMin.Location = new System.Drawing.Point(288, 131);
            nudShortBreakTimeMin.Margin = new Padding(5, 6, 5, 6);
            nudShortBreakTimeMin.Maximum = new decimal(new int[] { 30, 0, 0, 0 });
            nudShortBreakTimeMin.Name = "nudShortBreakTimeMin";
            nudShortBreakTimeMin.Size = new System.Drawing.Size(114, 34);
            nudShortBreakTimeMin.TabIndex = 5;
            // 
            // nudLongBreakTimeMin
            // 
            nudLongBreakTimeMin.Location = new System.Drawing.Point(288, 205);
            nudLongBreakTimeMin.Margin = new Padding(5, 6, 5, 6);
            nudLongBreakTimeMin.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            nudLongBreakTimeMin.Name = "nudLongBreakTimeMin";
            nudLongBreakTimeMin.Size = new System.Drawing.Size(114, 34);
            nudLongBreakTimeMin.TabIndex = 9;
            // 
            // nudWorkTimeSec
            // 
            nudWorkTimeSec.Location = new System.Drawing.Point(497, 48);
            nudWorkTimeSec.Margin = new Padding(5, 6, 5, 6);
            nudWorkTimeSec.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            nudWorkTimeSec.Name = "nudWorkTimeSec";
            nudWorkTimeSec.Size = new System.Drawing.Size(114, 34);
            nudWorkTimeSec.TabIndex = 3;
            // 
            // nudShortBreakTimeSec
            // 
            nudShortBreakTimeSec.Location = new System.Drawing.Point(497, 131);
            nudShortBreakTimeSec.Margin = new Padding(5, 6, 5, 6);
            nudShortBreakTimeSec.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            nudShortBreakTimeSec.Name = "nudShortBreakTimeSec";
            nudShortBreakTimeSec.Size = new System.Drawing.Size(114, 34);
            nudShortBreakTimeSec.TabIndex = 7;
            // 
            // nudLongBreakTimeSec
            // 
            nudLongBreakTimeSec.Location = new System.Drawing.Point(497, 205);
            nudLongBreakTimeSec.Margin = new Padding(5, 6, 5, 6);
            nudLongBreakTimeSec.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            nudLongBreakTimeSec.Name = "nudLongBreakTimeSec";
            nudLongBreakTimeSec.Size = new System.Drawing.Size(114, 34);
            nudLongBreakTimeSec.TabIndex = 11;
            // 
            // PomodoroSettingsForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(819, 623);
            Controls.Add(lblWorkTime);
            Controls.Add(nudWorkTimeMin);
            Controls.Add(lblWorkTimeSec);
            Controls.Add(nudWorkTimeSec);
            Controls.Add(lblShortBreakTime);
            Controls.Add(nudShortBreakTimeMin);
            Controls.Add(lblShortBreakTimeSec);
            Controls.Add(nudShortBreakTimeSec);
            Controls.Add(lblLongBreakTime);
            Controls.Add(nudLongBreakTimeMin);
            Controls.Add(lblLongBreakTimeSec);
            Controls.Add(nudLongBreakTimeSec);
            Margin = new Padding(5, 6, 5, 6);
            Name = "PomodoroSettingsForm";
            Text = "番茄时钟设置";
            Controls.SetChildIndex(nudLongBreakTimeSec, 0);
            Controls.SetChildIndex(lblLongBreakTimeSec, 0);
            Controls.SetChildIndex(nudLongBreakTimeMin, 0);
            Controls.SetChildIndex(lblLongBreakTime, 0);
            Controls.SetChildIndex(nudShortBreakTimeSec, 0);
            Controls.SetChildIndex(lblShortBreakTimeSec, 0);
            Controls.SetChildIndex(nudShortBreakTimeMin, 0);
            Controls.SetChildIndex(lblShortBreakTime, 0);
            Controls.SetChildIndex(nudWorkTimeSec, 0);
            Controls.SetChildIndex(lblWorkTimeSec, 0);
            Controls.SetChildIndex(nudWorkTimeMin, 0);
            Controls.SetChildIndex(lblWorkTime, 0);
            Controls.SetChildIndex(btnConfirm, 0);
            Controls.SetChildIndex(btnCancel, 0);
            ((ISupportInitialize)nudWorkTimeMin).EndInit();
            ((ISupportInitialize)nudShortBreakTimeMin).EndInit();
            ((ISupportInitialize)nudLongBreakTimeMin).EndInit();
            ((ISupportInitialize)nudWorkTimeSec).EndInit();
            ((ISupportInitialize)nudShortBreakTimeSec).EndInit();
            ((ISupportInitialize)nudLongBreakTimeSec).EndInit();
            ResumeLayout(false);
        }

        protected override void UpdateUI() {
            base.UpdateUI();

            // 1. 设置数值
            nudWorkTimeMin.Value = Math.Max(nudWorkTimeMin.Minimum, Math.Min(nudWorkTimeMin.Maximum, WorkTimeMinutes));
            nudWorkTimeSec.Value = Math.Max(nudWorkTimeSec.Minimum, Math.Min(nudWorkTimeSec.Maximum, WorkTimeSeconds));
            nudShortBreakTimeMin.Value = Math.Max(nudShortBreakTimeMin.Minimum, Math.Min(nudShortBreakTimeMin.Maximum, ShortBreakMinutes));
            nudShortBreakTimeSec.Value = Math.Max(nudShortBreakTimeSec.Minimum, Math.Min(nudShortBreakTimeSec.Maximum, ShortBreakSeconds));
            nudLongBreakTimeMin.Value = Math.Max(nudLongBreakTimeMin.Minimum, Math.Min(nudLongBreakTimeMin.Maximum, LongBreakMinutes));
            nudLongBreakTimeSec.Value = Math.Max(nudLongBreakTimeSec.Minimum, Math.Min(nudLongBreakTimeSec.Maximum, LongBreakSeconds));

            // 2. 本地化文本
            this.Text = LanguageManager.GetString("PomodoroSettings") ?? "番茄时钟设置";
            lblWorkTime.Text = LanguageManager.GetString("WorkTime") ?? "工作时间(分):";
            lblShortBreakTime.Text = LanguageManager.GetString("ShortBreakTime") ?? "短休息时间(分):";
            lblLongBreakTime.Text = LanguageManager.GetString("LongBreakTime") ?? "长休息时间(分):";

            if (btnConfirm != null) btnConfirm.Text = LanguageManager.GetString("Save") ?? "保存";
        }

        protected override void BtnConfirm_Click(object? sender, EventArgs e) {
            WorkTimeMinutes = (int)nudWorkTimeMin.Value;
            WorkTimeSeconds = (int)nudWorkTimeSec.Value;
            ShortBreakMinutes = (int)nudShortBreakTimeMin.Value;
            ShortBreakSeconds = (int)nudShortBreakTimeSec.Value;
            LongBreakMinutes = (int)nudLongBreakTimeMin.Value;
            LongBreakSeconds = (int)nudLongBreakTimeSec.Value;

            base.BtnConfirm_Click(sender, e);
        }

        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}