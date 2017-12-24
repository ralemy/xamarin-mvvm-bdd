Feature: Settings Page
    To have a configurable application 
    I need a settings page where parameters can be stored and updated.
    
@Settings_Page
Scenario: 01_Should Navigate from Main to Settings Page
    Given I am in Main Page
    And There is a 'Settings' button on the page
    When I press the 'Settings' button
    Then the application will Navigate to Settings Page