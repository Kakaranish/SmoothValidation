using System;

namespace SmoothValidation.Utils
{
    public static class Common
    {
        public static TTarget Cast<TTarget>(object obj)
        {
            if (obj is TTarget toValidate)
            {
                return toValidate;
            }

            if (typeof(TTarget).IsValueType || obj != null)
            {
                throw new ArgumentException($"'{nameof(obj)}' is not {typeof(TTarget).Name} type");
            }

            return default;
        }
    }
}
