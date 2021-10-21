using Carrier.CCP.Common.Utility.Helper;
using Carrier.CCP.Common.Validation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace Carrier.CCP.Common.Validation
{
    /// <summary>
    ///     CCP base class for validation which fetches the data enhancers to be validate the model against
    /// </summary>
    public class CCPValidationAttribute : ValidationAttribute
    {
        public string BusinessCodeHeader { get; set; } = string.Empty;

        /// <summary>
        ///     Gets the validation metadata given the validation context and a model name
        /// </summary>
        /// <param name="validationContext"></param>
        /// <param name="dtoName"></param>
        /// <returns></returns>
        protected IDictionary<string, ValidatorsValidatorProperty> GetValidationMetadata(
            ValidationContext validationContext, string dtoName)
        {
            var businessCode = GetBusinessCode(validationContext);
            var validationObj = (IValidationCacheService)validationContext.GetService(typeof(IValidationCacheService));
            if (validationObj != null) return validationObj.TryGetValidationData(dtoName, businessCode);
            return null;
        }

        /// <summary>
        ///     Checks if the dataenhancers are available, otherwise returns an error
        /// </summary>
        /// <param name="dataenhancers"></param>
        /// <param name="dtoName"></param>
        /// <returns></returns>
        protected ValidationResult RaiseDataEnhancersNullMessage(string dtoName)
        {
            var errorMessage = FormatErrorMessage(dtoName);
            return new ValidationResult(errorMessage);
        }

        /// <summary>
        ///     Gets the business code of the current request , in case of failure the method returns default business code(i.e
        ///     ccp) to support fall back
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected string GetBusinessCode(ValidationContext validationContext)
        {

            try
            {
                var token = GetTokenProperty(validationContext);
                if (string.IsNullOrEmpty(token))
                    return null;
                return GetBusinessSchema(validationContext, token);
            }
            catch
            {
                return null;
            }
        }
        protected string GetTokenProperty(ValidationContext validationContext)
        {
            var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            var httpContext = httpContextAccessor?.HttpContext;
            var token = httpContext.User?.Claims.Where(x => x.Type == BusinessCodeHeader).FirstOrDefault()?.Value;
            if (!string.IsNullOrEmpty(token)) return token;
            
            if (httpContext.Request?.Headers != null)
            {
                httpContext.Request.Headers.TryGetValue(BusinessCodeHeader, out StringValues htoken);
                return htoken.ToString();
            }
            return null;
        }
        protected ValidationResult CheckPropertyType(ValidationContext validationContext, Type expectedType)
        {
            if (validationContext == null)
                throw new ArgumentNullException($"The validationContext is null {nameof(validationContext)}");

            var propertyInfo = validationContext.ObjectType.GetProperty(validationContext.MemberName);

            if (propertyInfo == null)
                throw new ArgumentNullException(
                    $"The object does not contain any property with name '{validationContext.MemberName}'");

            if (propertyInfo.PropertyType != expectedType)
            {
                var errorMessage = GetFormattedErrorMessage(
                    $"The {nameof(CCPValidationAttributeString)} is not valid on property type {propertyInfo.PropertyType}." +
                    $" This Attribute is only valid on {typeof(string)} type.", validationContext.DisplayName);
                return new ValidationResult(errorMessage);
            }

            return null;
        }

        protected string GetFormattedErrorMessage(string errorMessage, string propertyName)
        {
            return string.Format(CultureInfo.InvariantCulture, errorMessage, propertyName);
        }

        protected virtual ValidationResult IsRegExValid(object value, ValidationContext validationContext,
            ValidatorsValidatorProperty property)
        {
            // Regular expression check.
            if (string.IsNullOrEmpty(property.RegularExpression))
                return ValidationResult.Success;

            ValidationAttribute validateRegex = new RegularExpressionAttribute(property.RegularExpression)
            { ErrorMessage = property.ErrorMessage };
            if (validateRegex != null && !validateRegex.IsValid(value))
            {
                var errorMessage = GetFormattedErrorMessage(property.ErrorMessage, validationContext.DisplayName);
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }

        protected virtual ValidationResult IsRequiredValid(object value, ValidationContext validationContext,
            ValidatorsValidatorProperty property)
        {
            if (!property.Required) return ValidationResult.Success;

            var errorMsg = $"The {property.Name} field is required.";
            ValidationAttribute validateRequired = new RequiredAttribute { ErrorMessage = errorMsg };
            if (value == null || !validateRequired.IsValid(value))
            {
                var errorMessage = GetFormattedErrorMessage(errorMsg, validationContext.DisplayName);
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }

        protected virtual ValidationResult IsRangeValid(object value, ValidationContext validationContext,
            ValidatorsValidatorProperty property)
        {
            ValidationAttribute validateAttribute = null;
            // String length check.
            if (!string.IsNullOrEmpty(property.MinValue) && !string.IsNullOrEmpty(property.MaxValue))
                validateAttribute = new StringLengthAttribute(Convert.ToInt32(property.MaxValue))
                { MinimumLength = Convert.ToInt32(property.MinValue), ErrorMessage = property.ErrorMessage };
            // Minimum lenght check - only.
            else if (!string.IsNullOrEmpty(property.MinValue))
                validateAttribute = new MinLengthAttribute(Convert.ToInt32(property.MinValue))
                { ErrorMessage = property.ErrorMessage };
            // Maximum lenght check - only.
            else if (!string.IsNullOrEmpty(property.MaxValue))
                validateAttribute = new MaxLengthAttribute(Convert.ToInt32(property.MaxValue))
                { ErrorMessage = property.ErrorMessage };

            if (validateAttribute != null && !validateAttribute.IsValid(value))
            {
                var errorMessage = GetFormattedErrorMessage(property.ErrorMessage, validationContext.DisplayName);
                return new ValidationResult(errorMessage);
            }

            return ValidationResult.Success;
        }

        protected virtual ValidationResult ValidateCCPAttribute(object value, ValidationContext validationContext,
            ValidatorsValidatorProperty property)
        {
            // Regular expression check.
            var result = IsRegExValid(value, validationContext, property);
            if (result != ValidationResult.Success)
                return result;

            // Is Required propety required check.
            result = IsRequiredValid(value, validationContext, property);
            if (result != ValidationResult.Success)
                return result;

            //Check Range
            return IsRangeValid(value, validationContext, property);
        }

        /// <summary>
        /// Formats the error message for respective DTOs(models)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            return $"Valiadtions for {name} model is not available";
        }

        private string GetBusinessSchema(ValidationContext validationContext, string businessGuid)
        {
            var resolver = (IResolver)validationContext.GetService(typeof(IResolver));
            if (resolver == null)
                return null;
            return resolver.GetBusinessSchema(businessGuid);
        }
    }
}