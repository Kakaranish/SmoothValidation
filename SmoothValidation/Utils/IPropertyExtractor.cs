using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SmoothValidation.Utils
{
    public interface IPropertyExtractor<TObject>
    {
        MemberInfo Extract<TProp>(Expression<Func<TObject, TProp>> expression);
    }
}