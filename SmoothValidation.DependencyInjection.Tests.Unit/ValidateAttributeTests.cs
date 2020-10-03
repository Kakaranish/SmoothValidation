using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using NUnit.Framework;
using SmoothValidation.DependencyInjection.Filters;
using SmoothValidation.Types.ValidationResult;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmoothValidation.DependencyInjection.Tests.Unit
{
    [TestFixture]
    public class ValidateAttributeTests
    {
        [Test]
        public async Task For_OnActionExecutionAsync_When_ValidationSuccesses_Then_HttpRequestPipelineIsContinued()
        {
            // Arrange:
            var actionArguments = new List<object> { new Person() };

            var actionContextMock = CreateActionExecutingContextMock();
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

            var actionContextMock = CreateActionExecutingContextMock();
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

        private static Mock<ActionExecutingContext> CreateActionExecutingContextMock()
        {
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("name", "invalid");

            var actionContext = new ActionContext(
                Mock.Of<HttpContext>(),
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                modelState
            );

            return new Mock<ActionExecutingContext>(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>()
            );
        }
    }
}
