using System;
using SmoothValidation.ValidatorsAbstraction;

namespace SmoothValidation.Types
{
    public class ValidationTask
    {
        public IValidator Validator { get; }
        public bool IsOtherValidator { get; set; } = false;
        public bool StopValidationAfterFailure { get; set; }

        public PropertyValidationErrorTransformation ErrorTransformation { get; set; } 
            = new PropertyValidationErrorTransformation();

        public ValidationTask(IValidator validator)
        {
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }
    }
}