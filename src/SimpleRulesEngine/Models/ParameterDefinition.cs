namespace SimpleRulesEngine.Models
{
    public class ParameterDefinition
    {
        /// <summary>
        /// Must either provide `ConstantValue` or `PropertyPath` but not both.
        /// If this value is set, an external Context will not be needed to evaluate/extract the value for this parameter.
        /// Note: Value must be serializable.
        /// </summary>
        public object ConstantValue { get; set; }

        /// <summary>
        /// Must either provide `PropertyPath` or `ConstantValue` but not both.
        /// If this value is set, an external Context must be provided to evaluate/extract the value for this parameter.
        /// </summary>
        public string PropertyPath { get; set; }

        /// <summary>
        /// Defines the type of value that will be extracted from this ParameterDefinition.
        /// Note: If the ParameterType is set to `ArrayAggregation`, an `ArrayAggregationMethod` is also required.
        /// </summary>
        public ParameterType ParameterType { get; set; }

        /// <summary>
        /// Optional.
        /// Defines how the array referenced by this parameter will be aggregated/consolidated into a single value when the ExpressionDefinition is evaluated.
        /// Note: This value is only required if the `ParameterType` is set to `ArrayAggregation`.
        /// </summary>
        public ArrayAggregationMethod? ArrayAggregationMethod { get; set; }

        /// <summary>
        /// Optional.
        /// If `ParameterType` is `ArrayAggregation`, and the array referenced by this parameter is an array of complex types,
        /// this property can be used to specify the path to the value within each array item/complex type.
        /// Note: Leave this value blank to indicate array item itself is the value.
        /// ex: You would not provide a property path if the array referenced by this parameter was an array of simple types.
        /// </summary>
        public string ArrayAggregationArrayItemPropertyPath { get; set; }
    }
}
