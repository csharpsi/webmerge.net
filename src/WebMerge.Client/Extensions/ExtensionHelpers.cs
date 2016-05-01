using System;
using System.Collections.Generic;

namespace WebMerge.Client.Extensions
{
    public static class ExtensionHelpers
    {
        private static HashSet<Type> NumericTypes => new HashSet<Type>
        {
            typeof (int), typeof (double), typeof (decimal),
            typeof (long), typeof (short), typeof (sbyte),
            typeof (byte), typeof (ulong), typeof (ushort),
            typeof (uint), typeof (float)
        };

        public static bool IsNumericType(this Type type) => NumericTypes.Contains(Nullable.GetUnderlyingType(type) ?? type);

        public static object GetDefaultValue(this Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}