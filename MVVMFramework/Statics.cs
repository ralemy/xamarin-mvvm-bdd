using System;
namespace MVVMFramework.Statics
{
    public static class Backdoor
    {
    }

    public static class UIID
    {
        public const string BarcodeButton = "BarcodeButton";
    }
    public static class UIStrings
    {
        public const string BarcodeButton = "Scan Barcode";
        public const string TopTextDefault = "Hold your phone up to the barcode";
        public const string BottomTextDefault = "Scanning will happen automatically";
    }
    public static class PageKeys
    {
        public const string MainPage = "MainPage";
        public const string BarcodePage = "BarcodePage";
    }

    public static class Fixtures{
        public const string SpecflowBackdoor = "SpecflowBackdoor";
        public const string WasCalled = "WasCalled";
        public const string RestPort = "3434";
    }
}
