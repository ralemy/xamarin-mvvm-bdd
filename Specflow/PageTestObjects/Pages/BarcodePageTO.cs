using System;
using MVVMFramework.Statics;
using Xamarin.UITest;

namespace Specflow.PageTestObjects.Pages
{
    public class BarcodePageTO : AppPageTO
    {
        public BarcodePageTO(IApp app):base(app)
        {
        }

        protected override string KeyOfPage => PageKeys.BarcodePage;

        public override void NavigateFromMain(AppPageTO main)
        {
            ((MainPageTO)main).GoToBarcodePage();
        }
    }
}
