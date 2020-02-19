﻿using ntbs_service.Helpers;
using ntbs_service.Models.Enums;

namespace ntbs_service.Models.Entities
{
    public class DataQualityTreatmentOutcome24 : Alert
    {
        public override string Action => 
            "No treatment outcome at 24 months can be found, please provide treatment outcome with appropriate date";

        public override string ActionLink =>
            RouteHelper.GetNotificationOverviewPathWithSectionAnchor(
                // ReSharper disable once PossibleInvalidOperationException
                NotificationId.Value,
                NotificationSubPaths.EditTreatmentEvents);

        public DataQualityTreatmentOutcome24()
        {
            AlertType = AlertType.DataQualityTreatmentOutcome24;
        }
    }
}