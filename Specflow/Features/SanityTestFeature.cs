using Xamarin.UITest;

namespace Specflow.Features
{
    public partial class _SanityTestFeature : FeatureBase
    {
        public _SanityTestFeature(Platform p, string iOSSimulator, bool resetSim = true) 
            : base(p,iOSSimulator,resetSim)
        {
        }
    }
}
