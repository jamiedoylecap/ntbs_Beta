﻿using System;
using System.ComponentModel.DataAnnotations;
using ntbs_service.Models.Enums;
using ntbs_service.Models.ReferenceEntities;

namespace ntbs_service.Models.Entities
{
    public abstract class Alert
    {
        public int AlertId { get; set; }
        [Display(Name = "NTBS Id")]
        public int? NotificationId { get; set; }
        public virtual Notification Notification { get; set; }
        public DateTime CreationDate { get; set; }
        public string TbServiceCode { get; set; }
        public virtual TBService TbService { get; set; }
        public string CaseManagerEmail { get; set; }
        public virtual CaseManager CaseManager { get; set; }
        public AlertStatus AlertStatus { get; set; }
        public DateTime? ClosureDate { get; set; }
        public string ClosingUserId { get; set; }
        [Display(Name = "Alert type")]
        public AlertType AlertType { get; set; }
        public virtual string ActionLink { get; }
        public virtual string Action { get; }
        [Display(Name = "Alert date")]
        public string FormattedCreationDate => FormatDate(CreationDate);
        [Display(Name = "Case manager")]
        public string CaseManagerFullName => CaseManager?.FullName ?? "System";
        [Display(Name = "TB Service")]
        public string TbServiceName => TbService?.Name;

        private string FormatDate(DateTime? date)
        {
            return date?.ToString("dd MMM yyyy");
        }
    }

}