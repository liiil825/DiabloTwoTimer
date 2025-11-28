#nullable disable

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.UI.Pomodoro;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Timer
{
    partial class TimerControl
    {
        // 组件引用
        private StatisticsControl statisticsControl;
        private HistoryControl historyControl;
        private CharacterSceneControl characterSceneControl;
        private LootRecordsControl lootRecordsControl;
        private AntdUI.LabelTime labelTime1;

        // 控件字段定义
        private Label btnStatusIndicator;
        private Button toggleLootButton;
        private PomodoroTimeDisplayLabel pomodoroTime;
        private Label lblTimeDisplay;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(TimerControl));
            btnStatusIndicator = new Label();
            lblTimeDisplay = new Label();
            statisticsControl = new StatisticsControl();
            historyControl = new HistoryControl();
            characterSceneControl = new CharacterSceneControl();
            lootRecordsControl = new LootRecordsControl();
            labelTime1 = new AntdUI.LabelTime();
            toggleLootButton = new Button();
            pomodoroTime = new PomodoroTimeDisplayLabel();
            SuspendLayout();
            //
            // btnStatusIndicator
            //
            btnStatusIndicator.Location = new Point(15, 19);
            btnStatusIndicator.Margin = new Padding(6);
            btnStatusIndicator.Name = "btnStatusIndicator";
            btnStatusIndicator.Size = new Size(24, 24);
            btnStatusIndicator.TabIndex = 0;
            //
            // lblTimeDisplay
            //
            lblTimeDisplay.AutoSize = true;
            lblTimeDisplay.Font = new Font("Microsoft YaHei UI", 28F, FontStyle.Regular, GraphicsUnit.Point, 134);
            lblTimeDisplay.Location = new Point(20, 61);
            lblTimeDisplay.Margin = new Padding(6, 0, 6, 0);
            lblTimeDisplay.Name = "lblTimeDisplay";
            lblTimeDisplay.Size = new Size(303, 86);
            lblTimeDisplay.TabIndex = 1;
            lblTimeDisplay.Text = "00:00:00";
            lblTimeDisplay.TextAlign = ContentAlignment.MiddleLeft;
            //
            // statisticsControl
            //
            statisticsControl.AverageTime = TimeSpan.Parse("00:00:00");
            statisticsControl.FastestTime = TimeSpan.Parse("00:00:00");
            statisticsControl.Location = new Point(9, 157);
            statisticsControl.Margin = new Padding(9, 8, 9, 8);
            statisticsControl.Name = "statisticsControl";
            statisticsControl.RunCount = 0;
            statisticsControl.Size = new Size(421, 116);
            statisticsControl.TabIndex = 2;
            //
            // historyControl
            //
            historyControl.Location = new Point(9, 289);
            historyControl.Margin = new Padding(9, 8, 9, 8);
            historyControl.Name = "historyControl";
            historyControl.Size = new Size(421, 117);
            historyControl.TabIndex = 3;
            //
            // characterSceneControl
            //
            characterSceneControl.Location = new Point(9, 409);
            characterSceneControl.Margin = new Padding(6);
            characterSceneControl.Name = "characterSceneControl";
            characterSceneControl.Size = new Size(421, 80);
            characterSceneControl.TabIndex = 4;
            //
            // lootRecordsControl
            //
            lootRecordsControl.Location = new Point(9, 495);
            lootRecordsControl.Margin = new Padding(9, 8, 9, 8);
            lootRecordsControl.Name = "lootRecordsControl";
            lootRecordsControl.Size = new Size(421, 100);
            lootRecordsControl.TabIndex = 6;
            //
            // labelTime1
            //
            labelTime1.Location = new Point(60, 9);
            labelTime1.Name = "labelTime1";
            labelTime1.ShowTime = false;
            labelTime1.Size = new Size(135, 40);
            labelTime1.TabIndex = 5;
            labelTime1.Text = "labelTime1";
            //
            // toggleLootButton
            //
            toggleLootButton.Location = new Point(299, 414);
            toggleLootButton.Name = "toggleLootButton";
            toggleLootButton.Size = new Size(131, 40);
            toggleLootButton.TabIndex = 7;
            toggleLootButton.Text = "ShowLoot";
            toggleLootButton.UseVisualStyleBackColor = true;
            toggleLootButton.Click += ToggleLootButton_Click;
            //
            // pomodoroTime
            //
            pomodoroTime.AutoSize = true;
            pomodoroTime.Font = new Font("微软雅黑", 16F, FontStyle.Bold);
            pomodoroTime.Location = new Point(299, 9);
            pomodoroTime.Name = "pomodoroTime";
            pomodoroTime.ShowMilliseconds = false;
            pomodoroTime.Size = new Size(125, 50);
            pomodoroTime.TabIndex = 8;
            pomodoroTime.Text = "00:00";
            //
            // TimerControl
            //
            AutoScaleDimensions = new SizeF(13F, 28F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pomodoroTime);
            Controls.Add(toggleLootButton);
            Controls.Add(lootRecordsControl);
            Controls.Add(labelTime1);
            Controls.Add(characterSceneControl);
            Controls.Add(historyControl);
            Controls.Add(statisticsControl);
            Controls.Add(lblTimeDisplay);
            Controls.Add(btnStatusIndicator);
            Margin = new Padding(6);
            Name = "TimerControl";
            Size = new Size(667, 850);
            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnsubscribeFromServiceEvents();
                LanguageManager.OnLanguageChanged -= LanguageManager_OnLanguageChanged;
            }
            base.Dispose(disposing);
        }
    }
}