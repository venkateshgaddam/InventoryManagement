﻿using System;

namespace Carrier.CCP.Common.Utils
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DescriptionAttribute : Attribute, ICcpAttribute<string>
    {
        public DescriptionAttribute(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}