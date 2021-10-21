using Carrier.CCP.Common.Utils.Exception;
using System;

namespace Carrier.CCP.Common.Utils.Extension
{
    public static class EnumExtensions
    {
        public static string GetDescription(this CcpErrorStatus status)
        {
            return status.GetAttributeValue<DescriptionAttribute, string>();
        }

        public static int GetHttpCode(this CcpErrorStatus status)
        {
            return status.GetAttributeValue<HttpCodeAttribute, int>();
        }

        public static R GetAttributeValue<TAttr, R>(this IConvertible enumValue)
        {
            R attributeValue = default;

            if (enumValue != null)
            {
                var fi = enumValue.GetType().GetField(enumValue.ToString());

                if (fi != null && fi.GetCustomAttributes(typeof(TAttr), false) is TAttr[] attributes &&
                    attributes.Length > 0 &&
                    attributes[0] is ICcpAttribute<R> attribute) attributeValue = attribute.Value;
            }

            return attributeValue;
        }
    }
}