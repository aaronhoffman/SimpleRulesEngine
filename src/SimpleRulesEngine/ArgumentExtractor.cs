using SimpleRulesEngine.Exceptions;
using SimpleRulesEngine.Models;
using System;
using System.Collections.Generic;

namespace SimpleRulesEngine
{
    public interface IArgumentExtractor
    {
        object ExtractSingleValueArgument(ParameterDefinition parameterDefinition, object context = null);

        IEnumerable<object> ExtractArrayArgument(ParameterDefinition parameterDefinition, object context = null);
    }

    public class ArgumentExtractor : IArgumentExtractor
    {
        private readonly IValueExtractor _valueExtractor;
        private readonly IArrayAggregator _arrayAggregator;

        public ArgumentExtractor(
            IValueExtractor valueExtractor,
            IArrayAggregator arrayAggregator)
        {
            _valueExtractor = valueExtractor ?? throw new ArgumentNullException(nameof(valueExtractor));
            _arrayAggregator = arrayAggregator ?? throw new ArgumentNullException(nameof(arrayAggregator));
        }

        public object ExtractSingleValueArgument(ParameterDefinition parameterDefinition, object context = null)
        {
            if (!parameterDefinition.ParameterType.IsSingleValueParameterType())
            {
                throw new SimpleRulesEngineException("Parameter type is not valid for single value.");
            }

            ValidateValueSource(parameterDefinition);

            switch (parameterDefinition.ParameterType)
            {
                case ParameterType.Text:
                case ParameterType.Numeric:
                    return ExtractSingleValueArgumentForSimpleType(parameterDefinition, context);
                case ParameterType.ArrayAggregation:
                    return ExtractSingleValueArgumentForArrayAggregation(parameterDefinition, context);
                case ParameterType.Array:
                    throw new SimpleRulesEngineException("Array ParameterType is invalid. Can not convert to value argument. Perhaps `ArrayAggregation` was the intended type?");
                default:
                    throw new SimpleRulesEngineException("Parameter Type is not supported.");
            }
        }

        public IEnumerable<object> ExtractArrayArgument(ParameterDefinition parameterDefinition, object context = null)
        {
            if (!parameterDefinition.ParameterType.IsArrayParameterType())
            {
                throw new SimpleRulesEngineException("ParameterType is not Array. Array Argument can not be extracted.");
            }

            ValidateValueSource(parameterDefinition);

            var array = RetrieveArray(parameterDefinition, context);

            return array;
        }

        private object ExtractSingleValueArgumentForSimpleType(ParameterDefinition parameterDefinition, object context)
        {
            if (parameterDefinition.ConstantValue != null)
            {
                return parameterDefinition.ConstantValue;
            }
            else
            {
                var value = _valueExtractor.ExtractValue(context, parameterDefinition.PropertyPath);

                return value;
            }
        }

        private object ExtractSingleValueArgumentForArrayAggregation(ParameterDefinition parameterDefinition, object context)
        {
            if (!parameterDefinition.ArrayAggregationMethod.HasValue)
            {
                // if there is no aggregation strategy, we can not consolidate/aggregate the array
                throw new SimpleRulesEngineException("ParameterType is ArrayAggregation but ArrayAggregationMethod is not specified.");
            }

            var array = RetrieveArray(parameterDefinition, context);

            var result = _arrayAggregator.AggregateArray(array, parameterDefinition.ArrayAggregationMethod.Value);

            return result;
        }

        private IEnumerable<object> RetrieveArray(ParameterDefinition parameterDefinition, object context)
        {
            if (parameterDefinition.ConstantValue != null)
            {
                return RetrieveArrayForConstantParameterSource(parameterDefinition);
            }
            else
            {
                var array = _valueExtractor.ExtractArray(context, parameterDefinition.PropertyPath, parameterDefinition.ArrayAggregationArrayItemPropertyPath);

                return array;
            }
        }

        private IEnumerable<object> RetrieveArrayForConstantParameterSource(ParameterDefinition parameterDefinition)
        {
            var array = (IEnumerable<object>)parameterDefinition.ConstantValue;

            return array;
        }

        private void ValidateValueSource(ParameterDefinition parameterDefinition)
        {
            if (parameterDefinition.ConstantValue != null && !string.IsNullOrWhiteSpace(parameterDefinition.PropertyPath))
            {
                throw new SimpleRulesEngineException("Both ConstantValue and PropertyPath are defined on the ParameterDefinition and only one is allowed.");
            }
        }
    }
}
