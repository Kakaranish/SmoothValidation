﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmoothValidation.Types;

namespace SmoothValidation.DependencyInjection.Filters
{
    internal static class Common
    {
        internal static ValueValidatorPair GetValueValidatorExplicitly(ActionExecutingContext context,
            Type notClosedValidatorType, Type typeToValidate)
        {
            var valueToValidate = context.ActionArguments.Values
                .FirstOrDefault(value => value.GetType() == typeToValidate);
            if (valueToValidate == null)
            {
                throw new InvalidOperationException(
                    $"There is no argument with type '{typeToValidate.Name}' within action context");
            }

            var validatorType = notClosedValidatorType.MakeGenericType(typeToValidate);
            var validator = context.HttpContext.RequestServices.GetRequiredService(validatorType);

            return new ValueValidatorPair
            {
                Value = valueToValidate,
                Validator = validator
            };
        }

        internal static ValueValidatorPair GetValueValidatorImplicitly(ActionExecutingContext context,
            Type notClosedValidatorType)
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

            throw new InvalidOperationException("There is no argument to validate in action context");
        }

        internal static MethodInfo GetValidationMethod(object validator)
        {
            return validator
                .GetType()
                .GetMethods()
                .FirstOrDefault(x =>
                    x.Name == "Validate" &&
                    x.ReturnType == typeof(IList<ValidationError>) &&
                    x.GetParameters()[0].ParameterType == typeof(object));
        }
    }
}
