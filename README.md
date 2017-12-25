
# Xamarin MVVM App delevopment using BDD

This is an effort to provide step by step demonstration of how to develop a Xamarin app for iOS and Android using MVVM architecture and Behaviour Driven Development Methodology. 

Once the repository is cloned, in the Docs/ directory there is a xamarin-boilerplate.pdf file, which contains steps required to Install Visual Studio, add Specflow BDD extension, and setup a skeleton application with passing Sanity tests.

The pdf file also contains general and introductionary instructions on how to use the MVVM framework to navigate between pages and to delegate the code-behind to a more testable ViewModel object.

## Branches

The **Settings Branch** was checked out from _Boilerplate.V1.1_ tag. it demonstrates the following techniques:

* Adding a native Settings object
* Using constants inside XAML files
* Using a Xamarin Backdoor to write tests against internal objects 
* Creating double bindings between internal objects, ViewModels and Page components.
* Allowing time for UI and Backend update in tests

Based on the experience of this branch, a new _Boilerplate.V1.2_ tag is created, from which the future branches are checked out.
The AppSettings.md file contains the specifics of the above. the more up to date version of the file can be found in the branch.

The **RestClient Branch** was checkedout from _Boilerplate.V1.2_ tag. It demonstrates the following techniques

* Accessing an Web resource by using HTTP calls, including use of RESTful services
* BDD testing of Web access by running mock servers and registering mock endpoints that would verify correct construction and consumption of Web resources
* Checking the connectivity of the device to the Internet
* Passing the mock server address to simulators and physical devices
* References for consuming Websockets and AMQP messages.

The RestClient.md file contains the specifics of the above. the more up to date version of the file can be found in the branch.

The **Barcode** branch was checked out from _Boilerplate.V1.2_ tag and demonstrates the use of Zxing libraries to turn the phone camera into a barcode scanner. 

This page Demonstraties the following Techniques:

* Overlaying two views using the Grid compoinent
* Using the ZXing libraries to scan a barcode
* Delegating events and properties to ViewModel
* Running code on UI Thread

The Barcode.md file contains the specifics of the above. the more up to date version of the file can be found in the branch.

# Credits

This work was inspired by [Rob Gibbens article for Specflow] (http://arteksoftware.com/bdd-tests-with-xamarin-uitest-and-specflow/)
# License
This work is released under Apache 2.0 License. please see the LICENSE file for more information.

