using System;
using System.Linq;
using MVVMFramework.Statics;
using Newtonsoft.Json.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Specflow.PageTestObjects
{
    public abstract class AppPageTO : IAppPageTO
    {
        protected IApp app;

        protected abstract string KeyOfPage { get; }

        public Func<AppQuery, AppQuery> PageContainer => c => c.Marked(PageKey);

        public string PageKey => KeyOfPage;

        public AppPageTO(IApp app) => this.app = app;

        public bool IsAndroidApp() => app is Xamarin.UITest.Android.AndroidApp;

        public void WaitForLoad() => app.WaitForElement(PageContainer);
        public abstract void NavigateFromMain(AppPageTO main);

        public string GetButtonText(Func<AppQuery, AppQuery> element) => IsAndroidApp()
                ? app.Query(element).First().Text : app.Query(element).First().Label;


        public Func<AppQuery, AppQuery> GetSwitch(string label)
        {
            if (IsAndroidApp())
                return c => c.Text(label).Parent(1).Child(1);
            return c => c.Marked(label).Index(1);
        }

        public string Invoke(string methodName, JObject json) =>
        (IsAndroidApp()
                 ? app.Invoke(methodName, json.ToString()).ToString()
                 : app.Invoke(methodName + ":", json.ToString())).ToString();

        public string Invoke(string key){
            JObject msg = new JObject(new JProperty("key", key));
            return Invoke(msg);
        }

        public string Invoke(string key, string payload)
        {
            JObject msg = new JObject(new JProperty("key", key),
                                      new JProperty("payload",payload));
            return Invoke(msg);
        }

        public string Invoke(JObject json) => Invoke(Fixtures.SpecflowBackdoor, json);
    }
}
