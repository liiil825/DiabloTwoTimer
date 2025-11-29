using System;
using System.Drawing;
using System.Windows.Forms;
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
    private readonly IMainServices _mainServices;
    private readonly IAppSettings _settings;

    // 为了创建 RecordLootForm，我们需要这两个服务 (或者你可以选择把弹窗逻辑封装进 TimerControl)
    private readonly IProfileService _profileService;
    private readonly ITimerHistoryService _timerHistoryService;

    // 子控件引用 (用于后续的方法调用)
    private readonly ProfileManager _profileManager;
    private readonly TimerControl _timerControl;
    private readonly PomodoroControl _pomodoroControl;
    private readonly SettingsControl _settingsControl;

    // 构造函数：通过依赖注入获取所有需要的组件
    public MainForm(
        IMainServices mainServices,
        IAppSettings settings,
        IProfileService profileService,
        ITimerHistoryService timerHistoryService,
        ProfileManager profileManager,
        TimerControl timerControl,
        PomodoroControl pomodoroControl,
        SettingsControl settingsControl
    )
    {
        _mainServices = mainServices;
        _settings = settings;
        _profileService = profileService;
        _timerHistoryService = timerHistoryService;

        _profileManager = profileManager;
        _timerControl = timerControl;
        _pomodoroControl = pomodoroControl;
        _settingsControl = settingsControl;

        InitializeComponent(); // 初始化设计器生成的控件 (TabControl 等)

        // 组装 UI：将注入的控件添加到 TabPage 中
        InitializeChildControls();

        // 初始化窗体属性
        InitializeForm();

        // 订阅 MainService 的事件
        SubscribeToEvents();

        SubscribeToChildControlEvents();

        // 注册窗体显示事件
        this.Shown += OnMainForm_Shown;
    }

    // UI 控件暴露 (虽然重构后应尽量减少直接访问，但保留以防万一)
    internal TabControl? TabControl => tabControl;

    private void OnMainForm_Shown(object? sender, EventArgs e)
    {
        // 关键：将窗口句柄传给 Service 用于注册热键
        _mainServices.InitializeApplication(this.Handle);

        // 应用窗口位置设置
        _mainServices.ApplyWindowSettings(this);
    }

    private void InitializeChildControls()
    {
        // 辅助方法：设置 Dock 并添加到 TabPage
        void AddControlToTab(TabPage page, Control control)
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
        var width = UISizeConstants.ClientWidth;
        var height = _settings.TimerShowLootDrops
            ? UISizeConstants.ClientHeightWithLoot
            : UISizeConstants.ClientHeightWithoutLoot;

        this.Size = new Size(width, height);
        this.StartPosition = FormStartPosition.Manual;
        this.ShowInTaskbar = true;
        this.Visible = true;
    }

    private void SubscribeToEvents()
    {
        // 1. 响应 Tab 切换请求
        _mainServices.OnRequestTabChange += (tabPage) =>
        {
            if (tabControl != null && (int)tabPage < tabControl.TabCount)
            {
                SafeInvoke(() => tabControl.SelectedIndex = (int)tabPage);
            }
        };

        // 2. 响应 UI 刷新请求 (例如切换语言后)
        _mainServices.OnRequestRefreshUI += () =>
        {
            SafeInvoke(UpdateFormTitleAndTabs);
        };

        // 3. 响应删除历史记录请求 (热键触发)
        _mainServices.OnRequestDeleteHistory += () =>
        {
            SafeInvoke(() =>
            {
                // 只有当前在 Timer 页时才响应删除
                if (tabControl.SelectedIndex == (int)Models.TabPage.Timer)
                {
                    _ = _timerControl.DeleteSelectedRecordAsync();
                }
            });
        };

        // 4. 响应记录战利品请求 (热键触发)
        _mainServices.OnRequestRecordLoot += () =>
        {
            SafeInvoke(ShowRecordLootDialog);
        };
    }

    private void SubscribeToChildControlEvents()
    {
        // -----------------------------------------------------------
        // 修复问题 1 & 2：监听设置界面的事件并作出响应
        // -----------------------------------------------------------

        // 1. 监听快捷键变更事件
        _settingsControl.HotkeysChanged += (s, e) =>
        {
            // 通知 MainServices 重新注册热键
            _mainServices.ReloadHotkeys();
        };

        // 2. 监听计时器设置变更 (显示/隐藏番茄钟或掉落列表)
        _settingsControl.TimerSettingsChanged += (s, e) =>
        {
            // 2.1 更新 TimerControl 的 UI (比如显示/隐藏 Loot 面板)
            // 注意：e.ShowLootDrops 包含了最新的设置
            _timerControl.SetLootRecordsVisible(e.ShowLootDrops);

            // 2.2 更新 UI 刷新
            UpdateFormTitleAndTabs();

            // 2.3 【关键】修复“原来逻辑是切换到 Tag 页面”
            // 调用 Service 请求切换 Tab，Service 会触发事件，MainForm 再响应切换
            _mainServices.SetActiveTabPage(Models.TabPage.Timer);
        };

        // 3. 监听窗口位置变更
        _settingsControl.WindowPositionChanged += (s, e) =>
        {
            // 立即应用新位置
            _mainServices.ApplyWindowSettings(this);
        };

        // 4. 监听语言变更
        _settingsControl.LanguageChanged += (s, e) =>
        {
            // 刷新整个 UI
            UpdateFormTitleAndTabs();
        };

        // 5. 监听始终置顶变更
        _settingsControl.AlwaysOnTopChanged += (s, e) =>
        {
            this.TopMost = e.IsAlwaysOnTop;
        };
    }

    // 线程安全的 UI 调用辅助方法
    private void SafeInvoke(Action action)
    {
        if (this.IsDisposed || !this.IsHandleCreated) return;

        if (this.InvokeRequired)
        {
            this.Invoke(action);
        }
        else
        {
            action();
        }
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
        using var lootForm = new UI.Timer.RecordLootForm(_profileService, _timerHistoryService);

        // 订阅保存成功事件，刷新 TimerControl 的显示
        lootForm.LootRecordSaved += (s, e) => _timerControl.RefreshUI();

        lootForm.ShowDialog(this);
    }

    /// <summary>
    /// UI 事件：Tab 切换
    /// </summary>
    private void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
    {
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
        // 将消息转发给 MainServices 处理
        _mainServices.ProcessHotKeyMessage(m);
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _mainServices.HandleApplicationClosing();
        base.OnFormClosing(e);
    }
}