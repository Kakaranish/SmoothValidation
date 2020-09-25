using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SmoothValidation.RootValidators
{
    public interface IPropertyExtractor<TObject>
    {
        PropertyInfo Extract<TProp>(Expression<Func<TObject, TProp>> expression);
    }
}