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

        // 1. 注册基础配置 (单例)
        services.AddSingleton<IAppSettings>(_ => AppSettings.Load());
        // 注册 Repository (必须是 Singleton 以保持缓存状态)
        services.AddSingleton<IProfileRepository, Repositories.YamlProfileRepository>();
        services.AddSingleton<IMessenger, Messenger>();
        services.AddSingleton<ISceneService, SceneService>();

        // 2. 注册核心业务服务 (单例)
        services.AddSingleton<IProfileService, ProfileService>();
        services.AddSingleton<ITimerHistoryService, TimerHistoryService>();
        services.AddSingleton<ITimerService, TimerService>();
        services.AddSingleton<IStatisticsService, StatisticsService>();
        services.AddSingleton<IPomodoroTimerService, PomodoroTimerService>();

        // MainServices 现在是纯逻辑协调者
        services.AddSingleton<IMainService, MainServices>();

        // 3. 注册 UI 组件 (瞬态)
        // MainForm 依赖以下控件，容器会自动注入它们
        services.AddTransient<UI.Profiles.ProfileManager>();
        services.AddTransient<UI.Timer.TimerControl>();
        services.AddTransient<UI.Pomodoro.PomodoroControl>();
        services.AddTransient<UI.Settings.SettingsControl>(); // 新增：之前是手动new的，现在交给DI

        // 注册 TimerControl 内部可能用到的子控件 (如果有需要注入逻辑的话，保留是个好习惯)
        services.AddTransient<UI.Timer.CharacterSceneControl>();

        // 注册主窗体
        services.AddTransient<MainForm>();

        return services.BuildServiceProvider();
    }
}
