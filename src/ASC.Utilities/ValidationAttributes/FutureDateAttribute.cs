using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ASC.Utilities.ValidationAttributes
{

    /// <summary>
    ///  A Service Request cannot be submitted if Requested Date is after 90 days from current date.
    ///  A Customer cannot submit a service request, if admin has denied his service request in past 90 days.
    ///  This class implements ValidationAttribute and IClientModelValidator which are used to achieve 
    ///  server and client side validations respectively.
    ///  The validation which we have implemented is server side validation.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class FutureDateAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly int _days;
        private readonly string _errorMessage = "Date cannot be after {0} days from current date.";


        private FutureDateAttribute() { }
        public FutureDateAttribute(int days)
        {
            _days = days;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date > DateTime.UtcNow.AddDays(_days))
                {
                    return new ValidationResult(string.Format(_errorMessage, _days));
                }
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage);
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val-futuredate", string.Format(_errorMessage, _days));
            context.Attributes.Add("data-val-futuredate-days", _days.ToString());
        }
    }
}
