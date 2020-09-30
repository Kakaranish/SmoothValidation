using System;

namespace SmoothValidation.Types
{
    public class ValidationError
    {
        public PropertyPath PropertyPath { get; }
        public string ErrorMessage { get; private set; }
        public string ErrorCode { get; private set; }
        public object ProvidedValue { get; }

        internal ValidationError(string propertyName, string errorMessage, object providedValue, string errorCode = null)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentException($"'{nameof(errorMessage)}' cannot be null or whitespace");
            }

            PropertyPath = new PropertyPath(propertyName);
            ErrorMessage = errorMessage;
            ProvidedValue = providedValue;
            ErrorCode = errorCode;
        }

        internal ValidationError(string errorMessage, object providedValue, string errorCode = null)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentException($"'{nameof(errorMessage)}' cannot be null or whitespace");
            }

            PropertyPath = new PropertyPath();
            ErrorMessage = errorMessage;
            ProvidedValue = providedValue;
            ErrorCode = errorCode;
        }

        public ValidationError ApplyTransformation(ValidationErrorTransformation transformation)
        {
            if (!string.IsNullOrWhiteSpace(transformation.OverridenMessage))
            {
                ErrorMessage = transformation.OverridenMessage;
            }

            if (transformation.OverriddenCode != null)
            {
                ErrorCode = transformation.OverriddenCode;
            }

            return this;
        }
    }
}