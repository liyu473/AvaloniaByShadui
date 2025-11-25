using AvaByShadUI.Services;
using AvaByShadUI.ViewModels;
using AvaByShadUI.Views;
using ShadUI;

namespace AvaByShadUI.Extensions;

public static class RegisterDialog
{
    public static ServiceProvider RegisterDialogs(this ServiceProvider service)
    {
        var dialogService = service.GetService<DialogManager>();

        dialogService.Register<AboutContent, AboutViewModel>();

        return service;
    }
}