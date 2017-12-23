using System;
using MVVMFramework.Statics;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Specflow.Server
{
    [TestFixture(Fixtures.RestPort)]
    public class RestServerFeatureBase
    {
        protected string Port;
        public RestTestServer RestServer;

        public RestServerFeatureBase(string port)
        {
            Port = port;
            RestServer = new RestTestServer(Port);
        }

        [TestFixtureSetUp]
        public void RegisterWithFeatureContext()
        {
            RestServer.LogToConsole().Start();
        }
        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            RestServer.Stop();
        }
        [SetUp]
        public void RegisterServerWithContext(){
            FeatureContext.Current.Set<RestTestServer>(RestServer);
        }

        [TearDown]
        public void UnRegisterServerFromContext(){
            FeatureContext.Current.Remove(typeof(RestTestServer).ToString());
        }
    }
}
