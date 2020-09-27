using System.Collections.Generic;
using SmoothValidation.Types;

namespace SmoothValidation.ValidatorsAbstraction
{
    public interface ISyncValidator : IValidator
    {
        IList<ValidationError> Validate(object obj);
    }

    public interface ISyncValidator<in TObject> : ISyncValidator
    {
        IList<ValidationError> Validate(TObject obj);
    }
}