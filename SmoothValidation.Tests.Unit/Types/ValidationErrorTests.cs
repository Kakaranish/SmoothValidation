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
        public void For_CreateTransient_When_ErrorMessageIsNullOrWhitespace_Then_ExceptionIsThrown()
        {
            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() =>
                ValidationError.CreateTransient(null, It.IsAny<object>()));
            exception.Message.Should().Contain("cannot be null or whitespace");
        }

        [Test]
        public void For_CreateTransient_When_ValidValuesArePassedAsArgs_Then_TransientValidationErrorIsReturned()
        {
            // Arrange:
            const string errorMessage = "SOME_MESSAGE";
            const string errorCode = "SOME_CODE";
            const string providedValue = "SOME_VALUE";

            // Act:
            var validationError = ValidationError.CreateTransient(errorMessage, providedValue, errorCode);

            // Assert:
            validationError.ErrorMessage.Should().Be(errorMessage);
            validationError.ProvidedValue.Should().Be(providedValue);
            validationError.ErrorCode.Should().Be(errorCode);
            validationError.IsTransient.Should().Be(true);
            validationError.PropertyName.Should().BeNull();
        }

        [Test]
        public void For_SetPropertyName_When_ValidationErrorIsNotTransient_Then_ExceptionIsThrown()
        {
            // Arrange:
            const string propertyName = "PROPERTY_NAME";
            const string errorMessage = "SOME_MESSAGE";
            const string errorCode = "SOME_CODE";
            const string providedValue = "SOME_VALUE";
            var validationError = new ValidationError(propertyName, errorMessage, providedValue, errorCode);

            // Act & Assert:
            var exception = Assert.Throws<InvalidOperationException>(() =>
                validationError.SetPropertyName(It.IsAny<string>()));
            exception.Message.Should().Be("Unable to directly set property name when object is not transient");
        }

        [TestCase(null)]
        [TestCase("   ")]
        public void For_SetPropertyName_When_ValidationErrorIsTransientButNewPropertyNameIsNullOrWhitespace_Then_ExceptionIsThrown(string newPropertyName)
        {
            // Arrange:
            const string errorMessage = "SOME_MESSAGE";
            const string errorCode = "SOME_CODE";
            const string providedValue = "SOME_VALUE";
            var validationError = ValidationError.CreateTransient(errorMessage, providedValue, errorCode);

            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() =>
                validationError.SetPropertyName(newPropertyName));
            exception.Message.Should().Contain("cannot be null or whitespace");
        }

        [Test]
        public void For_SetPropertyName_When_ValidationErrorIsTransientAndNewPropertyNameIsValid_Then_ItIsSet()
        {
            // Arrange:
            const string errorMessage = "SOME_MESSAGE";
            const string errorCode = "SOME_CODE";
            const string providedValue = "SOME_VALUE";
            var validationError = ValidationError.CreateTransient(errorMessage, providedValue, errorCode);

            // Act::
            validationError.SetPropertyName("NEW_PROPERTY_NAME");

            // Assert:
            validationError.PropertyName.Should().Be("NEW_PROPERTY_NAME");
        }

        [Test]
        public void For_PrependParentPropertyName_When_ValidationErrorIsTransient_Then_ExceptionIsThrown()
        {
            // Arrange:
            const string errorMessage = "SOME_MESSAGE";
            const string errorCode = "SOME_CODE";
            const string providedValue = "SOME_VALUE";
            var validationError = ValidationError.CreateTransient(errorMessage, providedValue, errorCode);

            // Act & Assert:
            var exception = Assert.Throws<InvalidOperationException>(() =>
                validationError.PrependParentPropertyName(It.IsAny<string>()));
            exception.Message.Should().Contain("Unable to append parent property name when in transient state");
        }

        [TestCase(null)]
        [TestCase("    ")]
        public void For_PrependParentPropertyName_When_ValidationErrorIsNotTransientButProvidedPropertyNameIsNullOrWhitespace_Then_ExceptionIsThrown(string newPropertyName)
        {
            // Arrange:
            const string propertyName = "PROPERTY_NAME";
            const string errorMessage = "SOME_MESSAGE";
            const string errorCode = "SOME_CODE";
            const string providedValue = "SOME_VALUE";
            var validationError = new ValidationError(propertyName, errorMessage, providedValue, errorCode);

            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() =>
                validationError.PrependParentPropertyName(newPropertyName));
            exception.Message.Should().Contain("cannot be null or whitespace");
        }

        [Test]
        public void For_PrependParentPropertyName_When_ValidationErrorIsNotTransientAndParentPropNameIsValid_Then_PropertyNameIsModified()
        {
            // Arrange:
            const string propertyName = "PROPERTY_NAME";
            const string errorMessage = "SOME_MESSAGE";
            const string errorCode = "SOME_CODE";
            const string providedValue = "SOME_VALUE";
            var validationError = new ValidationError(propertyName, errorMessage, providedValue, errorCode);

            // Act:
            validationError.PrependParentPropertyName("PARENT_PROP_NAME");

            // Assert:
            validationError.PropertyName.Should().Be("PARENT_PROP_NAME.PROPERTY_NAME");
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
