using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmoothValidation.Types
{
    public class Member
    {
        public static readonly IList<MemberTypes> LegalTypes = new List<MemberTypes>
        {
            MemberTypes.Field,
            MemberTypes.Property
        };

        public MemberInfo MemberInfo { get; }

        public bool IsField { get; }
        public bool IsProperty => !IsField;
        public string Name => MemberInfo.Name;

        public Member(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo ?? throw new ArgumentNullException(nameof(memberInfo));

            if (!LegalTypes.Contains(memberInfo.MemberType))
            {
                throw new ArgumentException($"'{nameof(memberInfo)}' is illegal type");
            }

            IsField = memberInfo is FieldInfo;
        }

        public object GetValue(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return IsField
                ? ((FieldInfo) MemberInfo).GetValue(obj)
                : ((PropertyInfo) MemberInfo).GetValue(obj);
        }
    }
}
