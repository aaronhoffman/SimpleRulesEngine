namespace SimpleRulesEngine.Models
{
    public enum ParameterType
    {
        NotSet = 0,

        Text = 1,
        Numeric = 2,
        Array = 3,

        /// <summary>
        /// Parameter is an array but items will be aggregated into a single value.
        /// Note: If this option is set on ParameterDefinition, the ArrayAggregessionType must also be provided.
        /// </summary>
        ArrayAggregation = 4,
    }

    public static class ParameterTypeExtensions
    {
        public static bool HasValue(this ParameterType parameterType)
        {
            return parameterType != ParameterType.NotSet;
        }

        public static bool IsSingleValueParameterType(this ParameterType parameterType)
        {
            return
                parameterType == ParameterType.Text
                || parameterType == ParameterType.Numeric
                || parameterType == ParameterType.ArrayAggregation;
        }

        public static bool IsArrayParameterType(this ParameterType parameterType)
        {
            return parameterType == ParameterType.Array;
        }
    }
}
