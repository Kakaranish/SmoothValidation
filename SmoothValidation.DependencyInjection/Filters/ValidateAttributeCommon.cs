using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace SmoothValidation.DependencyInjection.Filters
{
    public static class ValidateAttributeCommon
    {
        public static ValueValidatorPair GetValueValidatorImplicitly(ActionExecutingContext context, Type notClosedValidatorType)
        {
            var serviceProvider = context.HttpContext.RequestServices;

            foreach (var value in context.ActionArguments.Values)
            {
                var closedValidatorType = notClosedValidatorType.MakeGenericType(value.GetType());
                var validator = serviceProvider.GetService(closedValidatorType);

                if (validator != null)
                {
                    return new ValueValidatorPair
                    {
                        Value = value,
                        Validator = validator
                    };
                }
            }

            throw new InvalidOperationException($"There is no argument to validate in action context");
        }

        public static ValueValidatorPair GetValueValidatorExplicitly(ActionExecutingContext context, 
            Type notClosedValidatorType, Type typeToValidate)
        {
            var valueToValidate = context.ActionArguments.Values.FirstOrDefault(value => value.GetType() == typeToValidate);
            if (valueToValidate == null)
            {
                throw new InvalidOperationException($"There is no argument with type '{typeToValidate.Name}' within action context");
            }

            var closedValidatorType = notClosedValidatorType.MakeGenericType(typeToValidate);
            var validator = context.HttpContext.RequestServices.GetRequiredService(closedValidatorType);

            return new ValueValidatorPair
            {
                Value = valueToValidate,
                Validator = validator
            };
        }
    }
}
