using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SmoothValidation.ClassValidators;
using SmoothValidation.Types;
using System;
using System.Collections.Generic;

namespace SmoothValidation.DependencyInjection.Tests.Unit
{
    [TestFixture]
    public class CommonTests
    {
        [Test]
        public void For_GetValueValidatorExplicitly_When_ThereIsNoValueToValidateInActionContext_Then_ExceptionIsThrown()
        {
            // Arrange:
            var actionContextMock = TestsCommon.Utils.CreateActionExecutingContextMock();
            actionContextMock.Setup(x => x.ActionArguments.Values)
                .Returns(new List<object>());

            var actionContext = actionContextMock.Object;
            var notClosedValidatorType = typeof(ClassValidator<>);
            var typeToValidate = typeof(TypeToValidate);

            // Act & Assert:
            var exception = Assert.Throws<InvalidOperationException>(() => Common.GetValueValidatorExplicitly(
                actionContext, notClosedValidatorType, typeToValidate));
            exception.Message.Should().Contain($"There is no argument with type '{nameof(TypeToValidate)}'");
        }

        [Test]
        public void For_GetValueValidatorExplicitly_When_ValidatorServiceIsNotRegistered_Then_ExceptionIsThrown()
        {
            // Arrange:
            var actionContextMock = TestsCommon.Utils.CreateActionExecutingContextMock();
            var actionArguments = new List<object> { new TypeToValidate { SomeProperty = "SOME_VALUE" } };
            actionContextMock.Setup(x => x.ActionArguments.Values)
                .Returns(actionArguments);
            var actionContext = actionContextMock.Object;

            var serviceProvider = Mock.Of<IServiceProvider>(x => x.GetService(It.IsAny<Type>()) == null);
            var httpContext = Mock.Of<HttpContext>(x => x.RequestServices == serviceProvider);
            actionContext.HttpContext = httpContext;

            var notClosedValidatorType = typeof(ClassValidator<>);
            var typeToValidate = typeof(TypeToValidate);

            // Act & Assert:
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Common.GetValueValidatorExplicitly(actionContext, notClosedValidatorType, typeToValidate));
            exception.Message.Should().Contain("No service for type");
        }

        [Test]
        public void For_GetValueValidatorExplicitly_When_ValidatorIsRegistered_Then_ValueValidatorPairIsReturned()
        {
            // Arrange:
            var actionContextMock = TestsCommon.Utils.CreateActionExecutingContextMock();
            var actionArguments = new List<object> { new TypeToValidate { SomeProperty = "SOME_VALUE" } };
            actionContextMock.Setup(x => x.ActionArguments.Values)
                .Returns(actionArguments);
            var actionContext = actionContextMock.Object;

            var validator = new TypeToValidateValidator();
            var serviceProvider = Mock.Of<IServiceProvider>(x => x.GetService(It.IsAny<Type>()) == validator);
            var httpContext = Mock.Of<HttpContext>(x => x.RequestServices == serviceProvider);
            actionContext.HttpContext = httpContext;

            var notClosedValidatorType = typeof(ClassValidator<>);
            var typeToValidate = typeof(TypeToValidate);

            // Act:
            var result = Common.GetValueValidatorExplicitly(actionContext, notClosedValidatorType, typeToValidate);

            // Assert:
            result.Value.Should().BeAssignableTo<TypeToValidate>();
            var value = (TypeToValidate)result.Value;
            value.SomeProperty.Should().Be("SOME_VALUE");
            result.Validator.Should().BeAssignableTo<ClassValidator<TypeToValidate>>();
            result.Validator.Should().BeOfType<TypeToValidateValidator>();
        }

        [Test]
        public void For_GetValueValidatorImplicitly_When_NoActionArgumentHasCorrespondingValidator_Then_ExceptionIsThrown()
        {
            // Arrange:
            var actionArguments = new List<object> { new TypeToValidate { SomeProperty = "SOME_VALUE" } };

            var actionContextMock = TestsCommon.Utils.CreateActionExecutingContextMock();
            actionContextMock.Setup(x => x.ActionArguments.Values).Returns(actionArguments);
            var actionContext = actionContextMock.Object;

            var serviceProvider = Mock.Of<IServiceProvider>(x => x.GetService(It.IsAny<Type>()) == null);
            var httpContext = Mock.Of<HttpContext>(x => x.RequestServices == serviceProvider);
            actionContext.HttpContext = httpContext;

            var notClosedValidatorType = typeof(ClassValidator<>);

            // Act & Assert:
            var exception = Assert.Throws<InvalidOperationException>(() =>
                Common.GetValueValidatorImplicitly(actionContext, notClosedValidatorType));
            exception.Message.Should().Be("There is no argument to validate in action context");
        }

        [Test]
        public void For_GetValueValidatorImplicitly_When_AnyActionArgumentHasCorrespondingValidator_Then_ValueValidatorPairForFirstSuchArgumentIsReturned()
        {
            // Arrange:
            var actionContextMock = TestsCommon.Utils.CreateActionExecutingContextMock();
            var actionArguments = new List<object>
            {
                "SOME_STR",
                123,
                new TypeToValidate { SomeProperty = "SOME_VALUE" }
            };
            actionContextMock.Setup(x => x.ActionArguments.Values).Returns(actionArguments);
            var actionContext = actionContextMock.Object;

            var validator = new TypeToValidateValidator();
            var serviceProvider = Mock.Of<IServiceProvider>(x =>
                x.GetService(typeof(ClassValidator<TypeToValidate>)) == validator);
            var httpContext = Mock.Of<HttpContext>(x => x.RequestServices == serviceProvider);
            actionContext.HttpContext = httpContext;

            var notClosedValidatorType = typeof(ClassValidator<>);

            // Act:
            var result = Common.GetValueValidatorImplicitly(actionContext, notClosedValidatorType);

            // Assert:
            result.Value.Should().BeAssignableTo<TypeToValidate>();
            var value = (TypeToValidate)result.Value;
            value.SomeProperty.Should().Be("SOME_VALUE");
            result.Validator.Should().BeAssignableTo<ClassValidator<TypeToValidate>>();
            result.Validator.Should().BeOfType<TypeToValidateValidator>();
        }

        [Test]
        public void For_GetValidationMethod_When_ObjectContainsNoValidValidateMethod_Then_NullIsReturned()
        {
            // Arrange:
            var testObj = new ValidTestValidatorClass();

            // Act: 
            var result = Common.GetValidateSyncMethod(testObj);

            // Assert: 
            result.Should().NotBeNull();
            result.ReturnType.Should().Be(typeof(IList<ValidationError>));
        }

        [Test]
        public void For_GetValidationMethod_When_ObjectContainsValidValidateMethod_Then_ItIsReturned()
        {
            // Arrange:
            var testObj = new InvalidTestValidatorClass();

            // Act: 
            var result = Common.GetValidateSyncMethod(testObj);

            // Assert: 
            result.Should().BeNull();
        }

        private class TypeToValidate
        {
            public string SomeProperty { get; set; }
        }

        private class TypeToValidateValidator : ClassValidator<TypeToValidate>
        {
            protected override void SetupRules() { }
        }

        private class ValidTestValidatorClass
        {
            public IList<ValidationError> Validate(object obj)
            {
                return new List<ValidationError>();
            }
        }

        private class InvalidTestValidatorClass
        {
            public void Validate(object obj) { }

            public IList<ValidationError> ValidateButWithOtherName(object obj)
            {
                return new List<ValidationError>();
            }
        }
    }
}
