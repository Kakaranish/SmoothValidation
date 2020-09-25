using SmoothValidation.PropertyValidators;
using System;
using System.Text.RegularExpressions;

namespace SmoothValidation.ValidationExtensions
{
    public static class StringValidationExtensions
    {
        public static TBuilder IsEqual<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator,
            string value, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            var message = "Values are not equal but should be";
            const string errorCode = "STR_IS_NOT_EQUAL";

            propertyValidator.AddRule(x => x.Equals(value, stringComparison), message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsNotEqual<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator,
            string value, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            var message = "Values are equal but should not be";
            const string errorCode = "STR_IS_EQUAL";

            propertyValidator.AddRule(x => !x.Equals(value, stringComparison), message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsNotNullOrEmpty<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator)
        {
            var message = "Value is null or empty but should not be";
            const string errorCode = "STR_IS_NULL_OR_EMPTY";

            propertyValidator.AddRule(x => !string.IsNullOrWhiteSpace(x), message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsNotNullOrWhiteSpace<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator)
        {
            var message = "Value is null or whitespace but should not be";
            const string errorCode = "STR_IS_NULL_OR_WHITESPACE";

            propertyValidator.AddRule(x => !string.IsNullOrWhiteSpace(x), message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder HasMinLength<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator,
            int minLength)
        {
            if (minLength < 0)
            {
                throw new ArgumentException($"'{nameof(minLength)}' must be >= 0");
            }

            var message = $"Cannot have value less than {minLength}";
            const string errorCode = "STR_LESS_THAN_MIN_LENGTH";

            propertyValidator.AddRule(x => x?.Length >= minLength, message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder HasMaxLength<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator,
            int maxLength)
        {
            if (maxLength < 0)
            {
                throw new ArgumentException($"'{nameof(maxLength)}' must be >= 0");
            }

            var message = $"Cannot have value greater than {maxLength}";
            const string errorCode = "STR_GREATER_THAN_MAX_LENGTH";

            propertyValidator.AddRule(x => x?.Length <= maxLength, message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder HasLengthBetween<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator,
            int minLength, int maxLength)
        {
            if (minLength < 0 || maxLength < 0)
            {
                throw new ArgumentException($"'{nameof(minLength)}' and '{nameof(maxLength)}' must be >= 0");
            }
            if (minLength > maxLength)
            {
                throw new ArgumentException($"'{nameof(minLength)}' cannot be greater than '{nameof(maxLength)}'");
            }

            var message = $"Must have value in range [{minLength}, {maxLength}](inclusive)";
            const string errorCode = "STR_LENGTH_OUT_RANGE";

            propertyValidator.AddRule(x => x?.Length > minLength &&
                                           x.Length <= maxLength, message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsEmailAddress<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator)
        {
            var message = "Has invalid email format";
            const string errorCode = "STR_INVALID_EMAIL";

            var emailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            propertyValidator.AddRule(x => Regex.IsMatch(x, emailRegex, RegexOptions.IgnoreCase),
                message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder MatchesRegex<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator,
            string regex)
        {
            try
            {
                Regex.Match("", regex);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"'{nameof(regex)}' represents invalid regex");
            }

            var message = "Does not match regex";
            const string errorCode = "STR_NOT_MATCHING_TO_REGEX";


            propertyValidator.AddRule(x => Regex.IsMatch(x, regex), message, errorCode);

            return propertyValidator.PropertyValidator;
        }
    }
}
