using System.Linq;

namespace SmoothValidation.Types
{
    public class PropertyValidationError
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public object ProvidedValue { get; set; }

        public PropertyValidationError ApplyTransformation(PropertyValidationErrorTransformation transformation)
        {
            if (!string.IsNullOrWhiteSpace(transformation.OverridenMessage))
            {
                ErrorMessage = transformation.OverridenMessage;
            }

            if (transformation.OverridenMessage != null)
            {
                ErrorCode = transformation.OverriddenCode;
            }

            return this;
        }
    }
}