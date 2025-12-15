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

    private class OpacityOption
    {
        public string Name { get; set; } = "";
        public double Value { get; set; }

        public override string ToString() => Name;
    }

    public GeneralSettingsControl()
    {
        InitializeComponent();
        InitializeScaleOptions();
        InitializeOpacityOptions(); // 新增初始化
    }

    private void InitializeScaleOptions()
    {
        if (cmbUiScale == null)
            return;

        cmbUiScale.Items.Clear();
        cmbUiScale.Items.Add(new ScaleOption { Name = "Auto", Value = 0f });
        cmbUiScale.Items.Add(new ScaleOption { Name = "100% (1080P)", Value = 1.0f });
        cmbUiScale.Items.Add(new ScaleOption { Name = "150% (2K)", Value = 1.5f });
        cmbUiScale.Items.Add(new ScaleOption { Name = "175%", Value = 1.75f });
        cmbUiScale.Items.Add(new ScaleOption { Name = "200% (4K)", Value = 2.0f });
        cmbUiScale.Items.Add(new ScaleOption { Name = "250%", Value = 2.5f });
        cmbUiScale.SelectedIndex = 0;
    }

    private void InitializeOpacityOptions()
    {
        if (cmbOpacity == null)
            return;
        cmbOpacity.Items.Clear();
        // 添加 100% 到 10%
        for (int i = 100; i >= 10; i -= 10)
        {
            cmbOpacity.Items.Add(new OpacityOption { Name = $"{i}%", Value = i / 100.0 });
        }
        cmbOpacity.SelectedIndex = 0; // 默认 100%
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
                if (langOption == Models.LanguageOption.English)
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
                    case Models.WindowPosition.TopRight:
                        radioTopRight!.Checked = true;
                        break;
                    case Models.WindowPosition.BottomLeft:
                        radioBottomLeft!.Checked = true;
                        break;
                    case Models.WindowPosition.BottomRight:
                        radioBottomRight!.Checked = true;
                        break;
                    default:
                        radioTopLeft!.Checked = true;
                        break;
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

            // 5. Opacity (加载逻辑)
            if (cmbOpacity != null)
            {
                double current = settings.Opacity;
                int targetIndex = 0;
                // 查找最接近的选项
                for (int i = 0; i < cmbOpacity.Items.Count; i++)
                {
                    if (cmbOpacity.Items[i] is OpacityOption opt && Math.Abs(opt.Value - current) < 0.01)
                    {
                        targetIndex = i;
                        break;
                    }
                }
                cmbOpacity.SelectedIndex = targetIndex;
            }
        });
    }

    public void RefreshUI()
    {
        this.SafeInvoke(() =>
        {
            if (groupBoxPosition == null)
                return;
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
                grpOpacity!.Text = LanguageManager.GetString("Opacity");

                grpUiScale!.Text = LanguageManager.GetString("UiScale");
            }
            catch { }
        });
    }

    // Properties
    public Models.WindowPosition SelectedPosition
    {
        get
        {
            if (radioTopRight?.Checked ?? false)
                return Models.WindowPosition.TopRight;
            if (radioBottomLeft?.Checked ?? false)
                return Models.WindowPosition.BottomLeft;
            if (radioBottomRight?.Checked ?? false)
                return Models.WindowPosition.BottomRight;
            return Models.WindowPosition.TopLeft;
        }
        set
        {
            switch (value)
            {
                case Models.WindowPosition.TopLeft:
                    radioTopLeft!.Checked = true;
                    break;
                case Models.WindowPosition.TopRight:
                    radioTopRight!.Checked = true;
                    break;
                case Models.WindowPosition.BottomLeft:
                    radioBottomLeft!.Checked = true;
                    break;
                case Models.WindowPosition.BottomRight:
                    radioBottomRight!.Checked = true;
                    break;
            }
        }
    }

    public Models.LanguageOption SelectedLanguage =>
        (chineseRadioButton?.Checked ?? false) ? Models.LanguageOption.Chinese : Models.LanguageOption.English;

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

    public double SelectedOpacity
    {
        get
        {
            if (cmbOpacity?.SelectedItem is OpacityOption option)
            {
                return option.Value;
            }
            return 1.0;
        }
    }
}
