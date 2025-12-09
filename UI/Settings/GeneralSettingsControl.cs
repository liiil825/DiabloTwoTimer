using System;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Settings;

public partial class GeneralSettingsControl : UserControl
{
    // 定义内部类用于 ComboBox 显示
    private class ScaleOption
    {
        public string Name { get; set; } = "";
        public float Value { get; set; }
        public override string ToString() => Name;
    }

    public GeneralSettingsControl()
    {
        InitializeComponent();
        InitializeScaleOptions();
    }

    private void InitializeScaleOptions()
    {
        if (cmbUiScale == null) return;

        cmbUiScale.Items.Clear();
        cmbUiScale.Items.Add(new ScaleOption { Name = "Auto", Value = 0f });
        cmbUiScale.Items.Add(new ScaleOption { Name = "100% (1080P)", Value = 1.0f });
        cmbUiScale.Items.Add(new ScaleOption { Name = "150% (2K)", Value = 1.5f });
        cmbUiScale.Items.Add(new ScaleOption { Name = "175%", Value = 1.6f });
        cmbUiScale.Items.Add(new ScaleOption { Name = "200% (4K)", Value = 2.0f });
        cmbUiScale.Items.Add(new ScaleOption { Name = "250%", Value = 2.5f });
        cmbUiScale.SelectedIndex = 0;
    }

    public void LoadSettings(IAppSettings settings)
    {
        this.SafeInvoke(() =>
        {
            // 1. Always On Top
            if (alwaysOnTopCheckBox != null)
                alwaysOnTopCheckBox.Checked = settings.AlwaysOnTop;

            // 2. Language
            if (groupBoxLanguage != null)
            {
                var langOption = AppSettings.StringToLanguage(settings.Language);
                if (langOption == SettingsControl.LanguageOption.English)
                    englishRadioButton!.Checked = true;
                else
                    chineseRadioButton!.Checked = true;
            }

            // 3. Window Position
            if (groupBoxPosition != null)
            {
                var position = AppSettings.StringToWindowPosition(settings.WindowPosition);
                switch (position)
                {
                    case SettingsControl.WindowPosition.TopCenter: radioTopCenter!.Checked = true; break;
                    case SettingsControl.WindowPosition.TopRight: radioTopRight!.Checked = true; break;
                    case SettingsControl.WindowPosition.BottomLeft: radioBottomLeft!.Checked = true; break;
                    case SettingsControl.WindowPosition.BottomCenter: radioBottomCenter!.Checked = true; break;
                    case SettingsControl.WindowPosition.BottomRight: radioBottomRight!.Checked = true; break;
                    default: radioTopLeft!.Checked = true; break;
                }
            }

            // 4. UI Scale (加载逻辑)
            if (cmbUiScale != null)
            {
                float currentScale = settings.UiScale;
                int targetIndex = 0;
                // 遍历寻找最接近的选项
                for (int i = 0; i < cmbUiScale.Items.Count; i++)
                {
                    if (cmbUiScale.Items[i] is ScaleOption opt && Math.Abs(opt.Value - currentScale) < 0.01f)
                    {
                        targetIndex = i;
                        break;
                    }
                }
                cmbUiScale.SelectedIndex = targetIndex;
            }
        });
    }

    public void RefreshUI()
    {
        this.SafeInvoke(() =>
        {
            if (groupBoxPosition == null) return;
            try
            {
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

                alwaysOnTopCheckBox!.Text = LanguageManager.GetString("AlwaysOnTop");

                if (grpUiScale != null) grpUiScale.Text = "界面大小 (需重启)";
            }
            catch { }
        });
    }

    // Properties
    public SettingsControl.WindowPosition SelectedPosition
    {
        get
        {
            if (radioTopCenter?.Checked ?? false) return SettingsControl.WindowPosition.TopCenter;
            if (radioTopRight?.Checked ?? false) return SettingsControl.WindowPosition.TopRight;
            if (radioBottomLeft?.Checked ?? false) return SettingsControl.WindowPosition.BottomLeft;
            if (radioBottomCenter?.Checked ?? false) return SettingsControl.WindowPosition.BottomCenter;
            if (radioBottomRight?.Checked ?? false) return SettingsControl.WindowPosition.BottomRight;
            return SettingsControl.WindowPosition.TopLeft;
        }
    }

    public SettingsControl.LanguageOption SelectedLanguage =>
        (chineseRadioButton?.Checked ?? false) ? SettingsControl.LanguageOption.Chinese : SettingsControl.LanguageOption.English;

    public bool IsAlwaysOnTop => alwaysOnTopCheckBox?.Checked ?? false;

    // 获取选中的缩放比例
    public float SelectedUiScale
    {
        get
        {
            if (cmbUiScale?.SelectedItem is ScaleOption option)
            {
                return option.Value;
            }
            return 0f;
        }
    }
}