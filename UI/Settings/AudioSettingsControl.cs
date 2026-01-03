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

    // 取消按钮引用，用于逻辑绑定
    private ThemedButton? btnCancelTimerStart = null!;
    private ThemedButton? btnCancelTimerPause = null!;
    private ThemedButton? btnCancelBreakStart = null!;
    private ThemedButton? btnCancelBreakEnd = null!;

    // 播放按钮映射 (Button -> Original Text)
    private Dictionary<Button, string> _previewButtonMap = new();

    // 行控件映射，用于状态管理
    private Dictionary<ThemedButton, Tuple<ThemedLabel, ThemedLabel, ThemedButton>> _rowControlsMap = new();

    // 取消按钮状态映射
    private Dictionary<ThemedButton, bool> _cancelButtonStates = new();

    private const string _playIcon = "\uE102";
    private const string _stopIcon = "\uE004";
    private const string _cancelIcon = "\uE711";

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
        void AddRow(string labelKey, string soundFileName, out ThemedButton btnPreview, out ThemedButton btnCancel, int rowIndex)
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
            btnPreview = new ThemedButton
            {
                Text = _playIcon, // Play Icon
                Font = new Font("Segoe MDL2 Assets", 18, FontStyle.Regular),
                Width = 40,
                Height = 30, // 稍微高一点方便点击
                Dock = DockStyle.Left,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 5, 10, 5)
            };

            // 4. Cancel Button
            btnCancel = new ThemedButton
            {
                Text = _cancelIcon, // Cancel Icon
                Font = new Font("Segoe MDL2 Assets", 18, FontStyle.Regular),
                Width = 40,
                Height = 30,
                Dock = DockStyle.Left,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 5, 0, 5),
                ForeColor = UI.Theme.AppTheme.TextColor
            };

            // 添加到 TableLayout
            this.tblAudioRows.Controls.Add(lbl, 0, rowIndex);
            this.tblAudioRows.Controls.Add(lblSoundFile, 1, rowIndex);
            this.tblAudioRows.Controls.Add(btnPreview, 2, rowIndex);
            this.tblAudioRows.Controls.Add(btnCancel, 3, rowIndex);

            // 存储行控件映射
            _rowControlsMap[btnCancel] = Tuple.Create(lbl, lblSoundFile, btnPreview);
            _cancelButtonStates[btnCancel] = false;
        }

        // 获取音频文件名，使用默认值或从设置中读取
        string timerStartFile = _appSettings?.SoundTimerStart ?? "timer_start.mp3";
        string timerPauseFile = _appSettings?.SoundTimerPause ?? "timer_pause.mp3";
        string breakStartFile = _appSettings?.SoundBreakStart ?? "break_start.mp3";
        string breakEndFile = _appSettings?.SoundBreakEnd ?? "break_end.mp3";

        AddRow("Settings.Audio.TimerStart", timerStartFile, out btnPreviewTimerStart, out btnCancelTimerStart, 0);
        AddRow("Settings.Audio.TimerPause", timerPauseFile, out btnPreviewTimerPause, out btnCancelTimerPause, 1);
        AddRow("Settings.Audio.BreakStart", breakStartFile, out btnPreviewBreakStart, out btnCancelBreakStart, 2);
        AddRow("Settings.Audio.BreakEnd", breakEndFile, out btnPreviewBreakEnd, out btnCancelBreakEnd, 3);
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

        // 加载取消按钮状态
        LoadCancelButtonStates();

        if (chkEnableAudio != null)
            UpdateControlState(chkEnableAudio.Checked);
    }

    private void LoadCancelButtonStates()
    {
        if (_appSettings == null) return;

        // 加载各取消按钮状态
        if (btnCancelTimerStart != null)
        {
            _cancelButtonStates[btnCancelTimerStart] = !_appSettings.SoundTimerStartEnabled;
            UpdateCancelButtonState(btnCancelTimerStart, _cancelButtonStates[btnCancelTimerStart]);
        }
        if (btnCancelTimerPause != null)
        {
            _cancelButtonStates[btnCancelTimerPause] = !_appSettings.SoundTimerPauseEnabled;
            UpdateCancelButtonState(btnCancelTimerPause, _cancelButtonStates[btnCancelTimerPause]);
        }
        if (btnCancelBreakStart != null)
        {
            _cancelButtonStates[btnCancelBreakStart] = !_appSettings.SoundBreakStartEnabled;
            UpdateCancelButtonState(btnCancelBreakStart, _cancelButtonStates[btnCancelBreakStart]);
        }
        if (btnCancelBreakEnd != null)
        {
            _cancelButtonStates[btnCancelBreakEnd] = !_appSettings.SoundBreakEndEnabled;
            UpdateCancelButtonState(btnCancelBreakEnd, _cancelButtonStates[btnCancelBreakEnd]);
        }
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

        // 绑定取消按钮
        if (btnCancelTimerStart != null)
            BindCancelButton(btnCancelTimerStart, () => { _appSettings.SoundTimerStartEnabled = !_appSettings.SoundTimerStartEnabled; });
        if (btnCancelTimerPause != null)
            BindCancelButton(btnCancelTimerPause, () => { _appSettings.SoundTimerPauseEnabled = !_appSettings.SoundTimerPauseEnabled; });
        if (btnCancelBreakStart != null)
            BindCancelButton(btnCancelBreakStart, () => { _appSettings.SoundBreakStartEnabled = !_appSettings.SoundBreakStartEnabled; });
        if (btnCancelBreakEnd != null)
            BindCancelButton(btnCancelBreakEnd, () => { _appSettings.SoundBreakEndEnabled = !_appSettings.SoundBreakEndEnabled; });
    }

    private void BindCancelButton(ThemedButton btn, Action onStateChanged)
    {
        btn.Click += (s, e) =>
        {
            // 切换状态
            _cancelButtonStates[btn] = !_cancelButtonStates[btn];

            // 更新UI
            UpdateCancelButtonState(btn, _cancelButtonStates[btn]);

            // 触发状态变更回调
            onStateChanged();
        };
    }

    private void UpdateCancelButtonState(ThemedButton btn, bool isActive)
    {
        if (!_rowControlsMap.TryGetValue(btn, out var controls)) return;

        var (lbl, lblSoundFile, btnPreview) = controls;

        // 使用 ThemedButton 的 IsSelected 属性触发高亮效果
        btn.IsSelected = isActive;

        if (isActive)
        {
            // 激活状态：禁用播放按钮，灰色文字
            btnPreview.Enabled = false;
            lbl.ForeColor = UI.Theme.AppTheme.Colors.TextSecondaryColor;
            lblSoundFile.ForeColor = UI.Theme.AppTheme.Colors.TextSecondaryColor;
        }
        else
        {
            // 非激活状态：启用播放按钮，正常文字
            btnPreview.Enabled = true;
            lbl.ForeColor = UI.Theme.AppTheme.TextColor;
            lblSoundFile.ForeColor = UI.Theme.AppTheme.Colors.TextSecondaryColor;
        }
    }

    private void BindPreviewButton(Button btn, Func<string> getFileName)
    {
        _previewButtonMap[btn] = _playIcon; // 注册默认图标
        btn.Click += (s, e) =>
        {
            if (_audioService == null) return;

            string fileName = getFileName();
            if (string.IsNullOrEmpty(fileName)) return;

            // 核心交互：如果是"正在试听"且点击的是"自己"，则是停止
            bool isCurrentButtonPlaying = btn.Text == _stopIcon;

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
                btn.Text = _stopIcon; // 暂停图标
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
        {
            trackVolume.Enabled = enabled;
            trackVolume.ForeColor = enabled ? UI.Theme.AppTheme.TextColor : UI.Theme.AppTheme.Colors.TextSecondaryColor;
        }

        if (lblVolumeTitle != null)
        {
            lblVolumeTitle.ForeColor = enabled ? UI.Theme.AppTheme.TextColor : UI.Theme.AppTheme.Colors.TextSecondaryColor;
        }

        if (lblVolumeValue != null)
        {
            lblVolumeValue.ForeColor = enabled ? UI.Theme.AppTheme.TextColor : UI.Theme.AppTheme.Colors.TextSecondaryColor;
        }

        if (tblAudioRows != null)
        {
            tblAudioRows.Enabled = enabled;
        }
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        _audioService?.StopPreview(); // 确保关闭窗口时停止声音
        base.OnHandleDestroyed(e);
    }
}