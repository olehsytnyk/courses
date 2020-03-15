using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace STP.Common.Extensions
{
    public static class EnumExtensionMethods
    {
        public static string GetDescription(this Enum value)
        {
            return value.GetType().GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .Cast<DescriptionAttribute>()
                .FirstOrDefault()?.Description
                ?? value.ToString();
        }
    }
}
