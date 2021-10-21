using Carrier.CCP.Common.Utils.Exception;
using System;

namespace Carrier.CCP.Common.Utils
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ErrorReferenceAttribute : Attribute, ICcpAttribute<ErrorReferenceData>
    {
        public ErrorReferenceAttribute(string code, string description)
        {
            Value = new ErrorReferenceData { Code = code, Description = description };
        }

        public ErrorReferenceAttribute(ErrorReferenceData value)
        {
            Value = value;
        }

        public ErrorReferenceData Value { get; }
    }
}