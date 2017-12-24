using System;
namespace MVVMFramework.Statics
{
    public static class Backdoor
    {
        public const string Key = "key";
        public const string Payload = "payload";
        public const string GetUseHttps = "GetUseHttps";
    }
    public static class SettingsDefaults
    {
        public const bool UseHttps = false;
    }
    public static class PageKeys
    {
        public const string MainPage = "MainPage";
        public const string SettingsPage = "SettingsPage";
    }
    public static class UIStrings
    {
        public const string SettingsButton = "Settings";
        public const string UseHttps = "Use Https";
    }
    public static class UIID
    {
        public const string SettingsButton = "SettingsButton";
    }
    public static class Fixtures
    {
        public const string SpecflowBackdoor = "SpecflowBackdoor";
        public const string WasCalled = "WasCalled";
        public const string RestPort = "3434";
    }
}
