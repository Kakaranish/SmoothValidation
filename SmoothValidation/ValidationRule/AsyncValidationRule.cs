using SmoothValidation.RootValidator;
using SmoothValidation.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmoothValidation.ValidationRule
{
    public class AsyncValidationRule<TProp> : ValidationRuleBase, IAsyncValidatable<TProp>
    {
        private readonly Func<TProp, Task<bool>> _validationPredicate;

        public AsyncValidationRule(Func<TProp, Task<bool>> validationPredicate, string message, string errorCode = null) :
            base(message, errorCode)
        {
            _validationPredicate = validationPredicate ?? throw new ArgumentNullException(nameof(validationPredicate));
        }

        public async Task<IList<PropertyValidationError>> Validate(object obj)
        {
            return await Validate((TProp)obj);
        }

        public async Task<IList<PropertyValidationError>> Validate(TProp obj)
        {
            var isValid = await _validationPredicate.Invoke(obj);

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
