﻿using SmoothValidation.ClassValidators;

namespace SmoothValidation.DependencyInjection.Tests.Unit.TestsCommon
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

    internal class TestAlwaysFailingPersonValidatorSync : ClassValidator<Person>
    {
        protected override void SetupRules()
        {
            Setup(x => x.FirstName)
                .AddRule(x => false, "Invalid FirstName");
            Setup(x => x.LastName)
                .AddRule(x => false, "Invalid LastName");
        }
    }

    internal class TestAlwaysFailingPersonValidatorAsync : ClassValidatorAsync<Person>
    {
        protected override void SetupRules()
        {
            SetupAsync(x => x.FirstName)
                .AddRule(x => false, "Invalid FirstName");
            SetupAsync(x => x.LastName)
                .AddRule(x => false, "Invalid LastName");
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
