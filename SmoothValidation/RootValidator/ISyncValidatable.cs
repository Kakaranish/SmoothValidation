using System.Collections.Generic;
using SmoothValidation.PropertyValidator;
using SmoothValidation.Types;

namespace SmoothValidation.RootValidator
{
    public interface ISyncValidatable : IValidatable
    {
        IList<PropertyValidationError> Validate(object obj);
    }

    public interface ISyncValidatable<in TObject> : ISyncValidatable
    {
        IList<PropertyValidationError> Validate(TObject obj);
    }
}