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
    private readonly IProfileService _profileService;

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
        IProfileService profileService
    )
    {
        _dispatcher = dispatcher;
        _timerService = timerService;
        _pomodoroTimerService = pomodoroTimerService;
        _mainService = mainService;
        _appSettings = appSettings;
        _messenger = messenger;
        _profileService = profileService;
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
                    Utils.Toast.Info("番茄钟已暂停");
                }
                else
                {
                    _pomodoroTimerService.Start();
                    Utils.Toast.Success("番茄钟已启动");
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
            "Pomodoro.PlusOneMinute",
            (arg) =>
            {
                if (int.TryParse(arg?.ToString(), out int minutes) && minutes > 0 && minutes <= 59)
                {
                    _pomodoroTimerService.AddMinutes(minutes);
                    Utils.Toast.Info($"已增加 {minutes} 分钟");
                }
                else
                {
                    Utils.Toast.Error("请输入 1 - 59 之间的数值");
                }
            }
        );

        _dispatcher.Register(
            "Pomodoro.SwitchToNextState",
            () =>
            {
                _pomodoroTimerService.SwitchToNextState();
                Utils.Toast.Success($"已切换到 {_pomodoroTimerService.CurrentState}");
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

        // --- 系统/导航 ---
        _dispatcher.Register(
            "App.Exit",
            () =>
            {
                _mainService.HandleApplicationClosing();
                Application.Exit();
            }
        );

        // 最小化到托盘
        _dispatcher.Register(
            "App.Minimize",
            () =>
            {
                _messenger.Publish(new MinimizeToTrayMessage());
            }
        );

        // 从托盘恢复 (这个命令通常通过全局 LeaderKey 触发)
        _dispatcher.Register(
            "App.Restore",
            () =>
            {
                _messenger.Publish(new RestoreFromTrayMessage());
            }
        );

        _dispatcher.Register(
            "App.SetPosition",
            (arg) =>
            {
                if (Enum.TryParse(arg?.ToString(), true, out Models.WindowPosition position))
                {
                    _appSettings.WindowPosition = position.ToString();
                    _appSettings.Save();
                    _messenger.Publish(new WindowPositionChangedMessage());
                    Utils.Toast.Success($"已设置位置为 {position}");
                }
                else
                {
                    Utils.Toast.Error("请输入有效的位置");
                }
            }
        );

        _dispatcher.Register(
            "App.SetOpacity",
            (arg) =>
            {
                if (double.TryParse(arg?.ToString(), out double val))
                {
                    // 调用调整透明度的逻辑
                    if (val < 0.1 || val > 1.0)
                    {
                        Utils.Toast.Error("透明度值必须在 0.1-1.0 之间");
                        return;
                    }
                    _appSettings.Opacity = val;
                    _appSettings.Save();
                    _messenger.Publish(new OpacityChangedMessage());
                    Utils.Toast.Success($"已设置透明度为 {val}");
                }
            }
        );
        _dispatcher.Register(
            "App.SetSize",
            (arg) =>
            {
                if (float.TryParse(arg?.ToString(), out float val) && val >= 1.0f && val <= 2.5f)
                {
                    var result = DiabloTwoMFTimer.UI.Components.ThemedMessageBox.Show(
                        "界面缩放设置已保存。需要重启程序才能完全生效。\n\n是否立即重启？",
                        "需要重启",
                        MessageBoxButtons.YesNo
                    ); // 使用 YesNo 按钮
                    _appSettings.UiScale = val;
                    _appSettings.Save();
                    if (result == DialogResult.Yes)
                    {
                        Application.Restart();
                        Application.Exit();
                    }
                }
                else
                {
                    Utils.Toast.Error("请输入 1.0 - 2.5 之间的数值");
                }
            }
        );

        // 导航：切换 Tab
        _dispatcher.Register("Nav.Timer", () => _mainService.SetActiveTabPage(Models.TabPage.Timer));
        _dispatcher.Register("Nav.Profile", () => _mainService.SetActiveTabPage(Models.TabPage.Profile));
        _dispatcher.Register("Nav.Pomodoro", () => _mainService.SetActiveTabPage(Models.TabPage.Pomodoro));
        _dispatcher.Register("Nav.Settings", () => _mainService.SetActiveTabPage(Models.TabPage.Settings));

        // 角色管理命令
        _dispatcher.Register(
            "Character.Create",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Profile);
                _messenger.Publish(new CreateCharacterMessage());
            }
        );

        _dispatcher.Register(
            "Character.Switch",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Profile);
                _messenger.Publish(new SwitchCharacterMessage());
            }
        );

        _dispatcher.Register(
            "Character.Export",
            () =>
            {
                _messenger.Publish(new ExportCharacterMessage());
            }
        );

        _dispatcher.Register(
            "Character.ShowLootHistory",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Profile);
                _messenger.Publish(new ShowLootHistoryMessage());
            }
        );

        _dispatcher.Register(
            "Character.Delete",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Profile);
                _messenger.Publish(new DeleteCharacterMessage());
            }
        );

        // 场景操作
        _dispatcher.Register(
            "Scene.Switch",
            (arg) =>
            {
                string? shortEnName = arg?.ToString();
                if (!string.IsNullOrEmpty(shortEnName))
                {
                    _profileService.SwitchSceneByShortEnName(shortEnName);
                }
                else
                {
                    Utils.Toast.Error($"未输入场景名称");
                }
            }
        );

        // 工具
        _dispatcher.Register(
            "System.Screenshot",
            () => _messenger.Publish(new ScreenshotRequestedMessage("LeaderKeyShot"))
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
                        Utils.Toast.Error("请输入 1-3 之间的数字");
                    }
                }
                else
                {
                    Utils.Toast.Error("请输入有效的数字");
                }
            }
        );
    }
}
