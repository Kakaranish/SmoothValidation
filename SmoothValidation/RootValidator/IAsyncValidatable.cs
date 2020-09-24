using System.Collections.Generic;
using System.Threading.Tasks;
using SmoothValidation.PropertyValidator;
using SmoothValidation.Types;

namespace SmoothValidation.RootValidator
{
    public interface IAsyncValidatable : IValidatable
    {
        Task<IList<PropertyValidationError>> Validate(object obj);
    }

    public interface IAsyncValidatable<in TObject> : IAsyncValidatable
    {
        Task<IList<PropertyValidationError>> Validate(TObject obj);
    }
}