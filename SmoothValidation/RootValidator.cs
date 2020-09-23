using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SmoothValidation
{
    public class RootValidator<TObject> : IValidator<TObject>
    {
        private readonly IDictionary<string, IPropertyValidator> _propertyValidators;

        public RootValidator()
        {
            _propertyValidators = new Dictionary<string, IPropertyValidator>();
        }

        public IList<string> Validate(TObject obj)
        {
            var validationErrors = new List<string>();
            foreach (var propertyValidator in _propertyValidators.Values)
            {
                validationErrors.AddRange(propertyValidator.Validate(obj));
            }

            return validationErrors.Any()
                ? validationErrors
                : null;
        }

        public PropertyValidator<TProp> Setup<TProp>(Expression<Func<TObject, TProp>> expression)
        {
            var propertyInfo = ExtractProperty(expression);

            if (_propertyValidators.TryGetValue(propertyInfo.Name, out var propertyValidator))
            {
                return (PropertyValidator<TProp>) propertyValidator;
            }

            var newPropertyValidator = new PropertyValidator<TProp>(propertyInfo);
            _propertyValidators.Add(propertyInfo.Name, newPropertyValidator);
            
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