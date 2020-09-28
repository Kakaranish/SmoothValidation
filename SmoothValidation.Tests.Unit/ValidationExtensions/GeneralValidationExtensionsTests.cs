using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.InlineValidators;
using SmoothValidation.ValidationExtensions;

namespace SmoothValidation.Tests.Unit.ValidationExtensions
{
    [TestFixture]
    public class GeneralValidationExtensionsTests
    {
        [Test]
        public void For_IsNotNull_When_PropertyValueIsNull_Then_ValidationErrorIsReturned()
        {
            // Arrange:
            var validator = new InlineValidator<Person>();
            validator.Setup(x => x.Name)
                .IsNotNull();
            validator.Setup(x => x.Age)
                .IsNotNull();

            var toValidate = new Person
            {
                Name = null,
                Age = default
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("Value cannot be null");
            validationErrors[0].ErrorCode.Should().Be("NOT_NULL");
            validationErrors[0].PropertyPath.IsEmpty.Should().BeFalse();
            validationErrors[0].PropertyPath.ToString().Should().Be("Name");
        }

        [Test]
        public void For_IsNotNull_When_PropertyIsNotNull_Then_ValidationErrorIsNotReturned()
        {
            // Arrange:
            var validator = new InlineValidator<Person>();
            validator.Setup(x => x.Name)
                .IsNotNull();
            validator.Setup(x => x.Age)
                .IsNotNull();

            var toValidate = new Person
            {
                Name = "Some Name",
                Age = default
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(0);
        }

        [Test]
        public void For_IsNull_When_PropertyValueIsNull_Then_ValidationErrorIsNotReturned()
        {
            // Arrange:
            var validator = new InlineValidator<Person>();
            validator.Setup(x => x.Name)
                .IsNull();

            var toValidate = new Person
            {
                Name = null,
                Age = default
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(0);
        }

        [Test]
        public void For_IsNull_When_PropertyValuesAreNotNull_Then_ValidationErrorsAreReturned()
        {
            // Arrange:
            var validator = new InlineValidator<Person>();
            validator.Setup(x => x.Name)
                .IsNull();
            validator.Setup(x => x.Age)
                .IsNull();

            var toValidate = new Person
            {
                Name = "Some Name",
                Age = default
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(2);

            validationErrors[0].ErrorMessage.Should().Be("Value must be null");
            validationErrors[0].ErrorCode.Should().Be("NULL");
            validationErrors[0].PropertyPath.IsEmpty.Should().BeFalse();
            validationErrors[0].PropertyPath.ToString().Should().Be("Name");
            validationErrors[0].ProvidedValue.Should().Be("Some Name");

            validationErrors[1].ErrorMessage.Should().Be("Value must be null");
            validationErrors[1].ErrorCode.Should().Be("NULL");
            validationErrors[1].PropertyPath.IsEmpty.Should().BeFalse();
            validationErrors[1].PropertyPath.ToString().Should().Be("Age");
            validationErrors[1].ProvidedValue.Should().Be(0);
        }

        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}
