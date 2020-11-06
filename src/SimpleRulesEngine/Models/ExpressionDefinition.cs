namespace SimpleRulesEngine.Models
{
    public class ExpressionDefinition
    {
        public ParameterDefinition LeftParameter { get; set; }

        public ParameterDefinition RightParameter { get; set; }

        /// <summary>
        /// If the Left Parameter is an array, the ExpressionAggregationMethod must be provided.
        /// Must the comparison evaluate to true for `All` items in the array, or only one of the items (`Any`).
        /// </summary>
        public ExpressionAggregationMethod? ExpressionAggregationMethod { get; set; }

        /// <summary>
        /// Either NumericComparison or TextComparison must be provided.
        /// </summary>
        public NumericComparison? NumericComparison { get; set; }

        /// <summary>
        /// Either TextComparison or NumericComparison must be provided.
        /// </summary>
        public TextComparison? TextComparison { get; set; }

        // For ArrayComparison, Either both sides are an array and we are testing some Intersection, or only the right side is an array.
        public ArrayComparison? ArrayComparison { get; set; }
    }
}
