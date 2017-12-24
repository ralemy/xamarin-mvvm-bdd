using System;
using MVVMFramework.Statics;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Specflow.PageTestObjects.Pages
{
    public class MainPageTO : AppPageTO
    {
        public MainPageTO(IApp app) : base(app)
        {
        }
        public Func<AppQuery, AppQuery> SettingsButton = c => c.Marked(UIID.SettingsButton);
        protected override string KeyOfPage => PageKeys.MainPage;

        public override void NavigateFromMain(AppPageTO main)
        {
        }
        public void GotoSettingsPage() => app.Tap(SettingsButton);
    }
}
