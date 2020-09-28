using System;
using System.Collections.Generic;
using SmoothValidation.Types;
using SmoothValidation.ValidatorsAbstraction;

namespace SmoothValidation.ValidationRules
{
    internal class SyncValidationRule<TProp> : ValidationRuleBase, ISyncValidator<TProp>
    {
        private readonly Predicate<TProp> _validationPredicate;

        public SyncValidationRule(Predicate<TProp> validationPredicate, string errorMessage, string errorCode = null) 
            : base(errorMessage, errorCode)
        {
            _validationPredicate = validationPredicate ?? throw new ArgumentNullException(nameof(validationPredicate));
        }

        public IList<ValidationError> Validate(object obj)
        {
            return Validate((TProp)obj);
        }

        public IList<ValidationError> Validate(TProp obj)
        {
            var isValid = _validationPredicate.Invoke(obj);
            
            return isValid
                ? new List<ValidationError>()
                : new List<ValidationError> { new ValidationError(ErrorMessage, obj, ErrorCode)};
        }
    }
}