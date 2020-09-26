using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SmoothValidation.PropertyValidators;
using SmoothValidation.Utils;

namespace SmoothValidation.RootValidators
{
    public abstract class RootValidatorBase<TObject>
    {
        protected readonly Dictionary<string, IPropertyValidator> PropertyValidators =
            new Dictionary<string, IPropertyValidator>();
        protected IPropertyExtractor<TObject> PropertyExtractor = new PropertyExtractor<TObject>();

        internal IReadOnlyDictionary<string, IPropertyValidator> PropertyValidatorsAsReadonly => PropertyValidators;

        public SyncPropertyValidator<TProp> Setup<TProp>(Expression<Func<TObject, TProp>> expression)
        {
            var memberInfo = PropertyExtractor.Extract(expression);

            if (PropertyValidators.TryGetValue(memberInfo.Name, out var propertyValidator))
            {
                return (SyncPropertyValidator<TProp>)propertyValidator;
            }

            var newPropertyValidator = new SyncPropertyValidator<TProp>(memberInfo);
            PropertyValidators.Add(memberInfo.Name, newPropertyValidator);

            return newPropertyValidator;
        }
    }
}