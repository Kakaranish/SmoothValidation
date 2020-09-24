using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmoothValidation.RootValidators;
using SmoothValidation.Types;
using SmoothValidation.ValidatorsAbstraction;

namespace SmoothValidation.PropertyValidators
{
    public class SyncPropertyValidator<TProp> : PropertyValidatorBase<SyncPropertyValidator<TProp>, TProp>, 
        ISyncPropertyValidator, ISyncValidator<TProp>
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

            foreach (var validationTask in ValidationTasks)
            {
                var validator = (ISyncValidator) validationTask.Validator;

                var validationErrorsForValidator = validator.Validate(obj);
                
                    foreach (var propertyValidationError in validationErrorsForValidator)
                    {
                        if (!(validator is IRootValidator))
                        {
                            propertyValidationError.PropertyName = propertyValidationError.PropertyName == string.Empty
                                ? Property.Name
                                : $"{Property.Name}.{propertyValidationError.PropertyName}";
                        }

                        if (!string.IsNullOrWhiteSpace(OverridenPropertyDisplayName))
                        {
                            var notChangedPropertyNamePart = string.Join(".", propertyValidationError.PropertyName.Split('.').Skip(1));
                            propertyValidationError.PropertyName = notChangedPropertyNamePart != string.Empty
                                ? $"{OverridenPropertyDisplayName}.{notChangedPropertyNamePart}"
                                : OverridenPropertyDisplayName;
                        }
                        propertyValidationError.ApplyTransformation(validationTask.PropertyValidationErrorTransformation);
                    }

                    validationErrors.AddRange(validationErrorsForValidator);
                    
                    if (validationErrorsForValidator.Any() && validationTask.StopValidationAfterFailure)
                    {
                        break;
                    }
            }

            return validationErrors;
        }
    }
}