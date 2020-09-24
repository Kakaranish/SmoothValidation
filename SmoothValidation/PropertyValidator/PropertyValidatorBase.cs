using System;
using System.Collections.Generic;
using System.Reflection;
using SmoothValidation.RootValidator;
using SmoothValidation.Types.Exceptions;
using SmoothValidation.ValidationRule;

namespace SmoothValidation.PropertyValidator
{
    public abstract class PropertyValidatorBase<TPropertyValidator, TProp>
    {
        protected readonly List<IValidatable> Rules = new List<IValidatable>();
        protected IValidatable OtherValidator;
        protected readonly List<IValidatable> Validators = new List<IValidatable>();

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

        public TPropertyValidator SetValidator(ISyncValidatable<TProp> otherValidatable)
        {
            if (otherValidatable == this)
            {
                throw new ValidatorSetupException("Detected circular reference");
            }

            // TODO: Change exception type?
            OtherValidator = otherValidatable ?? throw new ArgumentNullException(nameof(otherValidatable));
            Validators.Add(otherValidatable);

            return PropertyValidator;
        }
    }
}
