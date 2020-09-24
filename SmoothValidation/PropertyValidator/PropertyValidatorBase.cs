using System;
using System.Collections.Generic;
using System.Reflection;
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

        public abstract TPropertyValidator PropertyValidator { get; }
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
    }
}
