using System;
using System.Text.RegularExpressions;

namespace SmoothValidation.Utils
{
    internal static class Common
    {
        internal static TTarget Cast<TTarget>(object obj)
        {
            if (obj is TTarget toValidate)
            {
                return toValidate;
            }

            if (typeof(TTarget).IsValueType || obj != null)
            {
                throw new ArgumentException($"'{nameof(obj)}' is not {typeof(TTarget).Name} type");
            }

            return default;
        }

        internal static void EnsurePropertyNameIsValid(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or whitespace");
            }

            const string regex = @"^[a-zA-Z_]\w*(\.[a-zA-Z_]\w*)*$";
            if (!Regex.IsMatch(propertyName, regex))
            {
                throw new ArgumentException($"'{nameof(propertyName)}' must match to regex {regex}");
            }
        }
    }
}
