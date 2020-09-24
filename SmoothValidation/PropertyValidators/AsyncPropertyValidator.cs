using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using SmoothValidation.RootValidators;
using SmoothValidation.Types;
using SmoothValidation.Types.Exceptions;
using SmoothValidation.ValidationRules;
using SmoothValidation.ValidatorsAbstraction;

namespace SmoothValidation.PropertyValidators
{
    public class AsyncPropertyValidator<TProp> : PropertyValidatorBase<AsyncPropertyValidator<TProp>, TProp>,
        IAsyncPropertyValidator, IAsyncValidator<TProp>
    {
        public AsyncPropertyValidator(PropertyInfo property) : base(property)
        {
        }

        protected override AsyncPropertyValidator<TProp> PropertyValidator => this;

        public async Task<IList<PropertyValidationError>> Validate(object obj)
        {
            return await Validate((TProp) obj);
        }

        public async Task<IList<PropertyValidationError>> Validate(TProp obj)
        {
            var validationErrors = new List<PropertyValidationError>();

            foreach (var validator in Validators)
            {

                IList<PropertyValidationError> validationErrorsForValidator;
                IValidator currentValidator;
                if (validator is ISyncValidator syncValidator)
                {
                    currentValidator = syncValidator;
                    validationErrorsForValidator = syncValidator.Validate(obj);
                }
                else if (validator is IAsyncValidator asyncValidator)
                {
                    currentValidator = asyncValidator;
                    validationErrorsForValidator = await asyncValidator.Validate(obj);
                }
                else
                {
                    throw new InvalidOperationException(); // TODO: Add exception info
                }
                
                if (!(currentValidator is IRootValidator))
                {
                    foreach (var propertyValidationError in validationErrorsForValidator)
                    {
                        propertyValidationError.PropertyName = propertyValidationError.PropertyName == string.Empty
                            ? Property.Name
                            : $"{Property.Name}.{propertyValidationError.PropertyName}";
                    }
                }

                validationErrors.AddRange(validationErrorsForValidator);
            }

            return validationErrors;
        }

        public AsyncPropertyValidator<TProp> SetValidator(IAsyncValidator<TProp> otherValidator)
        {
            if (otherValidator == this)
            {
                throw new ValidatorSetupException("Detected circular reference");
            }

            OtherValidator = otherValidator ?? throw new ArgumentNullException(nameof(otherValidator)); // TODO: Change exception type?
            Validators.Add(otherValidator);

            return this;
        }

        public AsyncPropertyValidator<TProp> AddRule(Func<TProp, Task<bool>> predicate, string errorMessage, string errorCode = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));

            var validationRule = new AsyncValidationRule<TProp>(predicate, errorMessage, errorCode);
            Rules.Add(validationRule);
            Validators.Add(validationRule);

            return this;
        }
    }
}
