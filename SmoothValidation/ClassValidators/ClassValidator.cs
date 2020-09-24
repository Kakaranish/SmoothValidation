using SmoothValidation.RootValidators;

namespace SmoothValidation.ClassValidators
{
    public abstract class ClassValidator<TObject> : RootSyncValidator<TObject>
    {
        protected ClassValidator()
        {
            SetupRules();
        }

        protected abstract void SetupRules();
    }
}
