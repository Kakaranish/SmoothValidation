using System;
using System.Collections.Generic;
using System.Linq;
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

            foreach (var validationTask in ValidationTasks)
            {
                IList<PropertyValidationError> validationErrorsForValidator;
                IValidator validator;

                if (validationTask.Validator is ISyncValidator syncValidator)
                {
                    validator = syncValidator;
                    validationErrorsForValidator = syncValidator.Validate(obj);
                }
                else if (validationTask.Validator is IAsyncValidator asyncValidator)
                {
                    validator = asyncValidator;
                    validationErrorsForValidator = await asyncValidator.Validate(obj);
                }
                else
                {
                    throw new InvalidOperationException(); // TODO: Add exception info
                }
                
                if (!(validator is IRootValidator))
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
            if (otherValidator == null) throw new ArgumentNullException(nameof(otherValidator));
            if (otherValidator == this) throw new ValidatorSetupException("Detected circular reference");

            if (ValidationTasks.Any(task => task.IsOtherValidator))
            {
                throw new ValidatorSetupException("There is already set other validator");
            }

            ValidationTasks.Add(new ValidationTask(otherValidator));

            return this;
        }

        public AsyncPropertyValidator<TProp> AddRule(Func<TProp, Task<bool>> predicate, string errorMessage, string errorCode = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));

            var validationRule = new AsyncValidationRule<TProp>(predicate, errorMessage, errorCode);
            ValidationTasks.Add(new ValidationTask(validationRule));

            return this;
        }
    }
}
