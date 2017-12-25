using System;
using System.Windows.Input;
using example.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MVVMFramework;

namespace example.ViewModels
{
    public class MainPageVM : ViewModelBase
    {
        private INavigationService Navigator;

        public ICommand RestApiCommand { get; private set; }
        public MainPageVM(INavigationService navigator)
        {
            Navigator = navigator;
            RestApiCommand = new RelayCommand(CallRestApi);
        }
        public async void CallRestApi()
        {
            await Initializer.GetDependency<RestService>().Call();
        }
    }
}
