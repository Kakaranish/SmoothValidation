using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmoothValidation.Types.Exceptions;
using SmoothValidation.ValidationRules;
using SmoothValidation.ValidatorsAbstraction;

namespace SmoothValidation.PropertyValidators
{
    public class ValidationTask
    {
        public IValidator Validator { get; }
        public bool IsOtherValidator { get; set; } = false;
        public bool StopValidationAfterFailure { get; set; }
        
        public ValidationTask(IValidator validator)
        {
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }
    }

    public abstract class PropertyValidatorBase<TPropertyValidator, TProp>
    {
        protected readonly List<ValidationTask> ValidationTasks = new List<ValidationTask>();

        protected PropertyValidatorBase(PropertyInfo property)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }

        protected abstract TPropertyValidator PropertyValidator { get; }
        public PropertyInfo Property { get; }

        public TPropertyValidator AddRule(Predicate<TProp> predicate, string errorMessage, string errorCode = null)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));

            var validationRule = new SyncValidationRule<TProp>(predicate, errorMessage, errorCode);
            ValidationTasks.Add(new ValidationTask(validationRule));

            return PropertyValidator;
        }

        public TPropertyValidator SetValidator(ISyncValidator<TProp> otherValidator)
        {
            if (otherValidator == null) throw new ArgumentNullException(nameof(otherValidator));
            if (otherValidator == this) throw new ValidatorSetupException("Detected circular reference");

            if(ValidationTasks.Any(task => task.IsOtherValidator))
            {
                throw new ValidatorSetupException("There is already set other validator");
            }

            ValidationTasks.Add(new ValidationTask(otherValidator));
            
            return PropertyValidator;
        }
    }
}
