using SimpleRulesEngine.Exceptions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SimpleRulesEngine
{
    public interface IValueExtractor
    {
        object ExtractValue(object context, string propertyPath);

        object ExtractValue(object context, string[] propertyPathParts);

        IEnumerable<object> ExtractArray(object context, string arrayPropertyPath, string arrayAggregationArrayItemPropertyPath = null);
    }

    public class ValueExtractor : IValueExtractor
    {
        public object ExtractValue(object context, string propertyPath)
        {
            // If propertyPath is empty, the entire context should be used.
            if (string.IsNullOrWhiteSpace(propertyPath))
            {
                return context;
            }

            var propertyPathParts = propertyPath.Split('.');

            return ExtractValue(context, propertyPathParts);
        }

        public object ExtractValue(object context, string[] propertyPathParts)
        {
            if (context == null)
            {
                throw new SimpleRulesEngineException("Parameter Context can not be null if ParameterSource is PropertyReference.");
            }

            if (propertyPathParts == null || propertyPathParts.Length == 0)
            {
                return context;
            }

            var currentContext = context;

            for (var i = 0; i < propertyPathParts.Length; i++)
            {
                var currentPropertyPathPart = propertyPathParts[i];
                var currentContextType = currentContext.GetType();
                var newContext = currentContextType.GetProperty(currentPropertyPathPart).GetValue(currentContext);

                currentContext = newContext;
            }

            return currentContext;
        }

        public IEnumerable<object> ExtractArray(object context, string arrayPropertyPath, string arrayAggregationArrayItemPropertyPath = null)
        {
            var potentialArray = ExtractValue(context, arrayPropertyPath);

            var array = Enumerable.Empty<object>();

            // strings are `IEnumerable`, but not valid in this case
            if (potentialArray is string)
            {
                throw new SimpleRulesEngineException("Referenced property is not an array.");
            }

            if (potentialArray is IEnumerable)
            {
                array = (potentialArray as IEnumerable).Cast<object>().ToList();
            }
            else
            {
                throw new SimpleRulesEngineException("Referenced property is not an array.");
            }

            // If ArrayItemPropertyPath is not defined, we don't need to retrieve anything out of each item, return the array from context.
            if (string.IsNullOrWhiteSpace(arrayAggregationArrayItemPropertyPath))
            {
                return array;
            }

            // if an ArrayItemPropertyPath is defined, evaluate that path on each item and return the extracted result.
            var arrayItemPropertyPathForValueParts = arrayAggregationArrayItemPropertyPath?.Split('.');

            var extractedArrayItems = array
                .Select(x => ExtractValue(x, arrayItemPropertyPathForValueParts))
                .ToList();

            return extractedArrayItems;
        }
    }
}
