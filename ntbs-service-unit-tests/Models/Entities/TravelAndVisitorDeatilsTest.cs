﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ntbs_service.Models.Entities;
using Xunit;

namespace ntbs_service_unit_tests.Models.Entities
{
    public class TravelAndVisitorDetailsTest
    {
        public static IEnumerable<object[]> BaseDetails()
        {
            yield return new object[] {new TravelDetails {ShouldValidateFull = false, HasTravel = true}};
            yield return new object[] {new TravelDetails {ShouldValidateFull = true, HasTravel = true}};
            yield return new object[] {new VisitorDetails {ShouldValidateFull = false, HasVisitor = true}};
            yield return new object[] {new VisitorDetails {ShouldValidateFull = true, HasVisitor = true}};
        }

        [Theory, MemberData(nameof(BaseDetails))]
        public void SelectingYes_AndLeavingRestBlank_IsValidInDraftAndFullModes(ITravelOrVisitorDetails details)
        {
            // Arrange
            var validationResults = new List<ValidationResult>();
            
            // Act
            var isValid = Validator.TryValidateObject(details, new ValidationContext(details), validationResults, true); 

            // Assert
            Assert.True(isValid, "Expected details to be valid");
        }
    }
}
