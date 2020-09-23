namespace SmoothValidation.Types
{
    public class PropertyValidationError
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
        public object ProvidedValue { get; set; }
    }
}