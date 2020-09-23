using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmoothValidation
{
    public class PropertyValidator<TProp> : IPropertyValidator, IValidator<TProp>
    {
        private readonly PropertyInfo _property;
        private IValidator<TProp> _otherValidator;
        private readonly List<ValidationRule<TProp>> _rules;

        public PropertyValidator(PropertyInfo property)
        {
            _property = property ?? throw new ArgumentNullException(nameof(property));
            _rules = new List<ValidationRule<TProp>>();
        }

        public IList<string> Validate(object obj)
        {
            var propertyValueAsObj = _property.GetValue(obj);
            var propertyValue = (TProp)Convert.ChangeType(propertyValueAsObj, typeof(TProp));

            return Validate(propertyValue);
        }

        public IList<string> Validate(TProp obj)
        {
            if (_otherValidator != null)
            {
                return _otherValidator.Validate(obj);
            }

            var validationErrors = new List<string>();
            foreach (var validationRule in _rules)
            {
                var validationResult = validationRule.Validate(obj);
                if (validationResult != null)
                {
                    validationErrors.Add(validationResult);
                }
            }

            return validationErrors;
        }

        public void SetValidator(IValidator<TProp> otherValidator)
        {
            _otherValidator = otherValidator ?? throw new ArgumentNullException(nameof(otherValidator));
        }

        public PropertyValidator<TProp> AddRule(Predicate<TProp> predicate, string errorMessage)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));

            _rules.Add(new ValidationRule<TProp>(predicate, errorMessage));

            return this;
        }
    }
}