using System;
using MVVMFramework.Statics;
using Xamarin.UITest;

namespace Specflow.PageTestObjects.Pages
{
    public class MainPageTO : AppPageTO
    {
        public MainPageTO(IApp app) : base(app)
        {
        }

        protected override string KeyOfPage => PageKeys.MainPage;

        public override void NavigateFromMain(AppPageTO main)
        {
        }
    }
}
