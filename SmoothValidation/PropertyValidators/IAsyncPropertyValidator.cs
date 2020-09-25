using SmoothValidation.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmoothValidation.PropertyValidators
{
    public interface IAsyncPropertyValidator : IPropertyValidator
    {
        Property Property { get; }

        Task<IList<PropertyValidationError>> Validate(object obj);
    }
}
