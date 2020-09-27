using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.InlineValidators;
using System;

namespace SmoothValidation.Tests.Unit.GeneralTests
{
    [TestFixture]
    public class InlineValidatorsTests
    {
        [Test]
        public void SimpleInlineValidatorReturnsValidationError()
        {
            // Arrange:
            var line1Validator = new InlineValidator<Line1>();
            line1Validator.Setup(x => x.Street)
                .AddRule(x => x != null, "cannot be null", "NOT_NULL")
                .AddRule(x => char.IsUpper(x[0]), "must start with uppercase", "STARTS_WITH_UPPER");
            var toValidate = new Line1
            {
                Street = "street"
            };

            // Act:
            var validationErrors = line1Validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("must start with uppercase");
            validationErrors[0].ErrorCode.Should().Be("STARTS_WITH_UPPER");
            validationErrors[0].PropertyName.Should().Be("Street");
            validationErrors[0].ProvidedValue.Should().Be("street");
        }

        [Test]
        public void SimpleInlineValidatorThrowsExceptionBecauseOfInvalidValidationRule()
        {
            // Arrange:
            var line1Validator = new InlineValidator<Line1>();
            line1Validator.Setup(x => x.Street)
                .AddRule(x => x != null, "cannot be null", "NOT_NULL")
                .AddRule(x => char.IsUpper(x[0]), "must start with uppercase", "STARTS_WITH_UPPER");
            var toValidate = new Line1
            {
                Street = null
            };

            // Act & Assert:
            Assert.Throws<NullReferenceException>(() => line1Validator.Validate(toValidate));
        }

        [Test]
        public void SimpleInlineValidatorStopsAfterValidationFailure()
        {
            // Arrange:
            var line1Validator = new InlineValidator<Line1>();
            line1Validator.Setup(x => x.Street)
                .AddRule(x => x != null, "cannot be null", "NOT_NULL")
                .StopValidationAfterFailure()
                .AddRule(x => char.IsUpper(x[0]), "must start with uppercase", "STARTS_WITH_UPPER");
            var toValidate = new Line1
            {
                Street = null
            };

            // Act:
            var validationErrors = line1Validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("cannot be null");
            validationErrors[0].ErrorCode.Should().Be("NOT_NULL");
            validationErrors[0].PropertyName.Should().Be("Street");
            validationErrors[0].ProvidedValue.Should().Be(null);
        }

        [Test]
        public void SimpleInlineValidatorStopsAfterValidationFailureAndOverridesPropertyNameAndApplyErrorTransformations()
        {
            // Arrange:
            var line1Validator = new InlineValidator<Line1>();
            line1Validator.Setup(x => x.Street)
                .SetPropertyDisplayName("StreetName")
                .AddRule(x => x != null, "cannot be null", "NOT_NULL")
                .WithMessage("not null")
                .WithCode("MUST_NOT_BE_NULL")
                .StopValidationAfterFailure()
                .AddRule(x => char.IsUpper(x[0]), "must start with uppercase", "STARTS_WITH_UPPER");
            var toValidate = new Line1
            {
                Street = null
            };

            // Act:
            var validationErrors = line1Validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("not null");
            validationErrors[0].ErrorCode.Should().Be("MUST_NOT_BE_NULL");
            validationErrors[0].PropertyName.Should().Be("StreetName");
            validationErrors[0].ProvidedValue.Should().Be(null);
        }

        [Test]
        public void SimpleNestedInlineValidatorsLvl1()
        {
            // Arrange:
            var line1Validator = new InlineValidator<Line1>();
            line1Validator.Setup(x => x.Street)
                .SetPropertyDisplayName("StreetName")
                .AddRule(x => x != null, "cannot be null", "NOT_NULL")
                .WithMessage("not null")
                .WithCode("MUST_NOT_BE_NULL")
                .StopValidationAfterFailure()
                .AddRule(x => char.IsUpper(x[0]), "must start with uppercase", "STARTS_WITH_UPPER");

            var addressValidator = new InlineValidator<Address>();
            addressValidator.Setup(x => x.Line1)
                .SetPropertyDisplayName("CustomLine1")
                .SetValidator(line1Validator);

            var toValidate = new Address
            {
                Line1 = new Line1
                {
                    Street = null
                }
            };

            // Act:
            var validationErrors = addressValidator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("not null");
            validationErrors[0].ErrorCode.Should().Be("MUST_NOT_BE_NULL");
            validationErrors[0].PropertyName.Should().Be("CustomLine1.StreetName");
            validationErrors[0].ProvidedValue.Should().Be(null);
        }

