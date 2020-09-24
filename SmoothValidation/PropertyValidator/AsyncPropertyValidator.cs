using SmoothValidation.RootValidator;
using SmoothValidation.Types;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using SmoothValidation.Types.Exceptions;
using SmoothValidation.ValidationRule;

namespace SmoothValidation.PropertyValidator
{
    public class AsyncPropertyValidator<TProp> : PropertyValidatorBase<AsyncPropertyValidator<TProp>, TProp>,
        IAsyncPropertyValidator, IAsyncValidatable<TProp>
    {
        public AsyncPropertyValidator(PropertyInfo property) : base(property)
        {
        }

        public override AsyncPropertyValidator<TProp> PropertyValidator => this;

        public async Task<IList<PropertyValidationError>> Validate(object obj)
        {
            return await Validate((TProp) obj);
        }

        public async Task<IList<PropertyValidationError>> Validate(TProp obj)
        {
            var validationErrors = new List<PropertyValidationError>();

            foreach (var validatable in Validators)
            {

                IList<PropertyValidationError> validationErrorsForValidator;
                IValidatable currentValidatable;
                if (validatable is ISyncValidatable syncValidatable)
                {
                    currentValidatable = syncValidatable;
                    validationErrorsForValidator = syncValidatable.Validate(obj);
                }
                else if (validatable is IAsyncValidatable asyncValidatable)
                {
                    currentValidatable = asyncValidatable;
                    validationErrorsForValidator = await asyncValidatable.Validate(obj);
                }
                else
                {
                    throw new InvalidOperationException(); // TODO: Add exception info
                }
                
                if (!(currentValidatable is IRootValidator))
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

        public AsyncPropertyValidator<TProp> SetValidator(IAsyncValidatable<TProp> otherValidatable)
        {
            if (otherValidatable == this)
            {
                throw new ValidatorSetupException("Detected circular reference");
            }

            OtherValidator = otherValidatable ?? throw new ArgumentNullException(nameof(otherValidatable)); // TODO: Change exception type?
            Validators.Add(otherValidatable);

            return this;
        }

        public AsyncPropertyValidator<TProp> AddAsyncRule(Func<TProp, Task<bool>> predicate, string errorMessage, string errorCode = null)
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
