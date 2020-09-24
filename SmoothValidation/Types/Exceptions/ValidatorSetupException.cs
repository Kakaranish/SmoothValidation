using System;

namespace SmoothValidation.Types.Exceptions
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
