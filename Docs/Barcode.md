[TOC]

# Barcode scanning with Xamarin

This branch adds barcode scanning to the Xamarin BDD project. Barcode scanning is done throuhg the [ZXing.Net.Mobile for Forms][] Nuget pakage. 

For starters, the package should be added to iOS and Android projects and permissions and initializations should be applied as described in the [ZXing.Net.Mobile for Forms][]

# Setting up

While the links provided above have good information regarding how to set up the projects, there are a few hiccups here and there that can be best prevented by re-listing the process again. Once the package is added, here are the steps to be taken:

* In the Droid project, open the AndroidMenifest.xml file and make sure the following are included:

~~~xml
    <uses-feature android:name="android.hardware.camera" />
    <uses-feature android:name="android.hardware.camera.autofocus"/>
	<uses-permission android:name="android.permission.CAMERA" />
	<uses-permission android:name="android.permission.FLASHLIGHT" />
~~~

In the AssemblyInfo.cs file, the following line should be added if not present:

~~~csharp
[assembly: UsesPermission(Android.Manifest.Permission.Flashlight)]
~~~

In the MainActivity.cs file, a line should be added to the OnCreate hook to start the Zxing framework. OnRequestPermissionResult should also be updated

~~~csharp
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
// ------------- Activate Zxing Net ---------------            
            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            LoadApplication(new App());
        }

        public override void OnRequestPermissionsResult(
        int requestCode, 
        string[] permissions, 
        Permission[] grantResults)
        {
            global::ZXing.Net.Mobile.Android
            .PermissionsHandler
            .OnRequestPermissionsResult(requestCode,
            									 permissions, 
            									 grantResults);
        }
~~~

* For iOS project, the info.plist file should contain the following key-value pair

~~~xml
	<key>NSCameraUsageDescription</key>
	<string>Can we use your camera</string>
~~~

The AppDelegate.cs file also requires to start the Zxing framework in FinishLaunching hook:

~~~csharp
public override bool FinishedLaunching(UIApplication app, NSDictionary options)
{
    
#if ENABLE_TEST_CLOUD
    global::Xamarin.Calabash.Start();
#endif
    global::Xamarin.Forms.Forms.Init();
// ------------- Activate Zxing Net ---------------            
    global::ZXing.Net.Mobile.Forms.iOS.Platform.Init();

    LoadApplication(new App());
    return base.FinishedLaunching(app, options);
}
~~~

# Getting to the barcode page

The first requirement is to have some way of getting to the barcode page. Easiest is to have a button that moves to the page. 

To write a test for that, we add a _Specflow.Specflow Feature_ file under the name of _BarcodePage.feature_ to _Specflow.Features.Pages_ (create the Pages directory if necessary) and compose our first scenario

~~~gherkin
Feature: Barcode
	I need to have a page that uses the phone camera to read barcodes.
	
@barcode_structure
Scenario: Main page should have a button to navigate to barcode page
    Given I am in the main page
    And the main page has a barcode button
    When I tap the barcode button
    Then the application goes to the barcode page
~~~

As usual, a second partial file is added to the same location to inherit FeatureBase:

* Add an Empty C# class by the name of _BarcodeFeatures.cs_ to _Specflow.Features.Pages_. 
* Have the class inherit from FeatureBase and call the base constructor

~~~csharp
public partial class BarcodeFeature : FeatureBase
{
    public BarcodeFeature (Platform p, string i, bool r)
    :base (p,i,r)
    {
    }
}
~~~

Next, we need to create the code behind for this scenario. 

* We add a _Specflow.Specflow Step Definition_ file called Scenario01 to _Specflow.Steps.BarcodePage_ (create the directory if needed)
* Make it a subclass of _StepsBase_
* build the Specflow project, run the test
* fill it with empty template copied from the Application output:

~~~csharp
[Binding]
    public class Scenario01 : StepsBase
    {
        [Given(@"I am in the main page")]
        public void GivenIAmInTheMainPage()
        {
            ScenarioContext.Current.Pending();
        }

        [Given(@"the main page has a barcode button")]
        public void GivenTheMainPageHasABarcodeButton()
        {
            ScenarioContext.Current.Pending();
        }

        [When(@"I tap the barcode button")]
        public void WhenITapTheBarcodeButton()
        {
            ScenarioContext.Current.Pending();
        }

        [Then(@"the application goes to the barcode page")]
        public void ThenTheApplicationGoesToTheBarcodePage()
        {
            ScenarioContext.Current.Pending();
        }

    }
~~~

# Going through the BDD process

We need to have the page test object of the main page expose a BarcodeButton property to check and tap, which will need us to make  a static declration in _MVVMFramework.Statics.UIID_ for the BarcodeButton AutomationID. This will put us at our first failing test when the test would fail to find the barcode button.

~~~csharp
[Given(@"I am in the main page")]
public void GivenIAmInTheMainPage()
{
    AddOrGetPageTO<MainPageTO>().WaitForLoad();
}

[Given(@"the main page has a barcode button")]
public void GivenTheMainPageHasABarcodeButton()
{
    app.WaitForElement(AddOrGetPageTO<MainPageTO>().BarcodeButton);
}
~~~

