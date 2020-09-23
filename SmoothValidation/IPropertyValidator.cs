using SmoothValidation.Types;
using System.Collections.Generic;
using System.Reflection;

namespace SmoothValidation
{
    public interface IPropertyValidator
    {
        PropertyInfo Property { get; }
        IList<PropertyValidationError> Validate(object obj);
    }
}