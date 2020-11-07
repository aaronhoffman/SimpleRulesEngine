using SimpleRulesEngine.Exceptions;
using SimpleRulesEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleRulesEngine
{
    public interface IExpressionEvaluator
    {
        bool EvaluateLeftSingleValueRightSingleValueExpression(ExpressionDefinition expressionDefinition, object leftArgument, object rightArgument);

        bool EvaluateLeftArrayRightArrayExpression(ExpressionDefinition expressionDefinition, IEnumerable<object> leftArgument, IEnumerable<object> rightArgument);

        bool EvaluateLeftArrayRightSingleValueExpression(ExpressionDefinition expressionDefinition, IEnumerable<object> leftArgument, object rightArgument);

        bool EvaluateLeftSingleValueRightArrayExpression(ExpressionDefinition expressionDefinition, object leftArgument, IEnumerable<object> rightArgument);
    }

    public class ExpressionEvaluator : IExpressionEvaluator
    {
        public bool EvaluateLeftSingleValueRightSingleValueExpression(
            ExpressionDefinition expressionDefinition,
            object leftArgument,
            object rightArgument)
        {
            if (expressionDefinition.NumericComparison.HasValue
                && expressionDefinition.TextComparison.HasValue)
            {
                throw new SimpleRulesEngineException("Both NumericComparison and TextComparison have been set and only one is allowed.");
            }

            if (expressionDefinition.NumericComparison.HasValue)
            {
                var leftArgumentNumber = Convert.ToDecimal(leftArgument);
                var rightArgumentNumber = Convert.ToDecimal(rightArgument);

                return PerformLeftSingleValueRightSingleValueNumericComparison(
                    expressionDefinition.NumericComparison.Value,
                    leftArgumentNumber,
                    rightArgumentNumber);
            }

            if (expressionDefinition.TextComparison.HasValue)
            {
                var leftArgumentText = Convert.ToString(leftArgument);
                var rightArgumentText = Convert.ToString(rightArgument);

                return PerformLeftSingleValueRightSingleValueTextComparison(
                    expressionDefinition.TextComparison.Value,
                    leftArgumentText,
                    rightArgumentText);
            }

            throw new SimpleRulesEngineException("Numeric or Text Comparison is required.");
        }

        public bool EvaluateLeftArrayRightArrayExpression(
            ExpressionDefinition expressionDefinition,
            IEnumerable<object> leftArgument,
            IEnumerable<object> rightArgument)
        {
            if (!expressionDefinition.ArrayComparison.HasValue)
            {
                throw new SimpleRulesEngineException("ArrayComparison was not provided.");
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

        public bool EvaluateLeftArrayRightSingleValueExpression(
            ExpressionDefinition expressionDefinition,
            IEnumerable<object> leftArgument,
            object rightArgument)
        {
            if (!expressionDefinition.ExpressionAggregationMethod.HasValue)
            {
                throw new SimpleRulesEngineException("ExpressionAggregationMethod not provided.");
            }

            switch (expressionDefinition.ExpressionAggregationMethod)
            {
                case ExpressionAggregationMethod.All:
                    return leftArgument.All(x => EvaluateLeftSingleValueRightSingleValueExpression(expressionDefinition, x, rightArgument));
                case ExpressionAggregationMethod.Any:
                    return leftArgument.Any(x => EvaluateLeftSingleValueRightSingleValueExpression(expressionDefinition, x, rightArgument));
                default:
                    throw new SimpleRulesEngineException("Expression Aggregation not supported.");
            }
        }

        public bool EvaluateLeftSingleValueRightArrayExpression(
            ExpressionDefinition expressionDefinition,
            object leftArgument,
            IEnumerable<object> rightArgument)
        {
            // todo: opportunity to consolidate.
            // e.g `IsAny` can be translated to `ContainsAny`

            if (!expressionDefinition.ArrayComparison.HasValue)
            {
                throw new SimpleRulesEngineException("ArrayComparison must be provided.");
            }

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

        private bool PerformLeftSingleValueRightSingleValueNumericComparison(
            NumericComparison numericComparison,
            decimal leftArgument,
            decimal rightArgument)
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

        private bool PerformLeftSingleValueRightSingleValueTextComparison(
            TextComparison textComparison,
            string leftArgument,
            string rightArgument)
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
