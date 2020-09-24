using System;
using System.Collections.Generic;
using System.Reflection;
using SmoothValidation.RootValidator;
using SmoothValidation.Types;
using SmoothValidation.Types.Exceptions;

namespace SmoothValidation.PropertyValidator
{
    public class SyncPropertyValidator<TProp> : PropertyValidatorBase<SyncPropertyValidator<TProp>, TProp>, 
        ISyncPropertyValidator, ISyncValidatable<TProp>
    {
        public SyncPropertyValidator(PropertyInfo property) : base(property)
        {
        }

        protected override SyncPropertyValidator<TProp> PropertyValidator => this;

        public IList<PropertyValidationError> Validate(object obj)
        {
            return Validate((TProp)obj);
        }

        public IList<PropertyValidationError> Validate(TProp obj)
        {
            var validationErrors = new List<PropertyValidationError>();

            foreach (var validatable in Validators)
            {
                var validator = (ISyncValidatable) validatable;

                var validationErrorsForValidator = validator.Validate(obj);
                if (!(validator is IRootValidator))
                {
                    foreach (var propertyValidationError in validationErrorsForValidator)
                    {
                        propertyValidationError.PropertyName = propertyValidationError.PropertyName == string.Empty
                            ? Property.Name
                            : $"{Property.Name}.{propertyValidationError.PropertyName}";
                    }
                }

                validationErrors.AddRange(validationErrorsForValidator);
            }

            return validationErrors;
        }
    }
}