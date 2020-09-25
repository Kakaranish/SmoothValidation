using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SmoothValidation.PropertyValidators;
using SmoothValidation.RootValidators;

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
            var propertyInfo = PropertyExtractor.Extract(expression);

            if (PropertyValidators.TryGetValue(propertyInfo.Name, out var propertyValidator))
            {
                return (SyncPropertyValidator<TProp>)propertyValidator;
            }

            var newPropertyValidator = new SyncPropertyValidator<TProp>(propertyInfo);
            PropertyValidators.Add(propertyInfo.Name, newPropertyValidator);

            return newPropertyValidator;
        }
    }
}