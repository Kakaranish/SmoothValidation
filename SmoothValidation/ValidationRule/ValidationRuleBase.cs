using System;

namespace SmoothValidation.ValidationRule
{
    internal abstract class ValidationRuleBase
    {
        protected readonly string ErrorMessage;
        protected readonly string ErrorCode;

        protected ValidationRuleBase(string errorMessage, string errorCode = null)
        {
            ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
            ErrorCode = errorCode;
        }
    }
}