using SmoothValidation.Types;
using SmoothValidation.ValidationRules;
using SmoothValidation.ValidatorsAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SmoothValidation.Utils;

namespace SmoothValidation.PropertyValidators
{
    public sealed class AsyncPropertyValidator<TProp> : PropertyValidatorBase<AsyncPropertyValidator<TProp>, TProp>,
        IAsyncPropertyValidator, IAsyncValidator<TProp>
    {
        public AsyncPropertyValidator(MemberInfo memberInfo) : base(memberInfo)
        {
        }

        internal override AsyncPropertyValidator<TProp> PropertyValidator => this;

        public async Task<IList<ValidationError>> Validate(object obj)
        {
            return await Validate(Common.Cast<TProp>(obj));
        }

        public async Task<IList<ValidationError>> Validate(TProp obj)
        {
            var validationErrors = new List<ValidationError>();

            foreach (var validationTask in ValidationTasks)
            {
                var validationErrorsForValidator = validationTask.Validator is ISyncValidator syncValidator
                    ? syncValidator.Validate(obj)
                    : await ((IAsyncValidator) validationTask.Validator).Validate(obj);

                foreach (var validationError in validationErrorsForValidator)
                {
                    ProcessValidationError(validationError, validationTask.ErrorTransformation);
                }

                validationErrors.AddRange(validationErrorsForValidator);

                if (validationErrorsForValidator.Any() && validationTask.StopValidationAfterFailure)
                {
                    break;
                }
            }

            return validationErrors;
        }

        public AsyncPropertyValidator<TProp> AddRule(Func<TProp, Task<bool>> predicate, string errorMessage, string errorCode = null)
        {
            var validationRule = new AsyncValidationRule<TProp>(predicate, errorMessage, errorCode);
            ValidationTasks.Add(new ValidationTask(validationRule, false));

            return this;
        }

        public AsyncPropertyValidator<TProp> SetValidator(IAsyncValidator<TProp> otherValidator)
        {
            if (otherValidator == null) throw new ArgumentNullException(nameof(otherValidator));
            if (otherValidator == this) throw new ArgumentException("Detected circular reference");

            if (ValidationTasks.Any(task => task.IsOtherValidator))
            {
                throw new InvalidOperationException("There is already set other validator");
            }

            ValidationTasks.Add(new ValidationTask(otherValidator, true));

            return this;
        }
    }
}
