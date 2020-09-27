using SmoothValidation.Types;
using System.Collections.Generic;

namespace SmoothValidation.PropertyValidators
{
    public interface ISyncPropertyValidator : IPropertyValidator
    {
        Property Property { get; }
        IList<ValidationError> Validate(object obj);
    }
}