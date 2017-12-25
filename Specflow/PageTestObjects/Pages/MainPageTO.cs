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

        protected override string KeyOfPage => PageKeys.MainPage;

        public Func<AppQuery, AppQuery> RestApiButton = c => c.Marked(UIID.RestApiButton);
        public override void NavigateFromMain(AppPageTO main)
        {
        }
    }
}
