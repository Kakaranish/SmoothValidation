using System.Collections.Generic;

namespace SmoothValidation
{
    public interface IPropertyValidator
    {
        IList<string> Validate(object obj);
    }
}