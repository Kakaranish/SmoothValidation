using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using SmoothValidation.Types;

namespace SmoothValidation.PropertyValidators
{
    public interface IAsyncPropertyValidator : IPropertyValidator
    {
        PropertyInfo Property { get; }
        Task<IList<PropertyValidationError>> Validate(object obj);
    }
}
