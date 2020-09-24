using System.Collections.Generic;
using System.Reflection;
using SmoothValidation.Types;

namespace SmoothValidation.PropertyValidator
{
    public interface ISyncPropertyValidator : IPropertyValidator
    {
        PropertyInfo Property { get; }
        IList<PropertyValidationError> Validate(object obj);
    }
}