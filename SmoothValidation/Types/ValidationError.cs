using System;

namespace SmoothValidation.Types
{
    public class ValidationError
    {
        public string PropertyName { get; private set; }
        public string ErrorMessage { get; private set; }
        public string ErrorCode { get; private set; }
        public object ProvidedValue { get; }
        
        internal bool IsTransient => PropertyName == null;

        internal ValidationError(string propertyName, string errorMessage, object providedValue, string errorCode = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or whitespace");
            }

            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentException($"'{nameof(errorMessage)}' cannot be null or whitespace");
            }

            PropertyName = propertyName;
            ErrorMessage = errorMessage;
            ProvidedValue = providedValue;
            ErrorCode = errorCode;
        }

        private ValidationError(string errorMessage, object providedValue, string errorCode = null)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new ArgumentException($"'{nameof(errorMessage)}' cannot be null or whitespace");
            }

            ErrorMessage = errorMessage;
            ProvidedValue = providedValue;
            ErrorCode = errorCode;
        }

        public static ValidationError CreateTransient(string errorMessage, object providedValue, string errorCode = null)
        {
            return new ValidationError(errorMessage, providedValue, errorCode);
        }

        public void SetPropertyName(string propertyName)
        {
            if (!IsTransient)
            {
                throw new InvalidOperationException("Unable to directly set property name when object is not transient");
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or whitespace");
            }

            PropertyName = propertyName;
        }

        public void PrependParentPropertyName(string parentPropertyName)
        {
            if (IsTransient)
            {
                throw new InvalidOperationException("Unable to append parent property name when in transient state");
            }

            if (string.IsNullOrWhiteSpace(parentPropertyName))
            {
                throw new ArgumentException($"'{nameof(parentPropertyName)}' cannot be null or whitespace");
            }

            PropertyName = $"{parentPropertyName}.{PropertyName}";
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