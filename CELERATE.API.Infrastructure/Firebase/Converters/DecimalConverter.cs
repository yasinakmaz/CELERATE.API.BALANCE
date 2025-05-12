// File: CELERATE.API.Infrastructure/Firebase/Converters/DecimalConverter.cs
using System;

namespace CELERATE.API.Infrastructure.Firebase.Converters
{
    /// <summary>
    /// Handles conversion between Firestore number/string types and .NET decimal type
    /// </summary>
    public static class DecimalConverter
    {
        /// <summary>
        /// Converts a decimal to a string representation for Firestore storage
        /// </summary>
        public static string ToString(decimal value)
        {
            // G29 format specifier preserves the full precision of decimal
            return value.ToString("G29");
        }

        /// <summary>
        /// Converts a Firestore value (potentially string, double, long, or int) to a decimal
        /// </summary>
        public static decimal FromObject(object value)
        {
            if (value == null)
                return 0m;

            // Handle different possible types from Firestore
            return value switch
            {
                string stringValue => decimal.Parse(stringValue),
                double doubleValue => (decimal)doubleValue,
                long longValue => (decimal)longValue,
                int intValue => (decimal)intValue,
                _ => throw new ArgumentException($"Cannot convert {value.GetType().Name} to decimal")
            };
        }
    }
}