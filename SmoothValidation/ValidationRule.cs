using System;

namespace SmoothValidation
{
    public class ValidationRule<TProp> : IValidationRule
    {
        private readonly Predicate<TProp> _validationPredicate;
        private readonly string _errorMessage;

        public ValidationRule(Predicate<TProp> validationPredicate, string errorMessage)
        {
            _validationPredicate = validationPredicate ?? throw new ArgumentNullException(nameof(validationPredicate));
            _errorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
        }

        public string Validate(object obj)
        {
            return Validate((TProp) obj);
        }

        public string Validate(TProp obj)
        {
            return _validationPredicate.Invoke(obj)
                ? string.Empty
                : _errorMessage;
        }
    }
}