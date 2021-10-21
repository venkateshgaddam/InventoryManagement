using Carrier.CCP.Common.Utils.Exception;
using System;
using System.ComponentModel.DataAnnotations;

namespace Carrier.CCP.Common.Validation
{
    /// <summary>
    ///     CCP validation class for guid object
    ///     1.Checks if validationContext is not null
    ///     2.Checks if specified property exists
    ///     3.Checks if specified property is of guid type
    ///     4.Checks if the field is required
    /// </summary>
    public class CCPValidationAttributeGuid : CCPValidationAttribute
    {
        public bool IsNullable { get; set; } = false;

        /// <summary>
        ///     To check whether the input guid is a per the dataenhancers rules specified.
        /// </summary>
        /// <param name="value">Type of <see cref="guid" />.</param>
        /// <param name="validationContext">The request validation context.</param>
        /// <exception cref="CcpValidationException">Thrown if <paramref name="validationContext" /> is null.</exception>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                //Check Datatype
                var isValid = CheckPropertyType(validationContext, IsNullable ? typeof(Guid?) : typeof(Guid));
                if (isValid != null) return isValid;

                // Check if we have validation for this class name.
                var dtoName = validationContext.ObjectType.Name;
                var validators = GetValidationMetadata(validationContext, dtoName);
                //Check if the validations are available in the store other wise throws an error to controller
                // Check if we have validation for this property name.
                if (validators == null || !validators.ContainsKey(validationContext.MemberName))
                    return RaiseDataEnhancersNullMessage(dtoName);

                //Validate
                var property = validators[validationContext.MemberName];
                return ValidateCCPAttribute(value, validationContext, property);
            }
            catch (Exception exception)
            {
                throw new CcpValidationException(exception.Message);
            }
        }

        protected override ValidationResult IsRequiredValid(object value, ValidationContext validationContext,
            ValidatorsValidatorProperty property)
        {
            if (!property.Required) return ValidationResult.Success;

            var errorMsg = $"The {property.Name} field is required and should not be empty Guid";
            ValidationAttribute validateRequired = new RequiredAttribute { ErrorMessage = errorMsg };
            if (value == null || (Guid)value == Guid.Empty || !validateRequired.IsValid(value))
            {
                var errorMessage = GetFormattedErrorMessage(errorMsg, validationContext.DisplayName);
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }

        protected override ValidationResult ValidateCCPAttribute(object value, ValidationContext validationContext,
            ValidatorsValidatorProperty property)
        {
            // Regular expression check.
            var result = base.IsRegExValid(value, validationContext, property);
            if (result != ValidationResult.Success)
                return result;

            // Is Required propety required check.
            result = IsRequiredValid(value, validationContext, property);
            if (result != ValidationResult.Success)
                return result;

            //Check Range
            return base.IsRangeValid(value, validationContext, property);
        }
    }
}