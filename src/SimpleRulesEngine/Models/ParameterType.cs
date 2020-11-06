namespace SimpleRulesEngine.Models
{
    public enum ParameterType
    {
        NotSet = 0,

        Text = 1,
        Numeric = 2,
        Array = 3,

        /// <summary>
        /// Parameter is an array but items will be aggregated into a single numeric value.
        /// If this option is set on ExpressionParameter, the ArrayAggregessionType must also be provided.
        /// </summary>
        ArrayAggregation = 4,
    }

    public static class ParameterTypeExtensions
    {
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
