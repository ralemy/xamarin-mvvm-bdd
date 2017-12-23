using NUnit.Framework;
using Xamarin.UITest;
using TechTalk.SpecFlow;
using Specflow;
using Specflow.PageTestObjects;

namespace Specflow.Features
{
    [TestFixture(Platform.Android, "", true)]
    [TestFixture(Platform.iOS, "iPhone 7 (11.2)", false)]
    //[TestFixture(Platform.iOS,"iPad Air (11.2)", false)]
    public class FeatureBase
    {
        protected static IApp app;
        protected Platform platform;
        protected string iOSSimulator;
        protected bool resetDevice;

        public FeatureBase(Platform platform, string iOSSimulator, bool resetDevice = false)
        {
            this.iOSSimulator = iOSSimulator;
            this.platform = platform;
            this.resetDevice = resetDevice;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform, iOSSimulator, resetDevice);
            FeatureContext.Current.Add("App", app);
        }
        
        [TearDown]
        public void AfterEachTest()
        {
            FeatureContext.Current.Clear();
        }
    }

}