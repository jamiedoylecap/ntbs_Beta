﻿using System.ComponentModel.DataAnnotations;
using ntbs_service.Helpers;
using ntbs_service.Models.Enums;

namespace ntbs_service.Models.Entities
{
    public class UnmatchedLabResultAlert : Alert
    {
        public UnmatchedLabResultAlert()
        {
            AlertType = AlertType.UnmatchedLabResult;
        }

        public string SpecimenId { get; set; }
        
        public override string Action => "Please review potential matches identified for notifications in your service";
        public override string ActionLink => RouteHelper.GetUnmatchedSpecimenPath(SpecimenId);
        public override bool NotDismissable => true;
    }
}
