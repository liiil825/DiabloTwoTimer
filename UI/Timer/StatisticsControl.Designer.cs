#nullable disable

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DiabloTwoMFTimer.UI.Timer
{
    partial class StatisticsControl
    {

        private Label lblRunCount;
        private Label lblFastestTime;
        private Label lblAverageTime;

        private void InitializeComponent()
        {
            lblRunCount = new Label();
            lblFastestTime = new Label();
            lblAverageTime = new Label();
            SuspendLayout();
            //
            // lblRunCount
            //
            lblRunCount.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            lblRunCount.Location = new Point(0, 0);
            lblRunCount.Margin = new Padding(4, 0, 4, 0);
            lblRunCount.Name = "lblRunCount";
            lblRunCount.Size = new Size(419, 35);
            lblRunCount.TabIndex = 0;
            lblRunCount.Text = "--- Run count 0 ---";
            lblRunCount.TextAlign = ContentAlignment.MiddleLeft;
            //
            // lblFastestTime
            //
            lblFastestTime.AutoSize = true;
            lblFastestTime.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 134);
            lblFastestTime.Location = new Point(0, 42);
            lblFastestTime.Margin = new Padding(4, 0, 4, 0);
            lblFastestTime.Name = "lblFastestTime";
            lblFastestTime.Size = new Size(253, 31);
            lblFastestTime.TabIndex = 1;
            lblFastestTime.Text = "Fastest time: --:--:--.-";
            //
            // lblAverageTime
            //
            lblAverageTime.AutoSize = true;
            lblAverageTime.Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point, 134);
            lblAverageTime.Location = new Point(0, 77);
            lblAverageTime.Margin = new Padding(4, 0, 4, 0);
            lblAverageTime.Name = "lblAverageTime";
            lblAverageTime.Size = new Size(268, 31);
            lblAverageTime.TabIndex = 2;
            lblAverageTime.Text = "Average time: --:--:--.-";
            //
            // StatisticsControl
            //
            AutoScaleDimensions = new SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(lblAverageTime);
            Controls.Add(lblFastestTime);
            Controls.Add(lblRunCount);
            Margin = new Padding(4, 4, 4, 4);
            Name = "StatisticsControl";
            Size = new Size(419, 119);
            ResumeLayout(false);
            PerformLayout();
        }


    }
}