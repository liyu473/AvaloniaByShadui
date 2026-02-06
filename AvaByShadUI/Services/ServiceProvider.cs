using AvaByShadUI.ViewModels;
using Avalonia;
using Jab;
using LyuLogExtension;
using Microsoft.Extensions.Logging;
using ShadUI;
using System;

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
    // 可替换 ZlogFactory实例，这里使用默认配置
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
