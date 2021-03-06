﻿using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.ValidationRules;
using System;
using System.Linq;

namespace SmoothValidation.Tests.Unit.ValidationRules
{
    [TestFixture]
    public class SyncValidationRuleTests
    {
        [Test]
        public void When_ValidationPredicateIsNull_Then_ExceptionIsThrown()
        {
            // Act & Assert:
            Assert.Throws<ArgumentNullException>(() => new SyncValidationRule<string>(null, "some"));
        }

        [Test]
        public void When_ObjectToValidateMatchPredicateRequirements_Then_EmptyListOfErrorsIsReturned()
        {
            // Arrange:
            var errorMessage = "cannot be null";
            var errorCode = "NOT_NULL";
            Predicate<string> isNotNullPredicate = s => s != null;

            var toValidate = "some value";
            var toValidateAsObj = (object)toValidate;

            // Act:
            var validationRule = new SyncValidationRule<string>(isNotNullPredicate, errorMessage, errorCode);
            var result = validationRule.Validate(toValidate);
            var resultForObj = validationRule.Validate(toValidateAsObj);

            // Assert:
            result.Count.Should().Be(0);
            resultForObj.Count.Should().Be(0);
        }

        [Test]
        public void When_ObjectToValidateDoesNotMatchPredicateRequirements_Then_ListWithValidationErrorIsReturned()
        {
            // Arrange:
            var errorMessage = "cannot be null";
            var errorCode = "NOT_NULL";
            Predicate<string> isNotNullPredicate = s => s != null;

            string toValidate = null;
            var toValidateAsObj = (object)toValidate;

            // Act:
            var validationRule = new SyncValidationRule<string>(isNotNullPredicate, errorMessage, errorCode);
            var result = validationRule.Validate(toValidate);
            var resultForObj = validationRule.Validate(toValidateAsObj);

            // Assert:
            result.Count.Should().Be(1);
            result.First().ErrorMessage.Should().Be("cannot be null");
            result.First().ErrorCode.Should().Be("NOT_NULL");
            result.First().PropertyPath.IsEmpty.Should().BeTrue();
            result.First().ProvidedValue.Should().Be(toValidate);

            resultForObj.Count.Should().Be(1);
            resultForObj.First().ErrorMessage.Should().Be("cannot be null");
            resultForObj.First().ErrorCode.Should().Be("NOT_NULL");
            resultForObj.First().PropertyPath.IsEmpty.Should().BeTrue();
            resultForObj.First().ProvidedValue.Should().Be(toValidate);
        }
    }
}
