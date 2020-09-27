using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.PropertyValidators;
using SmoothValidation.RootValidators;
using System.Linq;

namespace SmoothValidation.Tests.Unit.RootValidators
{
    [TestFixture]
    public class RootSyncValidatorTests
    {
        [Test]
        public void For_Setup_SyncPropertyValidatorIsReturned()
        {
            // Arrange:
            var rootSyncValidator = new RootSyncValidatorImplementation();

            // Act:
            var result = rootSyncValidator.Setup(x => x.SomeProperty);

            // Assert:
            result.Should().BeAssignableTo<SyncPropertyValidator<string>>();
        }

        [Test]
        public void For_ValidateObj_When_ValidObjectIsPassed_Then_ValidationErrorsAreReturned()
        {
            // Arrange:
            var rootSyncValidator = new RootSyncValidatorImplementation();
            var toValidate = (object)new RootValidatorsTestsCommon.TestClass();

            // Act:
            var result = rootSyncValidator.Validate(toValidate);

            // Assert:
            result.Count.Should().Be(0);
        }

        [Test]
        public void For_Validate_When_ValidObjectIsPassed_Then_ValidationErrorsAreReturned()
        {
            // Arrange:
            var rootSyncValidator = new RootSyncValidatorImplementation();
            rootSyncValidator.Setup(x => x.SomeProperty)
                .AddRule(x => x != null, "cannot be null", "NOT_NULL");
            rootSyncValidator.Setup(x => x.IntField)
                .AddRule(x => x > 0, "must be greater than 0", "GREATER_THAN_0");

            var toValidate = new RootValidatorsTestsCommon.TestClass
            {
                SomeProperty = null,
                IntField = -1
            };

            // Act:
            var validationErrors = rootSyncValidator.Validate(toValidate);

            // Assert:
            rootSyncValidator.PropertyValidatorsAsReadonly.Keys.Count().Should().Be(2);
            validationErrors.Count.Should().Be(2);

            validationErrors[0].ErrorMessage.Should().Be("cannot be null");
            validationErrors[0].ErrorCode.Should().Be("NOT_NULL");
            validationErrors[0].ProvidedValue.Should().Be(null);
            validationErrors[0].PropertyName.Should().Be("SomeProperty");

            validationErrors[1].ErrorMessage.Should().Be("must be greater than 0");
            validationErrors[1].ErrorCode.Should().Be("GREATER_THAN_0");
            validationErrors[1].ProvidedValue.Should().Be(-1);
            validationErrors[1].PropertyName.Should().Be("IntField");
        }

        private class RootSyncValidatorImplementation : RootSyncValidator<RootValidatorsTestsCommon.TestClass>
        {
        }
    }
}
