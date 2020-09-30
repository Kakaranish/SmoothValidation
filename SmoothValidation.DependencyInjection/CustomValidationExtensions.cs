using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using SmoothValidation.ClassValidators;

namespace SmoothValidation.DependencyInjection
{
    public static class CustomValidationExtensions
    {
        public static IServiceCollection AddSmoothValidation(this IServiceCollection services, params Assembly[] assemblies)
        {
            var validatorTypes = new List<Type> { typeof(ClassValidator<>), typeof(ClassValidatorAsync<>) };
            foreach (var assembly in assemblies)
            {
                foreach (var validatorType in validatorTypes)
                {
                    RegisterImplementationsOfTypeInAssembly(services, validatorType, assembly);
                }
            }

            return services;
        }

        private static void RegisterImplementationsOfTypeInAssembly(IServiceCollection services, Type genericTypeToRegister, 
            Assembly assembly)
        {
            var implementingTypes = assembly
                .GetTypes()
                .Where(type => (type.BaseType?.IsGenericType ?? false) && 
                               type.BaseType.GetGenericTypeDefinition() == genericTypeToRegister);

            foreach (var implementingType in implementingTypes)
            {
                var genericTypeArg = implementingType.BaseType.GenericTypeArguments.First();
                var filledGenericTypeToRegister = genericTypeToRegister.MakeGenericType(genericTypeArg);
                services.AddScoped(filledGenericTypeToRegister, implementingType);
            }
        }
    }
}
