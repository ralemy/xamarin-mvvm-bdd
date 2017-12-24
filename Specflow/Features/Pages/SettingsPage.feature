Feature: Settings Page
    To have a configurable application 
    I need a settings page where parameters can be stored and updated.
    
@Settings_Page
Scenario: 01_Should Navigate from Main to Settings Page
    Given I am in Main Page
    And There is a 'Settings' button on the page
    When I press the 'Settings' button
    Then the application will Navigate to Settings Page
    And the Settings Page has a UseHttps Switch

@Settings_Page
Scenario: 02_Should have a switch to Use Https
    Given I am in Settings page
    And  the settings page has a switch to select Https protocol
    And  I record the value of Settings.UseHttps
    When I tap the Use Https switch 
    Then the Value of the Settings.UseHttps will toggle