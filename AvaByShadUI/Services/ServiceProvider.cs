using System;
using AvaByShadUI.ViewModels;
using Avalonia;
using Jab;
using LogExtension;
using Microsoft.Extensions.Logging;
using ShadUI;

namespace AvaByShadUI.Services;

[ServiceProvider]
[Import<IUtilitiesModule>]
[Singleton<MainWindowViewModel>]
[Singleton<HomeViewModel>]
[Singleton<SettingsViewModel>]
[Singleton(typeof(ThemeWatcher), Factory = nameof(ThemeWatcherFactory))]
[Singleton(typeof(ILogger<>), Factory = nameof(CreateLoggerGeneric))]
[Singleton(typeof(PageManager), Factory = nameof(PageManagerFactory))]
public partial class ServiceProvider : IServiceProvider
{
    // 可替换 ZlogFactory实例，这里使用默认配置
    public ILogger<T> CreateLoggerGeneric<T>() => ZlogFactory.Get<T>();

    public ThemeWatcher ThemeWatcherFactory()
    {
        return new ThemeWatcher(Application.Current!);
    }
    
    public PageManager PageManagerFactory()
    {
        return new PageManager(this);
    }
}
