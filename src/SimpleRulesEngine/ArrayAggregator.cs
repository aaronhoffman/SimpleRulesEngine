using SimpleRulesEngine.Exceptions;
using SimpleRulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleRulesEngine
{
    public interface IArrayAggregator
    {
        object AggregateArray(IEnumerable<object> array, ArrayAggregationMethod arrayAggregationMethod);
    }

    public class ArrayAggregator : IArrayAggregator
    {
        public object AggregateArray(IEnumerable<object> array, ArrayAggregationMethod arrayAggregationMethod)
        {
            // todo: some aggregation may not require decimal (e.g. min, max), but that's all this can support at this time
            var decimalArray = array.Select(x => Convert.ToDecimal(x)).ToList();

            var aggregatedResult = 0m;

            switch (arrayAggregationMethod)
            {
                case ArrayAggregationMethod.Sum:
                    aggregatedResult = Enumerable.Sum(decimalArray);
                    break;
                case ArrayAggregationMethod.Avg:
                    aggregatedResult = Enumerable.Average(decimalArray);
                    break;
                case ArrayAggregationMethod.Max:
                    aggregatedResult = Enumerable.Max(decimalArray);
                    break;
                case ArrayAggregationMethod.Min:
                    aggregatedResult = Enumerable.Min(decimalArray);
                    break;
                default:
                    throw new SimpleRulesEngineException("Aggregation Method not supported.");
            }

            return aggregatedResult;
        }
    }
}
