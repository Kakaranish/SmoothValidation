using Microsoft.Extensions.DependencyInjection;
using SmoothValidation.ClassValidators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SmoothValidation.DependencyInjection
{
    public static class SmoothValidationExtensions
    {
        private static readonly List<Type> ValidatorTypes = new List<Type>
        {
            typeof(ClassValidator<>),
            typeof(ClassValidatorAsync<>)
        };

        public static IServiceCollection AddSmoothValidation(this IServiceCollection services, params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var validatorType in ValidatorTypes)
                {
                    RegisterImplementationsOfTypeInAssembly(services, validatorType, assembly);
                }
            }

            return services;
        }
        
        private static void RegisterImplementationsOfTypeInAssembly(IServiceCollection services,
            Type genericTypeToRegister, Assembly assembly)
        {
            var implementingTypes = assembly
                .GetTypes()
                .Where(type => (type.BaseType?.IsGenericType ?? false) &&
                               type.BaseType.GetGenericTypeDefinition() == genericTypeToRegister &&
                               (IsPublic(type) || IsInternal(type)));

            foreach (var implementingType in implementingTypes)
            {
                var genericTypeArg = implementingType.BaseType.GenericTypeArguments.First();
                var filledGenericTypeToRegister = genericTypeToRegister.MakeGenericType(genericTypeArg);
                services.AddScoped(filledGenericTypeToRegister, implementingType);
            }
        }

        private static bool IsPublic(Type t)
        {
            return
                t.IsVisible
                && t.IsPublic
                && !t.IsNotPublic
                && !t.IsNested
                && !t.IsNestedPublic
                && !t.IsNestedFamily
                && !t.IsNestedPrivate
                && !t.IsNestedAssembly
                && !t.IsNestedFamORAssem
                && !t.IsNestedFamANDAssem;
        }

        private static bool IsInternal(Type t)
        {
            return
                !t.IsVisible
                && !t.IsPublic
                && t.IsNotPublic
                && !t.IsNested
                && !t.IsNestedPublic
                && !t.IsNestedFamily
                && !t.IsNestedPrivate
                && !t.IsNestedAssembly
                && !t.IsNestedFamORAssem
                && !t.IsNestedFamANDAssem;
        }
    }
}
