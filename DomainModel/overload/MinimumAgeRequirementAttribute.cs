using System.ComponentModel.DataAnnotations;

namespace DomainModel.overload {
    public class MinimumAgeRequirementAttribute : ValidationAttribute {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var age = ((DateTime)value).getAge();
            if (age >= 16 && age !> 120) {
                return ValidationResult.Success;
            } else {
                return new ValidationResult("Age needs to be minimum of 16 and can't be higher than 120");
            }
        }
    }
}
