using Carrier.CCP.Common.Utils.Exception;
using System;
using System.ComponentModel.DataAnnotations;

namespace Carrier.CCP.Common.Validation
{
    /// <summary>
    ///     CCP validation class for string object
    ///     1.Checks if validationContext is not null
    ///     2.Checks if specified property exists
    ///     3.Checks if specified property is of string type
    ///     4.(i).Checks if the field is matchin regular expression provided (OR)
    ///     (ii).Checks if the field is in between Max and Min length provided (OR)
    ///     (iii).Checks if the field is more than Min length provided (OR)
    ///     (iv).Checks if the field is below Max length provided (OR)
    ///     (v).Checks if the field is required
    /// </summary>
    public class CCPValidationAttributeString : CCPValidationAttribute
    {
        /// <summary>
        ///     To check whether the input string is a per the dataenhancers rules specified.
        /// </summary>
        /// <param name="value">Type of <see cref="string" />.</param>
        /// <param name="validationContext">The request validation context.</param>
        /// <exception cref="CcpValidationException">Thrown if <paramref name="validationContext" /> is null.</exception>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                //Check Datatype
                var isValid = CheckPropertyType(validationContext, typeof(string));
                if (isValid != null) return isValid;

                // Check if we have validation for this class name.
                var dtoName = validationContext.ObjectType.Name;
                var validators = GetValidationMetadata(validationContext, dtoName);
                //Check if the validations are available in the store other wise throws an error to controller
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
    }
}