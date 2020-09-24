using SmoothValidation.Types;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace SmoothValidation.PropertyValidator
{
    public interface IAsyncPropertyValidator : IPropertyValidator
    {
        PropertyInfo Property { get; }
        Task<IList<PropertyValidationError>> Validate(object obj);
    }
}
