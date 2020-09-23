using System;

namespace SmoothValidation.Types
{
    public class ValidatorSetupException : Exception
    {
        public ValidatorSetupException()
        {
        }

        public ValidatorSetupException(string? message) : base(message)
        {
        }
    }
}
