using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;

namespace Carrier.CCP.Common.Validation
{
    /// <summary>
    ///     Generic adopter which introduces a new instance of the required validation attiribute based on the type of the
    ///     input attribute
    /// </summary>
    /// <typeparam name="T">CCPValidationAttribute</typeparam>
    internal class CCPValidationAttributeAdapter<T> : AttributeAdapterBase<T>
        where T : CCPValidationAttribute
    {
        public CCPValidationAttributeAdapter(T attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        public override void AddValidation(ClientModelValidationContext context)
        {
        }

        public override string GetErrorMessage(ModelValidationContextBase validationContext)
        {
            return null;
        }
    }
}