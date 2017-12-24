
### **This Document explains how to add native settings to Xamarin app. for complete running code, please switch to _settings_ branch.**

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

# Use of constants in XAML files

The example above reveals an interesting challenge. We have the MVVMFrameworks.Statics to keep all string constants. this is useful because both C# code and Specflow Tests can access the same constants, and therefore if one changes the other won't break. but how to extend this to XAML files? it would be very useful if the constants would be available in XAML files as well.

The technique to achieve this is through static namespaces. we add a namespace to the XAML file and then access the constants like below:

~~~xml
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage 
	xmlns="http://xamarin.com/schemas/2014/forms"
	xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
	xmlns:static = "clr-namespace:MVVMFramework.Statics;assembly=MVVMFramework"
	x:Class="example.Pages.MainPage">
	<ContentPage.Content>
        <StackLayout Padding="30">
            <Button AutomationId="{x:Static static:UIID.SettingsButton}" 
                    Text="{x:Static static:UIStrings.SettingsButton}" 
                    Command="{Binding SettingsCommand}"  />
        </StackLayout>
	</ContentPage.Content>
</ContentPage>
~~~

Pay special attention to the line marked with **_xmlns:static_**. This line creates a connection to MVVMFramework.Statics namespace which is part of MVVMFramework assembly. 


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

To be able to test the last step, the SettingsPageTO class has to be refactored to expose the UseHttpSwitch element. Most unfortunately, the Switch Element's AutomationID is not exposed for now. The only marker available is the text in the label associated with the switch, but that element is not useful to test the tapping of the switch. 

To make matters worse, the switch in iOS is the sibling of text element, but in android is the sibling of its parent. to account for all of this, the AppPageTO exposes a GetSwitch() function, which takes a label and returns the switch that can be tapped, based on the device in use. Under the hood, here is how it looks:

~~~csharp
 public Func<AppQuery, AppQuery> GetSwitch(string label)
        {
            if (IsAndroidApp())
                return c => c.Text(label).Parent(1).Child(1);
            return c => c.Marked(label).Index(1);
        }
~~~

So, to get the Switch in the settings page, we need to add the following property to it

~~~csharp
        public Func<AppQuery, AppQuery> UseHttpsSwitch 
        	{ get => GetSwitch(UIStrings.UseHttps); }
~~~

The test will fail properly. for it to pass, we need to define the XAML for the Settings Page.

~~~xml
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" 
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" 
    xmlns:static = "clr-namespace:MVVMFramework.Statics;assembly=MVVMFramework"
    x:Class="example.Pages.SettingsPage">
	<ContentPage.Content>
        <TableView Intent="Settings">
            <TableView.Root>
                <TableSection>
                    <SwitchCell Text="{x:Static static:UIStrings.UseHttps}"
                        On="{Binding UseHttps}" />
                </TableSection>
            </TableView.Root>
        </TableView>
	</ContentPage.Content>
</ContentPage>
~~~

Note the **Intent** of the _TableView_ element that is being set to _Settings_, and the use of Static directive to get the text from MVVMFramework.Statics namespace.

The tests pass, and we are assured that there is a Settings page, with a Switch Cell on it and the label of _Use Https_. now we need to make sure that the tapping of the switch will actually change the settings.

# Xamarin Backdoors
Now that we can tap the switch, we need to ensure that such tapping will change the Setting bound to the switch. i.e.:

~~~gherkin
@Settings_Page
Scenario: 02_Should have a switch to Use Https
    Given I am in Settings page
    And  the settings page has a switch to select Https protocol
    And  I record the value of Settings.UseHttps
    When I tap the Use Https switch 
    Then the Value of the Settings.UseHttps will toggle
~~~

We add the above to the SettingsPage.Feature file. So now this test needs a way to read the app settings, but those are internal to the app and not accessible from outside. for this, one should employ the concept of [Backdoors][], which allow the test platform to execute a function inside the app and return a value.

To define backdoors, Few steps need to be taken. 

* Add a SpecflowBackdoor class with an Execute function to the example.Helpers namespace. this will hold the shared code:

~~~csharp
    public static class SpecFlowBackdoor
    {
        public static string Execute(JObject msg){
            switch((string)msg[Backdoor.Key]){
                default:
                    return "Unknown Key" + (string)msg[Backdoor.Key];
            }
        }
    }
~~~

* For the Android Project, The Mono.Android.Export should be referenced in the Droid project. this was done in boilerplate and is demonstrated with screenshots in that document.
* In the MainActivity.cs add a method with Export (Java.Interop.Export) annotation:

~~~csharp
        [Export(MVVMFramework.Statics.Fixtures.SpecflowBackdoor)]
        public string SpecflowBakckdoor(string json)
        {
            return Helpers.SpecFlowBackdoor.Execute(JObject.Parse(json));
        }
~~~

* For iOS project, in AppDelegate.cs, add an Export decorated method.

~~~csharp
[Export(MVVMFramework.Statics.Fixtures.SpecflowBackdoor + ":")]
public NSString SpecflowBakckdoor(NSString json){
 return new
   NSString(Helpers.SpecFlowBackdoor.Execute(JObject.Parse(json)));
 }
~~~

* Note a few nuances with the iOS annotation:
	* The export key ends with a ":" This is most important.
	* The function always takes one and only one parameter.
	* It returns a **new NSString()**
	* It will show as an empty array in Repl()
	* It should be turned **toString()** before assertions.
	* Its type is Newtonsoft.Json.Linq.JValue

Because of these differences, invoking the backdoor is different between iOS and Android. The AppPageTO class abstracts this complication with helper Invoke functions.

