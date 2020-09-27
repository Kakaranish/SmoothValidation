using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.InlineValidators;
using SmoothValidation.ValidationExtensions;
using System;

namespace SmoothValidation.Tests.Unit.ValidationExtensions
{
    [TestFixture]
    public class StringValidationExtensionsTests
    {
        [Test]
        public void For_IsEqual_When_ValidatedValueIsNotEqualToRequiredValue_Then_PropertyErrorIsReturned()
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            validator.Setup(x => x.SomeProperty)
                .IsEqual("y");
            var toValidate = new TestClass
            {
                SomeProperty = "x"
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("Value should be equal to y");
            validationErrors[0].ErrorCode.Should().Be("STR_IS_NOT_EQUAL");
            validationErrors[0].IsTransient.Should().Be(false);
            validationErrors[0].PropertyName.Should().Be("SomeProperty");
            validationErrors[0].ProvidedValue.Should().Be("x");
        }

        [Test]
        public void For_IsNotEqual_When_ValidatedValueIsEqualToRequiredValue_Then_PropertyErrorIsReturned()
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            validator.Setup(x => x.SomeProperty)
                .IsNotEqual("x");
            var toValidate = new TestClass
            {
                SomeProperty = "x"
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("Value should not be equal to x");
            validationErrors[0].ErrorCode.Should().Be("STR_IS_EQUAL");
            validationErrors[0].IsTransient.Should().Be(false);
            validationErrors[0].PropertyName.Should().Be("SomeProperty");
            validationErrors[0].ProvidedValue.Should().Be("x");
        }

        [TestCase(null)]
        [TestCase("")]
        public void For_IsNotNullOrEmpty_When_ValidatedValueIsNullOrEmpty_Then_PropertyErrorIsReturned(string value)
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            validator.Setup(x => x.SomeProperty)
                .IsNotNullOrEmpty();
            var toValidate = new TestClass
            {
                SomeProperty = value
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("Value is null or empty but should not be");
            validationErrors[0].ErrorCode.Should().Be("STR_IS_NULL_OR_EMPTY");
            validationErrors[0].IsTransient.Should().Be(false);
            validationErrors[0].PropertyName.Should().Be("SomeProperty");
            validationErrors[0].ProvidedValue.Should().Be(value);
        }

        [Test]
        public void For_IsNotNullOrEmpty_When_ValidatedValueIsNotNullOrEmpty_Then_NoPropertyErrorIsReturned()
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            validator.Setup(x => x.SomeProperty)
                .IsNotNullOrEmpty();
            var toValidate = new TestClass
            {
                SomeProperty = "  "
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(0);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("  \t\n")]
        public void For_IsNotNullOrWhitespace_When_ValidatedValueIsNullOrWhitespace_Then_PropertyErrorIsReturned(string value)
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            validator.Setup(x => x.SomeProperty)
                .IsNotNullOrWhiteSpace();
            var toValidate = new TestClass
            {
                SomeProperty = value
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("Value is null or whitespace but should not be");
            validationErrors[0].ErrorCode.Should().Be("STR_IS_NULL_OR_WHITESPACE");
            validationErrors[0].IsTransient.Should().Be(false);
            validationErrors[0].PropertyName.Should().Be("SomeProperty");
            validationErrors[0].ProvidedValue.Should().Be(value);
        }

