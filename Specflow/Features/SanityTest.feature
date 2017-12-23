Feature: _Sanity Test
    As a programmer and developer
    who needs to develop app in Xamarin using BDD technique
    I need a simple test to show that the framework is installed and working

@Sanity
Scenario: 01Simple assertions should work across the scenario
    Given I store 50 into the feature context under key 'sanityTest'
    When I read the key 'sanityTest' from the context and add 10 to it and store it in 'sanityWhen'
    Then the 'sanityWhen' key in context should be 60.

@Sanity
Scenario: 02RestTestServer should work
    Given I start the RestTestServer on port '3434'
    And I register and endpoint on '/api/sanity' to return 'Server Sane'
    When I call '/api/sanity'
    Then the response would be 'Server Sane'        