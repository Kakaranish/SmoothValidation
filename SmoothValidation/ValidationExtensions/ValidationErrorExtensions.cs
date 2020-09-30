using SmoothValidation.Types;
using SmoothValidation.Types.ValidationResult;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmoothValidation.ValidationExtensions
{
    public static class ValidationErrorExtensions
    {
        public static ValidationResult ToValidationResult(this IList<ValidationError> validationErrors)
        {
            var validationResult = new ValidationResult();

            foreach (var validationError in validationErrors)
            {
                if (validationError.PropertyPath.IsEmpty)
                {
                    throw new InvalidOperationException("Detected PropertyPath is empty");
                }

                var propertyNames = validationError.PropertyPath.PropertyNames;

                var propNode = validationResult.GetOrCreateProperty(propertyNames[0]);
                propNode.PropertyName = propertyNames[0];

                foreach (var subPropertyName in propertyNames.Skip(1))
                {
                    propNode = propNode.GetOrCreateSubProperty(subPropertyName);
                }

                propNode.ProvidedValue = validationError.ProvidedValue;

                propNode.AddError(new PropertyError
                {
                    ErrorMessage = validationError.ErrorMessage,
                    ErrorCode = validationError.ErrorCode
                });
            }

            return validationResult;
        }
    }
}
