using System;
using Should;
using Specflow.PageTestObjects.Pages;
using TechTalk.SpecFlow;

namespace Specflow.Steps.SettingsPage
{
    [Binding]
    public class Scenario01 : StepsBase
    {
        [Given(@"I am in Main Page")]
        public void GivenIAmInMainPage()
        {
            var page = AddOrGetPageTO<MainPageTO>();
            app.WaitForElement(page.PageContainer);
        }

        [Given(@"There is a '(.*)' button on the page")]
        public void GivenThereIsAButtonOnThePage(string settings)
        {
            app.Query(c => c.Text(settings)).Length.ShouldEqual(1);
        }

        [When(@"I press the '(.*)' button")]
        public void WhenIPressTheButton(string settings0)
        {
            app.Tap(c => c.Text(settings0));
        }

        [Then(@"the application will Navigate to Settings Page")]
        public void ThenTheApplicationWillNavigateToSettingsPage()
        {
            var page = AddOrGetPageTO<SettingsPageTO>();
            page.WaitForLoad();
        }

        [Then(@"the Settings Page has a UseHttps Switch")]
        public void ThenTheSettingsPageHasAUseHttpsSwitch()
        {
            var page = AddOrGetPageTO<SettingsPageTO>();
            app.WaitForElement(page.UseHttpsSwitch);
        }
    }
}
