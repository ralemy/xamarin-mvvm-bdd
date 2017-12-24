using System;
using example.Models;
using MVVMFramework.Statics;
using Newtonsoft.Json.Linq;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace example.Helpers
{
    public static class Settings
    {
        public static event EventHandler<SettingsChangedArgs> SettingsChanged;

        private static ISettings AppSettings
        {
            get => CrossSettings.Current;
        }
        public static bool UseHttps
        {
            get => AppSettings.GetValueOrDefault(nameof(UseHttps), SettingsDefaults.UseHttps);
            set
            {
                AppSettings.AddOrUpdateValue(nameof(UseHttps), value);
                SettingsChanged?.Invoke(Settings.AppSettings, new SettingsChangedArgs
                {
                    key = nameof(UseHttps),
                    value = value
                });
            }
        }

    }
    public static class SpecFlowBackdoor
    {
        public static string Execute(JObject msg){
            switch((string)msg[Backdoor.Key]){
                default:
                    return "Unknown Key" + (string)msg[Backdoor.Key];
            }
        }
    }
}
