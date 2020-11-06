using SimpleRulesEngine.Exceptions;
using SimpleRulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleRulesEngine
{
    public interface IExpressionEvaluator
    {
        bool PerformSingleValuesComparison(ExpressionDefinition expressionDefinition, object leftArgument, object rightArgument);

        bool PerformArraysComparison(ExpressionDefinition expressionDefinition, IEnumerable<object> leftArgument, IEnumerable<object> rightArgument);

        bool PerformLeftArrayComparison(ExpressionDefinition expressionDefinition, IEnumerable<object> leftArgument, object rightArgument);

        bool PerformRightArrayComparison(ExpressionDefinition expressionDefinition, object leftArgument, IEnumerable<object> rightArgument);
    }

    public class ExpressionEvaluator : IExpressionEvaluator
    {
        public bool PerformSingleValuesComparison(ExpressionDefinition expressionDefinition, object leftArgument, object rightArgument)
        {
            // todo: validation could be added here to prevent undesired comparisons (e.g. are Numeric and Text both defined)

            if (expressionDefinition.NumericComparison.HasValue)
            {
                var leftArgumentNumber = Convert.ToDecimal(leftArgument);
                var rightArgumentNumber = Convert.ToDecimal(rightArgument);

                return PerformSingleValuesComparison(expressionDefinition.NumericComparison.Value, leftArgumentNumber, rightArgumentNumber);
            }

            if (expressionDefinition.TextComparison.HasValue)
            {
                var leftArgumentText = Convert.ToString(leftArgument);
                var rightArgumentText = Convert.ToString(rightArgument);

                return PerformSingleValuesComparison(expressionDefinition.TextComparison.Value, leftArgumentText, rightArgumentText);
            }

            throw new SimpleRulesEngineException("Numeric or Text Comparison is required.");
        }

        public bool PerformArraysComparison(ExpressionDefinition expressionDefinition, IEnumerable<object> leftArgument, IEnumerable<object> rightArgument)
        {
            if (!expressionDefinition.ArrayComparison.HasValue)
            {
                throw new SimpleRulesEngineException("Can not perform Array Comparison. Array Comparison is not defined.");
            }

            switch (expressionDefinition.ArrayComparison.Value)
            {
                case ArrayComparison.ContainsAny:
                    return leftArgument.Intersect(rightArgument).Any();
                case ArrayComparison.DoesNotContainAny:
                    return !leftArgument.Intersect(rightArgument).Any();
                case ArrayComparison.ContainsAll:
                    return !rightArgument.Except(leftArgument).Any();
                default:
                    throw new SimpleRulesEngineException("Array Comparison is not supported.");
            }
        }

        public bool PerformLeftArrayComparison(ExpressionDefinition expressionDefinition, IEnumerable<object> leftArgument, object rightArgument)
        {
            switch (expressionDefinition.ExpressionAggregationMethod)
            {
                case ExpressionAggregationMethod.All:
                    return leftArgument.All(x => PerformSingleValuesComparison(expressionDefinition, x, rightArgument));
                case ExpressionAggregationMethod.Any:
                    return leftArgument.Any(x => PerformSingleValuesComparison(expressionDefinition, x, rightArgument));
                default:
                    throw new SimpleRulesEngineException("Expression Aggregation not supported.");
            }
        }

        public bool PerformRightArrayComparison(ExpressionDefinition expressionDefinition, object leftArgument, IEnumerable<object> rightArgument)
        {
            // todo: opportunity to consolidate with PerformArraysComparison
            // e.g `IsAny` can be translated to `ContainsAny`

            switch (expressionDefinition.ArrayComparison.Value)
            {
                case ArrayComparison.IsAny:
                    return rightArgument.Any(x => x.Equals(leftArgument));
                case ArrayComparison.IsNotAny:
                    return !rightArgument.Any(x => x.Equals(leftArgument));
                default:
                    throw new SimpleRulesEngineException("Array Comparison not supported.");
            }
        }

        private bool PerformSingleValuesComparison(NumericComparison numericComparison, decimal leftArgument, decimal rightArgument)
        {
            switch (numericComparison)
            {
                case NumericComparison.Equal:
                    return leftArgument == rightArgument;
                case NumericComparison.NotEqual:
                    return leftArgument != rightArgument;
                case NumericComparison.GreaterThan:
                    return leftArgument > rightArgument;
                case NumericComparison.GreaterThanOrEqual:
                    return leftArgument >= rightArgument;
                case NumericComparison.LessThan:
                    return leftArgument < rightArgument;
                case NumericComparison.LessThanOrEqual:
                    return leftArgument <= rightArgument;
                default:
                    throw new SimpleRulesEngineException("Numeric Comparison not supported.");
            }
        }

        private bool PerformSingleValuesComparison(TextComparison textComparison, string leftArgument, string rightArgument)
        {
            switch (textComparison)
            {
                case TextComparison.Equal:
                    return leftArgument == rightArgument;
                case TextComparison.NotEqual:
                    return leftArgument != rightArgument;
                case TextComparison.Contains:
                    if (leftArgument == null || rightArgument == null)
                    {
                        return false;
                    }

                    return leftArgument.ToLower().Contains(rightArgument.ToLower());
                case TextComparison.StartsWith:
                    if (leftArgument == null || rightArgument == null)
                    {
                        return false;
                    }

                    return leftArgument.ToLower().StartsWith(rightArgument.ToLower());
                case TextComparison.EndsWith:
                    if (leftArgument == null || rightArgument == null)
                    {
                        return false;
                    }

                    return leftArgument.ToLower().EndsWith(rightArgument.ToLower());
                default:
                    throw new SimpleRulesEngineException("Text Comparison not supported.");
            }
        }
    }
}
