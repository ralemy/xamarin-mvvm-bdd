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

