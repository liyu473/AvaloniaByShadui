using System;
using AvaByShadUI.ViewModels;
using Avalonia;
using Jab;
using LyuLogExtension;
using LyuLogExtension.Builder;
using Microsoft.Extensions.Logging;
using ShadUI;
using ZLogger.Providers;

namespace AvaByShadUI.Services;

[ServiceProvider]
[Import<IUtilitiesModule>]
[Singleton<MainWindowViewModel>]
[Singleton<HomeViewModel>]
[Singleton<SettingsViewModel>]
[Transient<AboutViewModel>]
[Singleton(typeof(ThemeWatcher), Factory = nameof(ThemeWatcherFactory))]
[Singleton(typeof(ILogger<>), Factory = nameof(CreateLoggerGeneric))]
[Singleton(typeof(PageManager), Factory = nameof(PageManagerFactory))]
public partial class ServiceProvider : IServiceProvider
{
    public ServiceProvider()
    {
        ZLogFactory.Configure(builder =>
            builder
                .WithRetentionDays(30)
                .WithCleanupInterval(TimeSpan.FromDays(1))
                .AddFileOutput("logs/trace/", LogLevel.Trace)
                .AddFileOutput("logs/info/", LogLevel.Information)
                .FilterSystem()
                .FilterMicrosoft()
                .WithRollingInterval(RollingInterval.Day) // 按天滚动
                .WithRollingSizeKB(5 * 1024)
        );
    }

    public ILogger<T> CreateLoggerGeneric<T>() => ZLogFactory.Get<T>();

    public ThemeWatcher ThemeWatcherFactory()
    {
        return new ThemeWatcher(Application.Current!);
    }

    public PageManager PageManagerFactory()
    {
        return new PageManager(this);
    }
}
