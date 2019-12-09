﻿using System.ComponentModel.DataAnnotations.Schema;
using ntbs_service.Models.Enums;

namespace ntbs_service.Models.Entities
{
    public class ModelBase
    {
        [NotMapped]
        public bool ShouldValidateFull { get; set; }

        /* 
        * Full Validation is done if the form is being submitted or already been submitted.
        * Since the ModelBase does not have direct access to NotificationStatus, 
        * this methods is used to set Validation State from ViewModel
        */ 
        public void SetFullValidation(NotificationStatus notificationStatus, bool isBeingSubmitted = false) 
        {
            ShouldValidateFull = isBeingSubmitted || notificationStatus != NotificationStatus.Draft;
        }
    }
}