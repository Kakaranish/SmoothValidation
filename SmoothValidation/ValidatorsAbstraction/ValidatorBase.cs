using SmoothValidation.PropertyValidators;
using SmoothValidation.Utils;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SmoothValidation.ValidatorsAbstraction
{
    public abstract class ValidatorBase<TObject>
    {
        protected readonly IDictionary<string, IPropertyValidator> PropertyValidators =
            new Dictionary<string, IPropertyValidator>();

        protected IPropertyExtractor<TObject> PropertyExtractor;

        protected ValidatorBase()
        {
            PropertyExtractor = new PropertyExtractor<TObject>();
        }

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