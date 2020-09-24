using SmoothValidation.PropertyValidator;
using SmoothValidation.Types;
using System.Collections.Generic;

namespace SmoothValidation.RootValidator
{
    public class RootSyncValidator<TObject> : ValidatorBase<TObject>, IRootValidator, ISyncValidatable<TObject>
    {
        public IList<PropertyValidationError> Validate(object obj)
        {
            return Validate((TObject)obj);
        }

        public IList<PropertyValidationError> Validate(TObject obj)
        {
            var validationErrors = new List<PropertyValidationError>();

            foreach (var propertyValidatorKvp in PropertyValidators)
            {
                var propertyValidator = (ISyncPropertyValidator)propertyValidatorKvp.Value;
                var propertyValue = propertyValidator.Property.GetValue(obj);

                var errorsForValidator = propertyValidator.Validate(propertyValue);

                foreach (var propertyValidationError in errorsForValidator)
                {
                    if (propertyValidationError.PropertyName != propertyValidatorKvp.Key)
                    {
                        propertyValidationError.PropertyName = $"{propertyValidatorKvp.Key}.{propertyValidationError.PropertyName}";
                    }
                }

                validationErrors.AddRange(errorsForValidator);
            }

            return validationErrors;
        }
    }
}