﻿using SmoothValidation.Types;
using SmoothValidation.ValidatorsAbstraction;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmoothValidation.ValidationRules
{
    internal class AsyncValidationRule<TProp> : ValidationRuleBase, IAsyncValidator<TProp>
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
                : new List<PropertyValidationError> { PropertyValidationError.CreateTransient(ErrorMessage, obj, ErrorCode) };
        }
    }
}
