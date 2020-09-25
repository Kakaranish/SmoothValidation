using System;
using NUnit.Framework;
using SmoothValidation.ValidationRules;

namespace SmoothValidation.Tests.Unit.ValidationRules
{
    [TestFixture]
    public class ValidationRuleBaseTests
    {
        [Test]
        public void When_PassedErrorMessageIsNull_Then_ExceptionIsThrown()
        {
            // Arrange:
            Assert.Throws<ArgumentNullException>(() => new ValidationRuleImpl(null, "any"));
        }

        private class ValidationRuleImpl : ValidationRuleBase
        {
            public ValidationRuleImpl(string errorMessage, string errorCode = null)
                : base(errorMessage, errorCode)
            {
            }
        }
    }
}
