using System.Collections.Generic;

namespace SmoothValidation
{
    public interface IValidator<in TObject>
    {
        IList<string> Validate(TObject obj);
    }
}