using System;
using System.ComponentModel;
using System.Windows.Forms;
using DTwoMFTimerHelper.Services;
using DTwoMFTimerHelper.Utils;
using DTwoMFTimerHelper.UI.Common;

namespace DTwoMFTimerHelper.UI.Pomodoro {
    public class PomodoroSettingsForm : BaseForm {
        // 主要标签
        private Label lblWorkTime = null!;
        private Label lblShortBreakTime = null!;
        private Label lblLongBreakTime = null!;

        // 单位标签 (分)
        private Label lblWorkMinUnit = null!;
        private Label lblShortBreakMinUnit = null!;
        private Label lblLongBreakMinUnit = null!;

        // 输入框
        private NumericUpDown nudWorkTimeMin = null!;
        private NumericUpDown nudWorkTimeSec = null!;
        private NumericUpDown nudShortBreakTimeMin = null!;
        private NumericUpDown nudShortBreakTimeSec = null!;
        private NumericUpDown nudLongBreakTimeMin = null!;
        private NumericUpDown nudLongBreakTimeSec = null!;
        // 单位标签 (秒)
        private Label lblWorkSecUnit = null!;
        private Label lblShortBreakSecUnit = null!;
        private Label lblLongBreakSecUnit = null!;
        // 控件声明
        private Label lblWarningLongTime = null!;
        private Label lblWarningShortTime = null!;
        private Label lblWarningLongTimeUnit = null!;
        private Label lblWarningShortTimeUnit = null!;
        private NumericUpDown nudWarningLongTime = null!;
        private NumericUpDown nudWarningShortTime = null!;

        private readonly IContainer components = null!;
        private readonly IAppSettings _appSettings = null!;

        // 属性
        public int WorkTimeMinutes { get; private set; }
        public int WorkTimeSeconds { get; private set; }
        public int ShortBreakMinutes { get; private set; }
        public int ShortBreakSeconds { get; private set; }
        public int LongBreakMinutes { get; private set; }
        public int LongBreakSeconds { get; private set; }
        public int WarningLongTime { get; private set; }
        public int WarningShortTime { get; private set; }

        public PomodoroSettingsForm() {
            InitializeComponent();
        }

        public PomodoroSettingsForm(IAppSettings appSettings, int workTimeMinutes, int workTimeSeconds, int shortBreakMinutes, int shortBreakSeconds, int longBreakMinutes, int longBreakSeconds, int warningLongTime, int warningShortTime) : this() {
            _appSettings = appSettings;

            WorkTimeMinutes = workTimeMinutes;
            WorkTimeSeconds = workTimeSeconds;
            ShortBreakMinutes = shortBreakMinutes;
            ShortBreakSeconds = shortBreakSeconds;
            LongBreakMinutes = longBreakMinutes;
            LongBreakSeconds = longBreakSeconds;
            WarningLongTime = warningLongTime;
            WarningShortTime = warningShortTime;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            if (!this.DesignMode) {
                // 如果是默认初始化（全0），尝试加载配置
                if (WorkTimeMinutes == 0 && ShortBreakMinutes == 0 && LongBreakMinutes == 0) {
                    LoadSettings();
                }
                UpdateUI();
            }
        }

        private void LoadSettings() {
            WorkTimeMinutes = _appSettings.WorkTimeMinutes;
            WorkTimeSeconds = _appSettings.WorkTimeSeconds;
            ShortBreakMinutes = _appSettings.ShortBreakMinutes;
            ShortBreakSeconds = _appSettings.ShortBreakSeconds;
            LongBreakMinutes = _appSettings.LongBreakMinutes;
            LongBreakSeconds = _appSettings.LongBreakSeconds;
            WarningLongTime = _appSettings.PomodoroWarningLongTime;
            WarningShortTime = _appSettings.PomodoroWarningShortTime;
        }

