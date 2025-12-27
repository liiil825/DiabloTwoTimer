using System;
using System.Collections.Generic;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Models;

namespace DiabloTwoMFTimer.Services;

public class ProfileCMDService : IProfileCMDService
{
    private readonly IMainService _mainService;
    private readonly IMessenger _messenger;
    private readonly ICommandDispatcher _dispatcher;
    private readonly IProfileService _profileService;
    private readonly ITimerService _timerService;
    private readonly ISceneService _sceneService;

    public ProfileCMDService(
        IMainService mainService,
        IMessenger messenger,
        ICommandDispatcher dispatcher,
        IProfileService profileService,
        ITimerService timerService,
        ISceneService sceneService
    )
    {
        _mainService = mainService;
        _messenger = messenger;
        _dispatcher = dispatcher;
        _profileService = profileService;
        _timerService = timerService;
        _sceneService = sceneService;
    }

    public void Initialize()
    {
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
            "Loot.ShowHistory",
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
                    if (!_timerService.IsStopped)
                    {
                        _timerService.TogglePause();
                    }
                }
                else
                {
                    Utils.Toast.Error(Utils.LanguageManager.GetString("SceneNameNotProvided"));
                }
            }
        );

        RegisterSceneSwitchCommand();
    }

    private void RegisterSceneSwitchCommand()
    {
        // 根据 FarmingScenes 数组，给每个对象注册 Scene.Switch.{shortEnName} 命令
        foreach (var scene in _sceneService.FarmingScenes)
        {
            if (!string.IsNullOrEmpty(scene.ShortEnName))
            {
                string commandName = $"Scene.Switch.{scene.ShortEnName}";
                string shortEnName = scene.ShortEnName;

                _dispatcher.Register(
                    commandName,
                    () =>
                    {
                        _profileService.SwitchSceneByShortEnName(shortEnName);
                        if (!_timerService.IsStopped)
                        {
                            _timerService.TogglePause();
                        }
                    }
                );
            }
        }
    }
}
