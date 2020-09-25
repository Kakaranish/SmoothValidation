using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SmoothValidation.Utils
{
    public class PropertyExtractor<TObject> : IPropertyExtractor<TObject>
    {
        public MemberInfo Extract<TProp>(Expression<Func<TObject, TProp>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (!(expression.Body is MemberExpression memberExpression))
            {
                throw new ArgumentException($"'{nameof(expression)}' must be member expression");
            }

            var fieldInfo = typeof(TObject).GetField(memberExpression.Member.Name);
            if (fieldInfo != null)
            {
                return fieldInfo;
            }

            return typeof(TObject).GetProperty(memberExpression.Member.Name);
        }
    }
}
