using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Settings
{
    public partial class GeneralSettingsControl : UserControl
    {
        public GeneralSettingsControl()
        {
            InitializeComponent();
        }

        public void LoadSettings(Services.IAppSettings settings)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<Services.IAppSettings>(LoadSettings), settings);
                return;
            }

            // 1. 设置“总在最前”
            if (alwaysOnTopCheckBox != null)
            {
                alwaysOnTopCheckBox.Checked = settings.AlwaysOnTop;
            }

            // 2. 设置语言 (使用 SettingsManager 的转换逻辑)
            if (groupBoxLanguage != null)
            {
                var langOption = SettingsManager.StringToLanguage(settings.Language);
                if (langOption == SettingsControl.LanguageOption.English)
                {
                    englishRadioButton!.Checked = true;
                }
                else
                {
                    chineseRadioButton!.Checked = true;
                }
            }

            // 3. 设置窗口位置 (使用 SettingsManager 的转换逻辑)
            if (groupBoxPosition != null)
            {
                var position = SettingsManager.StringToWindowPosition(settings.WindowPosition);
                switch (position)
                {
                    case SettingsControl.WindowPosition.TopLeft:
                        radioTopLeft!.Checked = true;
                        break;
                    case SettingsControl.WindowPosition.TopCenter:
                        radioTopCenter!.Checked = true;
                        break;
                    case SettingsControl.WindowPosition.TopRight:
                        radioTopRight!.Checked = true;
                        break;
                    case SettingsControl.WindowPosition.BottomLeft:
                        radioBottomLeft!.Checked = true;
                        break;
                    case SettingsControl.WindowPosition.BottomCenter:
                        radioBottomCenter!.Checked = true;
                        break;
                    case SettingsControl.WindowPosition.BottomRight:
                        radioBottomRight!.Checked = true;
                        break;
                    default:
                        radioTopLeft!.Checked = true;
                        break;
                }
            }
        }

        public void RefreshUI()
        {
            // 运行时动态更新文本
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(RefreshUI));
                return;
            }

            // 再次检查以防万一
            if (groupBoxPosition == null)
                return;

            try
            {
                // 这里的 try-catch 是为了防止 Design 模式下偶然调用 LanguageManager 报错
                groupBoxPosition.Text = LanguageManager.GetString("WindowPosition");
                radioTopLeft!.Text = LanguageManager.GetString("TopLeft");
                radioTopCenter!.Text = LanguageManager.GetString("TopCenter");
                radioTopRight!.Text = LanguageManager.GetString("TopRight");
                radioBottomLeft!.Text = LanguageManager.GetString("BottomLeft");
                radioBottomCenter!.Text = LanguageManager.GetString("BottomCenter");
                radioBottomRight!.Text = LanguageManager.GetString("BottomRight");

                groupBoxLanguage!.Text = LanguageManager.GetString("Language");
                chineseRadioButton!.Text = LanguageManager.GetString("Chinese");
                englishRadioButton!.Text = LanguageManager.GetString("English");

                alwaysOnTopLabel!.Text = LanguageManager.GetString("AlwaysOnTop");
            }
            catch
            {
                // 如果出错（比如资源找不到），保持 InitializeComponent 中的默认文本
            }
        }

        // ... 属性部分保持不变 ...
        public SettingsControl.WindowPosition SelectedPosition
        {
            get
            {
                if (radioTopLeft?.Checked ?? false)
                    return SettingsControl.WindowPosition.TopLeft;
                if (radioTopCenter?.Checked ?? false)
                    return SettingsControl.WindowPosition.TopCenter;
                if (radioTopRight?.Checked ?? false)
                    return SettingsControl.WindowPosition.TopRight;
                if (radioBottomLeft?.Checked ?? false)
                    return SettingsControl.WindowPosition.BottomLeft;
                if (radioBottomCenter?.Checked ?? false)
                    return SettingsControl.WindowPosition.BottomCenter;
                if (radioBottomRight?.Checked ?? false)
                    return SettingsControl.WindowPosition.BottomRight;
                return SettingsControl.WindowPosition.TopLeft;
            }
        }

        public SettingsControl.LanguageOption SelectedLanguage
        {
            get
            {
                return (chineseRadioButton?.Checked ?? false)
                    ? SettingsControl.LanguageOption.Chinese
                    : SettingsControl.LanguageOption.English;
            }
        }

        public bool IsAlwaysOnTop => alwaysOnTopCheckBox?.Checked ?? false;
    }
}