~~~csharp
public string Invoke(string methodName, JObject json) =>
  (IsAndroidApp()
  ? app.Invoke(methodName, json.ToString()).ToString()
  : app.Invoke(methodName + ":", json.ToString())).ToString();
~~~

The way the backdoor works in this arrangement is that the tests will invoke the backdoor with a json object that has a _Backdoor.Key_ and a _Backdoor.Payload_. the key is a string and the payload can be anything, from a simple string to a complex json object. the switch statement in SpecflowBackdoor.Execute() will then decide how to interpret the payload. The AppPageTO object has overloaded Invoke() functions to address the more common use cases, such as key only, string payload, and constructed JObject.


## Test steps for Use Https switch

We can now define the steps for our second scenario. Add a Specflow.Specflow Step Definition to the Specflow.Steps.SettingsPage directory, run it and fill the steps in.

~~~csharp
    [Binding]
    public class Scenario02 : StepsBase
    {
        [Given(@"I am in Settings page")]
        public void GivenIAmInSettingsPage()
        {
            AddOrGetPageTO<MainPageTO>().GotoSettingsPage();
            AddOrGetPageTO<SettingsPageTO>().WaitForLoad();
        }

        [Given(@"the settings page has a switch to select Https protocol")]
        public void GivenTheSettingsPageHasASwitchToSelectHttpsProtocol()
        {
            app.WaitForElement(AddOrGetPageTO<SettingsPageTO>().UseHttpsSwitch);
        }


        [Given(@"I record the value of Settings\.UseHttps")]
        public void GivenIRecordTheValueOfSettings_UseHttps()
        {
            var origin = AddOrGetPageTO<SettingsPageTO>().Invoke(Backdoor.GetUseHttps);
            Context.Add("origin",Convert.ToBoolean(origin));
        }

        [When(@"I tap the Use Https switch")]
        public void WhenITapTheUseHttpsSwitch()
        {
            app.Tap(AddOrGetPageTO<SettingsPageTO>().UseHttpsSwitch);
        }

        [Then(@"the Value of the Settings\.UseHttps will toggle")]
        public void ThenTheValueOfTheSettings_UseHttpsWillToggle()
        {
            var result = AddOrGetPageTO<SettingsPageTO>().Invoke(Backdoor.GetUseHttps);
            Convert.ToBoolean(result).ShouldEqual(!Context.Get<Boolean>("origin"));
        }
    }
~~~
So we need to have our backdoor return the UseHttps setting when invoked with Backdoor.GetUseHttps key.

~~~csharp
   public static string Execute(JObject msg){
     switch((string)msg[Backdoor.Key]){
         case Backdoor.GetUseHttps:
             return Settings.UseHttps.ToString();
         default:
             return "Unknown Key" + (string)msg[Backdoor.Key];
      }
   }
~~~

The test fails properly, because the switch and the setting are not connected to each other. So we can create a binding between the switch and its ViewModel

~~~xml
<TableSection>
   <SwitchCell Text="{x:Static static:UIStrings.UseHttps}"
                        On="{Binding UseHttps}" />
</TableSection>
~~~

In ViewModel, the changes to the UseHttps property should be delegated to Settings object, which will in turn delegate it to AppSettings.

~~~csharp
bool _useHttps = Settings.UseHttps;
public bool UseHttps
{
    get => _useHttps;
    set
    {
        if (_useHttps == value) return;
        _useHttps = value;
        Settings.UseHttps = value;
        RaisePropertyChanged(nameof(UseHttps));
    }
}
~~~

While there is doublebinding between the ViewModel and the Switch element, there is no double binding between the Settings and the Switch element. i.e. if one directly sets Settings.UseHttps to false, the switch won't change, but changing ViewModel.UseHttps to false will change the Switch.

Settings <------- ViewModel <==========> Switch

To create proper doublebinding all the way, we need to listen to SettingsChanged event and update ourselves. we will use reflection to findout which setting has changed and will act accordingly.

~~~csharp
public SettingsPageVM(INavigationManager navigator) : base()
{
    Navigator = navigator;
    Settings.SettingsChanged += (sender, e) =>
    {
        var property = this.GetType().GetProperty(e.key);
        if (property?.GetValue(this, null) != e.value)
            property?.SetValue(this, e.value);
    };
}
~~~

The tests should pass, but they may not. that is because there is a delay between tapping the switch and all the underlying stuff that has to happen. We need to change the last step to wait a bit before deciding that the element was not tapped.

~~~csharp
[Then(@"the Value of the Settings\.UseHttps will toggle")]
public void ThenTheValueOfTheSettings_UseHttpsWillToggle()
{
    var page = AddOrGetPageTO<SettingsPageTO>();
    var expected = !Context.Get<bool>("originalUseHttps");
    app.WaitFor(() =>
     Convert.ToBoolean(page.Invoke(Backdoor.GetUseHttps)) == expected,
                "Use Https Value did not change");
}
~~~

# Next Steps
Adding other types of settigs controls is pretty much the same. _EnrtyCell_ is used for Text, Url, email, and Password. _ViewCell_ can be used for customized controls. Refer to the Xamarin documentation for more detail. bottom line, every setting needs to have a representaion in the ViewModel, and one in the Settings singleton. The backdoor, the Id and label texts, and other constants are supported by the MVVMFrameworks.Statics namespace.

[Xam.Plugins.Settings]: https://jamesmontemagno.github.io/SettingsPlugin/GettingStarted.html
[Backdoors]: https://developer.xamarin.com/guides/testcloud/uitest/working-with/backdoors/
