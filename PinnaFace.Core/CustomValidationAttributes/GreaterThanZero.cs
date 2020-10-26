using System.ComponentModel.DataAnnotations;

namespace PinnaFace.Core.CustomValidationAttributes
{
    public class GreaterThanZero : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //if (value != null)
            //{
            //    if (int.Parse(value.ToString()) <= 0)
            //    {
            //        return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            //    }
            //}
            return ValidationResult.Success;
        }
    }
}