        private void InitializeComponent() {
            // 初始化控件
            this.lblWorkTime = new System.Windows.Forms.Label();
            this.lblShortBreakTime = new System.Windows.Forms.Label();
            this.lblLongBreakTime = new System.Windows.Forms.Label();
            this.lblWarningLongTime = new System.Windows.Forms.Label();
            this.lblWarningShortTime = new System.Windows.Forms.Label();

            this.lblWorkMinUnit = new System.Windows.Forms.Label();
            this.lblShortBreakMinUnit = new System.Windows.Forms.Label();
            this.lblLongBreakMinUnit = new System.Windows.Forms.Label();
            this.lblWarningLongTimeUnit = new System.Windows.Forms.Label();
            this.lblWarningShortTimeUnit = new System.Windows.Forms.Label();

            this.lblWorkSecUnit = new System.Windows.Forms.Label();
            this.lblShortBreakSecUnit = new System.Windows.Forms.Label();
            this.lblLongBreakSecUnit = new System.Windows.Forms.Label();
            this.lblWarningLongTimeUnit = new System.Windows.Forms.Label();
            this.lblWarningShortTimeUnit = new System.Windows.Forms.Label();

            this.nudWorkTimeMin = new System.Windows.Forms.NumericUpDown();
            this.nudWorkTimeSec = new System.Windows.Forms.NumericUpDown();
            this.nudShortBreakTimeMin = new System.Windows.Forms.NumericUpDown();
            this.nudShortBreakTimeSec = new System.Windows.Forms.NumericUpDown();
            this.nudLongBreakTimeMin = new System.Windows.Forms.NumericUpDown();
            this.nudLongBreakTimeSec = new System.Windows.Forms.NumericUpDown();
            this.nudWarningLongTime = new System.Windows.Forms.NumericUpDown();
            this.nudWarningShortTime = new System.Windows.Forms.NumericUpDown();

            ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeSec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeSec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeSec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWarningLongTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWarningShortTime)).BeginInit();

            this.SuspendLayout();

            // 布局常量
            int labelX = 30;         // 标题X坐标
            int inputMinX = 140;     // 分钟输入框X坐标
            int labelMinX = 215;     // "分"字X坐标
            int inputSecX = 250;     // 秒输入框X坐标
            int labelSecX = 325;     // "秒"字X坐标
            int inputWarningX = 140; // 提示时间输入框X坐标
            int labelWarningX = 215; // 提示时间单位X坐标

            int row1Y = 30;          // 第一行Y
            int row2Y = 70;          // 第二行Y
            int row3Y = 110;         // 第三行Y
            int row4Y = 150;         // 第四行Y
            int row5Y = 190;         // 第五行Y

            // 文本对齐偏移量：Label通常比InputBox位置要靠下一点点才能视觉居中
            int textOffsetY = 4;

            // --- 第一行：工作时间 ---

            // 标题
            this.lblWorkTime.AutoSize = true;
            this.lblWorkTime.Location = new System.Drawing.Point(labelX, row1Y + textOffsetY);
            this.lblWorkTime.Name = "lblWorkTime";
            this.lblWorkTime.Size = new System.Drawing.Size(100, 15);
            this.lblWorkTime.Text = "工作时间:"; // 设计时默认显示

            // 分钟输入
            this.nudWorkTimeMin.Location = new System.Drawing.Point(inputMinX, row1Y);
            this.nudWorkTimeMin.Maximum = new decimal([999, 0, 0, 0]);
            this.nudWorkTimeMin.Name = "nudWorkTimeMin";
            this.nudWorkTimeMin.Size = new System.Drawing.Size(70, 25);

            // 分钟单位
            this.lblWorkMinUnit.AutoSize = true;
            this.lblWorkMinUnit.Location = new System.Drawing.Point(labelMinX, row1Y + textOffsetY);
            this.lblWorkMinUnit.Name = "lblWorkMinUnit";
            this.lblWorkMinUnit.Size = new System.Drawing.Size(22, 15);
            this.lblWorkMinUnit.Text = "分";

