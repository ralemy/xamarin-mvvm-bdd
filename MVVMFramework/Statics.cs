using System;
namespace MVVMFramework.Statics
{
    public static class Backdoor
    {
        public const string Key = "key";
        public const string Payload = "payload";
        public const string SetRestInfo = "SetRestInfo";
    }

    public static class PageKeys
    {
        public const string MainPage = "MainPage";
    }
    public static class UIID {
        public const string RestApiButton = "RestApiButton";
    }
    public static class UIStrings{
        public const string RestApiButton = "Call Rest Api";
    }
    public static class RestCalls{
        public const string ServerUrl = "ServerEndpoint";
        public const string RestPort = "3434";
        public const string TestEndPoint = "/api/testendpoint";
        public const string QueryString = "?key1=value1&key2=value2";
    }
    public static class Fixtures{
        public const string SpecflowBackdoor = "SpecflowBackdoor";
        public const string WasCalled = "WasCalled";
        public const string RestPort = "3434";
    }
}
