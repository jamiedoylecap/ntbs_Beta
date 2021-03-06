Feature: Notification creation
    Happy and error paths for notification creation
    Notification deletion

    Background: Create new notification
        Given I am on the Search page
        When I enter 1 into 'SearchParameters_IdFilter'
        And I click on the 'search-button' button
        Then I should be on the Search page
        When I click on the 'create-button' button
        Then I should be on the PatientDetails page

    Scenario: Create and submit notification without errors
        # PatientDetails page
        When I enter Test into 'PatientDetails_GivenName'
        And I enter User into 'PatientDetails_FamilyName'
        And I enter 1 into 'FormattedDob_Day'
        And I enter 1 into 'FormattedDob_Month'
        And I enter 1970 into 'FormattedDob_Year'
        And I select radio value 'sexId-1'
        And I select Mixed - White and Asian for 'PatientDetails_EthnicityId'
        And I select radio value 'nhs-number-unknown'
        And I enter Afghanistan into 'PatientDetails_CountryId'
        And I wait
        And I select radio value 'postcode-no'
        And I click on the 'save-button' button

        # HospitalDetails page
        Then I should be on the HospitalDetails page
        When I enter 1 into 'FormattedNotificationDate_Day'
        And I enter 1 into 'FormattedNotificationDate_Month'
        And I enter 2019 into 'FormattedNotificationDate_Year'
        And I select Ashford Hospital for 'HospitalDetails_TBServiceCode'
        # Wait until javascript has populated the hospital dropdown
        And I wait
        And I select ASHFORD HOSPITAL for 'HospitalDetails_HospitalId'
        And I click on the 'save-button' button

        # Clinical details page
        Then I should be on the ClinicalDetails page
        When I check 'NotificationSiteMap_PULMONARY_'
        And I enter 1 into 'FormattedDiagnosisDate_Day'
        And I enter 1 into 'FormattedDiagnosisDate_Month'
        And I enter 2018 into 'FormattedDiagnosisDate_Year'
        And I click on the 'save-button' button
        
        # Test results page
        Then I should be on the TestResults page
        When I select radio value 'test-carried-out-yes'
        And I click on the 'add-new-manual-test-result-button' button
        And I enter 1 into 'FormattedTestDate_Day'
        And I enter 1 into 'FormattedTestDate_Month'
        And I enter 2012 into 'FormattedTestDate_Year'
        And I enter Smear into 'TestResultForEdit_ManualTestTypeId'
        And I enter BronchialWashings into 'TestResultForEdit_SampleTypeId'
        And I enter Negative into 'TestResultForEdit_Result'
        And I click on the 'save-test-result-button' button
        And I click on the 'save-button' button
        
        # Contact tracing page 
        Then I should be on the ContactTracing page
        When I click on the 'save-button' button

        # Social risk factors page
        Then I should be on the SocialRiskFactors page
        When I click on the 'save-button' button

        # Travel/visitor history page
        Then I should be on the Travel page
        When I click on the 'save-button' button

        # Co-morbidities and immunosuppression page
        Then I should be on the Comorbidities page
        When I click on the 'save-button' button

        # Social context addresses page
        Then I should be on the SocialContextAddresses page
        When I click on the 'save-button' button
        
        # Social context venues page
        Then I should be on the SocialContextVenues page
        When I click on the 'save-button' button

        # Previous History page
        Then I should be on the PreviousHistory page
        When I click on the 'save-button' button

        # Treatment Events page
        Then I should be on the TreatmentEvents page
        When I click on the 'save-button' button

        # Treatment Events page again + submission
        Then I should be on the TreatmentEvents page
        When I click on the 'submit-button' button
      
        # Notification overview + checks that submission went correctly
        Then I should see the Notification
        And I can see the starting event 'Diagnosis made` dated `01 Jan 2018'

    Scenario: Create and submit notification without content
        When I click on the 'submit-button' button
        Then I should be on the PatientDetails page
        And I should see all submission error messages

    Scenario: Create and delete notification draft
        When I click on the 'delete-draft-button' button
        Then I should be on the Delete page
        When I click on the 'confirm-deletion-button' button
        Then I should be on the Confirm page
        When I click on the 'return-to-homepage' button
        Then I should be on the Homepage
