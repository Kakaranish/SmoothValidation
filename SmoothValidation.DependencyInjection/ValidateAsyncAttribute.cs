using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SmoothValidation.ClassValidators;
using SmoothValidation.Types;
using SmoothValidation.ValidationExtensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmoothValidation.DependencyInjection
{
    public class ValidateAsyncAttribute : ActionFilterAttribute
    {
        private static readonly Type ValidatorType = typeof(ClassValidatorAsync<>);

        public Type TypeToValidate { get; set; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var valueValidatorPair = TypeToValidate != null
                ? Common.GetValueValidatorExplicitly(context, ValidatorType, TypeToValidate)
                : Common.GetValueValidatorImplicitly(context, ValidatorType);

            var validateMethod = Common.GetValidateAsyncMethod(valueValidatorPair.Validator)
                ?? throw new InvalidOperationException($"No async validate method for {valueValidatorPair.Validator.GetType().Name}'");
            var validationTask = (Task<IList<ValidationError>>)validateMethod.Invoke(
                valueValidatorPair.Validator, new[] { valueValidatorPair.Value });

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
