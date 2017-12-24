[TOC]

# Adding REST clients to Xamarin app
Http communication with backend servers, including RESTFul calls, GET and POST calls, and Websockets

Start by adding the [RestSharp][] package to the android and iOS projects. This package will provide many common functionalities required for REST connection. These include:

* Automatic XML and JSON deserialization
* Supports custom serialization and deserialization via ISerializer and IDeserializer
* Fuzzy element name matching ('product_id' in XML/JSON will match C# property named 'ProductId')
* Automatic detection of type of content returned
* GET, POST, PUT, PATCH, HEAD, OPTIONS, DELETE, COPY supported
* Other non-standard HTTP methods also supported
* OAuth 1, OAuth 2, Basic, NTLM and Parameter-based Authenticators included
	* Supports custom authentication schemes via IAuthenticator
* Multi-part form/file uploads

Before starting to send and receive messages, there is some groundwork that needs to be in place.

# Handling JSON

While [RestSharp][] does have serialization and deserializaiton interfaces, one of the most popular packages for converting between JSON objects and application objects is [Newtonsoft Json][]. This powerful package allows the user to start small and scale into more complex use cases of JSON data communication.

## Serialization to/from Objects
When working with JSON, one has two main approaches.
In the first approach, the format of the JSON is (somehow) known, and an object can be created to represent the data structure that is being transferred.

~~~json
{
	"firstBarcode": "122344566",
	"secondBarcode": "111222333",
	"eventTime": "2018-01-01T18:00:00.000Z"
}
~~~

Can be transformed to

~~~csharp
public class AssociationEvent(){
	public string FirstBarcode {get; set;}
	public string SecondBarcode {get; set;}
	public DateTime EventTime {get; set;}
}
~~~

Conversion between the two representations can be done simply by:

~~~csharp
string output = JsonConvert.SerializeObject(eventObject);

AssociationEvent eventObject2 = JsonConvert.DeserializeObject<AssociationEvent>(output);
~~~

There are nuances to this approach. There are keys that may be expected and not present in the JSON, there may be keys that have a default value, the JSON object may have extra keys that are not expected, the values may need to be transformed from one format to another, etc. 

Following is a collection of most common issues faced. Most of the issues 

## JsonObject
This attribute is usually used to decorate the whole object. It has two most important uses that are worth mentioning.

### MemberSerialization
If set to OptOut, it serializes every member of the object, if set to OptIn, only those annotated with JsonProperty. Defautls to OptOut.

~~~csharp
[JsonObject(MemberSerialization.OptIn)]
public class ExampleObject{
	public string ThisWillNotSerialize = "Ignore Me";
	
	[JsonProperty]
	public string IncludeMe = "please";
	
	[JsonProperty]
	public string IncludeMeToo = "thankyou";
}
/* Output:
{"IncludeMe":"please","IncludeMeToo":"thankyou"}
Output for OptOut:
{"ThisWillNotSerialize":"Ignore Me","IncludeMe":"please","IncludeMeToo":"thankyou"}
*/
~~~

### NamingStrategy
This parameter allows for deciding how to map property names between Json and C#. 

~~~csharp
[JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class User
{
    public string FirstName { get; set; }
    [JsonProperty]
    public string LastName { get; set; }
    [JsonProperty(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public int SnakeRating { get; set; }
}
System.Console.WriteLine(JsonConvert.SerializeObject(new User()));
/* Output
{"lastName":null,"snake_rating":0}
*/
~~~

## IEnumerable Objects.

By default, an IEnumerable object will be serialized to an array and all other properties will be ignored. if the object is annotated with JsonObject however, then all of its other attributes will be serialized as well.

~~~csharp
using Newtonsoft.Json;
using System.Collections;
[JsonObject]
 public class Directory : IEnumerable<string>
 {
     public string Name { get; set; }
     public IList<string> Files { get; set; }
 
     public Directory()
     {
         Files = new List<string>();
    }
    
        public IEnumerator<string> GetEnumerator()
   {
        return Files.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
Directory directory = new Directory
 {
     Name = "My Documents",
     Files =
     {
         "ImportantLegalDocuments.docx",
         "WiseFinancalAdvice.xlsx"
     }
 };
string json = JsonConvert.SerializeObject(directory, Formatting.Indented);
System.Console.WriteLine(json);
/* Output
{
  "Name": "My Documents",
  "Files": [
    "ImportantLegalDocuments.docx",
    "WiseFinancalAdvice.xlsx"
  ]
}'
output without annotating the class with JsonObject:
[
  "ImportantLegalDocuments.docx",
  "WiseFinancalAdvice.xlsx"
]
*/
~~~

Notice the use of _Formatting.Indented_ in SerializeObject, which will result in indentation of the string into a more readable format.

# Handling Special cases

JsonObject, JsonProperty, and JsonSerializer all have builtin enumerations to help with handling special cases. some of these include:

* NullValueHandling. what to do when a poperty is set to null (or default). Ignore or Include.
* DefaultValueHandling. What to do when a property is missing to set to default value: Ignore, Include, Populate, Ignore and Populate
* MissingValueHandling. Ignore or Error.

Detailed information and [samples][] can be found on the [Json.Net][] documentation site.

In addition to the above techniques, generic solutions are available to create use case specific converstion engines for special cases. Needless to say, these should only be used if the more typical annotations could not solve the problem, as they affect the readability and scalability of the solution.

* A subclass of JsonConverter that overrides WriteJson, ReadJson, CanRead, and CanConvert can be supplied to the JsonSerializer for fine control on how the process would work.

~~~csharp
string json = JsonConvert.SerializeObject(object, Formatting.Indented, new CustomConvertor())
~~~

* Same class can also be set for just one property using the JsonConvert directive.

~~~csharp
public class myObject{
	[JsonConvert(typeof(MyCustomJsonConver))]
	public MyProperty SomeProperty {get; set;}
}
~~~

# BDD Testing RESTful calls

To test for restful calls, there must be a server such that the test framework can assert sending and receiving of messages. the test framework should be able to start the server, execute an action that should send a known payload to a known endpoint, assert that the message arrived with the correct payload, and close the server.

## Setting up the test Restful Server.

* Add the [Grapevine][] and [RestSharp][] packages to the Specflow project. 
* Add a folder named Server to the Specflow project.
* Move the RestTestServer.cs and RestServerFeatureBase files here
* Write a sanity feature for the REST server

~~~gherkin
Feature: Rest Server Sanity
    In order to test RESTful calls,
    I need a simple server that could act in place of a production one
    	
@rest_sanity
Scenario: Start and Stop Rest server
	Given I have a server set up
    And I have added a '/api/sanity' route to return 'server sane'
	When I call the '/api/sanity' endpoint
	Then the result should be a 'server sane' message
~~~

* As usual, add the second part of the partial class, but this time, it is a subclass of RestServerFeatureBase, because it doesn't need any phone or app.

~~~csharp
  public partial class RestServerSanityFeature: RestServerFeatureBase
    {
        public RestServerSanityFeature(string port):base(port){            
        }
    }
~~~

* Note that the RestServerFeatureBase has a [TextFeature("3434")] annotation, and that is why the server will run on port 3434 of the localhost. this file can be edited to change the Port number (or any other settings for that matter).

# Sanity Check for test REST server
* Finally, the steps for the above feature are implemented:

~~~csharp
    [Binding]
    public class ShouldStartAndStopRestServer
    {
        private RestTestServer RestServer;

        [Given(@"I have a server set up")]
        public void GivenIHaveAServerSetUp()
        {
            RestServer = FeatureContext.Current.Get<RestTestServer>("RestTestServer");
            RestServer.ShouldNotBeNull();
        }

        [Given(@"I have added a '(.*)' route to return '(.*)'")]
        public void GivenIHaveAddedARouteToReturn(string endpoint, string response)
        {
            RestServer.Register(
                delegate (IHttpContext context)
                {
                    context.Response.SendResponse(response);
                    return context;
                },
                HttpMethod.GET,
                endpoint
            );
        }

        [When(@"I call the '(.*)' endpoint")]
        public void WhenICallTheEndpoint(string p0)
        {
            ScenarioContext.Current.Add("returnvalue", RestServer.Get(p0));
        }

        [Then(@"the result should be a '(.*)' message")]
        public void ThenTheResultShouldBeAMessage(string p0)
        {
            ScenarioContext.Current.Get<string>("returnvalue").ShouldEqual(p0);
        }
    }
~~~

* The class doesn't need to be a subclass of StepsBase, because there is no mobile app.
* The RestServer is saved in the FeaturesContext object.
* There is a Register method that allows an endpoint to be associated with the Server.
* There is a Get method that allows an endpoint to be called on the server.
* Same techniques can be used to register other routes (e.g. POST or DELETE), or assing a method to the route instead of a delegate, or implementing POST requests and parametrized calls to the server.

# Testing the communication between App and Server

Start with the Features file, as always:

~~~gherkin
Feature: Rest Api communication
    the app needs to be able to consume RESTful apis 
    in order to send and receive information
    	
@rest_api
Scenario: Should Send data to REST Server
	Given The REST Server is running
    And I have added a '/api/connection' endpoint to return 'connection ok' 
    And I have configured the app for use of REST server endpoint
	And Am in the main page
	When I press Send data button
	Then The server will be called at the endpoint
~~~

The second part of the feature class is special. it is a FeatureBase that is also a RestServerFeatureBase, but multiple inheritence is not allowed. therefore, it has to basically implement the RestServerFeatureBase

~~~csharp
public partial class RestApiFeature : FeatureBase
    {
        private RestTestServer RestServer;

        public RestApiFeature(Platform p, string iOSSim, bool reset)
            : base(p, iOSSim, reset)
        {
            var Serverbase = new RestServerFeatureBase("3434");
            RestServer = Serverbase.RestServer;
        }

        [TestFixtureSetUp]
        public void StartRestServer()
        {
            RestServer.LogToConsole().Start();
        }
        [TestFixtureTearDown]
        public void StopRestServer(){
            RestServer.Stop();
        }

        [SetUp]
        public void AddServerToContext()
        {
            FeatureContext.Current.Add("RestTestServer", RestServer);
        }

        [TearDown]
        public void RemoveServerFromContext()
        {
            FeatureContext.Current.Remove("RestTestServer");
        }
    }
~~~

Note that this file should be edited if the Server is going to listen to any port other than 3434.

## Accessing the server from simulators

The Restserver will be thus running on port 3434 of the local machine, but since the host is set to 0.0.0.0, it will bind to all networks (not just localhost) and will be accessible from Android emulators and iOS simulators. However, the ip address is needed for the devices to communicate with endpoints. The RestTestServer exposes a GetLocalIP() method that implements one way to achieve this:

~~~csharp
public string GetLocalIP()
        {
            string localIP;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            return localIP;
        }
~~~
This is not guaranteed to work everywhere, but pretty much does. for a discussion and alternative methods of getting the local IP, one can start from the relevant [Stackflow Discussion](https://stackoverflow.com/questions/6803073/get-local-ip-address). 

LocalIP is needed if a physical device is being tested, emulators also work with that but if only emulators are being tested there is a simpler way: Android simulators use 10.0.2.2 for communicating with the host, and iOS just can use local host. in StepsBase class there is a HostAddress() method that returns the appropriate string based on the operating system under test.

# Configuring the app to use the RestTestServer

Usually we would add a Setting parameter for the base and target URL. therefore the most direct solution is to create the global static Settings object and use a backdoor to change it from our tests. 
here is the example for iOS.

~~~csharp
        [Export("SpecflowBackdoor:")]
        public NSString SpecflowBackdoor(NSString json)
        {
            JObject command = JObject.Parse(json.ToString());
            switch ((string)command["key"])
            {
                case "SetTarget":
                    Helpers.Settings.TargetURI = (string)command["payload"];
                    return new NSString("Target Set");
                default:
                    return new NSString("Unknown key " + (string)command["key"]);
            }
        }
~~~

Once the app is set, we need to tap the button to call the api and see that the method has been called with HttpMethod.GET. as usual, this would require a button whose command is bound to a RelayCommand in the MainPage ViewModel, running a method that uses [RestSharp][] to call the api. since the delegate for the route will change a class variable, we could wait for this change and ensure that the api has been called.

~~~csharp
	[Given(@"I have configured the app for use of REST server endpoint")]
        public void GivenIHaveConfiguredTheAppForUseOfRESTServerEndpoint()
        {
            var target = $"http://{HostAddress()}:{RestServer.Port}{restEndpoint}";
            var command = new JObject(new JProperty("key","SetTarget"),
                                      new JProperty("payload", target));
            Invoke("SpecflowBackdoor", command.ToString()).ShouldEqual("Target Set");
        }

        [Given(@"Am in the main page")]
        public void GivenAmInTheMainPage()
        {
            app.Query(c => c.Marked("TheMainPage")).Length.ShouldBeGreaterThan(0);
        }

        [When(@"I press Send data button")]
        public void WhenIPressSendDataButton()
        {
            app.Tap(c => c.Marked("CallApiButton"));
        }
        [Then(@"The server will be called at the endpoint")]
        public void ThenTheServerWillBeCalledAtTheEndpoint()
        {
            app.WaitFor(
                () => calledMethod != HttpMethod.ALL,
                "Didn't Call the endpoint"
            );
            calledMethod.ShouldEqual(HttpMethod.GET);
        }
~~~

[RestSharp]: https://www.nuget.org/packages/RestSharp
[Newtonsoft Json]: https://www.newtonsoft.com/json
[Json.Net]: https://www.newtonsoft.com/json/help/html/Introduction.htm
[samples]:https://www.newtonsoft.com/json/help/html/SerializeObject.htm
[Grapevine]: https://sukona.github.io/Grapevine/en/

