using System;
using MVVMFramework.Statics;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Specflow.PageTestObjects.Pages
{
    public class SettingsPageTO:AppPageTO
    {
        public SettingsPageTO(IApp app) : base(app)
        {
        }

        protected override string KeyOfPage => PageKeys.SettingsPage;

        public Func<AppQuery, AppQuery> UseHttpsSwitch { get => GetSwitch(UIStrings.UseHttps); }

        public override void NavigateFromMain(AppPageTO main) => ((MainPageTO)main).GotoSettingsPage();
    }
}
