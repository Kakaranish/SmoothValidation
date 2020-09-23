using System;

namespace SmoothValidation.Types
{
    public class ValidationException : Exception
    {
        public ValidationException()
        {
        }

        public ValidationException(string? message) : base(message)
        {
        }
    }
}
