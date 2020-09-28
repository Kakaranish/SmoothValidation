using SmoothValidation.Types;
using System.Collections.Generic;

namespace SmoothValidation.PropertyValidators
{
    public interface ISyncPropertyValidator : IPropertyValidator
    {
        Member Member { get; }
        IList<ValidationError> Validate(object obj);
    }
}