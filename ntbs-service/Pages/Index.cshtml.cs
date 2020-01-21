﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ntbs_service.DataAccess;
using ntbs_service.Models.Entities;
using ntbs_service.Models.Enums;
using ntbs_service.Models.ReferenceEntities;
using ntbs_service.Services;

namespace ntbs_service.Pages
{
    public class IndexModel : PageModel
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IAlertRepository _alertRepository;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserService _userService;
        private readonly IHomepageKpiService _homepageKpiService;

        public IndexModel(INotificationRepository notificationRepository, 
            IAlertRepository alertRepository, 
            IAuthorizationService authorizationService, 
            IUserService userService, 
            IHomepageKpiService homepageKpiService)
        {
            _notificationRepository = notificationRepository;
            _authorizationService = authorizationService;
            _alertRepository = alertRepository;
            _userService = userService;
            _homepageKpiService = homepageKpiService;
        }

        public IList<Alert> Alerts { get; set; }
        public IList<Notification> DraftNotifications { get;set; }
        public IList<Notification> RecentNotifications { get;set; }
        public SelectList TbServices { get; set; }
        public SelectList Codes { get; set; }
        public IEnumerable<HomepageKpi> HomepageKpiDetails { get; set; }

        public async Task OnGetAsync()
        {
            await SetUserNotificationsAsync();
            await SetUserAlertsAsync();
            await SetHomepageKpiDetails();
        }

        private async Task SetHomepageKpiDetails()
        {
            var userType = _userService.GetUserType(User);
            if (userType == UserType.NhsUser)
            {
                var tbServiceCodes = (await _userService.GetTbServicesAsync(User))
                    .Select(x => x.Code)
                    .ToList();
                Codes = new SelectList(tbServiceCodes);
                HomepageKpiDetails = await _homepageKpiService.GetKpiForTbService(tbServiceCodes);
            }
            else
            {
                var phecCodes = (await _userService.GetPhecCodesAsync(User)).ToList();
                Codes = new SelectList(phecCodes);
                HomepageKpiDetails = await _homepageKpiService.GetKpiForPhec(phecCodes);
            }
        }

        private async Task SetUserNotificationsAsync()
        {
            var draftNotificationsQueryable = _notificationRepository.GetDraftNotificationsIQueryable();
            var recentNotificationsQueryable = _notificationRepository.GetRecentNotificationsIQueryable();
            DraftNotifications = (await _authorizationService.FilterNotificationsByUserAsync(User, draftNotificationsQueryable)).Take(10).ToList();
            RecentNotifications = (await _authorizationService.FilterNotificationsByUserAsync(User, recentNotificationsQueryable)).Take(10).ToList();
        }

        private async Task SetUserAlertsAsync()
        {
            var services = await _userService.GetTbServicesAsync(User);
            var tbServiceCodes = services.Select(s => s.Code);
            TbServices = new SelectList(services, nameof(TBService.Code), nameof(TBService.Name));
            Alerts = await _alertRepository.GetAlertsByTbServiceCodesAsync(tbServiceCodes);
        }
    }
}
