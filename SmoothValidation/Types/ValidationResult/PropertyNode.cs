using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmoothValidation.Types.ValidationResult
{
    public class PropertyNode
    {
        private Dictionary<string, PropertyNode> _subProperties;
        private List<PropertyError> _propertyErrors;

        public string PropertyName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object ProvidedValue { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyDictionary<string, PropertyNode> SubProperties => _subProperties;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyList<PropertyError> PropertyErrors => _propertyErrors;

        public PropertyNode GetOrCreateSubProperty(string subPropertyName)
        {
            if (string.IsNullOrWhiteSpace(subPropertyName))
            {
                throw new ArgumentException($"{nameof(subPropertyName)} cannot be null or whitespace");
            }

            _subProperties ??= new Dictionary<string, PropertyNode>();

            if (!_subProperties.TryGetValue(subPropertyName, out var subProperty))
            {
                subProperty = new PropertyNode { PropertyName = subPropertyName };
                _subProperties.Add(subPropertyName, subProperty);
            }

            return subProperty;
        }

        public void AddError(PropertyError propertyError)
        {
            _propertyErrors ??= new List<PropertyError>();
            _propertyErrors.Add(propertyError);
        }
    }
}