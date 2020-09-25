using SmoothValidation.Types;
using SmoothValidation.ValidatorsAbstraction;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                var validator = (ISyncValidator)validationTask.Validator;
                var validationErrorsForValidator = validator.Validate(obj);

                foreach (var propertyValidationError in validationErrorsForValidator)
                {
                    ProcessPropertyValidationError(propertyValidationError, validationTask);
                }
                validationErrors.AddRange(validationErrorsForValidator);

                if(validationErrorsForValidator.Any() && validationTask.StopValidationAfterFailure)
                {
                    break;
                }
            }

            return validationErrors;
        }
    }
}