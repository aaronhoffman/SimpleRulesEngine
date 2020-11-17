namespace SimpleRulesEngine.Models
{
    public enum ExpressionAggregationMethod
    {
        NotSet = 0,
        Any = 1,
        All = 2,
    }

    public static class ExpressionAggregationMethodExtensions
    {
        public static bool HasValue(this ExpressionAggregationMethod expressionAggregationMethod)
        {
            return expressionAggregationMethod != ExpressionAggregationMethod.NotSet;
        }
    }
}
