using SmoothValidation.PropertyValidators;
using System.Reflection;

namespace SmoothValidation.Tests.Unit.TestsCommon
{
    internal class TestSyncPropertyValidator<TProp> : SyncPropertyValidator<object, TProp>
    {
        internal TestSyncPropertyValidator(MemberInfo memberInfo) : base(memberInfo)
        {
        }
    }

    internal class TestAsyncPropertyValidator<TProp> : AsyncPropertyValidator<object, TProp>
    {
        internal TestAsyncPropertyValidator(MemberInfo memberInfo) : base(memberInfo)
        {
        }
    }
}
