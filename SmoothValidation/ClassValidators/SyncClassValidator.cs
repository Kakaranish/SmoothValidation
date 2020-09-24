using SmoothValidation.RootValidator;

namespace SmoothValidation.ClassValidators
{
    public abstract class SyncClassValidator<TObject> : RootSyncValidator<TObject>
    {
        protected SyncClassValidator()
        {
            SetupRules();
        }

        protected abstract void SetupRules();
    }
}
