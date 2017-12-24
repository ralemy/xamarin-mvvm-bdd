using System;
using MVVMFramework.Statics;
using Xamarin.UITest;

namespace Specflow.PageTestObjects.Pages
{
    public class SettingsPageTO:AppPageTO
    {
        public SettingsPageTO(IApp app) : base(app)
        {
        }

        protected override string KeyOfPage => PageKeys.SettingsPage;

        public override void NavigateFromMain(AppPageTO main) => ((MainPageTO)main).GotoSettingsPage();
    }
}
