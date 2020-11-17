namespace SimpleRulesEngine.Models
{
    public class ExpressionDefinition
    {
        public ParameterDefinition LeftParameter { get; set; } = new ParameterDefinition();

        public ParameterDefinition RightParameter { get; set; } = new ParameterDefinition();

        /// <summary>
        /// If the Left Parameter is an array, the ExpressionAggregationMethod must be provided.
        /// Value indicates if the comparison must evaluate to true for `All` items in the array, or only one of the items (`Any`).
        /// </summary>
        public ExpressionAggregationMethod ExpressionAggregationMethod { get; set; } = ExpressionAggregationMethod.NotSet;

        /// <summary>
        /// Either `NumericComparison` or `TextComparison` must be provided.
        /// </summary>
        public NumericComparison NumericComparison { get; set; } = NumericComparison.NotSet;

        /// <summary>
        /// Either `TextComparison` or `NumericComparison` must be provided.
        /// </summary>
        public TextComparison TextComparison { get; set; } = TextComparison.NotSet;

        /// <summary>
        /// Must be provided if both the left and right arguments are arrays,
        /// or the left argument is a single value and the right argument is an array.
        /// </summary>
        public ArrayComparison ArrayComparison { get; set; } = ArrayComparison.NotSet;
    }
}
