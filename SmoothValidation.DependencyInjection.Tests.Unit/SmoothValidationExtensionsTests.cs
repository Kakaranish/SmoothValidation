using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SmoothValidation.ClassValidators;
using System.Reflection;
using SmoothValidation.DependencyInjection.Tests.Unit.TestsCommon;

namespace SmoothValidation.DependencyInjection.Tests.Unit
{
    [TestFixture]
    public class SmoothValidationExtensionsTests
    {
        [Test]
        public void For_AddSmoothValidation_PublicAndInternalImplementationsOfClassValidatorsShouldBeRegistered()
        {
            // Arrange:
            var assembly = Assembly.GetAssembly(typeof(SmoothValidationExtensionsTests));
            var services = new Mock<IServiceCollection>();

            // Act:
            services.Object.AddSmoothValidation(assembly);

            // Assert:
            // Only internal & public class validators from TestTypes should be registered
            services.Verify(x => x.Add(It.Is<ServiceDescriptor>(y =>
                y.Lifetime == ServiceLifetime.Scoped)), Times.Exactly(8));
        }

        // This class should not be registered
        private class PrivateTestValidatorSync : ClassValidator<Person>
        {
            protected override void SetupRules()
            {
            }
        }
    }
}
