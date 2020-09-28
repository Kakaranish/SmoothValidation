using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.PropertyValidators;
using System;

namespace SmoothValidation.Tests.Unit.PropertyValidators
{
    [TestFixture]
    public class SyncPropertyValidatorTests
    {
        [Test]
        public void For_Ctor_When_ProvidedMemberInfoValueIsNull_Then_ExceptionIsThrown()
        {
            // Act & Assert:
            Assert.Throws<ArgumentNullException>(() => new SyncPropertyValidator<string>(null));
        }

        [Test]
        public void For_Validate_When_ProvidedObjectCannotBeCastedToTPropType_Then_ExceptionIsThrown()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new SyncPropertyValidator<string>(memberInfo);
            validator.AddRule(x => x != "x", "cannot be x", "NOT_X");
            var toValidate = 10;

            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() => validator.Validate(toValidate));
            exception.Message.Should().Be("'obj' is not String type");
        }

        [Test]
        public void For_ValidateObj_When_ProvidedObjectToValidateIsObjectType_Then_ItIsCastedAndPassedToFurtherValidation()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new SyncPropertyValidator<string>(memberInfo);
            validator.AddRule(x => x != "x", "cannot be x", "NOT_X");
            var toValidate = (object) "x";

            // Act & Assert:
            Assert.DoesNotThrow(() => validator.Validate(toValidate));
        }

        [Test]
        public void For_Validate_When_ObjectToValidateIsNotValidFor2Rules_Then_ValidationErrorsAreReturned()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new SyncPropertyValidator<string>(memberInfo);
            validator.AddRule(x => x != "x", "cannot be x", "NOT_X");
            validator.AddRule(x => x == null, "must be null", "CANNOT_BE_NULL");
            var toValidate = "x";
            
            // Act:
            var result = validator.Validate(toValidate);

            // Assert:
            result.Count.Should().Be(2);
            result[0].PropertyPath.IsEmpty.Should().Be(false);
            result[0].ProvidedValue.Should().Be("x");
            result[0].PropertyPath.ToString().Should().Be("SomeProperty");
            result[0].ErrorMessage.Should().Be("cannot be x");
            result[0].ErrorCode.Should().Be("NOT_X");

            result[1].PropertyPath.IsEmpty.Should().Be(false);
            result[1].ProvidedValue.Should().Be("x");
            result[1].PropertyPath.ToString().Should().Be("SomeProperty");
            result[1].ErrorMessage.Should().Be("must be null");
            result[1].ErrorCode.Should().Be("CANNOT_BE_NULL");
        }

        [Test]
        public void For_Validate_WhenRuleHasSetToStopValidationAfterFailure_Then_ValidationStopsWhenErrorOccurs()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new SyncPropertyValidator<string>(memberInfo);
            validator.AddRule(x => x != "x", "cannot be x", "NOT_X");
            validator.StopValidationAfterFailure();
            validator.AddRule(x => x == null, "must be null", "CANNOT_BE_NULL");
            var toValidate = "y";

            // Act:
            var result = validator.Validate(toValidate);

            // Assert:
            result[0].PropertyPath.IsEmpty.Should().Be(false);
            result[0].ProvidedValue.Should().Be("y");
            result[0].PropertyPath.ToString().Should().Be("SomeProperty");
            result[0].ErrorMessage.Should().Be("must be null");
            result[0].ErrorCode.Should().Be("CANNOT_BE_NULL");
        }

        [Test]
        public void For_Validate_When_RuleHasSetToStopValidationAfterFailureAndErrorDoesNotOccur_Then_ValidationDoesNotStop()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new SyncPropertyValidator<string>(memberInfo);
            validator.AddRule(x => x != "x", "cannot be x", "NOT_X");
            validator.StopValidationAfterFailure();
            validator.AddRule(x => x == null, "must be null", "CANNOT_BE_NULL");
            var toValidate = "x";

            // Act:
            var result = validator.Validate(toValidate);

            // Assert:
            result.Count.Should().Be(1);
            result[0].PropertyPath.IsEmpty.Should().Be(false);
            result[0].ProvidedValue.Should().Be("x");
            result[0].PropertyPath.ToString().Should().Be("SomeProperty");
            result[0].ErrorMessage.Should().Be("cannot be x");
            result[0].ErrorCode.Should().Be("NOT_X");
        }

        [Test]
        public void For_Validate_When_PropertyHasSetMessageAndCodeOverrides_Then_AreAppliedToValidationError()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new SyncPropertyValidator<string>(memberInfo);
            validator.AddRule(x => x != "x", "cannot be x", "NOT_X");
            validator.WithMessage("overridden message");
            validator.WithCode("overridden code");
            var toValidate = "x";

            // Act:
            var result = validator.Validate(toValidate);

            // Assert:
            result.Count.Should().Be(1);
            result[0].PropertyPath.IsEmpty.Should().Be(false);
            result[0].ProvidedValue.Should().Be("x");
            result[0].PropertyPath.ToString().Should().Be("SomeProperty");
            result[0].ErrorMessage.Should().Be("overridden message");
            result[0].ErrorCode.Should().Be("overridden code");
        }
    }
}
