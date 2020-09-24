using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using SmoothValidation.PropertyValidator;

namespace SmoothValidation.RootValidator
{
    public abstract class ValidatorBase<TObject>
    {
        protected readonly IDictionary<string, IPropertyValidator> PropertyValidators =
            new Dictionary<string, IPropertyValidator>();

        public SyncPropertyValidator<TProp> Setup<TProp>(Expression<Func<TObject, TProp>> expression)
        {
            var propertyInfo = ExtractProperty(expression);

            if (PropertyValidators.TryGetValue(propertyInfo.Name, out var propertyValidator))
            {
                return (SyncPropertyValidator<TProp>)propertyValidator;
            }

            var newPropertyValidator = new SyncPropertyValidator<TProp>(propertyInfo);
            PropertyValidators.Add(propertyInfo.Name, newPropertyValidator);

            return newPropertyValidator;
        }

        protected PropertyInfo ExtractProperty<TProp>(Expression<Func<TObject, TProp>> expression)
        {
            if (!(expression.Body is MemberExpression memberExpression))
            {
                memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new InvalidOperationException(nameof(memberExpression));
            }

            var property = typeof(TObject).GetProperty(memberExpression.Member.Name);
            if (property == null)
            {
                throw new InvalidOperationException("Property from member expression is null");
            }

            return property;
        }
    }
}