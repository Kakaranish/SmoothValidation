using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SmoothValidation.RootValidators
{
    public class PropertyExtractor<TObject> : IPropertyExtractor<TObject>
    {
        public PropertyInfo Extract<TProp>(Expression<Func<TObject, TProp>> expression)
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
