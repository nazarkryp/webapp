using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;

namespace WebApp.Infrastructure.Extensions
{
    public static class TypeExtensions
    {
        private static readonly HashSet<Type> NumTypes = new HashSet<Type>
        {
            typeof(int),  typeof(double),  typeof(decimal),
            typeof(long), typeof(short),   typeof(sbyte),
            typeof(byte), typeof(ulong),   typeof(ushort),
            typeof(uint), typeof(float),   typeof(BigInteger)
        };

        public static bool PropertyExists(this Type type, string propertyName)
        {
            var property = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            return property != null;
        }

        public static bool IsNumeric<T>(this T type)
        {
            var isNumeric = false;

            if (type != null)
            {
                isNumeric = NumTypes.Contains(type.GetType());
            }

            return isNumeric;
        }

        public static bool IsNullableEnum(this Type type)
        {
            var isNullableEnum = Nullable.GetUnderlyingType(type)?.IsEnum;

            return isNullableEnum.GetValueOrDefault();
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}
