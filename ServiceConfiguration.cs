using System;
using DiabloTwoMFTimer.Interfaces;
using DiabloTwoMFTimer.Services;
using DiabloTwoMFTimer.UI;
using Microsoft.Extensions.DependencyInjection;

namespace DiabloTwoMFTimer;

public static class ServiceConfiguration
{
    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // --- 1. 注册基础配置 (Singleton) ---
        services.AddSingleton<IAppSettings>(_ => AppSettings.Load());

        // Repository
        services.AddSingleton<IProfileRepository, Repositories.YamlProfileRepository>();
        services.AddSingleton<IKeyMapRepository, Repositories.KeyMapRepository>();

        // Infrastructure
        services.AddSingleton<IMessenger, Messenger>();
        services.AddSingleton<ISceneService, SceneService>();
        services.AddSingleton<ICommandDispatcher, CommandDispatcher>();

        // --- 2. 注册核心业务服务 (Singleton) ---
        // [新增] 音频服务
        services.AddSingleton<IAudioService, AudioService>();

        services.AddSingleton<IProfileService, ProfileService>();
        services.AddSingleton<ITimerHistoryService, TimerHistoryService>();
        services.AddSingleton<ITimerService, TimerService>();
        services.AddSingleton<IStatisticsService, StatisticsService>();

        // PomodoroService 依赖注入更新 (容器会自动注入 IAudioService)
        services.AddSingleton<IPomodoroTimerService, PomodoroTimerService>();

        services.AddSingleton<IMainService, MainServices>();
        services.AddSingleton<IWindowCMDService, WindowCMDService>();
        services.AddSingleton<IProfileCMDService, ProfileCMDService>();

        // CommandInitializer (Transient, 只在启动时用一次)
        services.AddTransient<CommandInitializer>();

        // --- 3. 注册 UI 组件 (Transient) ---

        // 主窗体及其依赖控件
        services.AddTransient<UI.Profiles.ProfileManager>();
        services.AddTransient<UI.Timer.TimerControl>();
        services.AddTransient<UI.Pomodoro.PomodoroControl>();
        services.AddTransient<UI.Timer.CharacterSceneControl>();
        services.AddTransient<MainForm>();

        // [新增] 设置窗体 (SettingsForm)
        services.AddTransient<UI.Form.SettingsForm>();

        // [新增] 设置子控件 (SettingsForm 内部通过 DI 请求这些控件)
        services.AddTransient<UI.Settings.GeneralSettingsControl>();
        services.AddTransient<UI.Settings.TimerSettingsControl>();
        services.AddTransient<UI.Settings.HotkeySettingsControl>();
        services.AddTransient<UI.Settings.AudioSettingsControl>();

        // (旧的 SettingsControl 如果不再使用可以移除，为了兼容暂时保留)
        services.AddTransient<UI.Settings.SettingsControl>();

        return services.BuildServiceProvider();
    }
}