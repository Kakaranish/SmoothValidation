using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SmoothValidation.Tests.Unit.PropertyValidators
{
    internal static class PropertyValidatorTestsCommon
    {
        internal static MemberInfo CreateTestMemberInfo()
        {
            Expression<Func<TestClass, string>> expression = obj => obj.SomeProperty;
            var memberExpression = expression.Body as MemberExpression;
            return typeof(TestClass).GetProperty(memberExpression.Member.Name);
        }

        internal class TestClass
        {
            public string SomeProperty { get; set; }
        }
    }
}
