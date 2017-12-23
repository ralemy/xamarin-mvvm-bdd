using System;
namespace MVVMFramework
{
    public interface IUIRunner
    {
        void RunOnUIThread(Action action);
    }
}
