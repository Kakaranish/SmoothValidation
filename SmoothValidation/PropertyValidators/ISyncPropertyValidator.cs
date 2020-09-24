using System.Collections.Generic;
using System.Reflection;
using SmoothValidation.Types;

namespace SmoothValidation.PropertyValidators
{
    public interface ISyncPropertyValidator : IPropertyValidator
    {
        PropertyInfo Property { get; }
        IList<PropertyValidationError> Validate(object obj);
    }
}