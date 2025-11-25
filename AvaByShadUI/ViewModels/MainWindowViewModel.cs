using System;
using System.Reflection;
using AvaByShadUI.Model;
using AvaByShadUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Extensions;
using Microsoft.Extensions.Logging;
using ShadUI;
using ZLogger;

namespace AvaByShadUI.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial DialogManager DialogManager { get; set; }

    [ObservableProperty]
    public partial ToastManager ToastManager { get; set; }

    [ObservableProperty]
    public partial ThemeWatcher ThemeWatcher { get; set; }

    [ObservableProperty]
    public partial ThemeMode CurrentTheme { get; set; } = ThemeMode.System;

    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly NavigationService _navigationService;

    public MainWindowViewModel(
        DialogManager dialogManager,
        ToastManager toastManager,
        ThemeWatcher themeWatcher,
        PageManager pageManager,
        NavigationService navigationService,
        ILogger<MainWindowViewModel> logger
    )
    {
        DialogManager = dialogManager;
        ToastManager = toastManager;
        ThemeWatcher = themeWatcher;
        _logger = logger;

        _navigationService = navigationService;
        pageManager.OnNavigate = SwitchPage;
    }
    
    private void SwitchPage(INavigable page, string route = "")
    {
        if (!route.IsNullOrEmpty())
        {
            var vm = _navigationService.GetViewModel(route);
            SelectedPage = vm;
        }
        else
        {
            SelectedPage = page;
        }
    }

    [ObservableProperty]
    public partial object? SelectedPage { get; set; }

    /// <summary>
    /// 当前激活的菜单项路由
    /// </summary>
    [ObservableProperty]
    public partial string CurrentRoute { get; set; } = "home";
    
    [RelayCommand]
    private void SwitchPage(string route)
    {
        SwitchPage(null!, route);
    }

    [RelayCommand]
    private void SwitchTheme(ThemeMode themeMode)
    {
        CurrentTheme = themeMode switch
        {
            ThemeMode.System => ThemeMode.Light,
            ThemeMode.Light => ThemeMode.Dark,
            _ => ThemeMode.System,
        };

        ThemeWatcher.SwitchTheme(CurrentTheme);
        _logger.ZLogInformation($"切换主题 {CurrentTheme}");
    }

    public override void Dispose()
    {
        base.Dispose();

        if (SelectedPage is IDisposable disposableCurrent)
        {
            disposableCurrent.Dispose();
        }

        GC.SuppressFinalize(this);
    }

    ~MainWindowViewModel()
    {
        Dispose();
    }
}
