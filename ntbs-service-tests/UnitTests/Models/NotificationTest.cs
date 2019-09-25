using System;
using System.Collections.Generic;
using ntbs_service.Models;
using Xunit;

namespace ntbs_service_tests.UnitTests.Models
{
    public class NotificationTest
    {

        Notification TestNotification = new Notification {
            PatientDetails = new PatientDetails {},
            ClinicalDetails = new ClinicalDetails {},
            SocialRiskFactors = new SocialRiskFactors {}
        };

        [Fact]
        public void FormatsFullNameCorrectly()
        {
            // Arrange
            TestNotification.PatientDetails.FamilyName = "example";
            TestNotification.PatientDetails.GivenName = "name";

            // Act
            var fullName = TestNotification.FullName;

            // Assert
            Assert.Equal("EXAMPLE, name", fullName);
        }

        [Fact]
        public void FormatsPostcodeCorrectly()
        {
            // Arrange
            TestNotification.PatientDetails.NoFixedAbode = false;
            TestNotification.PatientDetails.Postcode = "NW 123 RT";

            // Act
            var postcode = TestNotification.FormattedNoAbodeOrPostcodeString;

            // Assert
            Assert.Equal("NW12 3RT", postcode);
        }

        [Fact]
        public void FormatsDateCorrectly() 
        {
            // Arrange
            TestNotification.ClinicalDetails.SymptomStartDate = new DateTime(2000, 1, 1);
            
            // Act
            var formattedDate = TestNotification.FormattedSymptomStartDate;

            // Assert
            Assert.Equal("01-Jan-2000", formattedDate);
        }

        [Fact]
        public void CalculatesDaysBetweenDates() 
        {
            // Arrange
            TestNotification.ClinicalDetails.SymptomStartDate = new DateTime(2000, 1, 1);
            TestNotification.ClinicalDetails.PresentationDate = new DateTime(2000, 1, 4);
            
            // Act
            var days = TestNotification.DaysFromOnsetToPresentation;

            // Assert
            Assert.Equal(3, days);
        }

        [Fact]
        public void CreatesSocialRiskTimePeriodsStringCorrectly() 
        {
            // Arrange
            TestNotification.SocialRiskFactors.RiskFactorDrugs = new RiskFactorBase {
                Status = ntbs_service.Models.Enums.Status.Yes,
                        IsCurrent = true,
                        InPastFiveYears = false,
                        MoreThanFiveYearsAgo = true
            };
            
            // Act
            var timePeriods = TestNotification.DrugRiskFactorTimePeriods;

            // Assert
            Assert.Equal("current, more than 5 years ago", timePeriods);
        }

        [Fact]
        public void CreatesVaccinationStateStringCorrectly() {
            // Arrange
            TestNotification.ClinicalDetails.BCGVaccinationState = ntbs_service.Models.Enums.Status.Yes;
            TestNotification.ClinicalDetails.BCGVaccinationYear = 2000;
            
            // Act
            var stateAndYear = TestNotification.BCGVaccinationStateAndYear;

            // Assert
            Assert.Equal("Yes - 2000", stateAndYear);
        }
    }
}