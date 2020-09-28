using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.ClassValidators;
using SmoothValidation.ValidationExtensions;
using System;
using System.Threading.Tasks;

namespace SmoothValidation.Tests.Unit.GeneralTests
{
    [TestFixture]
    public class ClassValidatorsTests
    {
        [Test]
        public void For_SyncClassValidator_When_ProvidedNotNullObjToValidate_Then_ValidationErrorsAreReturned()
        {
            // Arrange:
            var syncValidator = new SyncClassValidator();
            var toValidate = new TestClass
            {
                StringProperty = "",
                IntField = 0
            };

            // Act:
            var validationErrors = syncValidator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(2);

            validationErrors[0].ErrorMessage.Should().Contain("Cannot have value less");
            validationErrors[0].ErrorCode.Should().Be("STR_LESS_THAN_MIN_LENGTH");
            validationErrors[0].PropertyPath.IsEmpty.Should().BeFalse();
            validationErrors[0].PropertyPath.ToString().Should().Be("StringProperty");
            validationErrors[0].ProvidedValue.Should().Be("");

            validationErrors[1].ErrorMessage.Should().Contain("Value must be greater than");
            validationErrors[1].ErrorCode.Should().Be("NUM_LESS_OR_THAN_OR_EQUAL_TO");
            validationErrors[1].PropertyPath.IsEmpty.Should().BeFalse();
            validationErrors[1].PropertyPath.ToString().Should().Be("IntField");
            validationErrors[1].ProvidedValue.Should().Be(0);
        }

        [Test]
        public void For_SyncClassValidator_When_ProvidedNullObjToValidate_Then_ExceptionIsThrown()
        {
            // Arrange:
            var syncValidator = new SyncClassValidator();

            // Act:
            Assert.Throws<ArgumentNullException>(() => syncValidator.Validate(null));
        }

        [Test]
        public async Task For_AsyncClassValidator_When_ProvidedNotNullObjToValidate_Then_ValidationErrorsAreReturned()
        {
            // Arrange:
            var asyncClassValidator = new AsyncClassValidator();
            var toValidate = new TestClass
            {
                StringProperty = "",
                IntField = 0
            };

            // Act:
            var validationErrors = await asyncClassValidator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(2);

            validationErrors[0].ErrorMessage.Should().Contain("Cannot have value less");
            validationErrors[0].ErrorCode.Should().Be("STR_LESS_THAN_MIN_LENGTH");
            validationErrors[0].PropertyPath.IsEmpty.Should().BeFalse();
            validationErrors[0].PropertyPath.ToString().Should().Be("StringProperty");
            validationErrors[0].ProvidedValue.Should().Be("");

            validationErrors[1].ErrorMessage.Should().Contain("Value must be greater than");
            validationErrors[1].ErrorCode.Should().Be("NUM_LESS_OR_THAN_OR_EQUAL_TO");
            validationErrors[1].PropertyPath.IsEmpty.Should().BeFalse();
            validationErrors[1].PropertyPath.ToString().Should().Be("IntField");
            validationErrors[1].ProvidedValue.Should().Be(0);
        }

        [Test]
        public void For_AsyncClassValidator_When_ProvidedNullObjToValidate_Then_ExceptionIsThrown()
        {
            // Arrange:
            var asyncValidator = new AsyncClassValidator();

            // Act:
            Assert.ThrowsAsync<ArgumentNullException>(() => asyncValidator.Validate(null));
        }

        private class TestClass
        {
            public string StringProperty { get; set; }
            public int IntField;
        }

        private class SyncClassValidator : ClassValidator<TestClass>
        {
            protected override void SetupRules()
            {
                Setup(x => x.StringProperty)
                    .AddRule(x => x != null, "cannot be null", "CANNOT_BE_NULL")
                    .StopValidationAfterFailure()
                    .HasMinLength(1)
                    .StopValidationAfterFailure()
                    .AddRule(x => char.IsUpper(x[0]), "must starts with upper char", "MUST_START_WITH_UPPER_CHAR");
                Setup(x => x.IntField)
                    .IsGreaterThan(0);
            }
        }
        private class AsyncClassValidator : ClassValidatorAsync<TestClass>
        {
            protected override void SetupRules()
            {
                SetupAsync(x => x.StringProperty)
                    .AddRule(async x =>
                    {
                        await Task.CompletedTask;
                        return x != null;
                    }, "cannot be null", "CANNOT_BE_NULL")
                    .StopValidationAfterFailure()
                    .HasMinLength(1)
                    .StopValidationAfterFailure()
                    .AddRule(x => char.IsUpper(x[0]), "must starts with upper char", "MUST_START_WITH_UPPER_CHAR");

                Setup(x => x.IntField)
                    .IsGreaterThan(0);
            }
        }
    }
}
