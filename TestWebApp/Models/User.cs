using SmoothValidation.ClassValidators;
using SmoothValidation.ValidationExtensions;

namespace TestWebApp.Models
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    public class SyncUserValidator : ClassValidator<User>
    {
        protected override void SetupRules()
        {
            Setup(x => x.Age)
                .IsGreaterThan(0);
            
            Setup(x => x.FirstName)
                .IsNotNull().StopValidationAfterFailure()
                .HasMinLength(3)
                .AddRule(x => char.IsUpper(x[0]), "must start with upper char", "UPPER_CHAR_REQUIRED");

            Setup(x => x.LastName)
                .IsNotNull().StopValidationAfterFailure()
                .HasMinLength(3)
                .AddRule(x => char.IsUpper(x[0]), "must start with upper char", "UPPER_CHAR_REQUIRED");
        }
    }
}