To pass this step, we need to add a Barcode Button to the main page. we already have the static ID for it, we need to add a static text for it to the _MVVMFramework.Statics.UIStrings_ and reference it in the xaml file.

~~~xml
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" 
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" 
    xmlns:static ="clr-namespace:MVVMFramework.Statics;assembly=MVVMFramework"
    x:Class="example.Pages.MainPage">
	<ContentPage.Content>
        <StackLayout Padding="30">
            <Button AutomationId="{x:Static static:UIID.BarcodeButton}"
                Text="{x:Static static:UIStrings.BarcodeButton}"
                Command="{Binding BarcodeCommand}"/>
        </StackLayout>
	</ContentPage.Content>
</ContentPage>
~~~

The next failing step is when we are expecting to move to the Barcode page. to write the code-behind for ths step, we need to create a page test object and implement a way to navigate to it from Main page.

* Add a  static key in the _MVVMframework.Statics.PageKeys.BarcodePage_
*  Add a _GoToBarcodePage()_ function to _MainPageTO_

~~~csharp
    public class MainPageTO : AppPageTO
    {
        public MainPageTO(IApp app) : base(app)
        {
        }

        public Func<AppQuery, AppQuery> BarcodeButton = 
        	c => c.Marked(UIID.BarcodeButton);

        protected override string KeyOfPage => 
        	PageKeys.MainPage;

        public override void NavigateFromMain(AppPageTO main)
        {
        }
        public void GoToBarcodePage() => 
            app.Tap(BarcodeButton);
    }
~~~

*  Add an empty c# class to _Specflow.PageTestObjects.Pages_ under the name of _BarcodePageTO_ 
*  Make it a subclass of _AppPageTO_ and implement the abstract functions.

~~~csharp
public class BarcodePageTO : AppPageTO
{
    public BarcodePageTO(IApp app):base(app)
    {
    }

    protected override string KeyOfPage => PageKeys.BarcodePage;

    public override void NavigateFromMain(AppPageTO main)
    {
        ((MainPageTO)main).GoToBarcodePage();
    }
}
~~~
Now the final test code for the scenario can be written

~~~csharp
[When(@"I tap the barcode button")]
public void WhenITapTheBarcodeButton()
{
    app.Tap(AddOrGetPageTO<MainPageTO>().BarcodeButton);
}

[Then(@"the application goes to the barcode page")]
public void ThenTheApplicationGoesToTheBarcodePage()
{
    AddOrGetPageTO<BarcodePageTO>().WaitForLoad();
}
~~~

We now have our second failing test. and to pass it we need an acutal Barcode page to which to navigate. Quick overview of the steps to get to that are

* Add a _BarcodePageVM_ to _examples.ViewModels_ and make it a subclass of _ViewModelBase_

~~~csharp
namespace example.ViewModels
{
    public class BarcodePageVM : ViewModelBase
    {
        public BarcodePageVM()
        {
        }
    }
}
~~~

* Add a _Forms.Forms Content Page_ to _example.Pages_ under the name of _BarcodePage_
* Set the _PageKey_, _AutomationID_, and _BindingContext_ of the page.

~~~csharp
public class BarcodePage : ContentPage
{
    public static readonly string PageKey = PageKeys.BarcodePage;
    public BarcodePage()
    {
        Content = new StackLayout
        {
            Children = {
                new Label { Text = "Hello ContentPage" }
            }
        };
        AutomationId = PageKey;
        BindingContext = Initializer.GetDependency<BarcodePageVM>();
    }
}
~~~

* Register the page and the VM with _App.xaml.cs_

~~~csharp
private Page RegisterPages(NavigationPage page)
{
    var nav = Initializer.GetDependency<INavigationManager>();
    nav.SetMain(page);
    nav.Register(BarcodePage.PageKey,typeof(BarcodePage));
    return page;
}

private void RegisterDependencies()
{
    Initializer.SetupDI();
    Initializer.Register<MainPageVM>();
    Initializer.Register<BarcodePageVM>();
}
~~~

Finally, add a command to MainPageVM that navigates to the BarcodePage.

~~~csharp
public class MainPageVM :ViewModelBase
{
    private INavigationService Navigator;

    public ICommand BarcodeCommand { get; private set; }
    public MainPageVM(INavigationService n)
    {
        Navigator = n;
        BarcodeCommand = new RelayCommand(
            () => Navigator.NavigateTo(BarcodePage.PageKey));
    }
}
~~~

This will make the first scenario pass. it is time to add barcode scanning to our barcode page.

# The Zxing Page structure
Zxing provides a view for barcode scanning, called _ZXingScannerView_. one way of adding on screen information is to put this view as part of a grid and overlay it with another view. This is unnecessary but useful and we will put it in our demonstration. Zxing even provides a sample for this view, which is called _ZXingDefaultOverlay_. The reader can consult the [ZXing Github][] to study the source code of these files.

Thus, there are four things that the constructor of the _BarcodePage_ needs to do:

