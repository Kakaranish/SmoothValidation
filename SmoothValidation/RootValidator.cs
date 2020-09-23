using SmoothValidation.Types;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SmoothValidation
{
    public class RootValidator<TObject> : IRootValidator, IValidatable<TObject>
    {
        private readonly IDictionary<string, IPropertyValidator> _propertyValidators =
            new Dictionary<string, IPropertyValidator>();

        public IList<PropertyValidationError> Validate(object obj)
        {
            return Validate((TObject)obj);
        }

        public IList<PropertyValidationError> Validate(TObject obj)
        {
            var validationErrors = new List<PropertyValidationError>();

            foreach (var propertyValidatorKvp in _propertyValidators)
            {
                var propertyValidator = propertyValidatorKvp.Value;
                var propertyValue = propertyValidator.Property.GetValue(obj);

                var errorsForValidator = propertyValidator.Validate(propertyValue);

                foreach (var propertyValidationError in errorsForValidator)
                {
                    if (propertyValidationError.PropertyName != propertyValidatorKvp.Key)
                    {
                        propertyValidationError.PropertyName = $"{propertyValidatorKvp.Key}.{propertyValidationError.PropertyName}";
                    }
                }

                validationErrors.AddRange(errorsForValidator);
            }

            return validationErrors;
        }

        public PropertyValidator<TProp> Setup<TProp>(Expression<Func<TObject, TProp>> expression)
        {
            var propertyInfo = ExtractProperty(expression);

            if (_propertyValidators.TryGetValue(propertyInfo.Name, out var propertyValidator))
            {
                return (PropertyValidator<TProp>)propertyValidator;
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