namespace SimpleRulesEngine.Models
{
    public class ParameterDefinition
    {
        /// <summary>
        /// Must either provide `ConstantValue` or `PropertyPath` but not both.
        /// If this value is set, an external Context will not be needed to evaluate/extract the value from this parameter.
        /// Value must be serializable.
        /// </summary>
        public object ConstantValue { get; set; }

        /// <summary>
        /// Must either provide `PropertyPath` or `ConstantValue` but not both.
        /// If this value is set, an external Context will be needed to evaluate/extract the value from this parameter.
        /// </summary>
        public string PropertyPath { get; set; }

        /// <summary>
        /// Defines the type of value that will be extracted from this ParameterDefinition.
        /// </summary>
        public ParameterType ParameterType { get; set; }

        /// <summary>
        /// Required if `ParameterType` is `ArrayAggregation`.
        /// </summary>
        public ArrayAggregationMethod? ArrayAggregationMethod { get; set; }

        /// <summary>
        /// If `ParameterType` is `ArrayAggregation`, this property can be used to specify the path to the value within each array item.
        /// note: Leave this value blank to indicate array item itself is the value. (ex: you would not provide a property path if you wanted to aggregate an array of integer values.)
        /// </summary>
        public string ArrayAggregationArrayItemPropertyPath { get; set; }
    }
}
