using SmoothValidation.Types.Exceptions;
using SmoothValidation.ValidationRules;
using SmoothValidation.ValidatorsAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmoothValidation.Types;

namespace SmoothValidation.PropertyValidators
{
    public abstract class PropertyValidatorBase<TPropertyValidator, TProp>
    {
        protected readonly List<ValidationTask> ValidationTasks = new List<ValidationTask>();
        protected string OverridenPropertyDisplayName { get; private set; }

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

        public TPropertyValidator StopValidationAfterFailure()
        {
            var lastValidationTask = ValidationTasks.LastOrDefault();
            if (lastValidationTask != null)
            {
                lastValidationTask.StopValidationAfterFailure = true;
            }
            
            return PropertyValidator;
        }

        public TPropertyValidator WithMessage(string message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            var lastValidationTask = ValidationTasks.LastOrDefault();
            if (lastValidationTask != null)
            {
                lastValidationTask.PropertyValidationErrorTransformation.OverridenMessage = message;
            }

            return PropertyValidator;
        }

        public TPropertyValidator SetPropertyDisplayName(string propertyName)
        {
            OverridenPropertyDisplayName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));

            return PropertyValidator;
        }

        public TPropertyValidator WithCode(string code)
        {
            var lastValidationTask = ValidationTasks.LastOrDefault();
            if (lastValidationTask != null)
            {
                lastValidationTask.PropertyValidationErrorTransformation.OverriddenCode = code;
            }

            return PropertyValidator;
        }
    }
}
