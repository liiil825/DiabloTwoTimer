using System;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Input;
using DTwoMFTimerHelper.Resources;

namespace DTwoMFTimerHelper
{
    public partial class SettingsControl : UserControl
    {
        // 窗口位置枚举
        public enum WindowPosition
        {
            TopLeft,
            TopCenter,
            TopRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }

        // 事件
        public event EventHandler<WindowPositionChangedEventArgs>? WindowPositionChanged;
        public event EventHandler<LanguageChangedEventArgs>? LanguageChanged;
        public event EventHandler<AlwaysOnTopChangedEventArgs>? AlwaysOnTopChanged;
        public event EventHandler<HotkeyChangedEventArgs>? StartStopHotkeyChanged;
        public event EventHandler<HotkeyChangedEventArgs>? PauseHotkeyChanged;

        // 语言枚举
        public enum LanguageOption
        {
            Chinese,
            English
        }

        public SettingsControl()
        {
            InitializeComponent();
            UpdateUI();
            // 初始化快捷键显示
            UpdateHotkeyLabels();
        }

        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.btnConfirmSettings = new System.Windows.Forms.Button();
            this.radioTopLeft = new System.Windows.Forms.RadioButton();
            this.radioTopRight = new System.Windows.Forms.RadioButton();
            this.radioTopCenter = new System.Windows.Forms.RadioButton();
            this.radioBottomLeft = new System.Windows.Forms.RadioButton();
            this.radioBottomCenter = new System.Windows.Forms.RadioButton();
            this.radioBottomRight = new System.Windows.Forms.RadioButton();
            this.groupBoxPosition = new System.Windows.Forms.GroupBox();
            this.chineseRadioButton = new System.Windows.Forms.RadioButton();
            this.englishRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBoxLanguage = new System.Windows.Forms.GroupBox();
            this.alwaysOnTopCheckBox = new System.Windows.Forms.CheckBox();
            this.alwaysOnTopLabel = new System.Windows.Forms.Label();
            this.tabPageHotkeys = new System.Windows.Forms.TabPage();
            this.btnSetPauseHotkey = new System.Windows.Forms.Button();
            this.btnSetStartStopHotkey = new System.Windows.Forms.Button();
            this.labelPauseHotkey = new System.Windows.Forms.Label();
            this.labelStartStopHotkey = new System.Windows.Forms.Label();
            this.labelHotkeyPause = new System.Windows.Forms.Label();
            this.labelHotkeyStartStop = new System.Windows.Forms.Label();
            this.groupBoxHotkeys = new System.Windows.Forms.GroupBox();
            this.tabControl.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.groupBoxPosition.SuspendLayout();
            this.groupBoxLanguage.SuspendLayout();
            this.tabPageHotkeys.SuspendLayout();
            this.groupBoxHotkeys.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageGeneral);
            this.tabControl.Controls.Add(this.tabPageHotkeys);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(335, 310);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.btnConfirmSettings);
            this.tabPageGeneral.Controls.Add(this.alwaysOnTopCheckBox);
            this.tabPageGeneral.Controls.Add(this.alwaysOnTopLabel);
            this.tabPageGeneral.Controls.Add(this.groupBoxPosition);
            this.tabPageGeneral.Controls.Add(this.groupBoxLanguage);
            // 
            // tabPageHotkeys
            // 
            this.tabPageHotkeys.Controls.Add(this.btnConfirmSettings);
            this.tabPageHotkeys.Controls.Add(this.groupBoxHotkeys);
            this.tabPageHotkeys.Location = new System.Drawing.Point(4, 24);
            this.tabPageHotkeys.Name = "tabPageHotkeys";
            this.tabPageHotkeys.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHotkeys.Size = new System.Drawing.Size(333, 282);
            this.tabPageHotkeys.TabIndex = 1;
            this.tabPageHotkeys.Text = "快捷键";
            this.tabPageHotkeys.UseVisualStyleBackColor = true;
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 24);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(327, 282);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "通用";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // btnConfirmSettings
            // 
            this.btnConfirmSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirmSettings.Location = new System.Drawing.Point(242, 244);
            this.btnConfirmSettings.Name = "btnConfirmSettings";
            this.btnConfirmSettings.Size = new System.Drawing.Size(75, 23);
            this.btnConfirmSettings.TabIndex = 5;
            this.btnConfirmSettings.Text = "确认";
            this.btnConfirmSettings.UseVisualStyleBackColor = true;
            this.btnConfirmSettings.Click += new System.EventHandler(this.btnConfirmSettings_Click);
            // 
            // radioTopLeft
            // 
            this.radioTopLeft.AutoSize = true;
            this.radioTopLeft.Checked = true;
            this.radioTopLeft.Location = new System.Drawing.Point(8, 25);
            this.radioTopLeft.Name = "radioTopLeft";
            this.radioTopLeft.Size = new System.Drawing.Size(47, 19);
            this.radioTopLeft.TabIndex = 1;
            this.radioTopLeft.TabStop = true;
            this.radioTopLeft.Text = "左上";
            this.radioTopLeft.UseVisualStyleBackColor = true;
            // 
            // radioTopRight
            // 
            this.radioTopRight.AutoSize = true;
            this.radioTopRight.Location = new System.Drawing.Point(150, 25);
            this.radioTopRight.Name = "radioTopRight";
            this.radioTopRight.Size = new System.Drawing.Size(47, 19);
            this.radioTopRight.TabIndex = 2;
            this.radioTopRight.TabStop = true;
            this.radioTopRight.Text = "右上";
            this.radioTopRight.UseVisualStyleBackColor = true;
            // 
            // radioTopCenter
            // 
            this.radioTopCenter.AutoSize = true;
            this.radioTopCenter.Location = new System.Drawing.Point(8, 60);
            this.radioTopCenter.Name = "radioTopCenter";
            this.radioTopCenter.Size = new System.Drawing.Size(47, 19);
            this.radioTopCenter.TabIndex = 3;
            this.radioTopCenter.TabStop = true;
            this.radioTopCenter.Text = "上中";
            this.radioTopCenter.UseVisualStyleBackColor = true;
            // 
            // radioBottomCenter
            // 
            this.radioBottomCenter.AutoSize = true;
            this.radioBottomCenter.Location = new System.Drawing.Point(150, 60);
            this.radioBottomCenter.Name = "radioBottomCenter";
            this.radioBottomCenter.Size = new System.Drawing.Size(47, 19);
            this.radioBottomCenter.TabIndex = 4;
            this.radioBottomCenter.TabStop = true;
            this.radioBottomCenter.Text = "下中";
            this.radioBottomCenter.UseVisualStyleBackColor = true;
            // 
            // radioBottomLeft
            // 
            this.radioBottomLeft.AutoSize = true;
            this.radioBottomLeft.Location = new System.Drawing.Point(8, 95);
            this.radioBottomLeft.Name = "radioBottomLeft";
            this.radioBottomLeft.Size = new System.Drawing.Size(47, 19);
            this.radioBottomLeft.TabIndex = 5;
            this.radioBottomLeft.TabStop = true;
            this.radioBottomLeft.Text = "左下";
            this.radioBottomLeft.UseVisualStyleBackColor = true;
            // 
            // radioBottomRight
            // 
            this.radioBottomRight.AutoSize = true;
            this.radioBottomRight.Location = new System.Drawing.Point(150, 95);
            this.radioBottomRight.Name = "radioBottomRight";
            this.radioBottomRight.Size = new System.Drawing.Size(47, 19);
            this.radioBottomRight.TabIndex = 6;
            this.radioBottomRight.TabStop = true;
            this.radioBottomRight.Text = "右下";
            this.radioBottomRight.UseVisualStyleBackColor = true;
            // 
            // groupBoxPosition
            // 
            this.groupBoxPosition.Controls.Add(this.radioTopLeft);
            this.groupBoxPosition.Controls.Add(this.radioTopRight);
            this.groupBoxPosition.Controls.Add(this.radioTopCenter);
            this.groupBoxPosition.Controls.Add(this.radioBottomCenter);
            this.groupBoxPosition.Controls.Add(this.radioBottomLeft);
            this.groupBoxPosition.Controls.Add(this.radioBottomRight);
            this.groupBoxPosition.Location = new System.Drawing.Point(8, 8);
            this.groupBoxPosition.Name = "groupBoxPosition";
            this.groupBoxPosition.Size = new System.Drawing.Size(317, 110);
            this.groupBoxPosition.TabIndex = 0;
            this.groupBoxPosition.TabStop = false;
            this.groupBoxPosition.Text = "窗口位置";
            // 
            // chineseRadioButton
            // 
            this.chineseRadioButton.AutoSize = true;
            this.chineseRadioButton.Checked = true;
            this.chineseRadioButton.Location = new System.Drawing.Point(8, 30);
            this.chineseRadioButton.Name = "chineseRadioButton";
            this.chineseRadioButton.Size = new System.Drawing.Size(59, 19);
            this.chineseRadioButton.TabIndex = 8;
            this.chineseRadioButton.TabStop = true;
            this.chineseRadioButton.Text = "Chinese";
            this.chineseRadioButton.UseVisualStyleBackColor = true;
            // 
            // englishRadioButton
            // 
            this.englishRadioButton.AutoSize = true;
            this.englishRadioButton.Location = new System.Drawing.Point(100, 30);
            this.englishRadioButton.Name = "englishRadioButton";
            this.englishRadioButton.Size = new System.Drawing.Size(64, 19);
            this.englishRadioButton.TabIndex = 9;
            this.englishRadioButton.TabStop = true;
            this.englishRadioButton.Text = "English";
            this.englishRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBoxLanguage
            // 
            this.groupBoxLanguage.Controls.Add(this.chineseRadioButton);
            this.groupBoxLanguage.Controls.Add(this.englishRadioButton);
            this.groupBoxLanguage.Location = new System.Drawing.Point(8, 124);
            this.groupBoxLanguage.Name = "groupBoxLanguage";
            this.groupBoxLanguage.Size = new System.Drawing.Size(317, 70);
            this.groupBoxLanguage.TabIndex = 1;
            this.groupBoxLanguage.TabStop = false;
            this.groupBoxLanguage.Text = "语言";
            // 
            // alwaysOnTopCheckBox
            // 
            this.alwaysOnTopCheckBox.AutoSize = true;
            this.alwaysOnTopCheckBox.Checked = true;
            this.alwaysOnTopCheckBox.Location = new System.Drawing.Point(75, 202);
            this.alwaysOnTopCheckBox.Name = "alwaysOnTopCheckBox";
            this.alwaysOnTopCheckBox.Size = new System.Drawing.Size(15, 14);
            this.alwaysOnTopCheckBox.TabIndex = 3;
            this.alwaysOnTopCheckBox.UseVisualStyleBackColor = true;
            // 
            // alwaysOnTopLabel
            // 
            this.alwaysOnTopLabel.AutoSize = true;
            this.alwaysOnTopLabel.Location = new System.Drawing.Point(8, 202);
            this.alwaysOnTopLabel.Name = "alwaysOnTopLabel";
            this.alwaysOnTopLabel.Size = new System.Drawing.Size(70, 15);
            this.alwaysOnTopLabel.TabIndex = 4;
            this.alwaysOnTopLabel.Text = "总在最前";
            // 
            // tabPageHotkeys
            // 
            // 已移除重复的tabPageHotkeys定义
            // 
            // 已移除设置按钮，改为点击标签直接设置快捷键
            // 
            // labelPauseHotkey - 可点击设置快捷键
            // 
            this.labelPauseHotkey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelPauseHotkey.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelPauseHotkey.Location = new System.Drawing.Point(70, 70);
            this.labelPauseHotkey.Name = "labelPauseHotkey";
            this.labelPauseHotkey.Size = new System.Drawing.Size(237, 23);
            this.labelPauseHotkey.TabIndex = 3;
            this.labelPauseHotkey.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelPauseHotkey.Click += new System.EventHandler(this.btnSetPauseHotkey_Click);
            // 
            // labelStartStopHotkey - 可点击设置快捷键
            // 
            this.labelStartStopHotkey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelStartStopHotkey.Cursor = System.Windows.Forms.Cursors.Hand;
            this.labelStartStopHotkey.Location = new System.Drawing.Point(70, 30);
            this.labelStartStopHotkey.Name = "labelStartStopHotkey";
            this.labelStartStopHotkey.Size = new System.Drawing.Size(237, 23);
            this.labelStartStopHotkey.TabIndex = 2;
            this.labelStartStopHotkey.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelStartStopHotkey.Click += new System.EventHandler(this.btnSetStartStopHotkey_Click);
            // 
            // labelHotkeyPause
            // 
            this.labelHotkeyPause.AutoSize = true;
            this.labelHotkeyPause.Location = new System.Drawing.Point(8, 75);
            this.labelHotkeyPause.Name = "labelHotkeyPause";
            this.labelHotkeyPause.Size = new System.Drawing.Size(64, 15);
            this.labelHotkeyPause.TabIndex = 1;
            this.labelHotkeyPause.Text = "Pause"; // 将在UpdateUI中通过LanguageManager更新
            // 
            // labelHotkeyStartStop
            // 
            this.labelHotkeyStartStop.AutoSize = true;
            this.labelHotkeyStartStop.Location = new System.Drawing.Point(8, 35);
            this.labelHotkeyStartStop.Name = "labelHotkeyStartStop";
            this.labelHotkeyStartStop.Size = new System.Drawing.Size(64, 15);
            this.labelHotkeyStartStop.TabIndex = 0;
            this.labelHotkeyStartStop.Text = "Start/Stop"; // 将在UpdateUI中通过LanguageManager更新
            // 
            // groupBoxHotkeys
            // 
            this.groupBoxHotkeys.Controls.Add(this.labelPauseHotkey);
            this.groupBoxHotkeys.Controls.Add(this.labelStartStopHotkey);
            this.groupBoxHotkeys.Controls.Add(this.labelHotkeyPause);
            this.groupBoxHotkeys.Controls.Add(this.labelHotkeyStartStop);
            this.groupBoxHotkeys.Location = new System.Drawing.Point(8, 8);
            this.groupBoxHotkeys.Name = "groupBoxHotkeys";
            this.groupBoxHotkeys.Size = new System.Drawing.Size(317, 110);
            this.groupBoxHotkeys.TabIndex = 0;
            this.groupBoxHotkeys.TabStop = false;
            this.groupBoxHotkeys.Text = "Hotkey Settings"; // 将在UpdateUI中通过LanguageManager更新
            // 
            // SettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = false;
            this.Controls.Add(this.tabControl);
            this.Name = "SettingsControl";
            this.Size = new System.Drawing.Size(335, 310);
            this.groupBoxPosition.ResumeLayout(false);
            this.groupBoxPosition.PerformLayout();
            this.groupBoxLanguage.ResumeLayout(false);
            this.groupBoxLanguage.PerformLayout();
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.groupBoxHotkeys.ResumeLayout(false);
            this.groupBoxHotkeys.PerformLayout();
            this.tabPageHotkeys.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public void UpdateUI()
        {
            // 更新设置页面文本
            btnConfirmSettings!.Text = LanguageManager.GetString("ConfirmSettings");
            groupBoxPosition!.Text = LanguageManager.GetString("WindowPosition");
            radioTopLeft!.Text = LanguageManager.GetString("TopLeft");
            radioTopCenter!.Text = LanguageManager.GetString("TopCenter");
            radioTopRight!.Text = LanguageManager.GetString("TopRight");
            radioBottomLeft!.Text = LanguageManager.GetString("BottomLeft");
            radioBottomCenter!.Text = LanguageManager.GetString("BottomCenter");
            radioBottomRight!.Text = LanguageManager.GetString("BottomRight");

            // 更新语言选择文本
            groupBoxLanguage!.Text = LanguageManager.GetString("Language");
            chineseRadioButton!.Text = LanguageManager.GetString("Chinese");
            englishRadioButton!.Text = LanguageManager.GetString("English");

            // 更新窗口置顶文本
            alwaysOnTopLabel!.Text = LanguageManager.GetString("AlwaysOnTop");

            // 更新Tab页面文本
            tabPageGeneral!.Text = LanguageManager.GetString("General");
            tabPageHotkeys!.Text = LanguageManager.GetString("Hotkeys");

            // 更新快捷键设置文本
            groupBoxHotkeys!.Text = LanguageManager.GetString("HotkeySettings");
            labelHotkeyStartStop!.Text = LanguageManager.GetString("StartStop");
            labelHotkeyPause!.Text = LanguageManager.GetString("Pause");
            btnSetStartStopHotkey!.Text = LanguageManager.GetString("Set");
            btnSetPauseHotkey!.Text = LanguageManager.GetString("Set");

            // 更新快捷键显示
            UpdateHotkeyLabels();
        }

        public void ApplyWindowPosition(Form form)
        {
            WindowPosition position = GetSelectedPosition();
            MoveWindowToPosition(form, position);
        }

        private WindowPosition GetSelectedPosition()
        {
            if (radioTopLeft?.Checked ?? false) return WindowPosition.TopLeft;
            if (radioTopCenter?.Checked ?? false) return WindowPosition.TopCenter;
            if (radioTopRight?.Checked ?? false) return WindowPosition.TopRight;
            if (radioBottomLeft?.Checked ?? false) return WindowPosition.BottomLeft;
            if (radioBottomCenter?.Checked ?? false) return WindowPosition.BottomCenter;
            if (radioBottomRight?.Checked ?? false) return WindowPosition.BottomRight;
            return WindowPosition.TopLeft; // 默认左上
        }

        public void MoveWindowToPosition(Form form, WindowPosition position)
        {
            // 获取屏幕工作区域
            Rectangle screenBounds = Screen.GetWorkingArea(form);

            int x, y;

            switch (position)
            {
                case WindowPosition.TopLeft:
                    x = screenBounds.Left;
                    y = screenBounds.Top;
                    break;
                case WindowPosition.TopCenter:
                    x = screenBounds.Left + (screenBounds.Width - form.Width) / 2;
                    y = screenBounds.Top;
                    break;
                case WindowPosition.TopRight:
                    x = screenBounds.Right - form.Width;
                    y = screenBounds.Top;
                    break;
                case WindowPosition.BottomLeft:
                    x = screenBounds.Left;
                    y = screenBounds.Bottom - form.Height;
                    break;
                case WindowPosition.BottomCenter:
                    x = screenBounds.Left + (screenBounds.Width - form.Width) / 2;
                    y = screenBounds.Bottom - form.Height;
                    break;
                case WindowPosition.BottomRight:
                    x = screenBounds.Right - form.Width;
                    y = screenBounds.Bottom - form.Height;
                    break;
                default:
                    return;
            }

            // 设置窗口位置
            form.Location = new Point(x, y);
        }

        // 快捷键相关私有字段
        private Keys startStopHotkey = Keys.Q | Keys.Alt; // 默认 Alt + Q
        private Keys pauseHotkey = Keys.Space | Keys.Control; // 默认 Ctrl + 空格
        private bool isSettingHotkey = false;
        private string currentHotkeySetting = string.Empty;

        private void btnConfirmSettings_Click(object? sender, EventArgs e)
        {
            // 触发位置变更事件
            WindowPosition position = GetSelectedPosition();
            WindowPositionChanged?.Invoke(this, new WindowPositionChangedEventArgs(position));

            // 触发语言变更事件
            LanguageOption language = GetSelectedLanguage();
            LanguageChanged?.Invoke(this, new LanguageChangedEventArgs(language));

            // 触发窗口置顶变更事件
            bool isAlwaysOnTop = alwaysOnTopCheckBox?.Checked ?? false;
            AlwaysOnTopChanged?.Invoke(this, new AlwaysOnTopChangedEventArgs(isAlwaysOnTop));

            // 触发快捷键变更事件
            StartStopHotkeyChanged?.Invoke(this, new HotkeyChangedEventArgs(startStopHotkey));
            PauseHotkeyChanged?.Invoke(this, new HotkeyChangedEventArgs(pauseHotkey));
        }

        private void UpdateHotkeyLabels()
        {
            // 更新开始/结束快捷键显示
            labelStartStopHotkey!.Text = GetHotkeyDisplayText(startStopHotkey);

            // 更新暂停快捷键显示
            labelPauseHotkey!.Text = GetHotkeyDisplayText(pauseHotkey);
        }

        private string GetHotkeyDisplayText(Keys keys)
        {
            string text = string.Empty;

            if ((keys & Keys.Control) == Keys.Control)
                text += "Ctrl + ";
            if ((keys & Keys.Alt) == Keys.Alt)
                text += "Alt + ";
            if ((keys & Keys.Shift) == Keys.Shift)
                text += "Shift + ";

            // 获取剩余的键
            Keys key = keys & ~Keys.Control & ~Keys.Alt & ~Keys.Shift;
            if (key != Keys.None)
                text += key.ToString();

            return text;
        }

        private void btnSetStartStopHotkey_Click(object sender, EventArgs e)
        {
            StartHotkeySetup("StartStop");
        }

        private void btnSetPauseHotkey_Click(object sender, EventArgs e)
        {
            StartHotkeySetup("Pause");
        }

        private void StartHotkeySetup(string hotkeyType)
        {
            isSettingHotkey = true;
            currentHotkeySetting = hotkeyType;

            // 使用通用文本，避免中英文问题
            string pressKeyText = "Press Key...";
            if (hotkeyType == "StartStop")
                labelStartStopHotkey!.Text = pressKeyText;
            else
                labelPauseHotkey!.Text = pressKeyText;

            // 为当前控件添加键盘事件处理
            this.Focus();
            this.KeyDown += new KeyEventHandler(OnKeyDownWhileSettingHotkey);
        }

        private void OnKeyDownWhileSettingHotkey(object sender, KeyEventArgs e)
        {
            if (!isSettingHotkey) return;

            // 忽略特殊键单独按下的情况
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu || e.KeyCode == Keys.ShiftKey)
                return;

            // 构建快捷键组合
            Keys newHotkey = e.KeyCode;
            if (e.Control)
                newHotkey |= Keys.Control;
            if (e.Alt)
                newHotkey |= Keys.Alt;
            if (e.Shift)
                newHotkey |= Keys.Shift;

            // 保存快捷键
            if (currentHotkeySetting == "StartStop")
                startStopHotkey = newHotkey;
            else
                pauseHotkey = newHotkey;

            // 重置状态
            isSettingHotkey = false;
            currentHotkeySetting = string.Empty;
            this.KeyDown -= new KeyEventHandler(OnKeyDownWhileSettingHotkey);

            // 更新显示
            UpdateHotkeyLabels();

            // 阻止事件继续传播
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private LanguageOption GetSelectedLanguage()
        {
            if (chineseRadioButton?.Checked ?? false) return LanguageOption.Chinese;
            if (englishRadioButton?.Checked ?? false) return LanguageOption.English;
            return LanguageOption.Chinese; // 默认中文
        }

        // 位置变更事件参数
        public class WindowPositionChangedEventArgs : EventArgs
        {
            public WindowPosition Position { get; }

            public WindowPositionChangedEventArgs(WindowPosition position)
            {
                Position = position;
            }
        }

        // 语言变更事件参数
        public class LanguageChangedEventArgs : EventArgs
        {
            public LanguageOption Language { get; }

            public LanguageChangedEventArgs(LanguageOption language)
            {
                Language = language;
            }
        }

        // 窗口置顶变更事件参数
        public class AlwaysOnTopChangedEventArgs : EventArgs
        {
            public bool IsAlwaysOnTop { get; }

            public AlwaysOnTopChangedEventArgs(bool isAlwaysOnTop)
            {
                IsAlwaysOnTop = isAlwaysOnTop;
            }
        }

        // 快捷键变更事件参数
        public class HotkeyChangedEventArgs : EventArgs
        {
            public Keys Hotkey { get; }

            public HotkeyChangedEventArgs(Keys hotkey)
            {
                Hotkey = hotkey;
            }
        }

        // 控件字段
        private TabControl? tabControl;
        private TabPage? tabPageGeneral;
        private TabPage? tabPageHotkeys;
        private Button? btnConfirmSettings;
        private RadioButton? radioBottomLeft;
        private RadioButton? radioBottomCenter;
        private RadioButton? radioBottomRight;
        private RadioButton? radioTopRight;
        private RadioButton? radioTopCenter;
        private RadioButton? radioTopLeft;
        private GroupBox? groupBoxPosition;
        private RadioButton? chineseRadioButton;
        private RadioButton? englishRadioButton;
        private GroupBox? groupBoxLanguage;
        private CheckBox? alwaysOnTopCheckBox;
        private Label? alwaysOnTopLabel;
        private GroupBox? groupBoxHotkeys;
        private Label? labelHotkeyStartStop;
        private Label? labelHotkeyPause;
        private Label? labelStartStopHotkey;
        private Label? labelPauseHotkey;
        private Button? btnSetStartStopHotkey;
        private Button? btnSetPauseHotkey;
    }
}