using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.API.Validation
{
    /// <summary>
    /// Represents a custom validation attribute to ensure a date is in the future.
    /// </summary>
    public class FutureDateAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validates if the provided date is in the future.
        /// </summary>
        /// <param name="value">The date value to validate.</param>
        /// <param name="validationContext">The validation context.</param>
        /// <returns>A <see cref="ValidationResult"/> indicating whether the validation was successful.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var date = (DateTime?)value;

            if (!date.HasValue || date < DateTime.Today)
            {
                return new ValidationResult("Expiration date must be today or in the future.");
            }

            return ValidationResult.Success;
        }
    }
}
