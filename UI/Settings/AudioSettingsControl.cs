using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.UI.Components;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Settings;

public partial class AudioSettingsControl : UserControl
{
    private readonly IAppSettings? _appSettings = null!;
    private readonly IAudioService? _audioService = null!;

    // 控件引用，用于逻辑绑定
    private ThemedButton? btnPreviewTimerStart = null!;
    private ThemedButton? btnPreviewTimerPause = null!;
    private ThemedButton? btnPreviewBreakStart = null!;
    private ThemedButton? btnPreviewBreakEnd = null!;

    // 播放按钮映射 (Button -> Original Text)
    private Dictionary<Button, string> _previewButtonMap = new();

    // 默认构造函数，用于设计器
    public AudioSettingsControl()
    {
        InitializeComponent();
        InitializeAudioRows();
    }

    // 运行时构造函数，用于依赖注入
    public AudioSettingsControl(IAppSettings appSettings, IAudioService audioService)
        : this()
    {
        _appSettings = appSettings;
        _audioService = audioService;
        InitializeData();
        BindEvents();
    }

    private void InitializeAudioRows()
    {
        // 辅助方法：添加一行配置
        void AddRow(string labelKey, string soundFileName, out ThemedButton btn, int rowIndex)
        {
            // 1. Label
            var lbl = new ThemedLabel
            {
                Text = Utils.LanguageManager.GetString(labelKey),
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                AutoSize = true
            };

            // 2. Sound File Name Label
            var lblSoundFile = new ThemedLabel
            {
                Text = soundFileName,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                AutoSize = true,
                ForeColor = UI.Theme.AppTheme.Colors.TextSecondaryColor,
                Margin = new Padding(10, 0, 10, 0)
            };

            // 3. Preview Button
            btn = new ThemedButton
            {
                Text = "▶", // Play Icon
                Width = 40,
                Height = 30, // 稍微高一点方便点击
                Dock = DockStyle.Left,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 10, 0)
            };

            // 添加到 TableLayout
            this.tblAudioRows.Controls.Add(lbl, 0, rowIndex);
            this.tblAudioRows.Controls.Add(lblSoundFile, 1, rowIndex);
            this.tblAudioRows.Controls.Add(btn, 2, rowIndex);
        }

        // 获取音频文件名，使用默认值或从设置中读取
        string timerStartFile = _appSettings?.SoundTimerStart ?? "timer_start.mp3";
        string timerPauseFile = _appSettings?.SoundTimerPause ?? "timer_pause.mp3";
        string breakStartFile = _appSettings?.SoundBreakStart ?? "break_start.mp3";
        string breakEndFile = _appSettings?.SoundBreakEnd ?? "break_end.mp3";

