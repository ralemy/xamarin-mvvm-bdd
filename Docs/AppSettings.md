[TOC]

# Adding Settings to Mobile App

Most apps require settings feature. Settings are a collection of key-value pairs for the parameters that the app needs for its functionality. In order to have a platform independent settings feature, one needs to be able to access (read and write) the settings in a common way, as well as displaying them in a device-independent fashion.

# Crossplatform access to Settings

Install [Xam.Plugins.Settings][] Nuget package in the Droid and iOS projects. Once installed, Create a Settings.cs class in the root of the shared project and add a static class called Settings in the namespace of _$rootnamespace$.Helpers_:

~~~csharp
namespace example.Helpers{
	public static class Settings{
		private static ISettings AppSettings{
			get => CrossSettings.Current;
		}
	}
}
~~~

This will serve as the Singleton for accessing settings throughout the application. Before going further, let's take a moment to think how changes may be introduced to settings and how they may replicate. 

## Getting settings values
Getting values is straight forward. Each setting should have a getter, which can then be made available to different views using the ViewModel object for that view. The Settings singleton object can be used by the rest of the application to get setting values.

When getting the values, it would help if there is a default to return, and as all statics, we should put it in MVVMFrameWorks.Statics

~~~csharp
...
 get => AppSettings.GetValueOrDefault(nameof(UseHttps), 
 			SettingsDefaults.UseHttps);
... 			
~~~

## Changing settings values
Changing the settings values is tricky. The value can be changed through a view, which will report it to its ViewModel that in turn has to change the value in the Singlton. the value can be changed by some backend process, which directly changes the Singlton value and all ViewModels and Views using that setting need to update themselves. Finally, the value can be changed using the ViewModel, which means that the view should be updated accordingly.

Therefore, the Settings Singleton needs to publish an event when a settings changes. Anyone interested in the event will then listen to it and act accordingly.

