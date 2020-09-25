﻿using System;

namespace SmoothValidation.Types
{
    public class PropertyValidationError
    {
        public string PropertyName { get; private set; }
        public string ErrorMessage { get; private set; }
        public string ErrorCode { get; private set; }
        public object ProvidedValue { get; }
        
        public bool IsTransient => PropertyName == null;

        public PropertyValidationError(string propertyName, string errorMessage, object providedValue, string errorCode = null)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
            ProvidedValue = providedValue;
            ErrorCode = errorCode;
        }

        private PropertyValidationError(string errorMessage, object providedValue, string errorCode = null)
        {
            if (string.IsNullOrWhiteSpace(errorMessage)) throw new ArgumentNullException(nameof(errorMessage));

            ErrorMessage = errorMessage;
            ProvidedValue = providedValue;
            ErrorCode = errorCode;
        }

        public static PropertyValidationError CreateTransient(string errorMessage, object providedValue, string errorCode = null)
        {
            return new PropertyValidationError(errorMessage, providedValue, errorCode);
        }

        public void PrependParentPropertyName(string parentPropertyName)
        {
            if (parentPropertyName == null) throw new ArgumentNullException(nameof(parentPropertyName));

            if (IsTransient)
            {
                throw new InvalidOperationException("Unable to append parent property name when in transient state");
            }

            PropertyName = $"{parentPropertyName}.{PropertyName}";
        }

        public void SetPropertyName(string propertyName)
        {
            if (!IsTransient)
            {
                throw new InvalidOperationException("Unable to directly set property name when object is not transient");
            }
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        public PropertyValidationError ApplyTransformation(PropertyValidationErrorTransformation transformation)
        {
            if (!string.IsNullOrWhiteSpace(transformation.OverridenMessage))
            {
                ErrorMessage = transformation.OverridenMessage;
            }

            if (transformation.OverridenMessage != null)
            {
                ErrorCode = transformation.OverriddenCode;
            }

            return this;
        }
    }
}