using System;
using System.Collections.Generic;
using SmoothValidation.RootValidator;
using SmoothValidation.Types;

namespace SmoothValidation.ValidationRule
{
    internal class SyncValidationRule<TProp> : ValidationRuleBase, ISyncValidatable<TProp>
    {
        private readonly Predicate<TProp> _validationPredicate;

        public SyncValidationRule(Predicate<TProp> validationPredicate, string errorMessage, string errorCode = null) : 
            base(errorMessage, errorCode)
        {
            _validationPredicate = validationPredicate ?? throw new ArgumentNullException(nameof(validationPredicate));
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
                        PropertyName = string.Empty,
                        ErrorMessage = ErrorMessage,
                        ErrorCode = ErrorCode,
                        ProvidedValue = obj
                    }
                };
        }
    }
}