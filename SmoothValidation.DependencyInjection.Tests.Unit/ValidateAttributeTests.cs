﻿using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using NUnit.Framework;
using SmoothValidation.Types.ValidationResult;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmoothValidation.DependencyInjection.Tests.Unit.TestsCommon;

namespace SmoothValidation.DependencyInjection.Tests.Unit
{
    [TestFixture]
    public class ValidateAttributeTests
    {
        [Test]
        public void For_OnActionExecutionAsync_When_AsyncValidatorProvidedAndThereIsNoValidateAction_Then_ExceptionIsThrown()
        {
            // Arrange:
            var actionArguments = new List<object> { new Person() };

            var actionContextMock = TestsCommon.Utils.CreateActionExecutingContextMock();
            actionContextMock.Setup(x => x.ActionArguments.Values).Returns(actionArguments);

            IActionResult result = null;
            actionContextMock.SetupSet(context => context.Result = It.IsAny<IActionResult>())
                .Callback<IActionResult>(value => result = value);
            var actionContext = actionContextMock.Object;

            var validator = new TestPersonValidatorAsync(); // Async validator should return null Validate() method
            var serviceProvider = Mock.Of<IServiceProvider>(x => x.GetService(It.IsAny<Type>()) == validator);
            var httpContext = Mock.Of<HttpContext>(x => x.RequestServices == serviceProvider);
            actionContext.HttpContext = httpContext;

            Task<ActionExecutedContext> Next()
            {
                var ctx = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());
                return Task.FromResult(ctx);
            }

            // Act & Assert:
            var validateAttribute = new ValidateAttribute { TypeToValidate = typeof(Person) };
            var exception = Assert.ThrowsAsync<InvalidOperationException>(() =>
                validateAttribute.OnActionExecutionAsync(actionContextMock.Object, Next));
            exception.Message.Should().Contain("No sync validate method for");
        }

        [Test]
        public async Task For_OnActionExecutionAsync_When_ValidationSuccesses_Then_HttpRequestPipelineIsContinued()
        {
            // Arrange:
            var actionArguments = new List<object> { new Person() };

            var actionContextMock = TestsCommon.Utils.CreateActionExecutingContextMock();
            actionContextMock.Setup(x => x.ActionArguments.Values).Returns(actionArguments);
            var actionContext = actionContextMock.Object;

            var validator = new TestPersonValidatorSync();
            var serviceProvider = Mock.Of<IServiceProvider>(x => x.GetService(It.IsAny<Type>()) == validator);
            var httpContext = Mock.Of<HttpContext>(x => x.RequestServices == serviceProvider);
            actionContext.HttpContext = httpContext;

            var nextDelegateCalled = false;
            Task<ActionExecutedContext> Next()
            {
                var ctx = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());
                nextDelegateCalled = true;
                return Task.FromResult(ctx);
            }

            // Act:
            var validateAttribute = new ValidateAttribute { TypeToValidate = typeof(Person) };
            await validateAttribute.OnActionExecutionAsync(actionContextMock.Object, Next);

            // Assert:
            nextDelegateCalled.Should().BeTrue();
        }

        [Test]
        public async Task For_OnActionExecutionAsync_When_ValidationFails_Then_BadRequestIsReturned()
        {
            // Arrange:
            var actionArguments = new List<object> { new Person() };

            var actionContextMock = TestsCommon.Utils.CreateActionExecutingContextMock();
            actionContextMock.Setup(x => x.ActionArguments.Values).Returns(actionArguments);

            IActionResult result = null;
            actionContextMock.SetupSet(context => context.Result = It.IsAny<IActionResult>())
                .Callback<IActionResult>(value => result = value);
            var actionContext = actionContextMock.Object;

            var validator = new TestAlwaysFailingPersonValidatorSync();
            var serviceProvider = Mock.Of<IServiceProvider>(x => x.GetService(It.IsAny<Type>()) == validator);
            var httpContext = Mock.Of<HttpContext>(x => x.RequestServices == serviceProvider);
            actionContext.HttpContext = httpContext;

            var nextDelegateCalled = false;
            Task<ActionExecutedContext> Next()
            {
                nextDelegateCalled = true;
                var ctx = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());
                return Task.FromResult(ctx);
            }

            // Act:
            var validateAttribute = new ValidateAttribute { TypeToValidate = typeof(Person) };
            await validateAttribute.OnActionExecutionAsync(actionContextMock.Object, Next);

            // Assert:
            nextDelegateCalled.Should().BeFalse();
            result.Should().BeOfType<BadRequestObjectResult>();
            var validationResult = (ValidationResult)((BadRequestObjectResult)result).Value;
            validationResult.Failure.Should().BeTrue();
            validationResult.Properties[0].PropertyName.Should().Be("FirstName");
            validationResult.Properties[1].PropertyName.Should().Be("LastName");
        }
    }
}
