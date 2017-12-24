using System;
using System.Windows.Input;
using example.Pages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

namespace example.ViewModels
{
    public class MainPageVM : ViewModelBase
    {
        private readonly INavigationService Navigator;

        public ICommand SettingsCommand { get; private set; }
        public MainPageVM(INavigationService nav)
        {
            Navigator = nav;
            SettingsCommand = new RelayCommand(()=>Navigator.NavigateTo(SettingsPage.PageKey));
        }
    }
}
