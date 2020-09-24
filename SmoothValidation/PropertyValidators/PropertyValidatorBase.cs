using System;
using System.Collections.Generic;
using System.Reflection;
using SmoothValidation.Types.Exceptions;
using SmoothValidation.ValidationRules;
using SmoothValidation.ValidatorsAbstraction;

namespace SmoothValidation.PropertyValidators
{
    public abstract class PropertyValidatorBase<TPropertyValidator, TProp>
    {
        protected readonly List<IValidator> Rules = new List<IValidator>();
        protected IValidator OtherValidator;
        protected readonly List<IValidator> Validators = new List<IValidator>();

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
            Rules.Add(validationRule);
            Validators.Add(validationRule);

            return PropertyValidator;
        }

        public TPropertyValidator SetValidator(ISyncValidator<TProp> otherValidator)
        {
            if (otherValidator == this)
            {
                throw new ValidatorSetupException("Detected circular reference");
            }

            // TODO: Change exception type?
            OtherValidator = otherValidator ?? throw new ArgumentNullException(nameof(otherValidator));
            Validators.Add(otherValidator);

            return PropertyValidator;
        }
    }
}
