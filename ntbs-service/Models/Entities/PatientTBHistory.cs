﻿using System.ComponentModel.DataAnnotations;
using ExpressiveAnnotations.Attributes;
using Microsoft.EntityFrameworkCore;
using ntbs_service.Models.Validations;

namespace ntbs_service.Models.Entities
{
    [Owned]
    public class PatientTBHistory : ModelBase
    {
        public bool? NotPreviouslyHadTB { get; set; }
        
        // NotPreviouslyHadTB flag can bu null and therefore we need exact match on "false" to avoid seeing null as false
        [RequiredIf("ShouldValidateFull && NotPreviouslyHadTB == false", ErrorMessage = ValidationMessages.FieldRequired)]
        [Range(1900, 2000, ErrorMessage = ValidationMessages.PreviousTBDiagnosisYear)]
        public int? PreviousTBDiagnosisYear { get; set; }
    }
}