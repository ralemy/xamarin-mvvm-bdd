using System;
using Newtonsoft.Json.Linq;
using MVVMFramework.Statics;

namespace example.Helpers
{
    public static class SpecflowBackdoor
    {
        public static string Execute(JObject msg){
            switch((string)msg[Backdoor.Key]){
                case Backdoor.SetRestInfo:
                    Settings.ServerUrl = (string)msg[Backdoor.Payload];
                    break;
                default:
                    return "Unknown Key: " + (string)msg[Backdoor.Key];
            }
            return "OK";
        }
    }
    public static class Settings
    {
        public static string ServerUrl = "";
    }
}
