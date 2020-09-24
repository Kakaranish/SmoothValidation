using SmoothValidation.RootValidator;

namespace SmoothValidation.ClassValidators
{
    public abstract class AsyncClassValidator<TObject> : RootAsyncValidator<TObject>
    {
        protected AsyncClassValidator()
        {
            SetupRules();
        }

        protected abstract void SetupRules();
    }
}
