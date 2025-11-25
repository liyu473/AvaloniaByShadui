using Jab;
using ShadUI;

namespace AvaByShadUI.Services;

[ServiceProviderModule]
[Singleton<DialogManager>]
[Singleton<ToastManager>]
[Singleton<NavigationService>]
public interface IUtilitiesModule
{
    
}