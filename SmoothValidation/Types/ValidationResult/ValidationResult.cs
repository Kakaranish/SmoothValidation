using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SmoothValidation.Types.ValidationResult
{
    public class ValidationResult
    {
        private Dictionary<string, PropertyNode> _properties;

        public bool Success => _properties == null || _properties.Count == 0;
        public bool Failure => !Success;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<PropertyNode> Properties => _properties?.Values.ToList();

        public PropertyNode GetOrCreateProperty(string subPropertyName)
        {
            _properties ??= new Dictionary<string, PropertyNode>();

            if (!_properties.TryGetValue(subPropertyName, out var subProperty))
            {
                subProperty = new PropertyNode();
                _properties.Add(subPropertyName, subProperty);
            }

            return subProperty;
        }
    }
}
