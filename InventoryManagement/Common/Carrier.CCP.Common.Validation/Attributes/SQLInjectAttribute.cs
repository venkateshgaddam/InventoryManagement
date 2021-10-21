using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Carrier.CCP.Common.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class SQLInjectAttribute : RegularExpressionAttribute
    {
        // Setting a default of 2 seconds for the regex timeout. 
        // This will stop larger strings from blocking the server and preventing a DDOS attack.
        private readonly TimeSpan regexMatchTimeout = new TimeSpan(0, 0, 2);

        public SQLInjectAttribute() : base(GetRegex())
        {
            ErrorMessage = "Invalid keywords in Input String";
        }

        private static string GetRegex()
        {
            // TODO make this better. 
            //return @"(;)|(')|(--)|(\*)|(xp_)|(delete)|(drop)|(execute)|(insert)|(truncate)|(exec)";
            return @"(;)|(--)|(\*)|(xp_)|(delete)|(drop)|(execute)|(insert)|(truncate)|(exec)";
        }

        public override bool IsValid(object value)
        {
            // if value = null, consider it as empty string
            var valid = Regex.IsMatch(value as string ?? string.Empty, Pattern, RegexOptions.IgnoreCase,
                regexMatchTimeout);
            return !valid;
        }
    }
}