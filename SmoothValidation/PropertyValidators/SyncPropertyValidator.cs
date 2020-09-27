using System;
using SmoothValidation.Types;
using SmoothValidation.ValidatorsAbstraction;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmoothValidation.Utils;

namespace SmoothValidation.PropertyValidators
{
    public sealed class SyncPropertyValidator<TProp> : PropertyValidatorBase<SyncPropertyValidator<TProp>, TProp>,
        ISyncPropertyValidator, ISyncValidator<TProp>
    {
        public SyncPropertyValidator(MemberInfo memberInfo) : base(memberInfo)
        {
        }

        internal override SyncPropertyValidator<TProp> PropertyValidator => this;

        public IList<ValidationError> Validate(object obj)
        {
            return Validate(Common.Cast<TProp>(obj));
        }

        public IList<ValidationError> Validate(TProp obj)
        {
            var validationErrors = new List<ValidationError>();

            foreach (var validationTask in ValidationTasks)
            {
                var validator = (ISyncValidator)validationTask.Validator;
                var validationErrorsForValidator = validator.Validate(obj);

                foreach (var validationError in validationErrorsForValidator)
                {
                    ProcessValidationError(validationError, validationTask.ErrorTransformation);
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