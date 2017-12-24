using System;
namespace MVVMFramework.Statics
{
    public static class Backdoor
    {
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
