using System;
using System.ComponentModel.DataAnnotations;

namespace Carrier.CCP.Common.Utils.Validation
{
    public class NotEmptyGuidAttribute : ValidationAttribute
    {
        internal new const string ErrorMessage = "The {0} field must not be empty";

        public NotEmptyGuidAttribute() : base(ErrorMessage)
        {
        }

        public override bool IsValid(object value)
        {
            if (value is null) return true; //dont enforce required validation
            return value switch
            {
                Guid guid => guid != Guid.Empty,//Checks whether the GUID is empty or not and returns false if GUID is empty
                _ => true,
            };
        }
    }
}