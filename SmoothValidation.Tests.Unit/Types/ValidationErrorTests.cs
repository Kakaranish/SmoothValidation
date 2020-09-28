using FluentAssertions;
using Moq;
using NUnit.Framework;
using SmoothValidation.Types;
using System;

namespace SmoothValidation.Tests.Unit.Types
{
    [TestFixture]
    public class ValidationErrorTests
    {
        [Test]
        public void For_Ctor_When_PropertyNameIsNullOrWhitespace_Then_ExceptionIsThrown()
        {
            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() =>
                new ValidationError(null, It.IsAny<string>(), It.IsAny<object>()));
            exception.Message.Should().Contain("cannot be null or whitespace");
        }

        [Test]
        public void For_Ctor_When_ErrorMessageIsNullOrWhitespace_Then_ExceptionIsThrown()
        {
            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() =>
                new ValidationError("SomePropName", null, It.IsAny<object>()));
            exception.Message.Should().Contain("cannot be null or whitespace");
        }

        [Test]
        public void For_CtorWithNoPropertyNameRequired_When_ErrorMessageIsNullOrWhitespace_Then_ExceptionIsThrown()
        {
            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() =>
                new ValidationError(null, It.IsAny<object>()));
            exception.Message.Should().Contain("cannot be null or whitespace");
        }

        [Test]
        public void For_CtorWithNoPropertyNameRequired_When_ValidValuesArePassedAsArgs_Then_TransientValidationErrorIsReturned()
        {
            // Arrange:
            const string errorMessage = "SOME_MESSAGE";
            const string errorCode = "SOME_CODE";
            const string providedValue = "SOME_VALUE";

            // Act:
            var validationError = new ValidationError(errorMessage, providedValue, errorCode);

            // Assert:
            validationError.ErrorMessage.Should().Be(errorMessage);
            validationError.ProvidedValue.Should().Be(providedValue);
            validationError.ErrorCode.Should().Be(errorCode);
            validationError.PropertyPath.IsEmpty.Should().Be(true);
        }

        [Test]
        public void For_ApplyTransformation_When_OverriddenMessageIsNotNullOrWhitespace_Then_ItIsAppliedToValidationError()
        {
            // Arrange:
            const string propertyName = "PROPERTY_NAME";
            const string errorMessage = "SOME_MESSAGE";
            const string errorCode = "SOME_CODE";
            const string providedValue = "SOME_VALUE";
            var validationError = new ValidationError(propertyName, errorMessage, providedValue, errorCode);
            var transformation = new ValidationErrorTransformation
            {
                OverridenMessage = "OVERRIDEN_MESSAGE"
            };

            // Act:
            validationError.ApplyTransformation(transformation);

            // Assert:
            validationError.ErrorMessage.Should().Be("OVERRIDEN_MESSAGE");
        }

        [Test]
        public void For_ApplyTransformation_When_OverriddenCodeIsNotNull_Then_ItIsAppliedToValidationError()
        {
            // Arrange:
            const string propertyName = "PROPERTY_NAME";
            const string errorMessage = "SOME_MESSAGE";
            const string errorCode = "SOME_CODE";
            const string providedValue = "SOME_VALUE";
            var validationError = new ValidationError(propertyName, errorMessage, providedValue, errorCode);
            var transformation = new ValidationErrorTransformation
            {
                OverriddenCode = "OVERRIDEN_CODE"
            };

            // Act:
            validationError.ApplyTransformation(transformation);

            // Assert:
            validationError.ErrorCode.Should().Be("OVERRIDEN_CODE");
        }
    }
}
