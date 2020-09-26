using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.RootValidators;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SmoothValidation.Tests.Unit.RootValidators
{
    [TestFixture]
    public class RootValidatorBaseTests
    {
        [Test]
        public void For_Setup_When_ExpressionIsNull_Then_ExceptionIsThrown()
        {
            // Arrange:
            Expression<Func<RootValidatorsTestsCommon.TestClass, string>> expression = null;
            var validator = new RootValidatorBaseImplementation();

            // Act:
            Assert.Throws<ArgumentNullException>(() => validator.Setup(expression));
        }

        [Test]
        public void For_Setup_When_ThereIsNoPropertyValidatorForExpression_Then_NewPropertyValidatorIsCreated()
        {
            // Arrange:
            Expression<Func<RootValidatorsTestsCommon.TestClass, string>> expression = obj => obj.SomeProperty;
            var validator = new RootValidatorBaseImplementation();

            // Act:
            var result = validator.Setup(expression);

            // Assert:
            validator.PropertyValidatorsAsReadonly.Keys.Count().Should().Be(1);
            var propertyValidator = validator.PropertyValidatorsAsReadonly["SomeProperty"];
            propertyValidator.Should().Be(result);
        }

        [Test]
        public void For_Setup_When_PropertyValidatorForExpressionAlreadyExists_Then_ItIsReturned()
        {
            // Arrange:
            Expression<Func<RootValidatorsTestsCommon.TestClass, string>> expression = obj => obj.SomeProperty;
            var validator = new RootValidatorBaseImplementation();
            var propertyValidator = validator.Setup(expression);

            // Act:
            var result = validator.Setup(expression);

            // Assert:
            result.Should().Be(propertyValidator);
        }

        private class RootValidatorBaseImplementation : RootValidatorBase<RootValidatorsTestsCommon.TestClass>
        {
        }
    }
}
