﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EFAuditer;
using ExpressiveAnnotations.Attributes;
using ntbs_service.Models.Enums;
using ntbs_service.Models.ReferenceEntities;
using ntbs_service.Models.Validations;

namespace ntbs_service.Models.Entities
{
    public class MBovisUnpasteurisedMilkConsumption : ModelBase, IHasRootEntityForAuditing
    {
        public int MBovisUnpasteurisedMilkConsumptionId { get; set; }
        public int NotificationId { get; set; }
        
        [Required]
        [AssertThat(nameof(YearOfConsumptionAfterBirth), ErrorMessage = ValidationMessages.DateShouldBeLaterThanDobYear)]
        [AssertThat(nameof(YearOfConsumptionNotInFuture), ErrorMessage = ValidationMessages.BeforeCurrentYear)]
        [Range(1900, 2100, ErrorMessage = ValidationMessages.InvalidYearForAttribute)]
        [Display(Name = "Year of consumption")]
        public int? YearOfConsumption { get; set; }

        [Required(ErrorMessage = ValidationMessages.RequiredSelect)]
        [Display(Name = "Product type")]
        public MilkProductType? MilkProductType { get; set; }
        
        [Required(ErrorMessage = ValidationMessages.RequiredSelect)]
        [Display(Name = "Frequency")]
        public ConsumptionFrequency? ConsumptionFrequency { get; set; }
        
        [Required(ErrorMessage = ValidationMessages.RequiredSelect)]
        [Display(Name = "Country")]
        public int? CountryId { get; set; }
        public virtual Country Country { get; set; }
        
        [MaxLength(150)]
        [RegularExpression(
            ValidationRegexes.CharacterValidationWithNumbersForwardSlashAndNewLine,
            ErrorMessage = ValidationMessages.StringWithNumbersAndForwardSlashFormat)]
        [Display(Name = "Other details")]
        public string OtherDetails { get; set; }
        
        public bool YearOfConsumptionAfterBirth => !DobYear.HasValue || YearOfConsumption >= DobYear;
        public bool YearOfConsumptionNotInFuture => YearOfConsumption <= DateTime.Now.Year;
        
        // For validation purposes only
        [NotMapped]
        public int? DobYear { get; set; }
        
        string IHasRootEntityForAuditing.RootEntityType => RootEntities.Notification;
        string IHasRootEntityForAuditing.RootId => NotificationId.ToString();
    }
}