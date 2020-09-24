using SmoothValidation.RootValidators;

namespace SmoothValidation.ClassValidators
{
    public abstract class ClassValidatorAsync<TObject> : RootAsyncValidator<TObject>
    {
        protected ClassValidatorAsync()
        {
            SetupRules();
        }

        protected abstract void SetupRules();
    }
}
