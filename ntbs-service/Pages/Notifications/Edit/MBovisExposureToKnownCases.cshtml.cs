﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ntbs_service.DataAccess;
using ntbs_service.Helpers;
using ntbs_service.Models;
using ntbs_service.Models.Entities;
using ntbs_service.Services;

namespace ntbs_service.Pages.Notifications.Edit
{
    public class MBovisExposureToKnownCasesModel : NotificationEditModelBase
    {
        public MBovisExposureToKnownCasesModel(
            INotificationService notificationService,
            IAuthorizationService authorizationService,
            INotificationRepository notificationRepository,
            IAlertRepository alertRepository) : base(notificationService, authorizationService,
                notificationRepository, alertRepository)
        {
            CurrentPage = NotificationSubPaths.EditMBovisExposureToKnownCases;
        }

        [BindProperty] 
        public MBovisDetails MBovisDetails { get; set; }

        protected override async Task<IActionResult> PrepareAndDisplayPageAsync(bool isBeingSubmitted)
        {
            if (!Notification.IsMBovis)
            {
                return NotFound();
            }
            
            MBovisDetails = Notification.MBovisDetails;
            
            await SetNotificationProperties(isBeingSubmitted, MBovisDetails);

            if (MBovisDetails.ShouldValidateFull)
            {
                TryValidateModel(this);
            }

            return Page();
        }
        
        protected override IActionResult RedirectToCreate()
        {
            return RedirectToPage("./Items/NewMBovisExposureToKnownCase", new { NotificationId });
        }
        
        protected override IActionResult RedirectForDraft(bool isBeingSubmitted)
        {
            return RedirectToPage("./MBovisUnpasteurisedMilkConsumptions", new { NotificationId, isBeingSubmitted });
        }
        
        protected override async Task ValidateAndSave()
        {
            if (ActionName == ActionNameString.Create)
            {
                MBovisDetails.HasExposureToKnownCases = true;
            }
            // Set the collection so it can be included in the validation
            MBovisDetails.MBovisExposureToKnownCases = Notification.MBovisDetails.MBovisExposureToKnownCases;
            MBovisDetails.ProceedingToAdd = ActionName == ActionNameString.Create;
            MBovisDetails.SetValidationContext(Notification);
            
            if (TryValidateModel(MBovisDetails, nameof(MBovisDetails)))
            {
                await Service.UpdateMBovisDetailsExposureToKnownCasesAsync(Notification, MBovisDetails);
            }
        }
        
        public ContentResult OnGetValidateMBovisDetailsProperty(string key, string value, bool shouldValidateFull)
        {
            return ValidationService.GetPropertyValidationResult<MBovisDetails>(key, value, shouldValidateFull);
        }
        
        protected override async Task<Notification> GetNotificationAsync(int notificationId)
        {
            return await NotificationRepository.GetNotificationWithMBovisExposureToKnownCasesAsync(notificationId);
        }
    }
}
