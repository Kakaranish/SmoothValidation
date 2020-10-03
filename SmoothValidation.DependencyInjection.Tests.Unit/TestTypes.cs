using SmoothValidation.ClassValidators;

namespace SmoothValidation.DependencyInjection.Tests.Unit
{
    internal class Person
    {
        internal string FirstName { get; set; }
        internal string LastName { get; set; }
        internal int Age { get; set; }
    }

    internal class Address
    {
        internal string City { get; set; }
        internal string Street { get; set; }
    }

    public class PublicTestAddressValidatorSync : ClassValidator<string>
    {
        protected override void SetupRules()
        {
        }
    }

    internal class TestAddressValidatorSync : ClassValidator<Address>
    {
        protected override void SetupRules()
        {
        }
    }

    internal class TestAddressValidatorAsync : ClassValidatorAsync<Address>
    {
        protected override void SetupRules()
        {
        }
    }

    internal class TestPersonValidatorSync : ClassValidator<Person>
    {
        protected override void SetupRules()
        {
        }
    }

    internal class TestDuplicatedPersonValidatorSync : ClassValidator<Person>
    {
        protected override void SetupRules()
        {
        }
    }

    internal class TestPersonValidatorAsync : ClassValidatorAsync<Person>
    {
        protected override void SetupRules()
        {
        }
    }

    internal class TestDuplicatedPersonValidatorAsync : ClassValidatorAsync<Person>
    {
        protected override void SetupRules()
        {
        }
    }
}
