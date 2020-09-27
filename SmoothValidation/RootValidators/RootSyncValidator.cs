using System.Collections.Generic;
using SmoothValidation.PropertyValidators;
using SmoothValidation.Types;
using SmoothValidation.Utils;
using SmoothValidation.ValidatorsAbstraction;

namespace SmoothValidation.RootValidators
{
    public abstract class RootSyncValidator<TObject> : RootValidatorBase<TObject>, IRootValidator, ISyncValidator<TObject>
    {
        public IList<ValidationError> Validate(object obj)
        {
            return Validate(Common.Cast<TObject>(obj));
        }

        public IList<ValidationError> Validate(TObject obj)
        {
            var validationErrors = new List<ValidationError>();

            foreach (var propertyValidatorKvp in PropertyValidators)
            {
                var propertyValidator = (ISyncPropertyValidator)propertyValidatorKvp.Value;
                var propertyValue = propertyValidator.Property.GetValue(obj);

                var errorsForValidator = propertyValidator.Validate(propertyValue);
                validationErrors.AddRange(errorsForValidator);
            }

            return validationErrors;
        }
    }
}