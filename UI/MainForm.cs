using System;
using System.Drawing;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.UI.Pomodoro;
using DiabloTwoMFTimer.UI.Profiles;
using DiabloTwoMFTimer.UI.Settings;
using DiabloTwoMFTimer.UI.Timer;
using DiabloTwoMFTimer.Utils;

namespace DiabloTwoMFTimer.UI;

public partial class MainForm : Form
{
    // 服务引用
    private readonly IMainService _mainService = null!;
    private readonly IAppSettings _appSettings = null!;

    // 为了创建 RecordLootForm，我们需要这两个服务 (或者你可以选择把弹窗逻辑封装进 TimerControl)
    private readonly IProfileService _profileService = null!;
    private readonly ITimerHistoryService _timerHistoryService = null!;
    private readonly IMessenger _messenger = null!;
    private readonly ISceneService _sceneService = null!;

    // 子控件引用 (用于后续的方法调用)
    private readonly ProfileManager _profileManager = null!;
    private readonly TimerControl _timerControl = null!;
    private readonly PomodoroControl _pomodoroControl = null!;
    private readonly SettingsControl _settingsControl = null!;

    public MainForm()
    {
        InitializeComponent();

        // 可选：为了防止设计器里看着太空，可以放一些假数据或空对象，避免空引用报错
        if (DesignMode)
        {
            // 仅在设计模式下不做任何事，或者给字段赋 null
        }
    }

    // 构造函数：通过依赖注入获取所有需要的组件
    public MainForm(
        IMainService mainService,
        IAppSettings settings,
        IProfileService profileService,
        ITimerHistoryService timerHistoryService,
        ISceneService sceneService,
        IMessenger messenger,
        ProfileManager profileManager,
        TimerControl timerControl,
        PomodoroControl pomodoroControl,
        SettingsControl settingsControl
    )
        : this()
    {
        _mainService = mainService;
        _appSettings = settings;
        _profileService = profileService;
        _timerHistoryService = timerHistoryService;
        _sceneService = sceneService;

        _profileManager = profileManager;
        _timerControl = timerControl;
        _pomodoroControl = pomodoroControl;
        _settingsControl = settingsControl;
        _messenger = messenger;

        // 组装 UI：将注入的控件添加到 TabPage 中
        InitializeChildControls();

        // 初始化窗体属性
        InitializeForm();

        // 订阅 MainService 的事件
        SubscribeToEvents();
        SubscribeToMessages();

        // 注册窗体显示事件
        this.Shown += OnMainForm_Shown;
    }

    // UI 控件暴露 (虽然重构后应尽量减少直接访问，但保留以防万一)
    internal TabControl? TabControl => tabControl;

    private void OnMainForm_Shown(object? sender, EventArgs e)
    {
        // 关键：将窗口句柄传给 Service 用于注册热键
        _mainService.InitializeApplication(this.Handle);

        // 应用窗口位置设置
        _mainService.ApplyWindowSettings(this);
    }

    private void InitializeChildControls()
    {
        // 辅助方法：设置 Dock 并添加到 TabPage
        static void AddControlToTab(System.Windows.Forms.TabPage page, Control control)
        {
            control.Dock = DockStyle.Fill;
            page.Controls.Add(control);
        }

        AddControlToTab(tabProfilePage, _profileManager);
        AddControlToTab(tabTimerPage, _timerControl);
        AddControlToTab(tabPomodoroPage, _pomodoroControl);
        AddControlToTab(tabSettingsPage, _settingsControl);
    }

    private void InitializeForm()
    {
        AdjustWindowHeight();
        this.StartPosition = FormStartPosition.Manual;
        this.ShowInTaskbar = true;
        this.Visible = true;
    }

    private void SubscribeToEvents()
    {
        // 1. 响应 Tab 切换请求
        _mainService.OnRequestTabChange += (tabPage) =>
        {
            if (tabControl != null && (int)tabPage < tabControl.TabCount)
            {
                this.SafeInvoke(() => tabControl.SelectedIndex = (int)tabPage);
            }
        };

        // 2. 响应 UI 刷新请求 (例如切换语言后)
        _mainService.OnRequestRefreshUI += () =>
        {
            this.SafeInvoke(UpdateFormTitleAndTabs);
        };

        // 3. 响应删除历史记录请求 (热键触发)
        _mainService.OnRequestDeleteHistory += () =>
        {
            this.SafeInvoke(() =>
            {
                // 只有当前在 Timer 页时才响应删除
                if (tabControl.SelectedIndex == (int)Models.TabPage.Timer)
                {
                    _ = _timerControl.DeleteSelectedRecordAsync();
                }
            });
        };

        // 4. 响应记录战利品请求 (热键触发)
        _mainService.OnRequestRecordLoot += () =>
        {
            this.SafeInvoke(ShowRecordLootDialog);
        };
    }

