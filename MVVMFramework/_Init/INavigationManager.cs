using System;
using Xamarin.Forms;

namespace MVVMFramework
{
    public interface INavigationManager
    {
        object parameter { get; }
        void Register(string key, Type pageClass);
        void SetMain(NavigationPage page);
    }
}
