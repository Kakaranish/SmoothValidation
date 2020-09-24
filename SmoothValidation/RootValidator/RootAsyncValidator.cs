using SmoothValidation.PropertyValidator;
using SmoothValidation.Types;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmoothValidation.RootValidator
{
    public class RootAsyncValidator<TObject> : RootValidatorBase<TObject>, IRootValidator, IAsyncValidatable<TObject>
    {
        public async Task<IList<PropertyValidationError>> Validate(object obj)
        {
            return await Validate((TObject)obj);
        }

        public async Task<IList<PropertyValidationError>> Validate(TObject obj)
        {
            var validationErrors = new List<PropertyValidationError>();

            foreach (var propertyValidatorKvp in PropertyValidators)
            {
                IList<PropertyValidationError> validationErrorsForValidator;
                if (propertyValidatorKvp.Value is ISyncPropertyValidator syncValidatable)
                {
                    var propertyValue = syncValidatable.Property.GetValue(obj);
                    validationErrorsForValidator = syncValidatable.Validate(propertyValue);
                }
                else if (propertyValidatorKvp.Value is IAsyncPropertyValidator asyncValidatable)
                {
                    var propertyValue = asyncValidatable.Property.GetValue(obj);
                    validationErrorsForValidator = await asyncValidatable.Validate(propertyValue);
                }
                else
                {
                    throw new InvalidOperationException(); // TODO: Add exception info
                }

                foreach (var propertyValidationError in validationErrorsForValidator)
                {
                    if (propertyValidationError.PropertyName != propertyValidatorKvp.Key)
                    {
                        propertyValidationError.PropertyName = $"{propertyValidatorKvp.Key}.{propertyValidationError.PropertyName}";
                    }
                }
                validationErrors.AddRange(validationErrorsForValidator);
            }

            return validationErrors;
        }

        public AsyncPropertyValidator<TProp> SetupAsync<TProp>(Expression<Func<TObject, TProp>> expression)
        {
            var propertyInfo = ExtractProperty(expression);

            if (PropertyValidators.TryGetValue(propertyInfo.Name, out var propertyValidator))
            {
                if (propertyValidator is AsyncPropertyValidator<TProp> asyncPropertyValidator)
                {
                    return asyncPropertyValidator;
                }

                throw new InvalidOperationException("Property already has assigned synchronous validator");
            }

            var newPropertyValidator = new AsyncPropertyValidator<TProp>(propertyInfo);
            PropertyValidators.Add(propertyInfo.Name, newPropertyValidator);

            return newPropertyValidator;
        }
    }
}
