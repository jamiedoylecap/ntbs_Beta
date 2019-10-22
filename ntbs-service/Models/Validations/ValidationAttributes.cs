using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using ntbs_service.Models;
using ntbs_service.Models.Validations;

namespace ntbs_service.Models.Validations
{
    public class ValidDateAttribute : RangeAttribute
    {
        private readonly string StartDate;
        public ValidDateAttribute(string startDateString) : base(typeof(DateTime),
            startDateString, DateTime.Now.ToShortDateString())
        {
            StartDate = startDateString;
        }

        public override string FormatErrorMessage(string name)
        {
            return ValidationMessages.DateValidityRange(StartDate);
        }
    }

    public class OnlyOneTrue : ValidationAttribute
    {

        public string ComparisonValue { get; set; }
        public OnlyOneTrue(string comparisonValue)
        {
            ComparisonValue = comparisonValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object instance = validationContext.ObjectInstance;
            var propertyToCompare = instance.GetType().GetProperty(ComparisonValue);
            object valueToCompare = propertyToCompare.GetValue(instance);

            if (IsTruthy(valueToCompare) && IsTruthy(value)) {
                return new ValidationResult(ErrorMessage);
            }
            return null;
        }

        private bool IsTruthy(object value)
        {
            return value != null && (bool)value;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AtLeastOnePropertyAttribute : ValidationAttribute
    {
        private string[] PropertyList { get; set; }

        public AtLeastOnePropertyAttribute(params string[] propertyList)
        {
            this.PropertyList  = propertyList;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) 
        {
            foreach (var propertyName in PropertyList)
            {
                var propertyInfo = value.GetType().GetProperty(propertyName);
                if (propertyInfo != null && propertyInfo.GetValue(value, null) != null)
                {
                    return null;
                }
            }
            return new ValidationResult(ErrorMessage);
        }
    }

    public class ValidPartialDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is PartialDate partialDate) || partialDate.IsEmpty())
            {
                return null;
            }
            if (!string.IsNullOrEmpty(partialDate.Day)) {
                if(string.IsNullOrEmpty(partialDate.Month) || string.IsNullOrEmpty(partialDate.Year)) {
                    return new ValidationResult(ValidationMessages.YearIfMonthRequired);
                }
            }
            if(!string.IsNullOrEmpty(partialDate.Month)) {
                if(string.IsNullOrEmpty(partialDate.Year)) {
                    return new ValidationResult(ValidationMessages.YearRequired);
                }
            }

            bool canConvert = partialDate.TryConvertToDateTimeRange(out _, out _);
            if (!canConvert) {
                return new ValidationResult(ErrorMessage);
            }
            return null;
        }
    }

    public class ValidFormattedDateCanConvertToDatetimeAttribute : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is FormattedDate formattedDate) || formattedDate.IsEmpty())
            {
                return null;
            }

            bool canConvert = formattedDate.TryConvertToDateTime(out _);
            if (!canConvert) {
                return new ValidationResult(ErrorMessage);
            }
            return null;
        }
    }
}
