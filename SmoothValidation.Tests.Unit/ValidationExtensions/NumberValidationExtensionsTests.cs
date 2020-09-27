using FluentAssertions;
using NUnit.Framework;
using SmoothValidation.InlineValidators;
using SmoothValidation.ValidationExtensions;

namespace SmoothValidation.Tests.Unit.ValidationExtensions
{
    [TestFixture]
    public class NumberValidationExtensionsTests
    {
        [Test]
        public void For_IsGreaterThan_WhenValueIsLessThanOrEqualToMinValue_Then_ValidationErrorIsReturned()
        {
            // Arrange:
            var numberValidator = new InlineValidator<NumberClass>();
            numberValidator.Setup(x => x.IntValue)
                .IsGreaterThan(0);
            numberValidator.Setup(x => x.FloatValue)
                .IsGreaterThan(0);
            numberValidator.Setup(x => x.DoubleValue)
                .IsGreaterThan(0);
            numberValidator.Setup(x => x.DecimalValue)
                .IsGreaterThan(0);

            var toValidate = new NumberClass
            {
                IntValue = 0,
                FloatValue = 0,
                DoubleValue = 0,
                DecimalValue = 0
            };

            // Act:
            var validationErrors = numberValidator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(4);
            foreach (var validationError in validationErrors)
            {
                validationError.ErrorMessage.Should().Be("Value must be greater than 0");
                validationError.ErrorCode.Should().Be("NUM_LESS_OR_THAN_OR_EQUAL_TO");
                validationError.IsTransient.Should().BeFalse();
                validationError.ProvidedValue.Should().Be(-0);
            }
        }

        [Test]
        public void For_IsGreaterThan_WhenValueGreaterThanMinValue_Then_NoValidationErrorIsReturned()
        {
            // Arrange:
            var numberValidator = new InlineValidator<NumberClass>();
            numberValidator.Setup(x => x.IntValue)
                .IsGreaterThan(0);
            numberValidator.Setup(x => x.FloatValue)
                .IsGreaterThan(0);
            numberValidator.Setup(x => x.DoubleValue)
                .IsGreaterThan(0);
            numberValidator.Setup(x => x.DecimalValue)
                .IsGreaterThan(0);

            var toValidate = new NumberClass
            {
                IntValue = 1,
                FloatValue = 1,
                DoubleValue = 1,
                DecimalValue = 1m
            };

            // Act:
            var validationErrors = numberValidator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(0);
        }

        [Test]
        public void For_IsGreaterThanOrEqualTo_WhenValueIsLessThanMinValue_Then_ValidationErrorIsReturned()
        {
            // Arrange:
            var numberValidator = new InlineValidator<NumberClass>();
            numberValidator.Setup(x => x.IntValue)
                .IsGreaterThanOrEqualTo(0);
            numberValidator.Setup(x => x.FloatValue)
                .IsGreaterThanOrEqualTo(0);
            numberValidator.Setup(x => x.DoubleValue)
                .IsGreaterThanOrEqualTo(0);
            numberValidator.Setup(x => x.DecimalValue)
                .IsGreaterThanOrEqualTo(0);

            var toValidate = new NumberClass
            {
                IntValue = -1,
                FloatValue = -1,
                DoubleValue = -1,
                DecimalValue = -1m
            };

            // Act:
            var validationErrors = numberValidator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(4);
            foreach (var validationError in validationErrors)
            {
                validationError.ErrorMessage.Should().Be("Value must be greater than or equal to 0");
                validationError.ErrorCode.Should().Be("NUM_LESS_THAN");
                validationError.IsTransient.Should().BeFalse();
                validationError.ProvidedValue.Should().Be(-1);
            }
        }

        [Test]
        public void For_IsGreaterThanOrEqualTo_WhenValueIsGreaterThanOrEqualToMinValue_Then_NoValidationErrorIsReturned()
        {
            // Arrange:
            var numberValidator = new InlineValidator<NumberClass>();
            numberValidator.Setup(x => x.IntValue)
                .IsGreaterThanOrEqualTo(0);
            numberValidator.Setup(x => x.FloatValue)
                .IsGreaterThanOrEqualTo(0);
            numberValidator.Setup(x => x.DoubleValue)
                .IsGreaterThanOrEqualTo(0);
            numberValidator.Setup(x => x.DecimalValue)
                .IsGreaterThanOrEqualTo(0);

            var toValidate = new NumberClass
            {
                IntValue = 0,
                FloatValue = 0,
                DoubleValue = 0,
                DecimalValue = 0
            };

            // Act:
            var validationErrors = numberValidator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(0);
        }

