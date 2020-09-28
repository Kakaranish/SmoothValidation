using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.Types;
using System;
using System.Linq.Expressions;

namespace SmoothValidation.Tests.Unit.Types
{
    [TestFixture]
    public class MemberTests
    {
        [Test]
        public void When_ProvidedMemberInfoIsNull_Then_ExceptionIsThrown()
        {
            // Act & Assert:
            Assert.Throws<ArgumentNullException>(() => new Member(null));
        }

        [Test]
        public void When_ProvidedMemberInfoConcernsIllegalMemberInfoType_Then_ExceptionIsThrown()
        {
            // Arrange:
            var memberInfo = typeof(TestClass).GetMethod("SomeMethod");

            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() => new Member(memberInfo));
            exception.Message.Should().Contain("is illegal type");
        }

        [Test]
        public void When_ProvidedMemberInfoConcernsField_Then_PropertyForFieldIsCreated()
        {
            // Arrange:
            var someObj = new TestClass();
            Expression<Func<TestClass, string>> expression = obj => obj.SomeField;
            var memberExpression = expression.Body as MemberExpression;
            var memberInfo = typeof(TestClass).GetField(memberExpression.Member.Name);

            // Act:
            var result = new Member(memberInfo);

            // Assert:
            result.MemberInfo.Should().NotBeNull();
            result.IsField.Should().BeTrue();
            result.IsProperty.Should().BeFalse();
            result.Name.Should().Be("SomeField");
        }

        [Test]
        public void When_ProvidedMemberInfoConcernsProperty_Then_PropertyForPropertyIsCreated()
        {
            // Arrange:
            var someObj = new TestClass(); ;
            Expression<Func<TestClass, string>> expression = obj => obj.SomeProperty;
            var memberExpression = expression.Body as MemberExpression;
            var memberInfo = typeof(TestClass).GetProperty(memberExpression.Member.Name);

            // Act:
            var result = new Member(memberInfo);

            // Assert:
            result.MemberInfo.Should().NotBeNull();
            result.IsProperty.Should().BeTrue();
            result.IsField.Should().BeFalse();
            result.Name.Should().Be("SomeProperty");
        }

        [Test]
        public void When_PropertyConcernsFieldOrPropertyButObjValueIsNull_Then_ExceptionIsThrown()
        {
            // Arrange:
            TestClass someObj = null;
            Expression<Func<TestClass, string>> expression = obj => obj.SomeField;
            var memberExpression = expression.Body as MemberExpression;
            var memberInfo = typeof(TestClass).GetField(memberExpression.Member.Name);

            // Act & Assert:
            var member = new Member(memberInfo);
            Assert.Throws<ArgumentNullException>(() => member.GetValue(someObj));
        }

        [Test]
        public void When_PropertyConcernsField_Then_GetValueReturnsValueForThatField()
        {
            // Arrange:
            var someObj = new TestClass
            {
                SomeField = "Value for field"
            };
            Expression<Func<TestClass, string>> expression = obj => obj.SomeField;
            var memberExpression = expression.Body as MemberExpression;
            var memberInfo = typeof(TestClass).GetField(memberExpression.Member.Name);

            // Act:
            var member = new Member(memberInfo);
            var result = member.GetValue(someObj);

            // Assert:
            result.Should().Be("Value for field");
        }

        [Test]
        public void When_PropertyConcernsProperty_Then_GetValueReturnsValueForThatProperty()
        {
            // Arrange:
            var someObj = new TestClass
            {
                SomeProperty = "Value for property"
            };
            Expression<Func<TestClass, string>> expression = obj => obj.SomeProperty;
            var memberExpression = expression.Body as MemberExpression;
            var memberInfo = typeof(TestClass).GetProperty(memberExpression.Member.Name);

            // Act:
            var member = new Member(memberInfo);
            var result = member.GetValue(someObj);

            // Assert:
            result.Should().Be("Value for property");
        }

        private class TestClass
        {
            public string SomeField;
            public string SomeProperty { get; set; }
            public string SomeMethod() => "Some return";
        }
    }
}
