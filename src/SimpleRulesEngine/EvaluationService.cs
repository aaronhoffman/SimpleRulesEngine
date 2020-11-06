using SimpleRulesEngine.Exceptions;
using SimpleRulesEngine.Models;
using System;

namespace SimpleRulesEngine
{
    public interface IEvaluationService
    {
        /// <summary>
        /// If a single context object is provided, it is the context for both the left and the right arguments.
        /// You only need to provide a context if one is required.
        /// </summary>
        /// <param name="expressionDefinition"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        bool Evaluate(ExpressionDefinition expressionDefinition, object context = null);

        bool Evaluate(ExpressionDefinition expressionDefinition, object leftContext, object rightContext);
    }

    public class EvaluationService : IEvaluationService
    {
        private readonly IArgumentExtractor _argumentExtractor;
        private readonly IExpressionEvaluator _expressionEvaluator;

        public EvaluationService(
            IArgumentExtractor argumentExtractor,
            IExpressionEvaluator expressionEvaluator)
        {
            _argumentExtractor = argumentExtractor ?? throw new ArgumentNullException(nameof(argumentExtractor));
            _expressionEvaluator = expressionEvaluator ?? throw new ArgumentNullException(nameof(expressionEvaluator));
        }

        public bool Evaluate(ExpressionDefinition expressionDefinition, object context = null)
        {
            return Evaluate(expressionDefinition, context, context);
        }

        public bool Evaluate(ExpressionDefinition expressionDefinition, object leftContext, object rightContext)
        {
            // valid parameter combinations:
            // value :: value
            // array :: array
            // array :: value
            // value :: array

            if (BothParametersAreSingleValues(expressionDefinition))
            {
                return EvaluateSingleValuesComparisonExpression(expressionDefinition, leftContext, rightContext);
            }
            else if (BothParametersAreArrays(expressionDefinition))
            {
                return EvaluateArraysComparisonExpression(expressionDefinition, leftContext, rightContext);
            }
            else if (expressionDefinition.LeftParameter.ParameterType.IsArrayParameterType())
            {
                return EvaluateLeftArrayComparisonExpression(expressionDefinition, leftContext, rightContext);
            }
            else if (expressionDefinition.RightParameter.ParameterType.IsArrayParameterType())
            {
                return EvaluateRightArrayComparisonExpression(expressionDefinition, leftContext, rightContext);
            }

            throw new SimpleRulesEngineException("Parameter type combination is not supported.");
        }

        private bool EvaluateSingleValuesComparisonExpression(ExpressionDefinition expressionDefinition, object leftContext, object rightContext)
        {
            var leftArgument = _argumentExtractor.ExtractSingleValueArgument(expressionDefinition.LeftParameter, leftContext);

            var rightArgument = _argumentExtractor.ExtractSingleValueArgument(expressionDefinition.RightParameter, rightContext);

            return _expressionEvaluator.PerformSingleValuesComparison(expressionDefinition, leftArgument, rightArgument);
        }

        private bool EvaluateArraysComparisonExpression(ExpressionDefinition expressionDefinition, object leftContext, object rightContext)
        {
            if (!expressionDefinition.ArrayComparison.IsValidArraysComparison())
            {
                throw new SimpleRulesEngineException("Array Comparison not valid.");
            }

            var leftArgument = _argumentExtractor.ExtractArrayArgument(expressionDefinition.LeftParameter, leftContext);

            var rightArgument = _argumentExtractor.ExtractArrayArgument(expressionDefinition.RightParameter, rightContext);

            return _expressionEvaluator.PerformArraysComparison(expressionDefinition, leftArgument, rightArgument);
        }

        private bool EvaluateLeftArrayComparisonExpression(ExpressionDefinition expressionDefinition, object leftContext, object rightContext)
        {
            if (!expressionDefinition.LeftParameter.ParameterType.IsArrayParameterType()
                || expressionDefinition.RightParameter.ParameterType.IsArrayParameterType())
            {
                throw new SimpleRulesEngineException("Parameter types are not valid for this type of comparison.");
            }

            var leftArgument = _argumentExtractor.ExtractArrayArgument(expressionDefinition.LeftParameter, leftContext);

            var rightArgument = _argumentExtractor.ExtractSingleValueArgument(expressionDefinition.RightParameter, rightContext);

            return _expressionEvaluator.PerformLeftArrayComparison(expressionDefinition, leftArgument, rightArgument);
        }

        private bool EvaluateRightArrayComparisonExpression(ExpressionDefinition expressionDefinition, object leftContext, object rightContext)
        {
            if (expressionDefinition.LeftParameter.ParameterType.IsArrayParameterType()
                || !expressionDefinition.RightParameter.ParameterType.IsArrayParameterType())
            {
                throw new SimpleRulesEngineException("Parameter types are not valid for this type of comparison.");
            }

            var leftArgument = _argumentExtractor.ExtractSingleValueArgument(expressionDefinition.LeftParameter, leftContext);

            var rightArgument = _argumentExtractor.ExtractArrayArgument(expressionDefinition.RightParameter, rightContext);

            return _expressionEvaluator.PerformRightArrayComparison(expressionDefinition, leftArgument, rightArgument);
        }

        private bool BothParametersAreSingleValues(ExpressionDefinition expressionDefinition)
        {
            return
                expressionDefinition.LeftParameter.ParameterType.IsSingleValueParameterType()
                && expressionDefinition.RightParameter.ParameterType.IsSingleValueParameterType();
        }

        private bool BothParametersAreArrays(ExpressionDefinition expressionDefinition)
        {
            return
                expressionDefinition.LeftParameter.ParameterType.IsArrayParameterType()
                && expressionDefinition.RightParameter.ParameterType.IsArrayParameterType();
        }
    }
}
