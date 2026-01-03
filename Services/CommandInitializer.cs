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
    private readonly IProfileService _profileService;
    private readonly IWindowCMDService _windowCMDService;
    private readonly IProfileCMDService _profileCMDService;

    // UI 相关的动作通常需要通过 Messenger 或者直接注入 Controller (不太推荐)
    // 这里我们通过 Messenger 发送指令，或者直接调用 Service
    private readonly IMessenger _messenger;

    public CommandInitializer(
        ICommandDispatcher dispatcher,
        ITimerService timerService,
        IPomodoroTimerService pomodoroTimerService,
        IMainService mainService,
        IAppSettings appSettings,
        IMessenger messenger,
        IProfileService profileService,
        IWindowCMDService windowCMDService,
        IProfileCMDService profileCMDService
    )
    {
        _dispatcher = dispatcher;
        _timerService = timerService;
        _pomodoroTimerService = pomodoroTimerService;
        _mainService = mainService;
        _appSettings = appSettings;
        _messenger = messenger;
        _profileService = profileService;
        _windowCMDService = windowCMDService;
        _profileCMDService = profileCMDService;
    }

    public void Initialize()
    {
        // --- 计时器相关 ---
        _dispatcher.Register(
            "Timer.Start",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Timer);
                _timerService.Start();
            }
        );
        _dispatcher.Register(
            "Timer.Pause",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Timer);
                _timerService.Pause();
            }
        );
        _dispatcher.Register(
            "Timer.Reset",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Timer);
                _timerService.Reset();
            }
        );
        // StartOrNextRun
        _dispatcher.Register(
            "Timer.Next",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Timer);
                _timerService.StartOrNextRun();
            }
        ); // Next 是同步的
        _dispatcher.Register(
            "Timer.Toggle",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Timer);
                _timerService.TogglePause();
            }
        );

        // 1. 切换状态 (核心逻辑：运行则暂停，否则开始)
        _dispatcher.Register(
            "Pomodoro.Toggle",
            () =>
            {
                if (_pomodoroTimerService.IsRunning)
                {
                    _pomodoroTimerService.Pause();
                }
                else
                {
                    _pomodoroTimerService.Start();
                }
            }
        );

        // 打开设置 (通过发消息)
        _dispatcher.Register(
            "Pomodoro.ShowSettings",
            () =>
            {
                _messenger.Publish(new ShowPomodoroSettingsMessage());
            }
        );

        _dispatcher.Register(
            "Pomodoro.ShowBreakForm",
            () =>
            {
                _messenger.Publish(new ShowPomodoroBreakFormMessage());
            }
        );

        _dispatcher.Register(
            "Pomodoro.AddMinutes",
            (arg) =>
            {
                if (int.TryParse(arg?.ToString(), out int minutes) && minutes > 0 && minutes <= 59)
                {
                    _pomodoroTimerService.AddMinutes(minutes);
                    Utils.Toast.Info(Utils.LanguageManager.GetString("PomodoroAddedMinutes", minutes));
                }
                else
                {
                    Utils.Toast.Error(Utils.LanguageManager.GetString("PomodoroInvalidMinutesRange"));
                }
            }
        );

        _dispatcher.Register(
            "Pomodoro.SwitchToNextState",
            () =>
            {
                _pomodoroTimerService.SwitchToNextState();
                Utils.Toast.Success(
                    Utils.LanguageManager.GetString("PomodoroStateSwitched", _pomodoroTimerService.CurrentState)
                );
            }
        );

        // --- 记录操作 ---
        // 删除选中的记录
        _dispatcher.Register(
            "Record.DeleteSelected",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Timer);
                _mainService.RequestDeleteSelectedRecord();
            }
        );

        // 删除最后一个时间记录
        _dispatcher.Register(
            "Record.DeleteLastHistory",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Timer);
                _mainService.RequestDeleteLastHistory();
            }
        );

        // 删除最后一个掉落记录
        _dispatcher.Register(
            "Record.DeleteLastLoot",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Timer);
                _mainService.RequestDeleteLastLoot();
            }
        );

        _dispatcher.Register(
            "Loot.Add",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Timer);
                _messenger.Publish(new ShowRecordLootFormMessage());
            }
        );

        _dispatcher.Register(
            "Loot.ToggleVisibility",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Timer);
                _messenger.Publish(new ToggleLootVisibilityMessage());
            }
        );

        // 导航：切换 Tab
        _dispatcher.Register("Nav.Timer", () => _mainService.SetActiveTabPage(Models.TabPage.Timer));
        _dispatcher.Register("Nav.Profile", () => _mainService.SetActiveTabPage(Models.TabPage.Profile));
        _dispatcher.Register("Nav.Pomodoro", () => _mainService.SetActiveTabPage(Models.TabPage.Pomodoro));
        _dispatcher.Register("Nav.Settings", () => _mainService.RequestShowSettings());

        // 工具
        _dispatcher.Register(
            "System.Screenshot",
            () => _messenger.Publish(new ScreenshotRequestedMessage("LeaderKeyShot"))
        );

        // 切换导航栏显示/隐藏
        _dispatcher.Register(
            "App.ToggleNavigation",
            () =>
            {
                _appSettings.ShowNavigation = !_appSettings.ShowNavigation;
                _appSettings.Save();
                _messenger.Publish(new NavigationVisibilityChangedMessage());
                Utils.Toast.Success(
                    Utils.LanguageManager.GetString(
                        _appSettings.ShowNavigation ? "NavigationShown" : "NavigationHidden"
                    )
                );
            }
        );

        // 修改番茄钟模式
        _dispatcher.Register(
            "Pomodoro.SetMode",
            (arg) =>
            {
                if (int.TryParse(arg?.ToString(), out int modeNumber))
                {
                    // 将用户输入的数字 (1-3) 转换为对应的枚举值 (0-2)
                    int enumValue = modeNumber - 1;
                    if (enumValue >= 0 && enumValue <= 2)
                    {
                        var newMode = (Models.PomodoroMode)enumValue;
                        _mainService.SetPomodoroMode(newMode);
                    }
                    else
                    {
                        Utils.Toast.Error(Utils.LanguageManager.GetString("PomodoroInvalidModeNumber"));
                    }
                }
                else
                {
                    Utils.Toast.Error(Utils.LanguageManager.GetString("PomodoroInvalidNumber"));
                }
            }
        );
        _dispatcher.Register(
            "Pomodoro.SetMode.Automatic",
            () =>
            {
                _mainService.SetPomodoroMode(Models.PomodoroMode.Automatic);
            }
        );
        _dispatcher.Register(
            "Pomodoro.SetMode.SemiAuto",
            () =>
            {
                _mainService.SetPomodoroMode(Models.PomodoroMode.SemiAuto);
            }
        );
        _dispatcher.Register(
            "Pomodoro.SetMode.Manual",
            () =>
            {
                _mainService.SetPomodoroMode(Models.PomodoroMode.Manual);
            }
        );

        _dispatcher.Register(
            "App.ShowSettings",
            () =>
            {
                _mainService.RequestShowSettings();
            }
        );

        _windowCMDService.Initialize();
        _profileCMDService.Initialize();
    }
}
