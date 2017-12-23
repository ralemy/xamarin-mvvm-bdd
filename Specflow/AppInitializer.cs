using Xamarin.UITest;
using System.Diagnostics;
using System;
using System.IO;
using System.Reflection;

namespace Specflow
{
    public static class AppInitializer
    {
        public static IApp StartApp(Platform platform, string iOSSimulator, bool resetDevice)
        {
            // TODO: If the iOS or Android app being tested is included in the solution 
            // then open the Unit Tests window, right click Test Apps, select Add App Project
            // and select the app projects that should be tested.
            if (platform == Platform.Android)
            {

                if (resetDevice)
                {
                    ResetEmulator();
                }

                return ConfigureApp
                    .Android
                    .EnableLocalScreenshots()
                    .StartApp();

            }
            else if (platform == Platform.iOS)
            {
                string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string binariesFolder = Path.Combine(assemblyFolder, "..", "..", "..", "Binaries");

                if (resetDevice)
                {
                    ResetSimulator(iOSSimulator);
                }
                if (iOSSimulator == "")
                    return ConfigureApp
                        .iOS
                        .InstalledApp("com.rfidpoc.barcode").StartApp();
                else
                    return ConfigureApp
                        .iOS
                        .PreferIdeSettings()
                        .EnableLocalScreenshots()
                        .DeviceIdentifier(iOSSimulator)
                        .StartApp();
            }

            throw new ArgumentException("Unsupported platform");
        }

        public static void ResetEmulator()
        {
            //TODO: This needs to be implemented based on operating system, but for now can safely remain commented out.
            //TODO : For example, on Mac osx this needs a symlink like sudo ln -s /Users/ralemy/Library/Developer/Xamarin/android-sdk-macosx/platform-tools/adb /opt/adb
            //TODO: then, add a static const AndroidBundleName to Statics collection "com.examples.xamarn.xamarin"
            //if (TestEnvironment.Platform.Equals(TestPlatform.Local))
            //{
            //    var eraseProcess = Process.Start("/opt/adb", "shell pm uninstall " + MVVMFramework.Statics.Fixtures.AndroidBundleName);
            //    eraseProcess.WaitForExit();
            //}
        }

        public static void ResetSimulator(string iOSSimulator)
        {
            //TODO: most tests can run without the need to repeatedly shudown and erase the simulator. 
            //TODO: this code is left here for educational purposes.
            if (TestEnvironment.Platform.Equals(TestPlatform.Local))
            {
                var deviceId = TestHelpers.GetDeviceID(iOSSimulator);

                if (string.IsNullOrEmpty(deviceId))
                {
                    throw new ArgumentException($"No simulator found with device name [{iOSSimulator}]", iOSSimulator);
                }

                var shutdownProcess = Process.Start("xcrun", string.Format("simctl shutdown {0}", deviceId));
                shutdownProcess.WaitForExit();
                var eraseProcess = Process.Start("xcrun", string.Format("simctl erase {0}", deviceId));
                eraseProcess.WaitForExit();
            }
        }

    }
}