    private void SubscribeToMessages()
    {
        _messenger.Subscribe<WindowPositionChangedMessage>(_ =>
        {
            this.SafeInvoke(() =>
            {
                // 直接调用 MainService 的辅助方法应用位置
                _mainService.ApplyWindowSettings(this);
            });
        });

        // [新增] 监听始终置顶变更
        _messenger.Subscribe<AlwaysOnTopChangedMessage>(_ =>
        {
            this.SafeInvoke(() =>
            {
                // 从配置中读取最新值
                this.TopMost = _appSettings.AlwaysOnTop;
            });
        });
        // 监听设置变更 -> 调整窗口大小
        // 窗口大小属于 MainForm 的职责，不应该由 TimerControl 来改 ParentForm
        _messenger.Subscribe<TimerSettingsChangedMessage>(msg =>
        {
            this.SafeInvoke(() =>
            {
                AdjustWindowHeight();
            });
        });
    }

    /// <summary>
    /// 更新窗体标题和 Tab 名称
    /// </summary>
    private void UpdateFormTitleAndTabs()
    {
        this.Text = LanguageManager.GetString("FormTitle");

        if (tabControl != null && tabControl.TabPages.Count >= 4)
        {
            tabControl.TabPages[(int)Models.TabPage.Profile].Text = LanguageManager.GetString("TabProfile");
            tabControl.TabPages[(int)Models.TabPage.Timer].Text = LanguageManager.GetString("TabTimer");
            tabControl.TabPages[(int)Models.TabPage.Pomodoro].Text = LanguageManager.GetString("TabPomodoro");
            tabControl.TabPages[(int)Models.TabPage.Settings].Text = LanguageManager.GetString("TabSettings");
        }

        // 刷新各个子控件
        _profileManager.RefreshUI();
        _timerControl.RefreshUI();
        _pomodoroControl.RefreshUI();
        _settingsControl.RefreshUI();
    }

    /// <summary>
    /// 显示掉落记录弹窗
    /// </summary>
    private void ShowRecordLootDialog()
    {
        // 这里需要创建 RecordLootForm，它是一个瞬态窗口
        // 使用注入到 MainForm 的 Service 来创建它
        using var lootForm = new UI.Timer.RecordLootForm(_profileService, _timerHistoryService, _sceneService);

        // 订阅保存成功事件，刷新 TimerControl 的显示
        lootForm.LootRecordSaved += (s, e) => _timerControl.RefreshUI();

        lootForm.ShowDialog(this);
    }

    private void AdjustWindowHeight()
    {
        // 1. 只有在 Timer 页面，且用户开启了显示掉落，才使用大高度
        bool showLoot = _appSettings.TimerShowLootDrops;

        int targetHeight;

        if (showLoot)
        {
            targetHeight = UISizeConstants.ClientHeightWithLoot;
        }
        else
        {
            targetHeight = UISizeConstants.ClientHeightWithoutLoot;
        }

        // 2. 执行调整 (避免重复设置导致闪烁)
        if (this.ClientSize.Height != targetHeight)
        {
            this.ClientSize = new Size(UISizeConstants.ClientWidth, targetHeight);

            // 重新应用位置 (防止窗口变大后底部超出屏幕)
            _mainService.ApplyWindowSettings(this);
        }
    }

    /// <summary>
    /// UI 事件：Tab 切换
    /// </summary>
    private void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
    {
        AdjustWindowHeight();
        // 当用户点击 Tab 切换时触发
        if (tabControl.SelectedIndex == (int)Models.TabPage.Timer)
        {
            _timerControl.HandleTabSelected();
        }
        else if (tabControl.SelectedIndex == (int)Models.TabPage.Profile)
        {
            _profileManager.RefreshUI();
        }
    }

    /// <summary>
    /// 拦截系统消息 (用于热键)
    /// </summary>
    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        // 将消息转发给 MainService 处理
        _mainService.ProcessHotKeyMessage(m);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _mainService.HandleApplicationClosing();
        base.OnFormClosing(e);
    }
}
