using TechTalk.SpecFlow;
using Xamarin.UITest;
using Specflow.Server;

namespace Specflow.Steps
{
    public class StepsBase
    {
        protected readonly IApp app;
        protected readonly ScenarioContext Context;

        public StepsBase(ScenarioContext context)
        {
            app = FeatureContext.Current.Get<IApp>("App");
            Context = context;
        }

        public T AddOrGetPageTO<T>() where T : AppPageTO
        {
            if (Context.ContainsKey(typeof(T).ToString()))
                return Context.Get<T>();
            var obj = Activator.CreateInstance(typeof(T), new object[] { app }) as T;
            Context.Set(obj);
            return obj;
        }

        public StepsBase() : this(ScenarioContext.Current)
        {

        }

        [AfterScenario]
        public void StopRestServer()
        {
            if (Context.ContainsKey(typeof(RestTestServer).ToString()))
                Context.Get<RestTestServer>().Stop();
        }
    }

}