        [Test]
        public void SimpleNestedInlineValidatorsLvl2()
        {
            // Arrange:
            var line1Validator = new InlineValidator<Line1>();
            line1Validator.Setup(x => x.Street)
                .SetPropertyDisplayName("StreetName")
                .AddRule(x => x != null, "cannot be null", "NOT_NULL")
                .WithMessage("not null")
                .WithCode("MUST_NOT_BE_NULL")
                .StopValidationAfterFailure()
                .AddRule(x => char.IsUpper(x[0]), "must start with uppercase", "STARTS_WITH_UPPER");

            var addressValidator = new InlineValidator<Address>();
            addressValidator.Setup(x => x.Line1)
                .SetPropertyDisplayName("CustomLine1")
                .SetValidator(line1Validator);

            var personValidator = new InlineValidator<Person>();
            personValidator.Setup(x => x.Address)
                .SetValidator(addressValidator)
                .SetPropertyDisplayName("CustomAddress");
            personValidator.Setup(x => x.Age)
                .AddRule(x => x > 0, "must be > 0", "GREATER_THAN_ZERO");

            var toValidate = new Person
            {
                Address = new Address
                {
                    Line1 = new Line1
                    {
                        Street = null
                    }
                },
                Age = -1
            };

            // Act:
            var validationErrors = personValidator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(2);

            validationErrors[0].ErrorMessage.Should().Be("not null");
            validationErrors[0].ErrorCode.Should().Be("MUST_NOT_BE_NULL");
            validationErrors[0].PropertyName.Should().Be("CustomAddress.CustomLine1.StreetName");
            validationErrors[0].ProvidedValue.Should().Be(null);

            validationErrors[1].ErrorMessage.Should().Be("must be > 0");
            validationErrors[1].ErrorCode.Should().Be("GREATER_THAN_ZERO");
            validationErrors[1].PropertyName.Should().Be("Age");
            validationErrors[1].ProvidedValue.Should().Be(-1);
        }

        [Test]
        public void SimpleNestedInlineValidatorsLvl2WithExceptionIfUnexpectedNullHappensInNestedValidator()
        {
            // Arrange:
            var line1Validator = new InlineValidator<Line1>();
            line1Validator.Setup(x => x.Street)
                .SetPropertyDisplayName("StreetName")
                .AddRule(x => x != null, "cannot be null", "NOT_NULL")
                .WithMessage("not null")
                .WithCode("MUST_NOT_BE_NULL")
                .StopValidationAfterFailure()
                .AddRule(x => char.IsUpper(x[0]), "must start with uppercase", "STARTS_WITH_UPPER");

            var addressValidator = new InlineValidator<Address>();
            addressValidator.Setup(x => x.Line1)
                .SetPropertyDisplayName("CustomLine1")
                .SetValidator(line1Validator);

            var personValidator = new InlineValidator<Person>();
            personValidator.Setup(x => x.Address)
                .SetValidator(addressValidator)
                .SetPropertyDisplayName("CustomAddress");
            personValidator.Setup(x => x.Age)
                .AddRule(x => x > 0, "must be > 0", "GREATER_THAN_ZERO");

            var toValidate = new Person
            {
                Address = null,
                Age = -1
            };

            // Act:
            Assert.Throws<ArgumentNullException>(() => personValidator.Validate(toValidate));
        }

        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public Address Address { get; set; }
        }

        private class Address
        {
            public Line1 Line1 { get; set; }
            public Line2 Line2 { get; set; }
        }

        private class Line1
        {
            public string Street { get; set; }
            public int Number { get; set; }
        }

        private class Line2
        {
            public string Country { get; set; }
            public string City { get; set; }
            public string ZipCode { get; set; }
        }
    }
}
