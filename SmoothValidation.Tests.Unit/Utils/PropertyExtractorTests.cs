using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.Utils;

namespace SmoothValidation.Tests.Unit.Utils
{
    [TestFixture]
    public class PropertyExtractorTests
    {
        [Test]
        public void For_Extract_When_ExpressionIsNull_Then_ExceptionIsThrown()
        {
            // Arrange:
            Expression<Func<TestPerson, string>> expression = null;

            // Act & Assert:
            var extractor = new PropertyExtractor<TestPerson>();
            Assert.Throws<ArgumentNullException>(() => extractor.Extract(expression));
        }

        [Test]
        public void For_Extract_When_ExpressionIsNotMemberExpression_Then_ExceptionIsThrown()
        {
            // Arrange:
            var testPerson = new TestPerson
            {
                AnyProperty = "PersonName"
            };
            Expression<Func<TestPerson, int>> expression = person => person.AnyMethod();

            // Act:
            var extractor = new PropertyExtractor<TestPerson>();
            var exception = Assert.Throws<ArgumentException>(() => extractor.Extract(expression));
            
            // Assert:
            exception.Message.Should().Contain("must be member expression");
        }

        [Test]
        public void For_Extract_When_ExpressionIsFieldInfo_Then_ItIsReturned()
        {
            // Arrange:
            var testPerson = new TestPerson();
            Expression<Func<TestPerson, string>> expression = person => person.AnyField;

            // Act:
            var extractor = new PropertyExtractor<TestPerson>();
            var result = extractor.Extract(expression);

            // Assert:
            result.MemberType.Should().Be(MemberTypes.Field);
        }

        [Test]
        public void For_Extract_When_ExpressionIsPropertyInfo_Then_ItIsReturned()
        {
            // Arrange:
            var testPerson = new TestPerson();
            Expression<Func<TestPerson, string>> expression = person => person.AnyProperty;

            // Act:
            var extractor = new PropertyExtractor<TestPerson>();
            var result = extractor.Extract(expression);

            // Assert:
            result.MemberType.Should().Be(MemberTypes.Property);
        }

        private class TestPerson
        {
            public string AnyField = "Some value";
            public string AnyProperty { get; set; }
            public int AnyMethod() => 1;
        }
    }
}
