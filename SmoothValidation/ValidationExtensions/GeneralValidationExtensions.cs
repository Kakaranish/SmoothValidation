using SmoothValidation.PropertyValidators;

namespace SmoothValidation.ValidationExtensions
{
    public static class GeneralValidationExtensions
    {
        public static TBuilder IsNull<TBuilder, TObject, TProperty>(this PropertyValidatorBase<TBuilder, TObject, TProperty> propertyValidator)
        {
            var message = "Value must be null";
            const string errorCode = "NULL";

            propertyValidator.AddRule(x => x == null, message, errorCode);

            return propertyValidator.PropertyValidator;
        }

        public static TBuilder IsNotNull<TBuilder, TObject, TProperty>(this PropertyValidatorBase<TBuilder, TObject, TProperty> propertyValidator)
        {
            var message = "Value cannot be null";
            const string errorCode = "NOT_NULL";

            propertyValidator.AddRule(x => x != null, message, errorCode);

            return propertyValidator.PropertyValidator;
        }
    }
}
