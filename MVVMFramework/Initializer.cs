using System;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;

namespace MVVMFramework
{
    public static class Initializer
    {
        private static bool AlreadySetUp = false;

        public static void SetupDI(){
            if (AlreadySetUp) return;
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            var container = SimpleIoc.Default;
            container.Register<INavigationService>(() => new NavigationManager());
            container.Register(() =>
                               container.GetInstance<INavigationService>()
                               as INavigationManager);
            container.Register<IUIRunner>(() => new UIRunner());
            AlreadySetUp = true;
        }
        public static T GetDependency<T>()
        {
            return (SimpleIoc.Default.IsRegistered<T>()) ?
                SimpleIoc.Default.GetInstance<T>() :
                         default(T);
        }

        public static void Register<I, T>() where T : class where I : class
        {
            if (!SimpleIoc.Default.IsRegistered<I>())
                SimpleIoc.Default.Register<I, T>();
        }

        public static void Register<T>() where T : class
        {
            if (!SimpleIoc.Default.IsRegistered<T>())
                SimpleIoc.Default.Register<T>();
        }

        public static void RegisterPages(INavigationManager navigation)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation), "Navigation Manager not registered. call SetupDI() before this function");
        }


        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }

    }
}
