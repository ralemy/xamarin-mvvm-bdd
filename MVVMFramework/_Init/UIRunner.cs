using System;
using Xamarin.Forms;

namespace MVVMFramework
{
    public class UIRunner:IUIRunner
    {
        public void RunOnUIThread(Action action)
        {
            Device.BeginInvokeOnMainThread(action);
        }

    }
}
