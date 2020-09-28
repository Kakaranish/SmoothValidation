using FluentAssertions;
using Moq;
using NUnit.Framework;
using SmoothValidation.PropertyValidators;
using SmoothValidation.ValidatorsAbstraction;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmoothValidation.Tests.Unit.PropertyValidators
{
    [TestFixture]
    public class AsyncPropertyValidatorTests
    {
        [Test]
        public void For_Ctor_When_ProvidedMemberInfoValueIsNull_Then_ExceptionIsThrown()
        {
            // Act & Assert:
            Assert.Throws<ArgumentNullException>(() => new AsyncPropertyValidator<string>(null));
        }

        [Test]
        public void For_AddRuleAsync_When_PredicateIsNull_Then_ExceptionIsThrown()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new AsyncPropertyValidator<string>(memberInfo);
            Func<string, Task<bool>> validationPredicate = null;

            // Act & Assert:
            Assert.Throws<ArgumentNullException>(() =>
                validator.AddRule(validationPredicate, It.IsAny<string>()));
        }

        [Test]
        public void For_AddRuleAsync_When_MessageIsNull_Then_ExceptionIsThrown()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new AsyncPropertyValidator<string>(memberInfo);
            Func<string, Task<bool>> validationPredicate = async x =>
            {
                await Task.CompletedTask;
                return x != "x";
            };

            // Act & Assert:
            Assert.Throws<ArgumentNullException>(() =>
                validator.AddRule(validationPredicate, null));
        }

        [Test]
        public void For_AddRuleAsync_When_PassedArgumentsAreValid_Then_NewValidationTaskIsAdded()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new AsyncPropertyValidator<string>(memberInfo);
            Func<string, Task<bool>> validationPredicate = async x =>
            {
                await Task.CompletedTask;
                return x != "x";
            };
            const string message = "SOME MESSAGE";

            // Act & Assert:
            Assert.DoesNotThrow(() => validator.AddRule(validationPredicate, message));
            validator.ValidationTasksAsReadonly.Count.Should().Be(1);
            var validationTask = validator.ValidationTasksAsReadonly.First();
            validationTask.Validator.Should().Should().NotBeNull();
            validationTask.Validator.GetType().Should().BeAssignableTo<IAsyncValidator>();
        }

        [Test]
        public void For_SetValidatorAsync_When_ProvidedValidatorIsNull_Then_ExceptionIsThrown()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new AsyncPropertyValidator<string>(memberInfo);

            // Act & Assert:
            Assert.Throws<ArgumentNullException>(() => validator.SetValidator(null));
        }

        [Test]
        public void For_SetValidatorAsync_When_PassedValidatorReferenceConcernsTargetValidator_Then_ExceptionIsThrown()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new AsyncPropertyValidator<string>(memberInfo);

            // Act & Assert:
            var exception = Assert.Throws<ArgumentException>(() => validator.SetValidator(validator));
            exception.Message.Should().Be("Detected circular reference");
        }

        [Test]
        public void For_SetValidatorAsync_When_ThereIsNoValidationTaskCorrespondingToOtherValidator_Then_NewValidationTaskForValidatorIsAdded()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new AsyncPropertyValidator<string>(memberInfo);

            // Act:
            validator.SetValidator(Mock.Of<IAsyncValidator<string>>());

            // Assert:
            validator.ValidationTasksAsReadonly.Count.Should().Be(1);
            var validationTask = validator.ValidationTasksAsReadonly.First();
            validationTask.IsOtherValidator.Should().Be(true);
            validationTask.Validator.Should().BeAssignableTo<IAsyncValidator<string>>();
        }

        [Test]
        public void For_SetValidatorAsync_When_OtherValidatorIsAlreadySet_Then_ExceptionIsThrown()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new AsyncPropertyValidator<string>(memberInfo);
            validator.SetValidator(Mock.Of<IAsyncValidator<string>>()); // First .SetValidator() call

            // Act & Assert:
            var exception = Assert.Throws<InvalidOperationException>(() =>
                validator.SetValidator(Mock.Of<IAsyncValidator<string>>()));
            exception.Message.Should().Be("There is already set other validator");
        }


        [Test]
        public void For_ValidateObj_When_ProvidedObjectCannotBeCastedToTPropType_Then_ExceptionIsThrown()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new AsyncPropertyValidator<string>(memberInfo);
            var toValidate = 10;

            // Act & Assert:
            var exception = Assert.ThrowsAsync<ArgumentException>(() => validator.Validate(toValidate));
            exception.Message.Should().Be("'obj' is not String type");
        }

        [Test]
        public void For_ValidateObj_When_ProvidedObjectCanBeCastedToTPropType_Then_ValidationForTPropTypeIsCalled()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new AsyncPropertyValidator<string>(memberInfo);
            var toValidate = (object) "x";

            // Act & Assert:
            Assert.DoesNotThrowAsync(() => validator.Validate(toValidate));
        }

        [Test]
        public async Task For_Validate_When_ThereIsValidationTaskForSyncRule_Then_ValidationErrorIsReturned()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new AsyncPropertyValidator<string>(memberInfo);
            Predicate<string> validationPredicate = value => value != "x";
            const string message = "SOME MESSAGE";
            const string code = "SOME CODE";
            validator.AddRule(validationPredicate, message, code);

            var toValidate = "x";

            // Act:
            var validationErrors = await validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            var validationError = validationErrors.First();
            validationError.ErrorMessage.Should().Be(message);
            validationError.ErrorCode.Should().Be(code);
            validationError.PropertyPath.ToString().Should().Be("SomeProperty");
            validationError.ProvidedValue.Should().Be(toValidate);
        }

        [Test]
        public async Task For_Validate_When_ThereIsValidationRuleForAsyncRule_Then_ValidationErrorIsReturned()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new AsyncPropertyValidator<string>(memberInfo);
            Func<string, Task<bool>> validationPredicate = async x =>
            {
                await Task.CompletedTask;
                return x != "x";
            };
            const string message = "SOME MESSAGE";
            const string code = "SOME CODE";
            validator.AddRule(validationPredicate, message, code);
            
            var toValidate = "x";

            // Act:
            var validationErrors = await validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            var validationError = validationErrors.First();
            validationError.ErrorMessage.Should().Be(message);
            validationError.ErrorCode.Should().Be(code);
            validationError.PropertyPath.ToString().Should().Be("SomeProperty");
            validationError.ProvidedValue.Should().Be(toValidate);
        }

        [Test]
        public async Task For_Validate_When_ThereAreOverridesToApplyToValidationErrors_Then_AreAppliedToValidationErrors()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new AsyncPropertyValidator<string>(memberInfo);
            Func<string, Task<bool>> validationPredicate = async x =>
            {
                await Task.CompletedTask;
                return x != "x";
            };
            const string message = "SOME MESSAGE";
            const string code = "SOME CODE";
            validator.AddRule(validationPredicate, message, code);
            validator.WithMessage("Cannot be x");
            validator.WithCode("MUST_NOT_BE_X");
            validator.AddRule(x => x == null, "must be null", "MUST_BE_NULL");
            validator.SetPropertyDisplayName("OverridenPropertyDisplayName");

            var toValidate = "x";

            // Act:
            var validationErrors = await validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(2);
            
            validationErrors[0].ErrorMessage.Should().Be("Cannot be x");
            validationErrors[0].ErrorCode.Should().Be("MUST_NOT_BE_X");
            validationErrors[0].PropertyPath.ToString().Should().Be("OverridenPropertyDisplayName");
            validationErrors[0].ProvidedValue.Should().Be(toValidate);
            
            validationErrors[1].ErrorMessage.Should().Be("must be null");
            validationErrors[1].ErrorCode.Should().Be("MUST_BE_NULL");
            validationErrors[1].PropertyPath.ToString().Should().Be("OverridenPropertyDisplayName");
            validationErrors[1].ProvidedValue.Should().Be(toValidate);
        }

        [Test]
        public async Task For_Validate_WhenRuleHasSetToStopValidationAfterFailureButThereIsNoValidationFailure_Then_ValidationContinues()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new AsyncPropertyValidator<string>(memberInfo);
            Func<string, Task<bool>> validationPredicate = async x =>
            {
                await Task.CompletedTask;
                return x != "x";
            };
            const string message = "SOME MESSAGE";
            const string code = "SOME CODE";
            validator.AddRule(validationPredicate, message, code);
            validator.StopValidationAfterFailure();
            validator.AddRule(x => x == null, "must be null", "MUST_BE_NULL");

            var toValidate = "y";

            // Act:
            var validationErrors = await validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("must be null");
            validationErrors[0].ErrorCode.Should().Be("MUST_BE_NULL");
            validationErrors[0].PropertyPath.ToString().Should().Be("SomeProperty");
            validationErrors[0].ProvidedValue.Should().Be(toValidate);
        }

        [Test]
        public async Task For_Validate_WhenRuleHasSetToStopValidationAfterFailure_Then_ValidationStopsWhenErrorOccurs()
        {
            // Arrange:
            var memberInfo = PropertyValidatorTestsCommon.CreateTestMemberInfo();
            var validator = new AsyncPropertyValidator<string>(memberInfo);
            Func<string, Task<bool>> validationPredicate = async x =>
            {
                await Task.CompletedTask;
                return x != "x";
            };
            const string message = "SOME MESSAGE";
            const string code = "SOME CODE";
            validator.AddRule(validationPredicate, message, code);
            validator.StopValidationAfterFailure();
            validator.AddRule(x => x == null, "must be null", "MUST_BE_NULL");

            var toValidate = "x";

            // Act:
            var validationErrors = await validator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("SOME MESSAGE");
            validationErrors[0].ErrorCode.Should().Be("SOME CODE");
            validationErrors[0].PropertyPath.ToString().Should().Be("SomeProperty");
            validationErrors[0].ProvidedValue.Should().Be(toValidate);
        }
    }
}
