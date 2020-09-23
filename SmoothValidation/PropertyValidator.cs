using SmoothValidation.Types;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmoothValidation
{
    public class PropertyValidator<TProp> : IPropertyValidator, IValidatable<TProp>
    {
        private readonly List<ValidationRule<TProp>> _rules = new List<ValidationRule<TProp>>();
        private IValidatable<TProp> _otherValidatable;
        private readonly List<IValidator> _validators = new List<IValidator>();

        public PropertyValidator(PropertyInfo property)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }

        public PropertyInfo Property { get; }

        public IList<PropertyValidationError> Validate(object obj)
        {
            return Validate((TProp)obj);
        }

        public IList<PropertyValidationError> Validate(TProp obj)
        {
            var validationErrors = new List<PropertyValidationError>();

            foreach (var validator in _validators)
            {
                var validationErrorsForValidator = validator.Validate(obj);
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

        public PropertyValidator<TProp> SetValidator(IValidatable<TProp> otherValidatable)
        {
            if (otherValidatable == this)
            {
                throw new ValidatorSetupException("Detected circular reference");
            }

            // TODO: Change exception type?
            _otherValidatable = otherValidatable ?? throw new ArgumentNullException(nameof(otherValidatable));
            _validators.Add(otherValidatable);

            return this;
        }

        public PropertyValidator<TProp> AddRule(Predicate<TProp> predicate, string errorMessage)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));

            var validationRule = new ValidationRule<TProp>(predicate, errorMessage);
            _validators.Add(validationRule);
            _rules.Add(validationRule);

            return this;
        }

        public PropertyValidator<TProp> UnsetValidator()
        {
            _otherValidatable = null;

            return this;
        }

        public PropertyValidator<TProp> ClearRules()
        {
            _rules.Clear();

            return this;
        }
    }
}