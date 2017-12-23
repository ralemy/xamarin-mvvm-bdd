using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Shared;
using MVVMFramework.Statics;
using Should;
using Specflow.Server;
using TechTalk.SpecFlow;

namespace Specflow.Steps
{
    [Binding]
    public class SanityTestSteps
    {

        [Given(@"I store (.*) into the feature context under key '(.*)'")]
        public void GivenIStoreIntoTheFeatureContextUnderKey(int p0, string sanityTest)
        {
            FeatureContext.Current.Add(sanityTest,p0);
        }

        [When(@"I read the key '(.*)' from the context and add (.*) to it and store it in '(.*)'")]
        public void WhenIReadTheKeyFromTheContextAndAddToItAndStoreItIn(string sanityTest0, int p1, string sanityWhen)
        {
            FeatureContext.Current.Add(sanityWhen,FeatureContext.Current.Get<int>(sanityTest0) + p1);
        }

        [Then(@"the '(.*)' key in context should be (.*)\.")]
        public void ThenTheKeyInContextShouldBe_(string sanityWhen0, int p1)
        {
            FeatureContext.Current.Get<int>(sanityWhen0).ShouldEqual(p1);
        }

        [Given(@"I start the RestTestServer on port '(.*)'")]
        public void GivenIStartTheRestTestServerOnPort(int p0)
        {
            var RestServer = new RestTestServer(p0.ToString());
            ScenarioContext.Current.Set(RestServer);
            RestServer.Start();
        }

        [Given(@"I register and endpoint on '(.*)' to return '(.*)'")]
        public void GivenIRegisterAndEndpointOnToReturn(string endpoint, string response)
        {
            var RestServer = ScenarioContext.Current.Get<RestTestServer>();
            RestServer.Register(
                delegate (IHttpContext context)
                {
                    ScenarioContext.Current.Add(Fixtures.WasCalled, true);
                    context.Response.SendResponse(response);
                    return context;
                },
                HttpMethod.GET,
                endpoint
            );        
        }

        [When(@"I call '(.*)'")]
        public void WhenICall(string p0)
        {
            var RestServer = ScenarioContext.Current.Get<RestTestServer>();
            ScenarioContext.Current.Add("returnvalue", RestServer.Get(p0));        
        }

        [Then(@"the response would be '(.*)'")]
        public void ThenTheResponseWouldBe(string p0)
        {
            ScenarioContext.Current.Get<string>("returnvalue").ShouldEqual(p0);
            var RestServer = ScenarioContext.Current.Get<RestTestServer>();
            RestServer.Stop();
            ScenarioContext.Current.ContainsKey(Fixtures.WasCalled).ShouldBeTrue();
        }
    }
}
