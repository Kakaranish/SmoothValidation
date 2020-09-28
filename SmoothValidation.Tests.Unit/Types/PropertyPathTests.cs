using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.Types;
using System;

namespace SmoothValidation.Tests.Unit.Types
{
    [TestFixture]
    public class PropertyPathTests
    {
        [Test]
        public void For_ParameterlessCtor_When_CtorIsCalled_Then_ObjectIsInstantiated()
        {
            // Act:
            var propertyPath = new PropertyPath();

            // Assert:
            propertyPath.IsEmpty.Should().BeTrue();
            propertyPath.PropertyNames.Should().BeNull();
        }
        
        [TestCase("@Bool")]
        [TestCase("9Type")]
        [TestCase("_Parent.@static")]
        [TestCase("\t\t")]
        [TestCase("  ")]
        public void For_Ctor_When_ProvidedPropertyNameIsNotValid_Then_ExceptionIsThrown(string propertyName)
        {
            // Act & Assert:
            Assert.Throws(Is.AssignableTo<Exception>(), () => new PropertyPath(propertyName));
        }

        [TestCase("Parent.Property")]
        [TestCase("Name")]
        public void For_Ctor_When_ProvidedPropertyNameIsValid_Then_ObjectIsInstantiated(string propertyName)
        {
            // Act:
            var propertyPath = new PropertyPath(propertyName);

            // Assert:
            propertyPath.IsEmpty.Should().BeFalse();
            propertyPath.PropertyNames.Count.Should().Be(1);
            propertyPath.PropertyNames[0].Should().Be(propertyName);
        }

        [TestCase(null)]
        [TestCase("\t\t")]
        [TestCase("  ")]
        [TestCase("@Bool")]
        [TestCase("9Type")]
        [TestCase("_Parent.@static")]
        public void For_PrependPropertyName_When_PropertyNameIsInvalid_Then_ExceptionIsThrown(string propertyName)
        {
            // Arrange:
            var propertyPath = new PropertyPath("SubProperty");

            // Act & Assert:
            Assert.Throws(Is.AssignableTo<Exception>(), () => propertyPath.PrependPropertyName(propertyName));
        }

        [Test]
        public void For_PrependPropertyName_When_PropertyNameIsValidAndPropertyNamesAreEmpty_Then_ItIsPrepended()
        {
            // Arrange:
            var propertyPath = new PropertyPath();

            // Act & Assert:
            propertyPath.IsEmpty.Should().BeTrue();
            propertyPath.PropertyNames.Should().BeNull();

            propertyPath.PrependPropertyName("SomeProperty");

            propertyPath.IsEmpty.Should().BeFalse();
            propertyPath.PropertyNames.Should().HaveCount(1);
            propertyPath.ToString().Should().Be("SomeProperty");
        }

        [Test]
        public void For_PrependPropertyName_When_PropertyNameIsValidAndPropertyNamesAreNotEmpty_Then_ItIsPrepended()
        {
            // Arrange:
            var propertyPath = new PropertyPath("SubProperty");

            // Act & Assert:
            propertyPath.IsEmpty.Should().BeFalse();
            propertyPath.PropertyNames.Should().HaveCount(1);

            propertyPath.PrependPropertyName("SomeProperty");

            propertyPath.IsEmpty.Should().BeFalse();
            propertyPath.PropertyNames.Should().HaveCount(2);
            propertyPath.ToString().Should().Be("SomeProperty.SubProperty");
        }

        [Test]
        public void For_ToString_When_ThereAreNoPropertyNamesProvided_Then_NullIsReturned()
        {
            // Arrange:
            var propertyPath = new PropertyPath();

            // Act:
            var result = propertyPath.ToString();

            // Assert:
            propertyPath.PropertyNames.Should().BeNull();
            result.Should().BeNull();
        }

        [Test]
        public void For_ToString_When_ThereArePropertyNamesProvided_Then_NotEmptyStringResultIsReturned()
        {
            // Arrange:
            var propertyPath = new PropertyPath("SubProperty");
            propertyPath.PrependPropertyName("Property");

            // Act:
            var result = propertyPath.ToString();

            // Assert:
            propertyPath.PropertyNames.Should().HaveCount(2);
            result.Should().Be("Property.SubProperty");
        }
    }
}
