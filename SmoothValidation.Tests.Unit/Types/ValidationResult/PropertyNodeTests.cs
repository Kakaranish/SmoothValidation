using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SmoothValidation.Types.ValidationResult;

namespace SmoothValidation.Tests.Unit.Types.ValidationResult
{
    [TestFixture]
    public class PropertyNodeTests
    {
        [Test]
        public void For_Ctor_WhenCtorIsCalled_Then_ErrorsAndSubPropertiesShouldNotBeInitialized()
        {
            // Act:
            var propertyNode = new PropertyNode();

            // Assert:
            propertyNode.PropertyErrors.Should().BeNull();
            propertyNode.SubProperties.Should().BeNull();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void For_GetOrCreateSubProperty_When_SubPropertyNameIsNullOrWhitespace_Then_ExceptionIsThrown(string subPropertyName)
        {
            // Arrange:
            var propertyNode = new PropertyNode();

            // Act:
            var exception = Assert.Throws<ArgumentException>(() => 
                propertyNode.GetOrCreateSubProperty(subPropertyName));
            exception.Message.Should().Contain("cannot be null or whitespace");
        }

        [Test]
        public void For_GetOrCreateSubProperty_When_SubPropertyForProvidedNameDoesNotExist_Then_ItIsCreated()
        {
            // Arrange:
            var propertyNode = new PropertyNode();

            // Act & Assert:
            propertyNode.SubProperties.Should().BeNull();
            var subProperty = propertyNode.GetOrCreateSubProperty("SubProperty");
            propertyNode.SubProperties.Should().HaveCount(1);
            propertyNode.SubProperties["SubProperty"].Should().Be(subProperty);
            subProperty.PropertyName.Should().Be("SubProperty");
            subProperty.ProvidedValue.Should().BeNull();
        }

        [Test]
        public void For_AddError_When_ErrorIsAdded_Then_ErrorListIsInitializedAndErrorIsAppended()
        {
            // Arrange:
            var propertyNode = new PropertyNode();

            // Act & Assert:
            propertyNode.PropertyErrors.Should().BeNull();
            propertyNode.AddError(It.IsAny<PropertyError>());
            propertyNode.PropertyErrors.Should().HaveCount(1);
        }
    }
}
