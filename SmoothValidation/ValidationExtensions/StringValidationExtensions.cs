﻿using SmoothValidation.PropertyValidators;
using System;
using System.Text.RegularExpressions;

namespace SmoothValidation.ValidationExtensions
{
    public static class StringValidationExtensions
    {
        public static TBuilder IsEqual<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator,
            string value, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            var message = $"Value should be equal to {value}";
            const string errorCode = "STR_IS_NOT_EQUAL";

            propertyValidator.AddRule(x => x.Equals(value, stringComparison), message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsNotEqual<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator,
            string value, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            var message = $"Value should not be equal to {value}";
            const string errorCode = "STR_IS_EQUAL";

            propertyValidator.AddRule(x => !x.Equals(value, stringComparison), message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsNotNullOrEmpty<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator)
        {
            const string message = "Value is null or empty but should not be";
            const string errorCode = "STR_IS_NULL_OR_EMPTY";

            propertyValidator.AddRule(x => !string.IsNullOrEmpty(x), message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsNotNullOrWhiteSpace<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator)
        {
            var message = "Value is null or whitespace but should not be";
            const string errorCode = "STR_IS_NULL_OR_WHITESPACE";

            propertyValidator.AddRule(x => !string.IsNullOrWhiteSpace(x), message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder HasLength<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator,
            int length)
        {
            if (length < 0)
            {
                throw new ArgumentException($"'{nameof(length)}' must be >= 0");
            }

            var message = $"Must have length equal to {length}";
            const string errorCode = "STR_DIFFERENT_THAN_REQUIRED_LENGTH";

            propertyValidator.AddRule(x => x.Length == length, message, errorCode);

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

            propertyValidator.AddRule(x => x?.Length >= minLength &&
                                           x.Length <= maxLength, message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsEmailAddress<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator)
        {
            const string message = "Has invalid email format";
            const string errorCode = "STR_INVALID_EMAIL";

            var emailRegex = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
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

            var message = $"Does not match regex '{regex}'";
            const string errorCode = "STR_NOT_MATCHING_TO_REGEX";

            propertyValidator.AddRule(x => Regex.IsMatch(x, regex), message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsGuid<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator)
        {
            const string message = "It is not guid";
            const string errorCode = "STR_IS_NOT_GUID";

            propertyValidator.AddRule(x => Guid.TryParse(x, out _), message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsIntegerParsable<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator)
        {
            var message = $"It is not parsable to integer";
            const string errorCode = "STR_NOT_PARSABLE_TO_INT";

            propertyValidator.AddRule(x => int.TryParse(x, out _), message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsDecimalParsable<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator)
        {
            var message = $"It is not parsable to decimal";
            const string errorCode = "STR_NOT_PARSABLE_TO_DECIMAL";

            propertyValidator.AddRule(x => decimal.TryParse(x, out _), message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsFloatParsable<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator)
        {
            var message = $"It is not parsable to float";
            const string errorCode = "STR_NOT_PARSABLE_TO_FLOAT";

            propertyValidator.AddRule(x => float.TryParse(x, out _), message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsDoubleParsable<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator)
        {
            var message = $"It is not parsable to double";
            const string errorCode = "STR_NOT_PARSABLE_TO_DOUBLE";

            propertyValidator.AddRule(x => double.TryParse(x, out _), message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsBoolParsable<TBuilder>(this PropertyValidatorBase<TBuilder, string> propertyValidator)
        {
            var message = $"It is not parsable to bool";
            const string errorCode = "STR_NOT_PARSABLE_TO_BOOL";

            propertyValidator.AddRule(x => bool.TryParse(x, out _), message, errorCode);

            return propertyValidator.PropertyValidator;
        }
    }
}
