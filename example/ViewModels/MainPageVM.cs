using System;
using System.Windows.Input;
using example.Pages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;

namespace example.ViewModels
{
    public class MainPageVM :ViewModelBase
    {
        private INavigationService Navigator;

        public ICommand BarcodeCommand { get; private set; }
        public MainPageVM(INavigationService n)
        {
            Navigator = n;
            BarcodeCommand = new RelayCommand(
                () => Navigator.NavigateTo(BarcodePage.PageKey));
        }
    }
}
