using System.Collections.Generic;
using SmoothValidation.Types;

namespace SmoothValidation.ValidatorsAbstraction
{
    public interface ISyncValidator : IValidator
    {
        IList<PropertyValidationError> Validate(object obj);
    }

    public interface ISyncValidator<in TObject> : ISyncValidator
    {
        IList<PropertyValidationError> Validate(TObject obj);
    }
}