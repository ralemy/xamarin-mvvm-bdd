[TOC]

# Adding REST clients to Xamarin app
Http communication with backend servers, including RESTFul calls, Websockets, and message brokers are some of the most common use cases in mobile applications. 

The payload of such messages is most commonly JSON. so we start by an overview of handling JSON objects and translating them to C# Objects, maps, or string representations.

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
# Checking for Connection

All the techiques and packages would be useless if there is no connection to Internet. this may happen for many reasons:

* The app is not allowed to use any Internet connection
* The app is only allowed to use Wi-Fi, while cellular connection is the only one available
* The device is out of coverage for WiFi and cellular.
* The device is in Airplane mode

Simplest way to just check if the device is connected is by using the [Connectivity Plugin][] which Xamarin includes when creating the project. Here is the snippet on checking if a connection is available:

~~~csharp
public bool DoIHaveInternet()
{
    return CrossConnectivity.Current.IsConnected;
}
~~~
 
 Read the manual of [Connectivity Plugin][] to see other options such as checking the type and bandwidth of connection.
 
 
# BDD Testing RESTful calls

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

To test for restful calls, there must be a server such that the test framework can assert sending and receiving of messages. the test framework should be able to start the server, execute an action that should send a known payload to a known endpoint, assert that the message arrived with the correct payload, and close the server.

## Setting up the test Restful Server.

Most of the groundwork has already been done in the master branch:

* Added the [Grapevine][] and [RestSharp][] packages to the Specflow project. 
* Added a folder named Server to the Specflow project.
* Moved the RestTestServer.cs and RestServerFeatureBase files here
* Wrote a sanity feature for the REST server

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

Start with the Features file, as always. We put a WebCommunications Specflow.Specflow feature file in Specflow.features:

~~~gherkin
Feature: WebCommunications
    As an app developer
    I need to know how to communicate with Web resources
    Through RESTful interfaces, Sockets, or AMQP broker.
	
@Restful
Scenario: 01_Should be able to send data using HTTP.GET 
	Given I am running a RestTestServer
    And   I have a button on Mainpage to call RestTestServer
    And   I have configured the App for RestCall
    And   I have registered an endpoint with the RestTestServer
    When  I tap the button
    Then  The RestTestServer is called with correct payload

~~~

The second part of the feature class is also added as usual

~~~csharp
    public partial class WebCommunicationsFeature :FeatureBase
    {
        public WebCommunicationsFeature(Platform p, string i, bool r)
            :base(p,i,r)
        {
        }
    }
~~~

## Accessing the server from simulators

The Restserver will be running on port 3434 of the local machine, but since the host is set to 0.0.0.0, it will bind to all networks (not just localhost) and will be accessible from Android emulators and iOS simulators. However, the ip address is needed for the devices to communicate with endpoints. The RestTestServer exposes a GetLocalIP() method that implements one way to achieve this:

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

# Going through the steps

The following is the completed code for the test steps, which we will put in Scenario01.cs file (a Specflow.Specflow Step Defintion) under Specflow.Steps.WebCommunications

~~~csharp
[Binding]
public class Scenario01 : StepsBase
{
    [Given(@"I am running a RestTestServer")]
    public void GivenIAmRunningARestTestServer()
    {
        var RestServer = new RestTestServer(RestCalls.RestPort);
        Context.Set<RestTestServer>(RestServer);
    }

    [Given(@"I have a button on Mainpage to call RestTestServer")]
    public void GivenIHaveAButtonOnMainpageToCallRestTestServer()
    {
        app.WaitForElement(AddOrGetPageTO<MainPageTO>().RestApiButton);
    }
    [Given(@"I have configured the App for RestCall")]
    public void GivenIHaveConfiguredTheAppForRestCall()
    {
        var RestServer = Context.Get<RestTestServer>();
        AddOrGetPageTO<MainPageTO>()
            .Invoke(Backdoor.SetRestInfo,
                   RestServer.GetUrl(RestCalls.TestEndPoint + RestCalls.QueryString));
    }
    [Given(@"I have registered an endpoint with the RestTestServer")]
    public void GivenIHaveRegisteredAnEndpointWithTheRestTestServer()
    {
        Context.Get<RestTestServer>().Register((context) =>
        {
            Context.Add(Fixtures.WasCalled, true);
            Context.Add("QueryString",
             context.Request.QueryString.GetValue<string>("key1"));
            context.Response.SendResponse("OK");
            return context;
        }, HttpMethod.GET, RestCalls.TestEndPoint);
    }

