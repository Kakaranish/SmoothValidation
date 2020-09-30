using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmoothValidation.ClassValidators;
using SmoothValidation.Types;
using SmoothValidation.ValidationExtensions;

namespace SmoothValidation.DependencyInjection.Filters
{
    public class ValidateAsyncAttribute : ActionFilterAttribute
    {
        private static readonly Type ValidatorType = typeof(ClassValidatorAsync<>);

        public Type TypeToValidate { get; set; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var valueValidatorPair = TypeToValidate != null
                ? ValidateAttributeCommon.GetValueValidatorExplicitly(context, ValidatorType, TypeToValidate)
                : ValidateAttributeCommon.GetValueValidatorImplicitly(context, ValidatorType);

            var validationMethod = valueValidatorPair.Validator.GetType().GetMethod("Validate");
            var validationTask = (Task<IList<ValidationError>>)validationMethod?.Invoke(
                valueValidatorPair.Validator, new[] { valueValidatorPair.Value });

            if (validationTask == null)
            {
                throw new InvalidOperationException(nameof(validationTask));
            }
            
            var validationErrors = await validationTask;
            var validationResult = validationErrors.ToValidationResult();

            if (validationResult.Success)
            {
                await base.OnActionExecutionAsync(context, next);
                return;
            }

            context.Result = new BadRequestObjectResult(validationResult);
        }
    }
}
