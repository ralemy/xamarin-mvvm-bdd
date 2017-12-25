Feature: Barcode
    I need to have a page that uses the phone camera to read barcodes.
    
@barcode_structure
Scenario: Main page should have a button to navigate to barcode page
    Given I am in the main page
    And the main page has a barcode button
    When I tap the barcode button
    Then the application goes to the barcode page
