using System;
using System.ComponentModel;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.UI.Form;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI.Timer;

public partial class RecordLootForm : BaseForm
{
    private readonly IProfileService _profileService = null!;
    private readonly ITimerHistoryService _timerHistoryService = null!;
    private readonly ISceneService _sceneService = null!;
    private readonly IAppSettings _appSettings = null!;
    private readonly IMessenger _messenger = null!;
    private bool _shouldCaptureScreenshot = false;
    private string _lootNameForCapture = string.Empty;

    public event EventHandler? LootRecordSaved;

    // 1. 无参构造函数 (VS设计器专用)
    public RecordLootForm()
    {
        InitializeComponent();
    }

    // 2. 依赖注入构造函数 (运行时专用)
    public RecordLootForm(
        IProfileService profileService,
        ITimerHistoryService timerHistoryService,
        ISceneService sceneService,
        IAppSettings appSettings,
        IMessenger messenger
    )
        : this()
    {
        _profileService = profileService;
        _timerHistoryService = timerHistoryService;
        _sceneService = sceneService;
        _appSettings = appSettings;
        _messenger = messenger;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (!this.DesignMode)
        {
            UpdateUI();
        }
    }

    // 添加 OnShown 覆盖代替构造函数里的事件绑定
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (!this.DesignMode)
        {
            this.Activate();
            txtLootName.Focus();
            txtLootName.SelectAll();
        }
    }

    protected override void UpdateUI()
    {
        base.UpdateUI();
        this.Text = LanguageManager.GetString("RecordLoot") ?? "记录掉落";
        txtLootName.PlaceholderText = LanguageManager.GetString("EnterLootName") ?? "输入掉落名称";
        label1.Text = LanguageManager.GetString("LootNameLabel") ?? "掉落名称:";
        chkPreviousRun.Text = LanguageManager.GetString("UsePreviousRun") ?? "使用上一次符文掉落";
        if (btnConfirm != null)
            btnConfirm.Text = LanguageManager.GetString("Save") ?? "保存";
    }

    protected override void BtnConfirm_Click(object? sender, EventArgs e)
    {
        SaveLootRecord();
    }
    private void SaveLootRecord()
    {
        if (string.IsNullOrWhiteSpace(txtLootName.Text))
        {
            Utils.Toast.Warning(LanguageManager.GetString("EnterLootNameMessage") ?? "请输入掉落名称");
            return;
        }

        // 1. 【修改】不再直接调用 PerformScreenshot，而是标记状态
        if (_appSettings.ScreenshotOnLoot)
        {
            _shouldCaptureScreenshot = true;
            _lootNameForCapture = txtLootName.Text.Trim();
        }

        int runCount = _timerHistoryService.RunCount + 1;
        if (chkPreviousRun.Checked)
        {
            runCount = Math.Max(0, runCount - 1);
        }

        string sceneName = _profileService.CurrentScene ?? "";
        string englishSceneName = _sceneService.GetEnglishSceneName(sceneName);

        var lootRecord = new LootRecord
        {
            Name = txtLootName.Text.Trim(),
            SceneName = englishSceneName,
            RunCount = runCount,
            DropTime = DateTime.Now,
        };

        var currentProfile = _profileService.CurrentProfile;
        if (currentProfile != null)
        {
            currentProfile.LootRecords.Add(lootRecord);
            _profileService.SaveCurrentProfile();
        }

        Utils.Toast.Success(LanguageManager.GetString("LootRecordSavedSuccessfully") ?? "掉落记录保存成功");
        OnLootRecordSaved(EventArgs.Empty);

        DialogResult = DialogResult.OK;
        Close();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        base.OnFormClosed(e);

        if (_shouldCaptureScreenshot && !string.IsNullOrEmpty(_lootNameForCapture))
        {
            // 发送消息给 MainForm 处理截图，此时本窗口已经销毁/隐藏，不会挡住截图
            _messenger.Publish(new ScreenshotRequestedMessage(_lootNameForCapture));
        }
    }

    protected virtual void OnLootRecordSaved(EventArgs e)
    {
        LootRecordSaved?.Invoke(this, e);
    }
}
