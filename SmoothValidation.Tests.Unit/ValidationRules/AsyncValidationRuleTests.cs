using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.ValidationRules;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmoothValidation.Tests.Unit.ValidationRules
{
    [TestFixture]
    public class AsyncValidationRuleTests
    {
        [Test]
        public void When_ValidationPredicateIsNull_Then_ExceptionIsThrown()
        {
            // Act & Assert:
            Assert.Throws<ArgumentNullException>(() => new AsyncValidationRule<string>(null, "some"));
        }

        [Test]
        public async Task When_ObjectToValidateMatchPredicateRequirements_Then_NoValidationErrorsAreReturned()
        {
            // Arrange:
            var errorMessage = "cannot be null";
            var errorCode = "NOT_NULL";
            Func<string, Task<bool>> isNotNullPredicate = async s =>
            {
                await Task.Delay(1);
                return s != null;
            };

            var toValidate = "some value";
            var toValidateAsObj = (object)toValidate;

            // Act:
            var validationRule = new AsyncValidationRule<string>(isNotNullPredicate, errorMessage, errorCode);
            var result = await validationRule.Validate(toValidate);
            var resultForObj = await validationRule.Validate(toValidateAsObj);

            // Assert:
            result.Count.Should().Be(0);
            resultForObj.Count.Should().Be(0);
        }

        [Test]
        public async Task When_ObjectToValidateDoesNotMatchPredicateRequirements_Then_ValidationErrorIsReturned()
        {
            // Arrange:
            var errorMessage = "cannot be null";
            var errorCode = "NOT_NULL";
            Func<string, Task<bool>> isNotNullPredicate = async s =>
            {
                await Task.Delay(1);
                return s != null;
            };

            string toValidate = null;
            var toValidateAsObj = (object)toValidate;

            // Act:
            var validationRule = new AsyncValidationRule<string>(isNotNullPredicate, errorMessage, errorCode);
            var result = await validationRule.Validate(toValidate);
            var resultForObj = await validationRule.Validate(toValidateAsObj);

            // Assert:
            result.Count.Should().Be(1);
            result.First().ErrorMessage.Should().Be("cannot be null");
            result.First().ErrorCode.Should().Be("NOT_NULL");
            result.First().ProvidedValue.Should().Be(toValidate);
            result.First().PropertyPath.IsEmpty.Should().BeTrue();

            resultForObj.Count.Should().Be(1);
            resultForObj.First().ErrorMessage.Should().Be("cannot be null");
            resultForObj.First().ErrorCode.Should().Be("NOT_NULL");
            resultForObj.First().PropertyPath.IsEmpty.Should().BeTrue();
            resultForObj.First().ProvidedValue.Should().Be(toValidate);
        }
    }
}
