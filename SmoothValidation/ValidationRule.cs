using System;
using System.Collections.Generic;
using SmoothValidation.Types;

namespace SmoothValidation
{
    public class ValidationRule<TProp> : IValidatable<TProp>
    {
        private readonly Predicate<TProp> _validationPredicate;
        private readonly string _errorMessage;

        public ValidationRule(Predicate<TProp> validationPredicate, string errorMessage)
        {
            _validationPredicate = validationPredicate ?? throw new ArgumentNullException(nameof(validationPredicate));
            _errorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        }

        public IList<PropertyValidationError> Validate(object obj)
        {
            return Validate((TProp)obj);
        }

        public IList<PropertyValidationError> Validate(TProp obj)
        {
            var isValid = _validationPredicate.Invoke(obj);
            
            return isValid
                ? new List<PropertyValidationError>()
                : new List<PropertyValidationError>
                {
                    new PropertyValidationError
                    {
                        // TODO: Add error code
                        PropertyName = string.Empty,
                        ErrorMessage = _errorMessage,
                        ProvidedValue = obj
                    }
                };
        }
    }
}