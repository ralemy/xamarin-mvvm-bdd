using System;
using Should;
using Grapevine.Shared;
using Grapevine.Server;
using MVVMFramework.Statics;
using Specflow.PageTestObjects.Pages;
using Specflow.Server;
using TechTalk.SpecFlow;
using Newtonsoft.Json.Linq;

namespace Specflow.Steps.WebCommunications
{
    [Binding]
    public class Scenario01 : StepsBase
    {
        [Given(@"I am running a RestTestServer")]
        public void GivenIAmRunningARestTestServer()
        {
            var RestServer = new RestTestServer(RestCalls.RestPort);
            Context.Set<RestTestServer>(RestServer);
        }

        [Given(@"I have a button on Mainpage to call RestTestServer")]
        public void GivenIHaveAButtonOnMainpageToCallRestTestServer()
        {
            app.WaitForElement(AddOrGetPageTO<MainPageTO>().RestApiButton);
        }
        [Given(@"I have configured the App for RestCall")]
        public void GivenIHaveConfiguredTheAppForRestCall()
        {
            var RestServer = Context.Get<RestTestServer>();
            var endpoint = $"http://{RestServer.GetLocalIP()}:{RestCalls.RestPort}";
            AddOrGetPageTO<MainPageTO>()
                .Invoke(Backdoor.SetRestInfo,endpoint
                       );
            RestServer.LogToConsole().Start();
        }
        [Given(@"I have registered an endpoint with the RestTestServer")]
        public void GivenIHaveRegisteredAnEndpointWithTheRestTestServer()
        {
            Context.Get<RestTestServer>().Register((context) =>
            {
                Context.Add(Fixtures.WasCalled, true);
                Context.Add("QueryString", context.Request.QueryString.GetValue<string>("key1"));
                context.Response.SendResponse("OK");
                return context;
            }, HttpMethod.GET, RestCalls.TestEndPoint);
        }

        [When(@"I tap the button")]
        public void WhenITapTheButton()
        {
            app.Tap(AddOrGetPageTO<MainPageTO>().RestApiButton);
        }

        [Then(@"The RestTestServer is called with correct payload")]
        public void ThenTheRestTestServerIsCalledWithCorrectPayload()
        {
            app.WaitFor(() => Context.ContainsKey(Fixtures.WasCalled),
                       "RestTestServer was not called!");
            Context.Get<string>("QueryString").ShouldEqual("value1");
        }

    }
}