    [When(@"I tap the button")]
    public void WhenITapTheButton()
    {
        app.Tap(AddOrGetPageTO<MainPageTO>().RestApiButton);
    }

    [Then(@"The RestTestServer is called with correct payload")]
    public void ThenTheRestTestServerIsCalledWithCorrectPayload()
    {
        app.WaitFor(() => Context.ContainsKey(Fixtures.WasCalled),
                   "RestTestServer was not called!");
        Context.Get<string>("QueryString").ShouldEqual(RestCalls.QueryString);
    }

}
~~~

looking at the code above, to pass the steps we need to implement the following:

* A Button on the Main Page that would call the Api Service

~~~xml
<?xml version="1.0" encoding="UTF-8"?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms" 
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" 
    xmlns:static ="clr-namespace:MVVMFramework.Statics;assembly=MVVMFramework"
    x:Class="example.Pages.MainPage">
	<ContentPage.Content>
        <StackLayout Padding="30">
            <Button AutomationId="{x:Static static:UUID.RestApiButton}"
                Text="{x:Static static:UIStrings.RestApiButton}"
                Command="{Binding RestApiCommand}"/>
        </StackLayout>
	</ContentPage.Content>
</ContentPage>
~~~

* The Api Service that would use the Settings to find the endpoint and make the call. We put a RestService.cs in Services directory under example in the shared project.

~~~csharp
public class RestService
{
    public async Task<string> Call()
    {
        if (!CheckConnectivity())
            return "";
        var Client = new RestSharp.RestClient(Settings.ServerUrl);
        var request = new RestRequest(RestCalls.TestEndPoint);
        request.AddParameter("key1", "value1");
        request.AddParameter("key2", "value2");
        var response = await Client.ExecuteGetTaskAsync(request);
        return response.Content;
    }

    public bool CheckConnectivity() => !CrossConnectivity.IsSupported ||
                                 !CrossConnectivity.Current.IsConnected;
}
~~~

* A backdoor to allow the test to divert the call to our Test Server

~~~csharp
public static string Execute(JObject msg){
    switch((string)msg[Backdoor.Key]){
        case Backdoor.SetRestInfo:
            Settings.ServerUrl = (string)msg[Backdoor.Payload];
            break;
        default:
            return "Unknown Key: " + (string)msg[Backdoor.Key];
    }
    return "OK";
}
~~~

The Settings branch and the AppSettings.md file in the Docs directory have more information about how to Setup Backdoors.

# Next Steps

* For Webservice connectivity the same approach can be taken by using [Websocket Clients][] and tested with [Websocket Listener][].

* For AMQP message brokers, the same approach can be taken by using [AMQP NET lite][] which contains a [Test AMQP Broker][] so that it won't be necessary to have a full installation of RabbitMQ, Apache ActiveMQ or some similar application just for testing.
  * However, it is noteworthy that getting RabbitMQ to work on Xamarin based mobile projects is tricky at the moment. it may be best to defer until better tools are available.

[RestSharp]: https://www.nuget.org/packages/RestSharp
[Newtonsoft Json]: https://www.newtonsoft.com/json
[Json.Net]: https://www.newtonsoft.com/json/help/html/Introduction.htm
[samples]:https://www.newtonsoft.com/json/help/html/SerializeObject.htm
[Grapevine]: https://sukona.github.io/Grapevine/en/
[Connectivity Plugin]: https://github.com/jamesmontemagno/ConnectivityPlugin
[Websocket Clients]: https://blog.xamarin.com/developing-real-time-communication-apps-with-websocket/
[Websocket Listener]: http://vtortola.github.io/WebSocketListener/
[AMQP NET lite]: https://github.com/Azure/amqpnetlite/blob/master/docs/articles/hello_amqp.md
[Test AMQP Broker]: https://github.com/Azure/amqpnetlite/blob/master/test/Common/TestAmqpBroker.cs
