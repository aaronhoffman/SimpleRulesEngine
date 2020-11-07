using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleRulesEngine.Models;

namespace SimpleRulesEngine.Tests
{
    [TestClass]
    public class EvaluationServiceTests
    {
        private Mock<IArgumentExtractor> _argumentExtractorMock;
        private Mock<IExpressionEvaluator> _expressionEvaluatorMock;

        private EvaluationService _target;

        [TestInitialize]
        public void BeforeEach()
        {
            _argumentExtractorMock = new Mock<IArgumentExtractor>();
            _expressionEvaluatorMock = new Mock<IExpressionEvaluator>();

            _target = new EvaluationService(
                _argumentExtractorMock.Object,
                _expressionEvaluatorMock.Object);
        }

        [TestMethod]
        public void Evaluate_GivenBothSingleValue_CallsDependenciesAppropriately()
        {
            var leftContext = new object();
            var rightContext = new object();

            var ed = new ExpressionDefinition()
            {
                LeftParameter = new ParameterDefinition()
                {
                    ParameterType = ParameterType.Numeric,
                },
                RightParameter = new ParameterDefinition()
                {
                    ParameterType = ParameterType.Numeric,
                },
            };

            var leftArg = 1;
            var rightArg = 2;

            _argumentExtractorMock
                .Setup(x => x.ExtractSingleValueArgument(ed.LeftParameter, leftContext))
                .Returns(leftArg);

            _argumentExtractorMock
                .Setup(x => x.ExtractSingleValueArgument(ed.RightParameter, rightContext))
                .Returns(rightArg);

            _expressionEvaluatorMock
                .Setup(x => x.EvaluateLeftSingleValueRightSingleValueExpression(ed, leftArg, rightArg))
                .Returns(true);

            _target.Evaluate(ed, leftContext, rightContext);

            _argumentExtractorMock.Verify(x => x.ExtractSingleValueArgument(ed.LeftParameter, leftContext), Times.Once);
            _argumentExtractorMock.Verify(x => x.ExtractSingleValueArgument(ed.RightParameter, rightContext), Times.Once);
            _expressionEvaluatorMock.Verify(x => x.EvaluateLeftSingleValueRightSingleValueExpression(ed, leftArg, rightArg), Times.Once);
        }

        [TestMethod]
        public void Evaluate_GivenBothArrays_CallsDependenciesAppropriately()
        {
            var leftContext = new object();
            var rightContext = new object();

            var ed = new ExpressionDefinition()
            {
                LeftParameter = new ParameterDefinition()
                {
                    ParameterType = ParameterType.Array,
                },
                RightParameter = new ParameterDefinition()
                {
                    ParameterType = ParameterType.Array,
                },
                ArrayComparison = ArrayComparison.ContainsAll,
            };

            var leftArg = new object[] { 1 };
            var rightArg = new object[] { 1 };

            _argumentExtractorMock
                .Setup(x => x.ExtractArrayArgument(ed.LeftParameter, leftContext))
                .Returns(leftArg);

            _argumentExtractorMock
                .Setup(x => x.ExtractArrayArgument(ed.RightParameter, rightContext))
                .Returns(rightArg);

            _expressionEvaluatorMock
                .Setup(x => x.EvaluateLeftArrayRightArrayExpression(ed, leftArg, rightArg))
                .Returns(true);

            _target.Evaluate(ed, leftContext, rightContext);

            _argumentExtractorMock.Verify(x => x.ExtractArrayArgument(ed.LeftParameter, leftContext), Times.Once);
            _argumentExtractorMock.Verify(x => x.ExtractArrayArgument(ed.RightParameter, rightContext), Times.Once);
            _expressionEvaluatorMock.Verify(x => x.EvaluateLeftArrayRightArrayExpression(ed, leftArg, rightArg), Times.Once);
        }

        [TestMethod]
        public void Evaluate_GivenLeftArray_CallsDependenciesAppropriately()
        {
            var leftContext = new object();
            var rightContext = new object();

            var ed = new ExpressionDefinition()
            {
                LeftParameter = new ParameterDefinition()
                {
                    ParameterType = ParameterType.Array,
                },
                RightParameter = new ParameterDefinition()
                {
                    ParameterType = ParameterType.Numeric,
                },
            };

            var leftArg = new object[] { 1 };
            var rightArg = 1;

            _argumentExtractorMock
                .Setup(x => x.ExtractArrayArgument(ed.LeftParameter, leftContext))
                .Returns(leftArg);

            _argumentExtractorMock
                .Setup(x => x.ExtractSingleValueArgument(ed.RightParameter, rightContext))
                .Returns(rightArg);

            _expressionEvaluatorMock
                .Setup(x => x.EvaluateLeftArrayRightSingleValueExpression(ed, leftArg, rightArg))
                .Returns(true);

            _target.Evaluate(ed, leftContext, rightContext);

            _argumentExtractorMock.Verify(x => x.ExtractArrayArgument(ed.LeftParameter, leftContext), Times.Once);
            _argumentExtractorMock.Verify(x => x.ExtractSingleValueArgument(ed.RightParameter, rightContext), Times.Once);
            _expressionEvaluatorMock.Verify(x => x.EvaluateLeftArrayRightSingleValueExpression(ed, leftArg, rightArg), Times.Once);
        }

        [TestMethod]
        public void Evaluate_GivenRightArray_CallsDependenciesAppropriately()
        {
            var leftContext = new object();
            var rightContext = new object();

            var ed = new ExpressionDefinition()
            {
                LeftParameter = new ParameterDefinition()
                {
                    ParameterType = ParameterType.Numeric,
                },
                RightParameter = new ParameterDefinition()
                {
                    ParameterType = ParameterType.Array,
                },
            };

            var leftArg = 1;
            var rightArg = new object[] { 1 };

            _argumentExtractorMock
                .Setup(x => x.ExtractSingleValueArgument(ed.LeftParameter, leftContext))
                .Returns(leftArg);

            _argumentExtractorMock
                .Setup(x => x.ExtractArrayArgument(ed.RightParameter, rightContext))
                .Returns(rightArg);

            _expressionEvaluatorMock
                .Setup(x => x.EvaluateLeftSingleValueRightArrayExpression(ed, leftArg, rightArg))
                .Returns(true);

            _target.Evaluate(ed, leftContext, rightContext);

            _argumentExtractorMock.Verify(x => x.ExtractSingleValueArgument(ed.LeftParameter, leftContext), Times.Once);
            _argumentExtractorMock.Verify(x => x.ExtractArrayArgument(ed.RightParameter, rightContext), Times.Once);
            _expressionEvaluatorMock.Verify(x => x.EvaluateLeftSingleValueRightArrayExpression(ed, leftArg, rightArg), Times.Once);
        }
    }
}
