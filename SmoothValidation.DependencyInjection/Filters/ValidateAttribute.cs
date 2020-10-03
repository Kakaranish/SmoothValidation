using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmoothValidation.ClassValidators;
using SmoothValidation.Types;
using SmoothValidation.ValidationExtensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmoothValidation.DependencyInjection.Filters
{
    public class ValidateAttribute : ActionFilterAttribute
    {
        private static readonly Type ValidatorType = typeof(ClassValidator<>);

        public Type TypeToValidate { get; set; }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var valueValidatorPair = TypeToValidate != null
                ? Common.GetValueValidatorExplicitly(context, ValidatorType, TypeToValidate)
                : Common.GetValueValidatorImplicitly(context, ValidatorType);

            var validationMethod = Common.GetValidationMethod(valueValidatorPair.Validator);
            var validationErrors = (IList<ValidationError>)validationMethod?.Invoke(
                valueValidatorPair.Validator, new[] { valueValidatorPair.Value });

            var validationResult = validationErrors.ToValidationResult();
            if (validationResult.Success)
            {
                return base.OnActionExecutionAsync(context, next);
            }

            context.Result = new BadRequestObjectResult(validationResult);

            return Task.CompletedTask;
        }
    }
}
