using System;
using example.Helpers;
using GalaSoft.MvvmLight;
using MVVMFramework;

namespace example.ViewModels
{
    public class SettingsPageVM : ViewModelBase
    {
        INavigationManager Navigator;

        bool _useHttps = Settings.UseHttps;
        public bool UseHttps
        {
            get => _useHttps;
            set
            {
                if (_useHttps == value) return;
                _useHttps = value;
                Settings.UseHttps = value;
                RaisePropertyChanged(nameof(UseHttps));
            }
        }

        public SettingsPageVM(INavigationManager navigator) : base()
        {
            Navigator = navigator;
            Settings.SettingsChanged += (sender, e) =>
            {
                var property = this.GetType().GetProperty(e.key);
                if (property?.GetValue(this, null) != e.value)
                    property?.SetValue(this, e.value);
            };
        }
    }
}
