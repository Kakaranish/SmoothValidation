using System;
using SmoothValidation.ValidatorsAbstraction;

namespace SmoothValidation.Types
{
    public class ValidationTask
    {
        public IValidator Validator { get; }
        public bool IsOtherValidator { get; }
        public bool StopValidationAfterFailure { get; set; }

        public ValidationErrorTransformation ErrorTransformation { get; set; }  = new ValidationErrorTransformation();

        public ValidationTask(IValidator validator, bool isOtherValidator)
        {
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
            IsOtherValidator = isOtherValidator;
        }
    }
}