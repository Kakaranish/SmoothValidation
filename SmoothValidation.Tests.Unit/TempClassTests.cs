using FluentAssertions;
using NUnit.Framework;

namespace SmoothValidation.Tests.Unit
{
    [TestFixture]
    public class TempClassTests
    {
        [Test]
        public void Test1()
        {
            var value = true;
            value.Should().Be(true);
        }
    }
}
