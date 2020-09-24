using System.Collections.Generic;
using System.Threading.Tasks;
using SmoothValidation.Types;

namespace SmoothValidation.ValidatorsAbstraction
{
    public interface IAsyncValidator : IValidator
    {
        Task<IList<PropertyValidationError>> Validate(object obj);
    }

    public interface IAsyncValidator<in TObject> : IAsyncValidator
    {
        Task<IList<PropertyValidationError>> Validate(TObject obj);
    }
}