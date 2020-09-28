using SmoothValidation.Utils;
using System.Collections.Generic;

namespace SmoothValidation.Types
{
    public class PropertyPath
    {
        private List<string> _propertyNames;

        public IReadOnlyList<string> PropertyNames => _propertyNames;
        public bool IsEmpty => _propertyNames == null || _propertyNames.Count == 0;

        public PropertyPath()
        {
        }

        public PropertyPath(string propertyName)
        {
            Common.EnsurePropertyNameIsValid(propertyName);

            _propertyNames = new List<string> { propertyName };
        }

        public void PrependPropertyName(string propertyName)
        {
            Common.EnsurePropertyNameIsValid(propertyName);
            _propertyNames ??= new List<string>();
            _propertyNames.Insert(0, propertyName);
        }

        public override string ToString()
        {
            return _propertyNames != null
                ? string.Join('.', _propertyNames)
                : null;
        }
    }
}
