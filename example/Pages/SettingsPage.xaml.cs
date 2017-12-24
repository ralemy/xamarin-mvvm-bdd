using System;
using System.Collections.Generic;
using example.ViewModels;
using MVVMFramework;
using MVVMFramework.Statics;
using Xamarin.Forms;

namespace example.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public static string PageKey = PageKeys.SettingsPage;
        public SettingsPage()
        {
            InitializeComponent();
            AutomationId = PageKey;
            BindingContext = Initializer.GetDependency<SettingsPageVM>();
        }
    }
}
