using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.PropertyValidators;
using SmoothValidation.RootValidators;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmoothValidation.Tests.Unit.RootValidators
{
    [TestFixture]
    public class RootAsyncValidatorTests
    {
        [Test]
        public void For_SetupAsync_When_ProvidedExpressionIsNull_Then_ExceptionIsThrown()
        {
            // Arrange:
            var rootAsyncValidator = new RootAsyncValidatorImplementation();
            Expression<Func<RootValidatorsTestsCommon.TestClass, string>> expression = null;

            // Act & Assert:
            Assert.Throws<ArgumentNullException>(() => rootAsyncValidator.SetupAsync(expression));
        }

        [Test]
        public void For_SetupAsync_When_ThereIsNoPropertyValidatorForExpression_Then_NewPropertyValidatorIsCreated()
        {
            // Arrange:
            var rootAsyncValidator = new RootAsyncValidatorImplementation();
            Expression<Func<RootValidatorsTestsCommon.TestClass, string>> expression = obj => obj.SomeProperty;

            // Act:
            var result = rootAsyncValidator.SetupAsync(expression);

            // Assert:
            result.Should().BeAssignableTo<AsyncPropertyValidator<string>>();
        }

        [Test]
        public void For_SetupAsync_When_AsyncPropertyValidatorForExpressionAlreadyExists_Then_ItIsReturned()
        {
            // Arrange:
            var rootAsyncValidator = new RootAsyncValidatorImplementation();
            Expression<Func<RootValidatorsTestsCommon.TestClass, string>> expression = obj => obj.SomeProperty;
            var propertyValidator = rootAsyncValidator.SetupAsync(expression);

            // Act: 
            var result = rootAsyncValidator.SetupAsync(expression);

            // Assert:
            result.Should().Be(propertyValidator);
        }

        [Test]
        public void For_SetupAsync_When_SyncPropertyValidatorForExpressionAlreadyExists_Then_ExceptionIsThrown()
        {
            // Arrange:
            var rootAsyncValidator = new RootAsyncValidatorImplementation();
            Expression<Func<RootValidatorsTestsCommon.TestClass, string>> expression = obj => obj.SomeProperty;
            var propertyValidator = rootAsyncValidator.Setup(expression);

            // Act & Assert: 
            var exception = Assert.Throws<InvalidOperationException>(() => rootAsyncValidator.SetupAsync(expression));
            exception.Message.Should().Be("Property already has assigned synchronous validator");
        }

        [Test]
        public async Task For_ValidateObj_When_ValidObjectIsPassed_Then_ValidationPropertyErrorsAreReturned()
        {
            // Arrange:
            var rootAsyncValidator = new RootAsyncValidatorImplementation();
            var toValidate = (object)new RootValidatorsTestsCommon.TestClass();

            // Act:
            var result = await rootAsyncValidator.Validate(toValidate);

            // Assert:
            result.Count.Should().Be(0);
        }

        [Test]
        public async Task For_Validate_When_RootValidatorHasSetUpSyncPropertyValidatorAndValidObjectIsPassed_Then_ValidationPropertyErrorsAreReturned()
        {
            // Arrange:
            var rootAsyncValidator = new RootAsyncValidatorImplementation();
            Expression<Func<RootValidatorsTestsCommon.TestClass, string>> expression = obj => obj.SomeProperty;

            rootAsyncValidator.Setup(x => x.SomeProperty)
                .AddRule(x => x != null, "cannot be null", "NOT_NULL");
            var toValidate = (object)new RootValidatorsTestsCommon.TestClass
            {
                SomeProperty = null
            };

            // Act:
            var validationErrors = await rootAsyncValidator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("cannot be null");
            validationErrors[0].ErrorCode.Should().Be("NOT_NULL");
            validationErrors[0].IsTransient.Should().Be(false);
            validationErrors[0].ProvidedValue.Should().Be(null);
        }

        [Test]
        public async Task For_Validate_When_RootValidatorHasSetUpAsyncPropertyValidatorAndValidObjectIsPassed_Then_ValidationPropertyErrorsAreReturned()
        {
            // Arrange:
            var rootAsyncValidator = new RootAsyncValidatorImplementation();
            Expression<Func<RootValidatorsTestsCommon.TestClass, string>> expression = obj => obj.SomeProperty;

            rootAsyncValidator.SetupAsync(x => x.SomeProperty)
                .AddRule(async x =>
                {
                    await Task.CompletedTask;
                    return x != null;
                }, "cannot be null", "NOT_NULL");
            var toValidate = (object)new RootValidatorsTestsCommon.TestClass
            {
                SomeProperty = null
            };

            // Act:
            var validationErrors = await rootAsyncValidator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(1);
            validationErrors[0].ErrorMessage.Should().Be("cannot be null");
            validationErrors[0].ErrorCode.Should().Be("NOT_NULL");
            validationErrors[0].IsTransient.Should().Be(false);
            validationErrors[0].ProvidedValue.Should().Be(null);
        }

        [Test]
        public async Task
            For_Validate_When_RootValidatorHasSetUpBothSyncAndAsyncPropertyValidatorAndValidObjectIsPassed_Then_ValidationPropertyErrorsAreReturned()
        {
            // Arrange:
            var rootAsyncValidator = new RootAsyncValidatorImplementation();
            Expression<Func<RootValidatorsTestsCommon.TestClass, string>> expression = obj => obj.SomeProperty;

            rootAsyncValidator.SetupAsync(x => x.SomeProperty)
                .AddRule(async x =>
                {
                    await Task.CompletedTask;
                    return x != null;
                }, "cannot be null", "NOT_NULL");
            rootAsyncValidator.Setup(x => x.OtherProperty)
                .AddRule(x => x == "x", "must be x", "MUST_BE_X");
            var toValidate = (object) new RootValidatorsTestsCommon.TestClass
            {
                SomeProperty = null,
                OtherProperty = "y"
            };

            // Act:
            var validationErrors = await rootAsyncValidator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(2);

            validationErrors[0].ErrorMessage.Should().Be("cannot be null");
            validationErrors[0].ErrorCode.Should().Be("NOT_NULL");
            validationErrors[0].IsTransient.Should().Be(false);
            validationErrors[0].ProvidedValue.Should().Be(null);

            validationErrors[1].ErrorMessage.Should().Be("must be x");
            validationErrors[1].ErrorCode.Should().Be("MUST_BE_X");
            validationErrors[1].IsTransient.Should().Be(false);
            validationErrors[1].ProvidedValue.Should().Be("y");
        }

        private class RootAsyncValidatorImplementation : RootAsyncValidator<RootValidatorsTestsCommon.TestClass>
        {
        }
    }
}
