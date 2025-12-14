using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models; // 引用 TabPage 枚举

namespace DiabloTwoMFTimer.Services;

public class CommandInitializer
{
    private readonly ICommandDispatcher _dispatcher;
    private readonly ITimerService _timerService;
    private readonly IPomodoroTimerService _pomodoroTimerService;
    private readonly IMainService _mainService;
    private readonly IAppSettings _appSettings;

    // UI 相关的动作通常需要通过 Messenger 或者直接注入 Controller (不太推荐)
    // 这里我们通过 Messenger 发送指令，或者直接调用 Service
    private readonly IMessenger _messenger;

    public CommandInitializer(
        ICommandDispatcher dispatcher,
        ITimerService timerService,
        IPomodoroTimerService pomodoroTimerService,
        IMainService mainService,
        IAppSettings appSettings,
        IMessenger messenger)
    {
        _dispatcher = dispatcher;
        _timerService = timerService;
        _pomodoroTimerService = pomodoroTimerService;
        _mainService = mainService;
        _appSettings = appSettings;
        _messenger = messenger;
    }

    public void Initialize()
    {
        // --- 计时器相关 ---
        _dispatcher.Register("Timer.Start", () =>
        {
            _mainService.SetActiveTabPage(Models.TabPage.Timer);
            _timerService.Start();
        });
        _dispatcher.Register("Timer.Pause", () =>
        {
            _mainService.SetActiveTabPage(Models.TabPage.Timer);
            _timerService.Pause();
        });
        _dispatcher.Register("Timer.Reset", () =>
        {
            _mainService.SetActiveTabPage(Models.TabPage.Timer);
            _timerService.Reset();
        });
        // StartOrNextRun
        _dispatcher.Register("Timer.Next", () =>
        {
            _mainService.SetActiveTabPage(Models.TabPage.Timer);
            _timerService.StartOrNextRun();
        }); // Next 是同步的
        _dispatcher.Register("Timer.Toggle", () =>
        {
            _mainService.SetActiveTabPage(Models.TabPage.Timer);
            _timerService.TogglePause();
        });

        // 1. 切换状态 (核心逻辑：运行则暂停，否则开始)
        _dispatcher.Register("Pomodoro.Toggle", () =>
        {
            if (_pomodoroTimerService.IsRunning)
            {
                _pomodoroTimerService.Pause();
                Utils.Toast.Info("番茄钟已暂停");
            }
            else
            {
                _pomodoroTimerService.Start();
                Utils.Toast.Success("番茄钟已启动");
            }
        });


        // 打开设置 (通过发消息)
        _dispatcher.Register("Pomodoro.ShowSettings", () =>
        {
            _messenger.Publish(new ShowPomodoroSettingsMessage());
        });

        // 打开休息界面 (通过发消息)
        _dispatcher.Register("Pomodoro.ShowBreakForm", () =>
        {
            _messenger.Publish(new ShowPomodoroBreakFormMessage());
        });

        _dispatcher.Register("Pomodoro.PlusOneMinute", () =>
        {
            _pomodoroTimerService.AddMinutes(1);
            Utils.Toast.Info("已增加 1 分钟");
        });

        // --- 记录操作 ---
        // 注意：删除操作涉及到 UI 确认，通常是通过 MainService 发出请求，MainForm 监听并弹窗
        // _dispatcher.Register("Record.DeleteLast", () => _mainService.RequestDeleteHistory());
        // _dispatcher.Register("Loot.Add", () => _mainService.RequestRecordLoot());

        // --- 系统/导航 ---
        _dispatcher.Register("App.Exit", () =>
        {
            _mainService.HandleApplicationClosing();
            Application.Exit();
        });
        // _dispatcher.Register("App.Minimize", () => _mainService.MinimizeToTray());

        // 导航：切换 Tab
        _dispatcher.Register("Nav.Timer", () => _mainService.SetActiveTabPage(Models.TabPage.Timer));
        _dispatcher.Register("Nav.Profile", () => _mainService.SetActiveTabPage(Models.TabPage.Profile));
        _dispatcher.Register("Nav.Pomodoro", () => _mainService.SetActiveTabPage(Models.TabPage.Pomodoro));
        _dispatcher.Register("Nav.Settings", () => _mainService.SetActiveTabPage(Models.TabPage.Settings));



        // 工具
        _dispatcher.Register("System.Screenshot", () => _messenger.Publish(new ScreenshotRequestedMessage("LeaderKeyShot")));
    }
}