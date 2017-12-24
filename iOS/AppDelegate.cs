using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Newtonsoft.Json.Linq;
using UIKit;

namespace example.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            
#if ENABLE_TEST_CLOUD
            global::Xamarin.Calabash.Start();
#endif
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());



            return base.FinishedLaunching(app, options);
        }

        [Export(MVVMFramework.Statics.Fixtures.SpecflowBackdoor + ":")]
        public NSString SpecflowBakckdoor(NSString json){
            return new NSString(Helpers.SpecFlowBackdoor.Execute(JObject.Parse(json)));
        }
    }
}
