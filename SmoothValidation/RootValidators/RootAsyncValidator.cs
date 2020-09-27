using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SmoothValidation.PropertyValidators;
using SmoothValidation.Types;
using SmoothValidation.Utils;
using SmoothValidation.ValidatorsAbstraction;

namespace SmoothValidation.RootValidators
{
    public abstract class RootAsyncValidator<TObject> : RootValidatorBase<TObject>, IRootValidator, IAsyncValidator<TObject>
    {
        public async Task<IList<ValidationError>> Validate(object obj)
        {
            return await Validate(Common.Cast<TObject>(obj));
        }

        public async Task<IList<ValidationError>> Validate(TObject obj)
        {
            var validationErrors = new List<ValidationError>();

            foreach (var propertyValidatorKvp in PropertyValidators)
            {
                IList<ValidationError> validationErrorsForValidator;
                if (propertyValidatorKvp.Value is ISyncPropertyValidator syncPropertyValidator)
                {
                    var propertyValue = syncPropertyValidator.Property.GetValue(obj);
                    validationErrorsForValidator = syncPropertyValidator.Validate(propertyValue);
                }
                else
                {
                    var asyncPropertyValidator = (IAsyncPropertyValidator) propertyValidatorKvp.Value;
                    var propertyValue = asyncPropertyValidator.Property.GetValue(obj);
                    validationErrorsForValidator = await asyncPropertyValidator.Validate(propertyValue);
                }

                validationErrors.AddRange(validationErrorsForValidator);
            }

            return validationErrors;
        }

        public AsyncPropertyValidator<TProp> SetupAsync<TProp>(Expression<Func<TObject, TProp>> expression)
        {
            var memberInfo = PropertyExtractor.Extract(expression);

            if (PropertyValidators.TryGetValue(memberInfo.Name, out var propertyValidator))
            {
                if (propertyValidator is AsyncPropertyValidator<TProp> asyncPropertyValidator)
                {
                    return asyncPropertyValidator;
                }

                throw new InvalidOperationException("Property already has assigned synchronous validator");
            }

            var newPropertyValidator = new AsyncPropertyValidator<TProp>(memberInfo);
            PropertyValidators.Add(memberInfo.Name, newPropertyValidator);

            return newPropertyValidator;
        }
    }
}