        AddRow("Settings.Audio.TimerStart", timerStartFile, out btnPreviewTimerStart, 0);
        AddRow("Settings.Audio.TimerPause", timerPauseFile, out btnPreviewTimerPause, 1);
        AddRow("Settings.Audio.BreakStart", breakStartFile, out btnPreviewBreakStart, 2);
        AddRow("Settings.Audio.BreakEnd", breakEndFile, out btnPreviewBreakEnd, 3);
    }

    public void LoadSettings(IAppSettings settings)
    {
        if (settings == null) return;

        this.SafeInvoke(() =>
        {
            // 3. 基础状态
            if (chkEnableAudio != null)
                chkEnableAudio.Checked = settings.AudioEnabled;
            if (trackVolume != null)
                trackVolume.Value = Math.Clamp(settings.AudioVolume, 0, 100);
            if (lblVolumeValue != null)
                lblVolumeValue.Text = $"{settings.AudioVolume}%";

            if (chkEnableAudio != null)
                UpdateControlState(chkEnableAudio.Checked);
        });
    }

    public void RefreshUI()
    {

    }

    private void InitializeData()
    {
        if (_appSettings == null || _audioService == null) return;

        // 翻译静态文本
        if (chkEnableAudio != null)
            chkEnableAudio.Text = Utils.LanguageManager.GetString("Settings.Audio.Enable");
        if (lblVolumeTitle != null)
            lblVolumeTitle.Text = Utils.LanguageManager.GetString("Settings.Audio.Volume");

        // 基础状态
        if (chkEnableAudio != null)
            chkEnableAudio.Checked = _appSettings.AudioEnabled;
        if (trackVolume != null)
            trackVolume.Value = Math.Clamp(_appSettings.AudioVolume, 0, 100);
        if (lblVolumeValue != null)
            lblVolumeValue.Text = $"{_appSettings.AudioVolume}%";

        if (chkEnableAudio != null)
            UpdateControlState(chkEnableAudio.Checked);
    }

    private void BindEvents()
    {
        if (_appSettings == null || _audioService == null) return;

        // 开关与音量
        if (chkEnableAudio != null)
            chkEnableAudio.CheckedChanged += (s, e) =>
            {
                _appSettings.AudioEnabled = chkEnableAudio.Checked;
                UpdateControlState(chkEnableAudio.Checked);
            };

        if (trackVolume != null && lblVolumeValue != null)
            trackVolume.Scroll += (s, e) =>
            {
                _appSettings.AudioVolume = trackVolume.Value;
                lblVolumeValue.Text = $"{trackVolume.Value}%";
            };

        // 绑定试听，直接使用配置中的声音文件
        if (btnPreviewTimerStart != null)
            BindPreviewButton(btnPreviewTimerStart, () => _appSettings.SoundTimerStart);
        if (btnPreviewTimerPause != null)
            BindPreviewButton(btnPreviewTimerPause, () => _appSettings.SoundTimerPause);
        if (btnPreviewBreakStart != null)
            BindPreviewButton(btnPreviewBreakStart, () => _appSettings.SoundBreakStart);
        if (btnPreviewBreakEnd != null)
            BindPreviewButton(btnPreviewBreakEnd, () => _appSettings.SoundBreakEnd);
    }

    private void BindPreviewButton(Button btn, Func<string> getFileName)
    {
        _previewButtonMap[btn] = "▶"; // 注册默认图标

        btn.Click += (s, e) =>
        {
            if (_audioService == null) return;

            string fileName = getFileName();
            if (string.IsNullOrEmpty(fileName)) return;

            // 核心交互：如果是"正在试听"且点击的是"自己"，则是停止
            bool isCurrentButtonPlaying = btn.Text == "■";

            if (isCurrentButtonPlaying)
            {
                _audioService.StopPreview();
                // UI update will happen in callback
            }
            else
            {
                // 先重置所有按钮UI（防御性）
                ResetAllPreviewButtons();

                // 开始试听
                btn.Text = "■"; // 暂停图标
                btn.ForeColor = Color.Orange; // 高亮

                _audioService.PreviewSound(fileName, () =>
                {
                    // 回调：播放结束或停止
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        this.Invoke(new Action(() => ResetButtonState(btn)));
                    }
                });
            }
        };
    }

    private void ResetButtonState(Button btn)
    {
        if (_previewButtonMap.ContainsKey(btn))
            btn.Text = _previewButtonMap[btn];
        btn.ForeColor = UI.Theme.AppTheme.TextColor; // 恢复主题色
    }

    private void ResetAllPreviewButtons()
    {
        foreach (var btn in _previewButtonMap.Keys)
        {
            ResetButtonState(btn);
        }
    }

    private void UpdateControlState(bool enabled)
    {
        if (trackVolume != null)
            trackVolume.Enabled = enabled;
        if (tblAudioRows != null)
            tblAudioRows.Enabled = enabled;
        // 变灰效果可由各控件自身处理，或统一调整透明度
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        _audioService?.StopPreview(); // 确保关闭窗口时停止声音
        base.OnHandleDestroyed(e);
    }
}