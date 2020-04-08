﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ntbs_service.Helpers;
using ntbs_service.Models.Enums;

namespace ntbs_service.Models.Entities
{
    public partial class Notification
    {
        public string SitesOfDiseaseList => CreateSitesOfDiseaseString();
        [Display(Name = "Date created")]
        public string FormattedCreationDate => CreationDate.ConvertToString();
        [Display(Name = "Date notified")]
        public string FormattedNotificationDate => NotificationDate.ConvertToString();
        public int? AgeAtNotification => GetAgeAtTimeOfNotification();
        public string LegacyId => LTBRID ?? ETSID;
        public bool TransferRequestPending => Alerts?.Any(x => x.AlertType == AlertType.TransferRequest && x.AlertStatus == AlertStatus.Open) == true;
        public bool IsLastLinkedNotificationOverOneYearOld => DateTime.Now > Group.Notifications.DefaultIfEmpty(this).Last().CreationDate.AddYears(1);
        public bool IsMdr => ClinicalDetails.IsMDRTreatment || DrugResistanceProfile.DrugResistanceProfileString == "RR/MDR/XDR";
        public bool IsMBovis => string.Equals("M. bovis", DrugResistanceProfile.Species, StringComparison.InvariantCultureIgnoreCase);
        
        public override bool? IsLegacy => LTBRID != null || ETSID != null;

        private string CreateSitesOfDiseaseString()
        {
            if (NotificationSites == null)
            {
                return string.Empty;
            }

            var siteNames = NotificationSites.Select(ns => ns.Site)
                .Where(ns => ns != null)
                .Select(s => s.Description);
            return string.Join(", ", siteNames);
        }

        private int? GetAgeAtTimeOfNotification()
        {
            if (NotificationDate == null || PatientDetails?.Dob == null)
            {
                return null;
            }

            var notificationDate = (DateTime)NotificationDate;
            var birthDate = (DateTime)PatientDetails.Dob;

            var yearDiff = notificationDate.Year - birthDate.Year;
            if ((birthDate.Month * 100 + birthDate.Day) > (notificationDate.Month * 100 + notificationDate.Day))
            {
                yearDiff -= 1;
            }
            return yearDiff;
        }
    }
}
