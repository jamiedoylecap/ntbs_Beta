using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using ntbs_service.Models;
using ntbs_service.Services;
using ntbs_service.Pages_Search;
using Xunit;
using ntbs_service.Models.Enums;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ntbs_service_tests.UnitTests.Search
{
    public class SearchPageTest
    {
        private Mock<INotificationService> mockNotificationService;
        private Mock<ISearchService> mockSearchService;
        private Mock<NtbsContext> mockContext;
        public SearchPageTest() 
        {
            mockNotificationService = new Mock<INotificationService>();
            mockSearchService = new Mock<ISearchService>();
            mockContext = new Mock<NtbsContext>();
        }

        [Fact]
        public async Task OnGetAsync_PopulatesPageModel_WithSearchResults()
        {
            // Arrange
            IList<Sex> sexes = new List<Sex> {};
            var sexList = Task.FromResult(sexes);
            mockContext.Setup(s => s.GetAllSexesAsync()).Returns(sexList);

            mockNotificationService.Setup(s => s.GetBaseQueryableNotificationByStatus(It.IsAny<List<NotificationStatus>>())).Returns(new List<Notification> { new Notification() {NotificationId = 1}}.AsQueryable());
            
            var unionAndPaginateResult = Task.FromResult(new Tuple<IList<int>, int>(new List<int>() {1}, 1)); 
            mockSearchService.Setup(s => s.UnionAndPaginateQueryables(It.IsAny<IQueryable<Notification>>(), It.IsAny<IQueryable<Notification>>(), 
                It.IsAny<PaginationParameters>())).Returns(unionAndPaginateResult);

            var notifications = Task.FromResult(GetNotifications());
            mockNotificationService.Setup(s => s.GetNotificationsByIdAsync(It.IsAny<List<int>>())).Returns(notifications);

            var httpContext = new DefaultHttpContext() {};
            var modelState = new ModelStateDictionary();
            var actionContext = new ActionContext(httpContext, new Microsoft.AspNetCore.Routing.RouteData(), new PageActionDescriptor(), modelState);
            var pageContext = new PageContext(actionContext) {};

            var pageModel = new IndexModel(mockNotificationService.Object, mockSearchService.Object, mockContext.Object) {
                SearchParameters = new SearchParameters() {IdFilter = "1"},
                PageContext = pageContext
            };


            // Act
            await pageModel.OnGetAsync(1);

            // Assert
            var results = Assert.IsAssignableFrom<PaginatedList<NotificationBannerModel>>(pageModel.SearchResults);
            Assert.True(results.Count == 2);
            Assert.Equal("Bob", results[0].Name);
            Assert.Equal("Ross", results[1].Name);
        }

        public IList<int> GetNotificationIds() {
            return new List<int>() {1, 2};
        }

        public IEnumerable<Notification> GetNotifications()
        {
            var patient1 = new PatientDetails() { GivenName = "Bob" };
            var patient2 = new PatientDetails() { GivenName = "Ross" };

            return new List<Notification> { new Notification{ PatientDetails = patient1, NotificationId = 1 }, new Notification{ PatientDetails = patient2, NotificationId = 2 }};
        }
    }
}