        [Test]
        public void For_HasMinLength_When_MinLengthIsSetToBeNegativeInteger_Then_ExceptionIsThrown()
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();

            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() =>
                validator.Setup(x => x.SomeProperty).HasMinLength(-1));
            exception.Message.Should().Be("'minLength' must be >= 0");
        }

        [TestCase("1234")]
        [TestCase("    ")]
        [TestCase(null)]
        public void For_HasMinLength_When_StrHasLengthLessThanMinimum_Then_ValidationErrorIsReturned(string value)
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            validator.Setup(x => x.SomeProperty)
                .HasMinLength(5);
            var toValidate = new TestClass
            {
                SomeProperty = value
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("Cannot have value less than 5");
            validationErrors[0].ErrorCode.Should().Be("STR_LESS_THAN_MIN_LENGTH");
            validationErrors[0].IsTransient.Should().Be(false);
            validationErrors[0].PropertyName.Should().Be("SomeProperty");
            validationErrors[0].ProvidedValue.Should().Be(value);
        }

        [TestCase("12345")]
        [TestCase("123456")]
        [TestCase("         ")]
        public void For_HasMinLength_When_StrHasLengthGreaterThanOrEqualToMinimum_Then_NoValidationErrorIsReturned(string value)
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            validator.Setup(x => x.SomeProperty)
                .HasMinLength(5);
            var toValidate = new TestClass
            {
                SomeProperty = value
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(0);
        }

        [Test]
        public void For_HasMaxLength_When_MaxLengthIsSetToBeNegativeInteger_Then_ExceptionIsThrown()
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();

            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() =>
                validator.Setup(x => x.SomeProperty).HasMaxLength(-1));
            exception.Message.Should().Be("'maxLength' must be >= 0");
        }

        [TestCase("123456")]
        [TestCase("       ")]
        [TestCase(null)]
        public void For_HasMaxLength_When_StrHasLengthGreaterThanMaxLength_Then_ValidationErrorIsReturned(string value)
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            validator.Setup(x => x.SomeProperty)
                .HasMaxLength(5);
            var toValidate = new TestClass
            {
                SomeProperty = value
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("Cannot have value greater than 5");
            validationErrors[0].ErrorCode.Should().Be("STR_GREATER_THAN_MAX_LENGTH");
            validationErrors[0].IsTransient.Should().Be(false);
            validationErrors[0].PropertyName.Should().Be("SomeProperty");
            validationErrors[0].ProvidedValue.Should().Be(value);
        }

        [TestCase("12345")]
        [TestCase("")]
        public void For_HasMaxLength_When_StrHasLengthLessThanOrEqualToMaxLength_Then_NoValidationErrorIsReturned(string value)
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            validator.Setup(x => x.SomeProperty)
                .HasMaxLength(5);
            var toValidate = new TestClass
            {
                SomeProperty = value
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(0);
        }

        [TestCase(-1, -1)]
        [TestCase(-1, 1)]
        [TestCase(1, -1)]
        public void For_HasLengthBetween_When_MinOrMaxLengthIsSetToBeNegativeInteger_Then_ExceptionIsThrown(int minLength, int maxLength)
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();

            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() =>
                validator.Setup(x => x.SomeProperty).HasLengthBetween(minLength, maxLength));
            exception.Message.Should().Be("'minLength' and 'maxLength' must be >= 0");
        }

        [Test]
        public void For_HasLengthBetween_When_MinLengthIsSetToBeGreaterThanMaxLength_Then_ExceptionIsThrown()
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();

            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() =>
                validator.Setup(x => x.SomeProperty).HasLengthBetween(10, 5));
            exception.Message.Should().Be("'minLength' cannot be greater than 'maxLength'");
        }

        [TestCase(null)]
        [TestCase("1234")]
        [TestCase("12345678")]
        public void For_HasLengthBetween_When_ValidatedValueIsOutOfRange_Then_ValidationErrorIsReturned(string value)
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            validator.Setup(x => x.SomeProperty)
                .HasLengthBetween(5, 7);
            var toValidate = new TestClass
            {
                SomeProperty = value
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("Must have value in range [5, 7](inclusive)");
            validationErrors[0].ErrorCode.Should().Be("STR_LENGTH_OUT_RANGE");
            validationErrors[0].IsTransient.Should().Be(false);
            validationErrors[0].PropertyName.Should().Be("SomeProperty");
            validationErrors[0].ProvidedValue.Should().Be(value);
        }

        [TestCase("12345")]
        [TestCase("123456")]
        [TestCase("1234567")]
        public void For_HasLengthBetween_When_ValidatedValueIsInRange_Then_NoValidationErrorIsReturned(string value)
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            validator.Setup(x => x.SomeProperty)
                .HasLengthBetween(5, 7);
            var toValidate = new TestClass
            {
                SomeProperty = value
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(0);
        }

        [Test]
        public void For_HasLength_When_RequiredLengthIsSetToNegativeInteger_Then_ExceptionIsThrown()
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();

            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() =>
                validator.Setup(x => x.SomeProperty).HasLength(-1));
            exception.Message.Should().Be("'length' must be >= 0");
        }

        [TestCase("1234")]
        [TestCase("123456")]
        public void For_HasLength_When_ValidatedStrHasLengthDifferentThanRequired_Then_ValidationErrorIsReturned(string value)
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            validator.Setup(x => x.SomeProperty).HasLength(5);
            var toValidate = new TestClass
            {
                SomeProperty = value
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("Must have length equal to 5");
            validationErrors[0].ErrorCode.Should().Be("STR_DIFFERENT_THAN_REQUIRED_LENGTH");
            validationErrors[0].IsTransient.Should().Be(false);
            validationErrors[0].PropertyName.Should().Be("SomeProperty");
            validationErrors[0].ProvidedValue.Should().Be(value);
        }

        [Test]
        public void For_MatchesRegex_When_InvalidRegexIsProvided_Then_ExceptionIsThrown()
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            var invalidRegex = "[sde;:\t";

            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() =>
                    validator.Setup(x => x.SomeProperty).MatchesRegex(invalidRegex));
            exception.Message.Should().Be($"'regex' represents invalid regex");
        }

        [Test]
        public void For_MatchesRegex_When_ValidatedStrDoesNotMatchRegex_Then_ValidationErrorIsReturned()
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            var regex = "^S+";
            validator.Setup(x => x.SomeProperty).MatchesRegex(regex);
            var toValidate = new TestClass
            {
                SomeProperty = "Blah blah"
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be($"Does not match regex '{regex}'");
            validationErrors[0].ErrorCode.Should().Be("STR_NOT_MATCHING_TO_REGEX");
            validationErrors[0].IsTransient.Should().Be(false);
            validationErrors[0].PropertyName.Should().Be("SomeProperty");
            validationErrors[0].ProvidedValue.Should().Be("Blah blah");
        }

        [Test]
        public void For_MatchesRegex_When_ValidatedStrMatchesRegex_Then_NoValidationErrorIsReturned()
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            var regex = "^S+";
            validator.Setup(x => x.SomeProperty).MatchesRegex(regex);
            var toValidate = new TestClass
            {
                SomeProperty = "SBlah blah"
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(0);
        }

        [TestCase("user")]
        [TestCase("user@mail")]
        [TestCase("!!!!")]
        [TestCase("@mail.com")]
        [TestCase("mail.com")]
        public void For_IsEmailAddress_When_ValidatedStrHasInvalidEmailFormat_Then_ValidationErrorIsReturned(string email)
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            validator.Setup(x => x.SomeProperty).IsEmailAddress();
            var toValidate = new TestClass
            {
                SomeProperty = email
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("Has invalid email format");
            validationErrors[0].ErrorCode.Should().Be("STR_INVALID_EMAIL");
            validationErrors[0].IsTransient.Should().Be(false);
            validationErrors[0].PropertyName.Should().Be("SomeProperty");
            validationErrors[0].ProvidedValue.Should().Be(email);
        }

        [TestCase("user@mail.com")]
        [TestCase("us.er@mail.com")]
        [TestCase("us.er@mail.pl")]
        [TestCase("us.er123@mail.wp")]
        [TestCase("US.er@mail.en")]
        [TestCase("user!#$%^@mail.com")]
        public void For_IsEmailAddress_When_ValidatedStrHasValidEmailFormat_Then_NoValidationErrorIsReturned(string email)
        {
            // Arrange:
            var validator = new InlineValidator<TestClass>();
            validator.Setup(x => x.SomeProperty).IsEmailAddress();
            var toValidate = new TestClass
            {
                SomeProperty = email
            };

            // Act:
            var validationErrors = validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(0);
        }

        private class TestClass
        {
            public string SomeProperty { get; set; }
        }
    }
}
