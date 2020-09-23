using System.Collections.Generic;
using SmoothValidation.Types;

namespace SmoothValidation
{
    public interface IValidator
    {
        IList<PropertyValidationError> Validate(object obj);
    }

    public interface IValidatable<in TObject> : IValidator
    {
        IList<PropertyValidationError> Validate(TObject obj);
    }
}