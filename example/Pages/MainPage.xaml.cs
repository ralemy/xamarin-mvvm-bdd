using System;
using System.Collections.Generic;
using example.ViewModels;
using MVVMFramework;
using MVVMFramework.Statics;
using Xamarin.Forms;

namespace example.Pages
{
    public partial class MainPage : ContentPage
    {
        public const string PageKey = PageKeys.MainPage;
        public MainPage()
        {
            InitializeComponent();
            AutomationId = PageKey;
            BindingContext = Initializer.GetDependency<MainPageVM>();
        }
    } 
}
