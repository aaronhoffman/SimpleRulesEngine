using SimpleRulesEngine.Exceptions;
using SimpleRulesEngine.Models;
using System;

namespace SimpleRulesEngine
{
    public interface IEvaluationService
    {
        /// <summary>
        /// Evaluate the ExpressionDefinition given the provided context.
        /// If a single context object is provided, it is the context for both the left and the right arguments.
        /// If an ExpressionDefinition uses only constant values, a context is not required.
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
                return EvaluateLeftSingleValueRightSingleValueExpression(expressionDefinition, leftContext, rightContext);
            }
            else if (BothParametersAreArrays(expressionDefinition))
            {
                return EvaluateLeftArrayRightArrayExpression(expressionDefinition, leftContext, rightContext);
            }
            else if (expressionDefinition.LeftParameter.ParameterType.IsArrayParameterType())
            {
                return EvaluateLeftArrayRightSingleValueExpression(expressionDefinition, leftContext, rightContext);
            }
            else if (expressionDefinition.RightParameter.ParameterType.IsArrayParameterType())
            {
                return EvaluateLeftSingleValueRightArrayComparisonExpression(expressionDefinition, leftContext, rightContext);
            }

            throw new SimpleRulesEngineException("Parameter type combination is not supported.");
        }

        private bool EvaluateLeftSingleValueRightSingleValueExpression(ExpressionDefinition expressionDefinition, object leftContext, object rightContext)
        {
            if (!expressionDefinition.LeftParameter.ParameterType.IsSingleValueParameterType())
            {
                throw new SimpleRulesEngineException("ParameterType of the left parameter is not a valid single value type.");
            }

            if (!expressionDefinition.RightParameter.ParameterType.IsSingleValueParameterType())
            {
                throw new SimpleRulesEngineException("ParameterType of the right parameter is not a valid single value type.");
            }

            var leftArgument = _argumentExtractor.ExtractSingleValueArgument(expressionDefinition.LeftParameter, leftContext);

            var rightArgument = _argumentExtractor.ExtractSingleValueArgument(expressionDefinition.RightParameter, rightContext);

            return _expressionEvaluator.EvaluateLeftSingleValueRightSingleValueExpression(expressionDefinition, leftArgument, rightArgument);
        }

        private bool EvaluateLeftArrayRightArrayExpression(ExpressionDefinition expressionDefinition, object leftContext, object rightContext)
        {
            if (!expressionDefinition.LeftParameter.ParameterType.IsArrayParameterType())
            {
                throw new SimpleRulesEngineException("The ParameterType of the Left Parameter is not a valid array type.");
            }

            if (!expressionDefinition.RightParameter.ParameterType.IsArrayParameterType())
            {
                throw new SimpleRulesEngineException("The ParameterType of the Right Parameter is not a valid array type.");
            }

            if (!expressionDefinition.ArrayComparison.HasValue())
            {
                throw new SimpleRulesEngineException("Both the Left and Right parameters are array type but a valid ArrayComparison was not provided.");
            }

            if (!expressionDefinition.ArrayComparison.IsValidArraysComparison())
            {
                throw new SimpleRulesEngineException($"Array Comparison of [{expressionDefinition.ArrayComparison.ToString()}] not valid when both parameters are arrays.");
            }

            var leftArgument = _argumentExtractor.ExtractArrayArgument(expressionDefinition.LeftParameter, leftContext);

            var rightArgument = _argumentExtractor.ExtractArrayArgument(expressionDefinition.RightParameter, rightContext);

            return _expressionEvaluator.EvaluateLeftArrayRightArrayExpression(expressionDefinition, leftArgument, rightArgument);
        }

        private bool EvaluateLeftArrayRightSingleValueExpression(ExpressionDefinition expressionDefinition, object leftContext, object rightContext)
        {
            if (!expressionDefinition.LeftParameter.ParameterType.IsArrayParameterType())
            {
                throw new SimpleRulesEngineException("The ParameterType of the Left Parameter is not a valid array type.");
            }

            if (!expressionDefinition.RightParameter.ParameterType.IsSingleValueParameterType())
            {
                throw new SimpleRulesEngineException("The ParameterType of the Right Parameter is not a valid single value type.");
            }

            var leftArgument = _argumentExtractor.ExtractArrayArgument(expressionDefinition.LeftParameter, leftContext);

            var rightArgument = _argumentExtractor.ExtractSingleValueArgument(expressionDefinition.RightParameter, rightContext);

            return _expressionEvaluator.EvaluateLeftArrayRightSingleValueExpression(expressionDefinition, leftArgument, rightArgument);
        }

        private bool EvaluateLeftSingleValueRightArrayComparisonExpression(ExpressionDefinition expressionDefinition, object leftContext, object rightContext)
        {
            if (!expressionDefinition.LeftParameter.ParameterType.IsSingleValueParameterType())
            {
                throw new SimpleRulesEngineException("The ParameterType of the Left Parameter is not a valid single value type.");
            }

            if (!expressionDefinition.RightParameter.ParameterType.IsArrayParameterType())
            {
                throw new SimpleRulesEngineException("The ParameterType of the Right Parameter is not a valid array type.");
            }

            var leftArgument = _argumentExtractor.ExtractSingleValueArgument(expressionDefinition.LeftParameter, leftContext);

            var rightArgument = _argumentExtractor.ExtractArrayArgument(expressionDefinition.RightParameter, rightContext);

            return _expressionEvaluator.EvaluateLeftSingleValueRightArrayExpression(expressionDefinition, leftArgument, rightArgument);
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