~~~csharp
    public static class Settings
    {
        public static event EventHandler<SettingsChangedArgs> 
        							SettingsChanged;
        							
        private static ISettings AppSettings
        ...

~~~
 For this to work, we need a _SettingsChangedArgs_ object defined. So we will add a _Models_ directory to the shared project and create the class there.

~~~csharp
    public class SettingsChangedArgs
    {
        public string key;
        public object value;
    }
~~~

Now, each individual setting can be defined using a getter and a setter method:

~~~csharp
        public static bool UseHttps
        {
            get => AppSettings.GetValueOrDefault(nameof(UseHttps),
            			 SettingsDefaults.UseHttps);
            set
            {
              	AppSettings.AddOrUpdateValue(nameof(UseHttps), value);
                SettingsChanged?.Invoke(Settings.AppSettings,
                 new SettingsChangedArgs
                {
                    key = nameof(UseHttps),
                    value = value
                });
            }
        }
~~~
The setter will update the settings value in the device, then if there are subscribers to the event it will publish an event with the key and value of the setting that changed.

# Adding a UI for the user to change the setting

To allow the user to change the settings, we need a UI page. Since we are following the MVVMLight model, we need some housekeeping:

* Add a constant to the MVVMFramework.Statics.PageKeys for the settings page
* Add a Forms ContentPage Xaml to the Pages directory and call it SettingsPage.xaml
* Add a C# subclass of ViewModelBase to the ViewModels directory and call it SettingsPageVM.cs
* In the xaml.cs file, set the PageKey to the static value, set the AutomationId to the PageKey, and set the BindingContext to the ViewModel object received from the DI Singleton.

~~~csharp
    public partial class SettingsPage : ContentPage
    {
        public static readonly string PageKey = PageKeys.SettingsPage;
        public SettingsPage()
        {
            InitializeComponent();
            AutomationId = PageKey;
            BindingContext =
            		Initializer.GetDependency<SettingsPageVM>()
        }
    }
~~~


* Register the page with NavigationManager and the ViewModel with dependency manager in App.xaml.cs

~~~csharp
        private Page RegisterPages(NavigationPage page)
        {
            var nav= Initializer.GetDependency<INavigationManager>();
            nav.SetMain(page);
            nav.Register(SettingsPage.PageKey, 
            				typeof(SettingsPage));
            return page;
        }

        private void RegisterDependencies()
        {
            Initializer.SetupDI();
            Initializer.Register<MainPageVM>();
            Initializer.Register<SettingsPageVM>();
        }

~~~


# Navigating to the Settings Page

The easiest way is to add a new button to the Main Page and Set the command for it to navigate to the Settigs Page. As usual, the ViewModel will have a command that is initialized in the constructor and uses the navigator to navigate to the settings page when invoked.

## The first test

To test for this part, We need a little more housekeeping on the test side. 

* First we need a Specflow feature file that elaborates what is being tested. This should be added to the Specflow.Features.Pages
  * Create a Pages directory under Specflow.Features
  * Add a new file to Pages directory
  * Select Specflow group and Specflow Feature from it
  * Name is **SettingsPage**

Visual studio will create a SettingsPage.feature and SettingsPage.feature.cs. replace contents of SettingsPage feature with the below:

~~~gherkin
Feature: Settings Page
    To have a configurable application 
    I need a settings page where parameters can be stored and updated.
	
@Settings_Page
Scenario: 01_Should Navigate from Main to Settings Page
	Given I am in Main Page
	And There is a 'Settings' button on the page
	When I press the 'Settings' button
	Then the application will Navigate to Settings Page
~~~
**Notice** that the Scenario name starts with 01. this helps with ensuring that this scenario runs first. when multiple scenarios are present in a feature file, they are first sorted by their name and then executed in an ascending fashion. 

* Once we save this file, we need the second part of the partial feature class that ensures the app is started and available. 

Add a general empty class by the name of _SettingsPageFeature_ to Specflow.Features.Pages, and replace the contents with below:

~~~csharp
namespace Specflow.Features.Pages
{
    public partial class SettingsPageFeature: FeatureBase
    {
        public SettingsPageFeature(Platform p, string i, bool r)
        : base(p,i,r)
        {
        }
    }
}
~~~

**Notice** that this is a partial class. The other part of the class is generated automatically by VisualStudio when we saved the feature file. The class is called _SettingsPageFeature_ because the feature is names _Settings Page_ in the first line in front of _Feature:_ key. Keep in mind that the title of the feature will turn into the name of the code-behind classes, so caution should be exercised selecting and changing those names.


Now we execute the test once to get the expected template in the error message and create a C# subclass of StepsBase to implement it. details about how to get to this step can be found in the boilerplate document. As a fast overview:

* Create a Settings directory under Specflow.Steps
* Add a Specflow.Specflow Step Definition file with the name of Scenario01
* Build the Specflow project, the test tab will show the scenario that can be run now.
* run the scenario, the Application Output pane will show the correct Given, When, and then statements.
* replace the contents of Scenario01.cs with the correct statements, and the result should look like below

~~~csharp
namespace Specflow.Steps.SettingsPage
{
    [Binding]
    public class Scenario01 : StepsBase
    {
        [Given(@"I am in Main Page")]
        public void GivenIAmInMainPage()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"There is a '(.*)' button on the page")]
        public void GivenThereIsAButtonOnThePage(string settings)
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I press the '(.*)' button")]
        public void WhenIPressTheButton(string settings0)
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the application will Navigate to Settings Page")]
        public void ThenTheApplicationWillNavigateToSettingsPage()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
~~~

## Creating the page test object

Now we need to create a page test object for the SettingsPage. The Settings Page is accessible through the MainPage, so we need a few changes to the MainPage page test object first.

First, we need to access the button on the main page that can get us to the settings page. this button needs to have an AutomationId, and so we need a const in the MVVMFramework Statics class as well:

~~~csharp
public Func<AppQuery, AppQuery> SettingsButton = 
		c => c.Marked(UIID.SettingsButton);
~~~

Then we need to expose a mechanism to move from main page to the settings page

~~~csharp
public void GotoSettingsPage() => app.Tap(SettingsButton);
~~~

OK, now we can add a SettingsPageTO empty class to the Specflow.PageTestObjects.Pages and replace its contents with the following:

~~~csharp
    public class SettingsPageTO:AppPageTO
    {
        public SettingsPageTO(IApp app) : base(app)
        {
        }

        protected override string KeyOfPage 
        			=> PageKeys.SettingsPage;

        public override void NavigateFromMain(AppPageTO main) 
        			=> ((MainPageTO)main).GotoSettingsPage();
    }

~~~

## Writing the Test Code

StepsBase adds a _Context_ field that is a shortcut for _ScenarioContext.Current_. It also provides a sugar function, _AddOrGetPageTO_ to make it simpler to access page test objects. 

In the interest of document size, we will show the entire test code here. in the purist form, we should only do one step at the time, fail, code to pass, refactor, and do the next step. detailed information can be found in the boilerplate document.


~~~csharp
        [Given(@"I am in Main Page")]
        public void GivenIAmInMainPage()
        {
            var page = AddOrGetPageTO<MainPageTO>();
            app.WaitForElement(page.PageContainer);
        }

        [Given(@"There is a '(.*)' button on the page")]
        public void GivenThereIsAButtonOnThePage(string settings)
        {
            app.Query(c => c.Text(settings)).Length.ShouldEqual(1);
        }

        [When(@"I press the '(.*)' button")]
        public void WhenIPressTheButton(string settings0)
        {
            app.Tap(c => c.Text(settings0));
        }

        [Then(@"the application will Navigate to Settings Page")]
        public void ThenTheApplicationWillNavigateToSettingsPage()
        {
            var page = AddOrGetPageTO<SettingsPageTO>();
            page.WaitForLoad();
        }
~~~
Notice there are no fields in the class and communication is done using the ScenarioContext object which is accessed by the _AddOrGetPageTO_ method.

## Writing Code to pass the test

To pass the test, the ViewModel of the Main page needs a command object to capture the tapping of the Settings button and navigating to the Settings page.

~~~csharp
    public class MainPageVM : ViewModelBase
    {
        private readonly INavigationService Navigator;

        public ICommand SettingsCommand { get; private set; }
        public MainPageVM(INavigationService nav)
        {
            Navigator = nav;
            SettingsCommand = new RelayCommand(()=>
            	Navigator.NavigateTo(SettingsPage.PageKey));
        }
    }
~~~

The xaml file in the Main page needs to have a button that invokes that command:

~~~xml
	<ContentPage.Content>
        <StackLayout Padding="30">
            <Button AutomationId="SettingsButton" Text="Settings"
            		Command="{Binding SettingsCommand}"  />
        </StackLayout>
	</ContentPage.Content>
~~~

The test will now pass. We are ready for the next steps.

# Binding the Settings Page to the Settings Object 

The easy test for this section is to check that there is an element on the Settings page that would change a particular setting:

~~~gherkin
	Then the application will Navigate to Settings Page
    And  the settings page has a button to select Https protocol
~~~

~~~csharp
        [Then(@"the settings page has a button to select Https protocol")]
        public void ThenTheSettingsPageHasAButtonToSelectHttpsProtocol()
        {
            app.Query(page.UseHttpSwitch).Length.ShouldEqual(1);
        }
~~~

## Testing a SwitchCell Setting

To be able to test the last step, the ISettingsScreen and SettingsScreen classes have to be refactored to expose the UseHttpSwitch element. Most unfortunately, the Switch Element's AutomationID is not exposed. The only marker available is the text in the label associated with the switch, but that element is not useful to test the tapping of the switch. 

To make matters worse, the switch in iOS is the sibling of text element, but in android is the sibling of its parent. fortunately, the page test object makes it possible to hide these differences.

~~~csharp
 public Func<AppQuery, AppQuery> UseHttpsSwitch
        {
            get
            {
                if (app is Xamarin.UITest.Android.AndroidApp)
                    return c => c.Text("Use Https").Parent(1).Child(1);
                else 
                    return c => c.Marked("Use Https").Index(1);
            }
        }
~~~

# Xamarin Backdoors
Now that we can tap the switch, we need to ensure that such tapping will change the Setting bound to the switch. i.e.:

~~~gherkin
@Settings_Page
Scenario: Should have a switch to Use Https
    Given I am in Settings page
    And  the settings page has a switch to select Https protocol
    And  the 'UseHttps' Setting is 'true'
    When I tap the switch 
    Then the 'UseHttps' Setting will change to 'false'
~~~

So this tests needs a way to read the app settings, but those are internal to the app and not accessible from outside. for this, one should employ the concept of [Backdoors][], which allow the test platform to execute a function inside the app and return a value.

To define backdoors, Few steps need to be taken.

* Right click on the Preferences directory of the Droid project and go to Edit Preferences, search for "export" and add Mono.Android.Export to the project.

![Export dll](images/Export-Reference.png)

* For the Android Project, in the MainActivity.cs add a method with Export (Java.Interop.Export) annotation:

~~~csharp
        [Export]
        public string ExamineSettings(string key)
        {
            switch (key)
            {
                case "UseHttps":
                    return Helpers.Settings.UseHttps ? "true" : "false";
                case "SetHttps":
                    Helpers.Settings.UseHttps = true;
                    return "true";
                case "ClearHttps":
                    Helpers.Settings.UseHttps = false;
                    return "false";
                default:
                    return "Unknown key " + key;
            }
        }
~~~

* For iOS project, in AppDelegate.cs, add an Export decorated method.

~~~csharp
[Export("ExamineSettings:")]
        public NSString ExamineSettings(NSString key)
        {
            switch(key){
                case "UseHttps":
                    return new NSString(Helpers.Settings.UseHttps ? "true" : "false");
                case "SetHttps":
                    Helpers.Settings.UseHttps = true;
                    return new NSString("true");
                case "ClearHttps":
                    Helpers.Settings.UseHttps = false;
                    return new NSString("false");
                default:
                    return new NSString("Unknown key " + key);
            }
~~~

* Note a few nuances with the iOS annotation:
	* The export key ends with a ":" This is most important.
	* The function always takes one and only one parameter.
	* It returns a **new NSString()**
	* It will show as an empty array in Repl()
	* It should be turned **toString()** before assertions.
	* Its type is Newtonsoft.Json.Linq.JValue

Because of these differences, invoking the backdoor is different between iOS and Android and an Invoke function is needed in StepsBase to encapsulate that:

~~~csharp
        public string Invoke(string id, string param)
        {
            if (app is Xamarin.UITest.Android.AndroidApp)
                return app.Invoke(id, param).ToString();
            return app.Invoke(id + ":", param).ToString();
        }
~~~

## Test steps for Use Https switch

~~~csharp
    public class ShouldHaveASwitchToUseHttps : StepsBase
    {
        ISettingsScreen page;
        [Given(@"I am in Settings page")]
        public void GivenIAmInSettingsPage()
        {
            page = FeatureContext.Current.Get<ISettingsScreen>();
            if(app.Query("SettingsButton").Length > 0)
            {
                app.Tap("SettingsButton");
                page.WaitForLoad();
            }
            app.Query(page.PageContainer).Length.ShouldEqual(1);
        }

        [Given(@"the settings page has a switch to select Https protocol")]
        public void GivenTheSettingsPageHasASwitchToSelectHttpsProtocol()
        {
            app.Query(page.UseHttpsSwitch).Length.ShouldEqual(1);
        }

        [Given(@"the '(.*)' Setting is '(.*)'")]
        public void GivenTheSettingIs(string useHttps, string state)
        {
            Invoke("ExamineSettings", "SetHttps"); //will set the state to true
        }

        [When(@"I tap the switch")]
        public void WhenITapTheSwitch()
        {
            app.Tap(page.UseHttpsSwitch);
        }

        [Then(@"the '(.*)' Setting will change to '(.*)'")]
        public void ThenTheSettingWillChangeTo(string useHttps0, string @false)
        {
            Invoke("ExamineSettings", "UseHttps").ShouldEqual("false");
        }

    }
~~~

The test fails, because the switch and the setting are not connected to each other. So we can create a binding between the switch and its ViewModel

~~~xml
   <SwitchCell Text="Use Https" On="{Binding UseHttps}" AutomationId="UseHttpsSwitch"/>
~~~

In ViewModel, the changes to the UseHttps property should be delegated to Settings object, which will in turn delegate it to AppSettings.

~~~csharp
        public bool UseHttps{
            get => Settings.UseHttps;
            set
            {
                if (Settings.UseHttps == value) return;
                Settings.UseHttps = value;
                RaisePropertyChanged("UseHttps");
            }
        }
~~~

While there is doublebinding between the ViewModel and the Switch element, there is no double binding between the Settings and the Switch element. i.e. if one directly sets Settings.UseHttps to false, the switch won't change, but changing ViewModel.UseHttps to false will change the Switch.

Settings <------- ViewModel <==========> Switch

Therefore, we have to refactor our test to pass:
# Passing Tests
~~~gherkin
@Settings_Page
Scenario: 02 Should have a switch to Use Https
    Given I am in Settings page
    And  the settings page has a switch to select Https protocol
    And  I record the state of the UseHttps Setting
    When I tap the switch 
    Then the UseHttps Setting will toggle
~~~

And the Step code for these would be

~~~csharp
...
        private string InitialUseHttpsStatus;
...
        [Given(@"I record the state of the UseHttps Setting")]
        public void GivenIRecordTheStateOfTheUseHttpsSetting()
        {
            InitialUseHttpsStatus = Invoke("ExamineSettings", "UseHttps");
        }

        [When(@"I tap the switch")]
        public void WhenITapTheSwitch()
        {
            app.Tap(page.UseHttpsSwitch);

        }

        [Then(@"the UseHttps Setting will toggle")]
        public void ThenTheUseHttpsSettingWillToggle()
        {
            app.WaitFor(() => Invoke("ExamineSettings", "UseHttps") ==
                        (InitialUseHttpsStatus == "false" ? "true" : "false")
                        , "Didn't change the Setting");
        }
~~~

Notice the app.WaitFor() call that allows time for the bindings to take effect for change in Switch to result in change in Settings.


[Xam.Plugins.Settings]: https://jamesmontemagno.github.io/SettingsPlugin/GettingStarted.html
[Backdoors]: https://developer.xamarin.com/guides/testcloud/uitest/working-with/backdoors/