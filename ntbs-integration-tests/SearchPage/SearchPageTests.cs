using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using ntbs_integration_tests.Helpers;
using ntbs_service;
using ntbs_service.Models.Validations;
using Xunit;

namespace ntbs_integration_tests.NotificationPages
{
    public class SearchPageTests : TestRunnerBase
    {
        protected override string PageRoute => Routes.SearchPage;

        public SearchPageTests(NtbsWebApplicationFactory<Startup> factory) : base(factory) {}

        [Fact]
        public async Task GetSearch_ReturnsPageWithModelErrors_IfSearchNotValid()
        {
            // Arrange
            var initialPage = await client.GetAsync(GetPageRouteForId(Utilities.DRAFT_ID));
            var pageContent = await GetDocumentAsync(initialPage);

            var formData = new Dictionary<string, string>
            {
                ["SearchParameters.IdFilter"] = "ABC",
                ["SearchParameters.FamilyName"] = "111",
                ["SearchParameters.GivenName"] = "111",
                ["SearchParameters.PartialDob.Day"] = "31",
                ["SearchParameters.PartialDob.Month"] = "13",
                ["SearchParameters.PartialDob.Year"] = "1899",
                ["SearchParameters.PartialNotificationDate.Day"] = "31",
                ["SearchParameters.PartialNotificationDate.Month"] = "13",
                ["SearchParameters.PartialNotificationDate.Year"] = "1899",
                ["SearchParameters.Postcode"] = "$$$",
            };

            // Act
            var result = await SendGetFormWithData(pageContent, formData);

            // Assert
            var resultDocument = await GetDocumentAsync(result);
            Assert.Equal(FullErrorMessage(ValidationMessages.NumberFormat), resultDocument.GetError("id"));
            Assert.Equal(FullErrorMessage(ValidationMessages.StandardStringFormat), resultDocument.GetError("family-name"));
            Assert.Equal(FullErrorMessage(ValidationMessages.StandardStringFormat), resultDocument.GetError("given-name"));
            Assert.Equal(FullErrorMessage(ValidationMessages.StandardStringWithNumbersFormat), resultDocument.GetError("postcode"));
            Assert.Equal(FullErrorMessage(ValidationMessages.InvalidDate), resultDocument.GetError("dob"));
            Assert.Equal(FullErrorMessage(ValidationMessages.InvalidDate), resultDocument.GetError("notification-date"));
        }


        [Fact]
        public async Task GetSearch_ReturnsPageWithMatchingResult_IfSearchValid()
        {
            // Arrange
            var initialPage = await client.GetAsync(GetPageRouteForId(Utilities.DRAFT_ID));
            var pageContent = await GetDocumentAsync(initialPage);
            var formData = new Dictionary<string, string>
            {
                ["SearchParameters.IdFilter"] = Utilities.DRAFT_ID.ToString()
            };

            // Act
            var result = await SendGetFormWithData(pageContent, formData);

            // Assert
            var resultDocument = await GetDocumentAsync(result);
            
            Assert.Equal(" #1 ", resultDocument.QuerySelector("a[id='notification-banner-id']").TextContent);
            
        }

    }
}