* Set the AutomationId for the page
* Set the BindingContext of the page to the ViewModel (obtained from dependency injection)
* Set the Content of page to a grid 
* Set the appearing and disappearing event handlers for the page.

~~~csharp
public BarcodePage() : base()
{
    AutomationId = PageKey;
    ViewModel = Initializer.GetDependency<BarcodePageVM>();
    Content = MakeScannerGrid();
    BindingContext = ViewModel;
    DelegateBaseEvents();
}
~~~

Making the Grid means making a _ZXingScannerView_ and a _ZXingDefaultOverlay_ and overlaying them on each other

~~~csharp
private View MakeScannerGrid()
{
    var grid = new Grid
    {
        VerticalOptions = LayoutOptions.FillAndExpand,
        HorizontalOptions = LayoutOptions.FillAndExpand
    };
    grid.Children.Add(InitZXingView());
    grid.Children.Add(InitOverlay());
    grid.BindingContext = ViewModel;
    return grid;
}
~~~

When Initializing the _ZXingScannerView_, we need to delegate the IsScanning, IsAnalyzing, and IsTorchOn events to the ViewModel, so binable properties need to be added to BarcodePage. please consult the source code for more details.

The page should also delegate the appearing and disappearing events to the ViewModel:

~~~csharp
private void DelegateBaseEvents()
{
    base.Appearing += (object sender, EventArgs e) =>
     ViewModel.Appear();
    base.Disappearing += (object sender, EventArgs e) =>
     ViewModel.Disappear();
}
~~~

# The BarcodePage ViewModel
The Barcode page ViewModel is responsible for binding of variables in the scanner view and overlay. These are described in the table below

property Name | usage |
--------------|-------|
IsScanning| puts the view to scanning mode if true, stops scanning if false|
InAnalyZXing| Puts the scanning to pause if false and resumes once true|
HasTorch| true if the device has flash light, false if not|
TopText| Text to show in the center of the top box on overlay|
BottomText| Text to show in the center of the bottom box on overlay|
IsTorchOn| True if flash is on|
ShowFlashButton| True if a button is to be added to the top box of overlay to turn on the flash|

Below is an example of implementing one of the bindings in the ViewModel

~~~csharp
private string _BottomText = UIStrings.BottomTextDefault;
public String BottomText
{
    get => _BottomText;
    private set => Set(() => BottomText, ref _BottomText, value);
}
~~~

There are also two commands that should be relayed by the ViewModel

Command|purpose|
-------|-------|
ScanResultCommand|called when a barcode is scanned. receives a Result object which contains the barcode scanned in the Text property|
FlashOperation| called when the flash button is clicked, if it is displayed using the above-mentioned _ShowFlashButton_ property|

In the implementation of the ScanResultCommannd it is important to note that the thread in which it is invoked **is different from the UI thread**, so any change to the UI should happen inside a safe closure. that is why the ViewModel constructor gets a copy of the _IUIRunner_ from the dependency injector:

~~~csharp
public BarcodePageVM(INavigationService navigator)
{
    this.navigator = navigator;
    this.runner = Initializer.GetDependency<IUIRunner>();
    ScanResultCommand = new RelayCommand<Result>(OnScanResult);
    FlashOperation = new RelayCommand(OperationFlash);
}
~~~

Now, when the ScanResultCommand wants to make a change to UI, it will do so inside the safe closure:

~~~csharp
public void OnScanResult(Result r)
{
    IsAnalyzing = false;
    IsScanning = false;
    runner.RunOnUIThread(navigator.GoBack);
}
~~~


# The appearing and disappearing events

The two events are delegated to the _Appear()_ and _Disappear()_ methods of the ViewModel. Here, we need to set the _IsAnalazing_ and _IsScanning_ to true so that the barcodes will be scanned, and to false so that the engine will stop scanning. Note the nested nature of the calls. _Appear()_ Sets IsScanning to True first and _Disappear()_ Sets it to False last. _Appear()_ will also restore any default value that may have been changed since last use:

~~~csharp
internal void Disappear()
{
    IsAnalyzing = false;
    IsScanning = false;
}

internal void Appear()
{
    IsScanning = true;
    IsAnalyzing = true;
    TopText = UIStrings.TopTextDefault;
    BottomText = UIStrings.BottomTextDefault;
}
~~~

So, in the appearing of the page, the IsScanning and IsAnalyZXing are set to true, and on the disappearing of the page the IsScanning is set to false. on the arrival of a new barcode, Both are set to false again. this allows for deciding whether to continue or not, as the scanning will pause until decision is made. If we then want to continue scanning, we will set the switches back to True.

# Next Steps

There is no requirement to limit the overlay to the provided default, or to use the full grid for barcode scanning. Experiments can be designed for fully customized overlay and views. The fundamentals of the scanning however will remain the same.

# References
[ZXing.Net.Mobile for Forms][]

[ZXing.Net.Mobile for Forms]:https://components.xamarin.com/gettingstarted/zxing.net.mobile.forms
[ZXing Github]:https://github.com/Redth/ZXing.Net.Mobile/tree/master/Source/ZXing.Net.Mobile.Forms