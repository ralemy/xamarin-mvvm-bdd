using System;
using Newtonsoft.Json.Linq;
using Xamarin.UITest.Queries;

namespace Specflow.PageTestObjects
{
    public interface IAppPageTO
    {
        string PageKey { get; }   //Get the key to the page
        bool IsAndroidApp();      //True if running on Android

        void WaitForLoad();
        void NavigateFromMain(AppPageTO main);

        Func<AppQuery, AppQuery> PageContainer { get; }  
        Func<AppQuery, AppQuery> GetSwitch(string label); 
        string GetButtonText(Func<AppQuery,AppQuery> element); 

        //Invoking Backdoor:
        string Invoke(string methodName, JObject json);
        string Invoke(string key);
        string Invoke(string key, string payload);
        string Invoke(JObject json);
    }
}
