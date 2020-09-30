using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.Types;
using SmoothValidation.ValidationExtensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmoothValidation.Tests.Unit.ValidationExtensions
{
    [TestFixture]
    public class ValidationErrorExtensionsTests
    {
        [Test]
        public void For_ToValidationResult_When_ProvidedOneValidationErrorThatHasEmptyPropertyPath_Then_ExceptionIsThrown()
        {
            // Arrange:
            var validationErrors = new List<ValidationError>();
            validationErrors.Add(new ValidationError("ERROR_MESSAGE", "PROVIDED_VALUE"));

            // Act & Assert:
            var exception = Assert.Throws<InvalidOperationException>(() => validationErrors.ToValidationResult());
            exception.Message.Should().Be("Detected PropertyPath is empty");
        }

        [Test]
        public void For_ToValidationResult_When_ProvidedOneValidationErrorThatHasNoSubProperties_Then_ValidationResultIsReturned()
        {
            // Arrange:
            var validationErrors = new List<ValidationError>();
            validationErrors.Add(CreateValidationError("SomeProperty"));

            // Act:
            var validationResult = validationErrors.ToValidationResult();

            // Assert:
            validationResult.Properties.Should().HaveCount(1);
            validationResult.Failure.Should().BeTrue();
            var property = validationResult.Properties.First();
            property.PropertyErrors.Should().HaveCount(1);
            property.PropertyName.Should().Be("SomeProperty");
            property.ProvidedValue.Should().Be("SomeProperty_PROVIDED_VALUE");
            property.PropertyErrors[0].ErrorMessage.Should().Be("SomeProperty_ERROR_MESSAGE");
            property.PropertyErrors[0].ErrorCode.Should().Be("SomeProperty_ERROR_CODE");
            property.SubProperties.Should().BeNull();
        }

        [Test]
        public void For_ToValidationResult_When_ProvidedMultipleValidationErrorsWithSubProperties_Then_ValidationResultIsReturneds()
        {
            // Arrange:
            var validationErrors = new List<ValidationError>
            {
                CreateValidationError("SomeProperty"),
                CreateValidationError("OtherProperty.SubProperty", "ERROR_MESSAGE1"),
                CreateValidationError("OtherProperty.SubProperty2", "ERROR_MESSAGE2")
            };

            // Act:
            var validationResult = validationErrors.ToValidationResult();

            // Assert:
            validationResult.Properties.Should().HaveCount(2);
            validationResult.Failure.Should().BeTrue();

            var property = validationResult.Properties[0];
            property.PropertyErrors.Should().HaveCount(1);
            property.PropertyName.Should().Be("SomeProperty");
            property.ProvidedValue.Should().Be("SomeProperty_PROVIDED_VALUE");
            property.PropertyErrors[0].ErrorMessage.Should().Be("SomeProperty_ERROR_MESSAGE");
            property.PropertyErrors[0].ErrorCode.Should().Be("SomeProperty_ERROR_CODE");
            property.SubProperties.Should().BeNull();

            property = validationResult.Properties[1];
            property.SubProperties.Should().HaveCount(2);
            property.PropertyName.Should().Be("OtherProperty");

            var subProperty = property.SubProperties["SubProperty"];
            subProperty.Should().NotBeNull();
            subProperty.PropertyName.Should().Be("SubProperty");
            subProperty.ProvidedValue.Should().Be("OtherProperty.SubProperty_PROVIDED_VALUE");
            subProperty.PropertyErrors.Should().HaveCount(1);
            subProperty.PropertyErrors[0].ErrorMessage.Should().Be("ERROR_MESSAGE1");
            subProperty.SubProperties.Should().BeNull();

            subProperty = property.SubProperties["SubProperty2"];
            subProperty.PropertyName.Should().Be("SubProperty2");
            subProperty.ProvidedValue.Should().Be("OtherProperty.SubProperty2_PROVIDED_VALUE");
            subProperty.PropertyErrors.Should().HaveCount(1);
            subProperty.PropertyErrors[0].ErrorMessage.Should().Be("ERROR_MESSAGE2");
            subProperty.SubProperties.Should().BeNull();
        }

        private static ValidationError CreateValidationError(string propertyPath, string errorMessage = null)
        {
            return new ValidationError(
                propertyName: propertyPath,
                errorMessage: errorMessage ?? $"{propertyPath}_ERROR_MESSAGE",
                providedValue: $"{propertyPath}_PROVIDED_VALUE",
                errorCode: $"{propertyPath}_ERROR_CODE");
        }
    }
}