            // 秒输入
            this.nudWorkTimeSec.Location = new System.Drawing.Point(inputSecX, row1Y);
            this.nudWorkTimeSec.Maximum = new decimal([59, 0, 0, 0]);
            this.nudWorkTimeSec.Name = "nudWorkTimeSec";
            this.nudWorkTimeSec.Size = new System.Drawing.Size(70, 25);

            // 秒单位
            this.lblWorkSecUnit.AutoSize = true;
            this.lblWorkSecUnit.Location = new System.Drawing.Point(labelSecX, row1Y + textOffsetY);
            this.lblWorkSecUnit.Name = "lblWorkSecUnit";
            this.lblWorkSecUnit.Size = new System.Drawing.Size(22, 15);
            this.lblWorkSecUnit.Text = "秒";

            // --- 第二行：短休息 ---

            this.lblShortBreakTime.AutoSize = true;
            this.lblShortBreakTime.Location = new System.Drawing.Point(labelX, row2Y + textOffsetY);
            this.lblShortBreakTime.Name = "lblShortBreakTime";
            this.lblShortBreakTime.Text = "短休息时间:";

            this.nudShortBreakTimeMin.Location = new System.Drawing.Point(inputMinX, row2Y);
            this.nudShortBreakTimeMin.Maximum = new decimal([999, 0, 0, 0]);
            this.nudShortBreakTimeMin.Name = "nudShortBreakTimeMin";
            this.nudShortBreakTimeMin.Size = new System.Drawing.Size(70, 25);

            this.lblShortBreakMinUnit.AutoSize = true;
            this.lblShortBreakMinUnit.Location = new System.Drawing.Point(labelMinX, row2Y + textOffsetY);
            this.lblShortBreakMinUnit.Name = "lblShortBreakMinUnit";
            this.lblShortBreakMinUnit.Text = "分";

            this.nudShortBreakTimeSec.Location = new System.Drawing.Point(inputSecX, row2Y);
            this.nudShortBreakTimeSec.Maximum = new decimal([59, 0, 0, 0]);
            this.nudShortBreakTimeSec.Name = "nudShortBreakTimeSec";
            this.nudShortBreakTimeSec.Size = new System.Drawing.Size(70, 25);

            this.lblShortBreakSecUnit.AutoSize = true;
            this.lblShortBreakSecUnit.Location = new System.Drawing.Point(labelSecX, row2Y + textOffsetY);
            this.lblShortBreakSecUnit.Name = "lblShortBreakSecUnit";
            this.lblShortBreakSecUnit.Text = "秒";

            // --- 第三行：长休息 ---

            this.lblLongBreakTime.AutoSize = true;
            this.lblLongBreakTime.Location = new System.Drawing.Point(labelX, row3Y + textOffsetY);
            this.lblLongBreakTime.Name = "lblLongBreakTime";
            this.lblLongBreakTime.Text = "长休息时间:";

            this.nudLongBreakTimeMin.Location = new System.Drawing.Point(inputMinX, row3Y);
            this.nudLongBreakTimeMin.Maximum = new decimal([999, 0, 0, 0]);
            this.nudLongBreakTimeMin.Name = "nudLongBreakTimeMin";
            this.nudLongBreakTimeMin.Size = new System.Drawing.Size(70, 25);

            this.lblLongBreakMinUnit.AutoSize = true;
            this.lblLongBreakMinUnit.Location = new System.Drawing.Point(labelMinX, row3Y + textOffsetY);
            this.lblLongBreakMinUnit.Name = "lblLongBreakMinUnit";
            this.lblLongBreakMinUnit.Text = "分";

            this.nudLongBreakTimeSec.Location = new System.Drawing.Point(inputSecX, row3Y);
            this.nudLongBreakTimeSec.Maximum = new decimal([59, 0, 0, 0]);
            this.nudLongBreakTimeSec.Name = "nudLongBreakTimeSec";
            this.nudLongBreakTimeSec.Size = new System.Drawing.Size(70, 25);

