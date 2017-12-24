using System;
using Should;
using MVVMFramework.Statics;
using Specflow.PageTestObjects.Pages;
using TechTalk.SpecFlow;

namespace Specflow.Steps.SettingsPage
{
    [Binding]
    public class Scenario02 : StepsBase
    {
        [Given(@"I am in Settings page")]
        public void GivenIAmInSettingsPage()
        {
            AddOrGetPageTO<MainPageTO>().GotoSettingsPage();
            AddOrGetPageTO<SettingsPageTO>().WaitForLoad();
        }

        [Given(@"the settings page has a switch to select Https protocol")]
        public void GivenTheSettingsPageHasASwitchToSelectHttpsProtocol()
        {
            app.WaitForElement(AddOrGetPageTO<SettingsPageTO>().UseHttpsSwitch);
        }


        [Given(@"I record the value of Settings\.UseHttps")]
        public void GivenIRecordTheValueOfSettings_UseHttps()
        {
            var origin = AddOrGetPageTO<SettingsPageTO>().Invoke(Backdoor.GetUseHttps);
            Context.Add("originalUseHttps", Convert.ToBoolean(origin));
        }

        [When(@"I tap the Use Https switch")]
        public void WhenITapTheUseHttpsSwitch()
        {
            app.Tap(AddOrGetPageTO<SettingsPageTO>().UseHttpsSwitch);
        }

        [Then(@"the Value of the Settings\.UseHttps will toggle")]
        public void ThenTheValueOfTheSettings_UseHttpsWillToggle()
        {
            var page = AddOrGetPageTO<SettingsPageTO>();
            var expected = !Context.Get<bool>("originalUseHttps");
            app.WaitFor(() =>
                        Convert.ToBoolean(page.Invoke(Backdoor.GetUseHttps)) == expected,
                        "Use Https Value did not change");
        }
    }
}
