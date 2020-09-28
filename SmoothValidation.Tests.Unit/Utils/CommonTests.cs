using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.Utils;
using System;

namespace SmoothValidation.Tests.Unit.Utils
{
    [TestFixture]
    public class CommonTests
    {
        [TestCase(null)]
        [TestCase("\t\t")]
        [TestCase("  ")]
        public void For_EnsurePropertyNameIsNullOrWhitespace_When_ProvidedPropertyNameIsValid_Then_ExceptionIsThrown(string propertyName)
        {
            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() => Common.EnsurePropertyNameIsValid(propertyName));
            exception.Message.Should().Contain("cannot be null or whitespace");
        }

        [TestCase("@Bool")]
        [TestCase("9Type")]
        [TestCase("_Parent.@static")]
        [TestCase("@class._")]
        [TestCase("@pou8_9iu.@")]
        [TestCase("@")]
        [TestCase("_Parent.9")]
        [TestCase("7")]
        public void For_EnsurePropertyNameIsInvalid_When_ProvidedPropertyNameIsValid_Then_ExceptionIsThrown(string propertyName)
        {
            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() => Common.EnsurePropertyNameIsValid(propertyName));
            exception.Message.Should().Contain("must match to regex");
        }

        [TestCase("Parent.Property")]
        [TestCase("Name")]
        [TestCase("_P.Age")]
        [TestCase("_Parent._Name")]
        [TestCase("p.Office_Address")]
        [TestCase("_")]
        public void For_EnsurePropertyNameIsValid_When_ProvidedPropertyNameIsValid_Then_NoExceptionIsThrown(string propertyName)
        {
            // Act & Assert:
            Assert.DoesNotThrow(() => Common.EnsurePropertyNameIsValid(propertyName));
        }
    }
}
