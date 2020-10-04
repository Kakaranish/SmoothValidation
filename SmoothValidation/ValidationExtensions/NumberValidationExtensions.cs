using SmoothValidation.PropertyValidators;
using System;

namespace SmoothValidation.ValidationExtensions
{
    public static class NumberValidationExtensions
    {
        public static TBuilder IsGreaterThan<TBuilder, TObject, TNumber>(
            this PropertyValidatorBase<TBuilder, TObject, TNumber> propertyValidator, TNumber value)
            where TNumber : IComparable<TNumber>
        {
            var message = $"Value must be greater than {value}";
            const string errorCode = "NUM_LESS_OR_THAN_OR_EQUAL_TO";

            propertyValidator.AddRule(x => x.CompareTo(value) > 0, message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsGreaterThanOrEqualTo<TBuilder, TObject, TNumber>(
            this PropertyValidatorBase<TBuilder, TObject, TNumber> propertyValidator, TNumber value)
            where TNumber : IComparable<TNumber>
        {
            var message = $"Value must be greater than or equal to {value}";
            const string errorCode = "NUM_LESS_THAN";

            propertyValidator.AddRule(x => x.CompareTo(value) >= 0, message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsLessThan<TBuilder, TObject, TNumber>(
            this PropertyValidatorBase<TBuilder, TObject, TNumber> propertyValidator, TNumber value)
            where TNumber : IComparable<TNumber>
        {
            var message = $"Value must be less than {value}";
            const string errorCode = "NUM_GREATER_OR_THAN_OR_EQUAL_TO";

            propertyValidator.AddRule(x => x.CompareTo(value) < 0, message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsLessThanOrEqualTo<TBuilder, TObject, TNumber>(
            this PropertyValidatorBase<TBuilder, TObject, TNumber> propertyValidator, TNumber value)
            where TNumber : IComparable<TNumber>
        {
            var message = $"Value must be less than or equal to {value}";
            const string errorCode = "NUM_GREATER_THAN";

            propertyValidator.AddRule(x => x.CompareTo(value) <= 0, message, errorCode);

            return propertyValidator.PropertyValidator;
        }
    }
}