            this.lblLongBreakSecUnit.AutoSize = true;
            this.lblLongBreakSecUnit.Location = new System.Drawing.Point(labelSecX, row3Y + textOffsetY);
            this.lblLongBreakSecUnit.Name = "lblLongBreakSecUnit";
            this.lblLongBreakSecUnit.Text = "秒";

            // --- 第四行：提前长时间提示 ---
            this.lblWarningLongTime.AutoSize = true;
            this.lblWarningLongTime.Location = new System.Drawing.Point(labelX, row4Y + textOffsetY);
            this.lblWarningLongTime.Name = "lblWarningLongTime";
            this.lblWarningLongTime.Text = "提前长时间提示:";

            this.nudWarningLongTime.Location = new System.Drawing.Point(inputWarningX, row4Y);
            this.nudWarningLongTime.Maximum = new decimal([300, 0, 0, 0]);
            this.nudWarningLongTime.Name = "nudWarningLongTime";
            this.nudWarningLongTime.Size = new System.Drawing.Size(70, 25);

            this.lblWarningLongTimeUnit.AutoSize = true;
            this.lblWarningLongTimeUnit.Location = new System.Drawing.Point(labelWarningX, row4Y + textOffsetY);
            this.lblWarningLongTimeUnit.Name = "lblWarningLongTimeUnit";
            this.lblWarningLongTimeUnit.Text = "秒";

            // --- 第五行：提前短时间提示 ---
            this.lblWarningShortTime.AutoSize = true;
            this.lblWarningShortTime.Location = new System.Drawing.Point(labelX, row5Y + textOffsetY);
            this.lblWarningShortTime.Name = "lblWarningShortTime";
            this.lblWarningShortTime.Text = "提前短时间提示:";

            this.nudWarningShortTime.Location = new System.Drawing.Point(inputWarningX, row5Y);
            this.nudWarningShortTime.Maximum = new decimal([60, 0, 0, 0]);
            this.nudWarningShortTime.Name = "nudWarningShortTime";
            this.nudWarningShortTime.Size = new System.Drawing.Size(70, 25);

            this.lblWarningShortTimeUnit.AutoSize = true;
            this.lblWarningShortTimeUnit.Location = new System.Drawing.Point(labelWarningX, row5Y + textOffsetY);
            this.lblWarningShortTimeUnit.Name = "lblWarningShortTimeUnit";
            this.lblWarningShortTimeUnit.Text = "秒";

            // --- 按钮 (继承自 BaseForm) ---
            this.btnConfirm.Location = new System.Drawing.Point(147, 230);
            this.btnConfirm.Text = "保存";

            this.btnCancel.Location = new System.Drawing.Point(273, 230);

