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

        public Func<AppQuery, AppQuery> BarcodeButton = c => c.Marked(UIID.BarcodeButton);

        protected override string KeyOfPage => PageKeys.MainPage;

        public override void NavigateFromMain(AppPageTO main)
        {
        }
        public void GoToBarcodePage() => app.Tap(BarcodeButton);
    }
}
