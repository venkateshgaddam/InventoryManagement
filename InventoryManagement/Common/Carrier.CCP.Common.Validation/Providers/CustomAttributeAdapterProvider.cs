using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace Carrier.CCP.Common.Validation
{
    /// <summary>
    ///     Adds a custom model validation binder, allowing data annotation attributes to be added to models dynamically.
    ///     This is the class which will be registered with the DI framework
    /// </summary>
    public class CustomAttributeAdapterProvider : IValidationAttributeAdapterProvider
    {
        private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();

        public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
        {
            return attribute switch
            {
                CCPValidationAttributeGuid ccpValidationAttributeGuid => new CCPValidationAttributeAdapter<CCPValidationAttributeGuid>(ccpValidationAttributeGuid,
                                       stringLocalizer),
                CCPValidationAttributeBool ccpValidationAttributeBool => new CCPValidationAttributeAdapter<CCPValidationAttributeBool>(ccpValidationAttributeBool,
stringLocalizer),
                CCPValidationAttributeInt ccpValidationAttributeInt => new CCPValidationAttributeAdapter<CCPValidationAttributeInt>(ccpValidationAttributeInt,
stringLocalizer),
                CCPValidationAttributeLong ccpValidationAttributeLong => new CCPValidationAttributeAdapter<CCPValidationAttributeLong>(ccpValidationAttributeLong,
stringLocalizer),
                CCPValidationAttributeString ccpValidationAttributeString => new CCPValidationAttributeAdapter<CCPValidationAttributeString>(ccpValidationAttributeString,
stringLocalizer),
                _ => _baseProvider.GetAttributeAdapter(attribute, stringLocalizer),
            };
        }
    }
}