namespace SmoothValidation
{
    public interface IValidationRule
    {
        string Validate(object obj);
    }
}