using System;
using Specflow.PageTestObjects.Pages;
using TechTalk.SpecFlow;

namespace Specflow.Steps.BarcodePage
{
    [Binding]
    public class Scenario01 : StepsBase
    {
        [Given(@"I am in the main page")]
        public void GivenIAmInTheMainPage()
        {
            AddOrGetPageTO<MainPageTO>().WaitForLoad();
        }

        [Given(@"the main page has a barcode button")]
        public void GivenTheMainPageHasABarcodeButton()
        {
            app.WaitForElement(AddOrGetPageTO<MainPageTO>().BarcodeButton);
        }

        [When(@"I tap the barcode button")]
        public void WhenITapTheBarcodeButton()
        {
            app.Tap(AddOrGetPageTO<MainPageTO>().BarcodeButton);
        }

        [Then(@"the application goes to the barcode page")]
        public void ThenTheApplicationGoesToTheBarcodePage()
        {
            AddOrGetPageTO<BarcodePageTO>().WaitForLoad();
        }

    }
}
