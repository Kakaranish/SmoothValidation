using System;
using SmoothValidation.ValidatorsAbstraction;

namespace SmoothValidation.Types
{
    public class ValidationTask
    {
        internal IValidator Validator { get; }
        internal bool IsOtherValidator { get; }
        internal bool StopValidationAfterFailure { get; set; }

        internal ValidationErrorTransformation ErrorTransformation { get; } = new ValidationErrorTransformation();

        internal ValidationTask(IValidator validator, bool isOtherValidator)
        {
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
            IsOtherValidator = isOtherValidator;
        }
    }
}