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

        if (_appSettings.ScreenshotOnLoot)
        {
            PerformScreenshot();
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

    protected virtual void OnLootRecordSaved(EventArgs e)
    {
        LootRecordSaved?.Invoke(this, e);
    }

    private void PerformScreenshot()
    {
        bool needHide = _appSettings.HideWindowOnScreenshot;

        try
        {
            if (needHide)
            {
                // 1. 发送消息通知主窗体隐藏 (解耦)
                _messenger.Publish(new HideMainWindowMessage());

                // 2. 隐藏自己
                this.Opacity = 0;

                // 3. 强制刷新与缓冲
                Application.DoEvents();
                System.Threading.Thread.Sleep(200);
            }

            // 4. 截图
            string lootName = txtLootName.Text.Trim();
            string? path = ScreenshotHelper.CaptureAndSave(lootName);
            if (path == null)
            {
                Utils.Toast.Error(LanguageManager.GetString("ScreenshotFailed") ?? "截图失败");
                return;
            }
            else
            {
                Utils.Toast.Success(LanguageManager.GetString("ScreenshotSavedSuccessfully") ?? "截图保存成功");
            }

            // ... (可选提示)
        }
        catch (Exception ex)
        {
            LogManager.WriteErrorLog("RecordLootForm", "截图流程异常", ex);
        }
        finally
        {
            // 5. 恢复显示
            if (needHide)
            {
                // 恢复自己
                this.Opacity = 1;
                // 发送消息通知主窗体恢复
                _messenger.Publish(new ShowMainWindowMessage());

                Application.DoEvents();
            }
        }
    }
}
