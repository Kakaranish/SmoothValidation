using SmoothValidation.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmoothValidation.PropertyValidators
{
    public interface IAsyncPropertyValidator : IPropertyValidator
    {
        Member Member { get; }

        Task<IList<ValidationError>> Validate(object obj);
    }
}
