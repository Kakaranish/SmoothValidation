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
        protected string PropertyDisplayName { get; private set; }
        
        protected PropertyValidatorBase(MemberInfo memberInfo)
        {
            Property = new Property(memberInfo);
            PropertyDisplayName = Property.Name;
        }

        internal abstract TPropertyValidator PropertyValidator { get; }
        public Property Property { get; }
        internal IReadOnlyList<ValidationTask> ValidationTasksAsReadonly => ValidationTasks.AsReadOnly();
        
        public TPropertyValidator AddRule(Predicate<TProp> predicate, string errorMessage, string errorCode = null)
        {
            var validationRule = new SyncValidationRule<TProp>(predicate, errorMessage, errorCode);
            ValidationTasks.Add(new ValidationTask(validationRule, false));
            
            return PropertyValidator;
        }

        public TPropertyValidator SetValidator(ISyncValidator<TProp> otherValidator)
        {
            if (otherValidator == null) throw new ArgumentNullException(nameof(otherValidator));
            if (otherValidator == this) throw new ArgumentException("Detected circular reference");

            if(ValidationTasks.Any(task => task.IsOtherValidator))
            {
                throw new InvalidOperationException("There is already set other validator");
            }

            ValidationTasks.Add(new ValidationTask(otherValidator, true));
            
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
                lastValidationTask.ErrorTransformation.OverridenMessage = message;
            }

            return PropertyValidator;
        }

        public TPropertyValidator WithCode(string code)
        {
            var lastValidationTask = ValidationTasks.LastOrDefault();
            if (lastValidationTask != null)
            {
                lastValidationTask.ErrorTransformation.OverriddenCode = code;
            }

            return PropertyValidator;
        }

        public TPropertyValidator SetPropertyDisplayName(string propertyDisplayName)
        {
            if (string.IsNullOrWhiteSpace(propertyDisplayName))
            {
                throw new ArgumentException($"'{nameof(propertyDisplayName)}' cannot be null or whitespace");
            }

            PropertyDisplayName = propertyDisplayName;

            return PropertyValidator;
        }

        protected void ProcessPropertyValidationError(PropertyValidationError propertyValidationError, PropertyValidationErrorTransformation errorTransformation)
        protected void ProcessValidationError(ValidationError validationError, ValidationErrorTransformation errorTransformation)
        {
            if (validationError.IsTransient)
            {
                validationError.SetPropertyName(PropertyDisplayName);
            }
            else
            {
                validationError.PrependParentPropertyName(PropertyDisplayName);
            }

            validationError.ApplyTransformation(errorTransformation);
        }
    }
}
