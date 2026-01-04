using System;
using System.Windows.Forms;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;

namespace DiabloTwoMFTimer.Services;

public class TimerCMDService : ITimerCMDService
{
    private readonly IMainService _mainService;
    private readonly ITimerService _timerService;
    private readonly ICommandDispatcher _dispatcher;

    public TimerCMDService(
        IMainService mainService,
        ITimerService timerService,
        ICommandDispatcher dispatcher
    )
    {
        _mainService = mainService;
        _timerService = timerService;
        _dispatcher = dispatcher;
    }

    public void Initialize()
    {
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
        _dispatcher.Register(
            "Timer.ResetAndStart",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Timer);
                _timerService.Reset();
                _timerService.Start();
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
        );
        _dispatcher.Register(
            "Timer.Toggle",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Timer);
                _timerService.TogglePause();
            }
        );
        _dispatcher.Register(
            "Timer.Action",
            () =>
            {
                _mainService.SetActiveTabPage(Models.TabPage.Timer);
                if (_timerService.IsRunning)
                {
                    _timerService.CompleteRun(false);
                    _timerService.Reset();
                }
                else if (_timerService.IsPaused)
                {
                    _timerService.Resume();
                }
                else if (_timerService.IsStopped)
                {
                    _timerService.Start();
                }
            }
        );

    }
}