        [Test]
        public void For_IsLessThan_WhenValueIsGreaterThanOrEqualToMinValue_Then_ValidationErrorIsReturned()
        {
            // Arrange:
            var numberValidator = new InlineValidator<NumberClass>();
            numberValidator.Setup(x => x.IntValue)
                .IsLessThan(0);
            numberValidator.Setup(x => x.FloatValue)
                .IsLessThan(0);
            numberValidator.Setup(x => x.DoubleValue)
                .IsLessThan(0);
            numberValidator.Setup(x => x.DecimalValue)
                .IsLessThan(0);

            var toValidate = new NumberClass
            {
                IntValue = 0,
                FloatValue = 0,
                DoubleValue = 0,
                DecimalValue = 0
            };

            // Act:
            var validationErrors = numberValidator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(4);
            foreach (var validationError in validationErrors)
            {
                validationError.ErrorMessage.Should().Be("Value must be less than 0");
                validationError.ErrorCode.Should().Be("NUM_GREATER_OR_THAN_OR_EQUAL_TO");
                validationError.IsTransient.Should().BeFalse();
                validationError.ProvidedValue.Should().Be(0);
            }
        }

        [Test]
        public void For_IsLessThan_WhenValueIsLessThanMinValue_Then_NoValidationErrorIsReturned()
        {
            // Arrange:
            var numberValidator = new InlineValidator<NumberClass>();
            numberValidator.Setup(x => x.IntValue)
                .IsLessThan(0);
            numberValidator.Setup(x => x.FloatValue)
                .IsLessThan(0);
            numberValidator.Setup(x => x.DoubleValue)
                .IsLessThan(0);
            numberValidator.Setup(x => x.DecimalValue)
                .IsLessThan(0);

            var toValidate = new NumberClass
            {
                IntValue = -1,
                FloatValue = -1,
                DoubleValue = -1,
                DecimalValue = -1
            };

            // Act:
            var validationErrors = numberValidator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(0);
        }

        [Test]
        public void For_IsLessThanOrEqualTo_WhenValueGreaterThanMinValue_Then_ValidationErrorIsReturned()
        {
            // Arrange:
            var numberValidator = new InlineValidator<NumberClass>();
            numberValidator.Setup(x => x.IntValue)
                .IsLessThanOrEqualTo(0);
            numberValidator.Setup(x => x.FloatValue)
                .IsLessThanOrEqualTo(0);
            numberValidator.Setup(x => x.DoubleValue)
                .IsLessThanOrEqualTo(0);
            numberValidator.Setup(x => x.DecimalValue)
                .IsLessThanOrEqualTo(0);

            var toValidate = new NumberClass
            {
                IntValue = 1,
                FloatValue = 1,
                DoubleValue = 1,
                DecimalValue = 1
            };

            // Act:
            var validationErrors = numberValidator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(4);
            foreach (var validationError in validationErrors)
            {
                validationError.ErrorMessage.Should().Be("Value must be less than or equal to 0");
                validationError.ErrorCode.Should().Be("NUM_GREATER_THAN");
                validationError.IsTransient.Should().BeFalse();
                validationError.ProvidedValue.Should().Be(1);
            }
        }

        [Test]
        public void For_IsLessThanOrEqualTo_WhenValueLessThanOrEqualToMinValue_Then_NoValidationErrorIsReturned()
        {
            // Arrange:
            var numberValidator = new InlineValidator<NumberClass>();
            numberValidator.Setup(x => x.IntValue)
                .IsLessThanOrEqualTo(0);
            numberValidator.Setup(x => x.FloatValue)
                .IsLessThanOrEqualTo(0);
            numberValidator.Setup(x => x.DoubleValue)
                .IsLessThanOrEqualTo(0);
            numberValidator.Setup(x => x.DecimalValue)
                .IsLessThanOrEqualTo(0);

            var toValidate = new NumberClass
            {
                IntValue = 0,
                FloatValue = 0,
                DoubleValue = 0,
                DecimalValue = 0
            };

            // Act:
            var validationErrors = numberValidator.Validate(toValidate);

            // Assert:
            validationErrors.Count.Should().Be(0);
        }

        private class NumberClass
        {
            public int IntValue { get; set; }
            public float FloatValue { get; set; }
            public double DoubleValue { get; set; }
            public decimal DecimalValue { get; set; }
        }

    }
}