            // --- Form 设置 ---
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 290); // 调整了窗口大小以适应新的控件
            this.Name = "PomodoroSettingsForm";
            this.Text = "番茄时钟设置";

            // 添加控件
            this.Controls.Add(this.lblWorkTime);
            this.Controls.Add(this.nudWorkTimeMin);
            this.Controls.Add(this.lblWorkMinUnit);
            this.Controls.Add(this.nudWorkTimeSec);
            this.Controls.Add(this.lblWorkSecUnit);

            this.Controls.Add(this.lblShortBreakTime);
            this.Controls.Add(this.nudShortBreakTimeMin);
            this.Controls.Add(this.lblShortBreakMinUnit);
            this.Controls.Add(this.nudShortBreakTimeSec);
            this.Controls.Add(this.lblShortBreakSecUnit);

            this.Controls.Add(this.lblLongBreakTime);
            this.Controls.Add(this.nudLongBreakTimeMin);
            this.Controls.Add(this.lblLongBreakMinUnit);
            this.Controls.Add(this.nudLongBreakTimeSec);
            this.Controls.Add(this.lblLongBreakSecUnit);

            this.Controls.Add(this.lblWarningLongTime);
            this.Controls.Add(this.nudWarningLongTime);
            this.Controls.Add(this.lblWarningLongTimeUnit);

            this.Controls.Add(this.lblWarningShortTime);
            this.Controls.Add(this.nudWarningShortTime);
            this.Controls.Add(this.lblWarningShortTimeUnit);

            // 保持按钮在最上层
            this.Controls.SetChildIndex(this.btnConfirm, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);

            ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWorkTimeSec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudShortBreakTimeSec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLongBreakTimeSec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWarningLongTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWarningShortTime)).EndInit();

            this.ResumeLayout(false);
            this.PerformLayout();
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

            nudWarningLongTime.Value = Math.Max(nudWarningLongTime.Minimum, Math.Min(nudWarningLongTime.Maximum, WarningLongTime));
            nudWarningShortTime.Value = Math.Max(nudWarningShortTime.Minimum, Math.Min(nudWarningShortTime.Maximum, WarningShortTime));

            // 2. 本地化文本 - 所有 Label 都需要更新
            this.Text = LanguageManager.GetString("PomodoroSettings") ?? "番茄时钟设置";

            // 行标题
            lblWorkTime.Text = LanguageManager.GetString("WorkTime") ?? "工作时间:";
            lblShortBreakTime.Text = LanguageManager.GetString("ShortBreakTime") ?? "短休息时间:";
            lblLongBreakTime.Text = LanguageManager.GetString("LongBreakTime") ?? "长休息时间:";
            lblWarningLongTime.Text = LanguageManager.GetString("PomodoroWarningLongTime") ?? "提前长时间提示:";
            lblWarningShortTime.Text = LanguageManager.GetString("PomodoroWarningShortTime") ?? "提前短时间提示:";

            // 单位 (分)
            string strMin = LanguageManager.GetString("Minutes") ?? "分";
            lblWorkMinUnit.Text = strMin;
            lblShortBreakMinUnit.Text = strMin;
            lblLongBreakMinUnit.Text = strMin;

            // 单位 (秒)
            string strSec = LanguageManager.GetString("Seconds") ?? "秒";
            lblWorkSecUnit.Text = strSec;
            lblShortBreakSecUnit.Text = strSec;
            lblLongBreakSecUnit.Text = strSec;
            lblWarningLongTimeUnit.Text = strSec;
            lblWarningShortTimeUnit.Text = strSec;

            // 按钮
            if (btnConfirm != null) btnConfirm.Text = LanguageManager.GetString("Save") ?? "保存";
            if (btnCancel != null) btnCancel.Text = LanguageManager.GetString("Cancel") ?? "取消";
        }

        protected override void BtnConfirm_Click(object? sender, EventArgs e) {
            WorkTimeMinutes = (int)nudWorkTimeMin.Value;
            WorkTimeSeconds = (int)nudWorkTimeSec.Value;
            ShortBreakMinutes = (int)nudShortBreakTimeMin.Value;
            ShortBreakSeconds = (int)nudShortBreakTimeSec.Value;
            LongBreakMinutes = (int)nudLongBreakTimeMin.Value;
            LongBreakSeconds = (int)nudLongBreakTimeSec.Value;
            WarningLongTime = (int)nudWarningLongTime.Value;
            WarningShortTime = (int)nudWarningShortTime.Value;

            // 如果有注入的_appSettings，直接更新设置
            _appSettings.WorkTimeMinutes = WorkTimeMinutes;
            _appSettings.WorkTimeSeconds = WorkTimeSeconds;
            _appSettings.ShortBreakMinutes = ShortBreakMinutes;
            _appSettings.ShortBreakSeconds = ShortBreakSeconds;
            _appSettings.LongBreakMinutes = LongBreakMinutes;
            _appSettings.LongBreakSeconds = LongBreakSeconds;
            _appSettings.PomodoroWarningLongTime = WarningLongTime;
            _appSettings.PomodoroWarningShortTime = WarningShortTime;

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