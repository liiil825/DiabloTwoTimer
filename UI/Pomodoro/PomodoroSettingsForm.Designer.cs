#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Pomodoro;
partial class PomodoroSettingsForm
{
    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
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
        int labelX = 30; // 标题X坐标
        int inputMinX = 140; // 分钟输入框X坐标
        int labelMinX = 215; // "分"字X坐标
        int inputSecX = 250; // 秒输入框X坐标
        int labelSecX = 325; // "秒"字X坐标
        int inputWarningX = 140; // 提示时间输入框X坐标
        int labelWarningX = 215; // 提示时间单位X坐标

        int row1Y = 30; // 第一行Y
        int row2Y = 70; // 第二行Y
        int row3Y = 110; // 第三行Y
        int row4Y = 150; // 第四行Y
        int row5Y = 190; // 第五行Y

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

    #endregion

    // 主要标签
    private Label lblWorkTime;
    private Label lblShortBreakTime;
    private Label lblLongBreakTime;

    // 单位标签 (分)
    private Label lblWorkMinUnit;
    private Label lblShortBreakMinUnit;
    private Label lblLongBreakMinUnit;

    // 输入框
    private NumericUpDown nudWorkTimeMin;
    private NumericUpDown nudWorkTimeSec;
    private NumericUpDown nudShortBreakTimeMin;
    private NumericUpDown nudShortBreakTimeSec;
    private NumericUpDown nudLongBreakTimeMin;
    private NumericUpDown nudLongBreakTimeSec;

    // 单位标签 (秒)
    private Label lblWorkSecUnit;
    private Label lblShortBreakSecUnit;
    private Label lblLongBreakSecUnit;

    // 控件声明
    private Label lblWarningLongTime;
    private Label lblWarningShortTime;
    private Label lblWarningLongTimeUnit;
    private Label lblWarningShortTimeUnit;
    private NumericUpDown nudWarningLongTime;
    private NumericUpDown nudWarningShortTime;
}