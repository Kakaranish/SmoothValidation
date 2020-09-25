using SmoothValidation.PropertyValidators;

namespace SmoothValidation.ValidationExtensions
{
    public static class GeneralValidationExtensions
    {
        public static TBuilder IsNull<TBuilder, TProperty>(this PropertyValidatorBase<TBuilder, TProperty> propertyValidator)
        {
            var message = "Value must be null";
            const string errorCode = "NULL";

            propertyValidator.AddRule(x => x == null, message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsNotNull<TBuilder, TProperty>(this PropertyValidatorBase<TBuilder, TProperty> propertyValidator)
        {
            var message = "Value cannot be null";
            const string errorCode = "NOT_NULL";

            propertyValidator.AddRule(x => x != null, message, errorCode);

            return propertyValidator.PropertyValidator;
        }
    }
}
