using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SimpleRulesEngine.Exceptions;
using SimpleRulesEngine.Models;
using System.Linq;

namespace SimpleRulesEngine.Tests
{
    [TestClass]
    public class ArgumentExtractorTests
    {
        private Mock<IValueExtractor> _valueExtractorMock;
        private Mock<IArrayAggregator> _arrayAggregatorMock;

        private ArgumentExtractor _target;

        [TestInitialize]
        public void BeforeEach()
        {
            _valueExtractorMock = new Mock<IValueExtractor>();
            _arrayAggregatorMock = new Mock<IArrayAggregator>();

            _target = new ArgumentExtractor(
                _valueExtractorMock.Object,
                _arrayAggregatorMock.Object);
        }


        [TestMethod]
        public void ExtractSingleValueArgument_GivenArrayType_ThrowsException()
        {
            var p = new ParameterDefinition() { ParameterType = ParameterType.Array };

            Assert.ThrowsException<SimpleRulesEngineException>(() => _target.ExtractSingleValueArgument(p));
        }

        [TestMethod]
        public void ExtractSingleValueArgument_GivenBothConstantAndPropertyPath_ThrowsException()
        {
            var p = new ParameterDefinition()
            {
                ConstantValue = "ConstantValue",
                PropertyPath = "PropertyPath",
            };

            Assert.ThrowsException<SimpleRulesEngineException>(() => _target.ExtractSingleValueArgument(p));
        }

        [TestMethod]
        public void ExtractSingleValueArgument_GivenSimpleType_GivenConstantValue_ReturnsValue()
        {
            var p = new ParameterDefinition();

            var textValue = "asdf";
            p.ParameterType = ParameterType.Text;
            p.ConstantValue = textValue;

            var textResult = _target.ExtractSingleValueArgument(p);

            Assert.AreEqual(textValue, textResult);


            var numberValue = 123;
            p.ParameterType = ParameterType.Numeric;
            p.ConstantValue = numberValue;

            var numberResult = _target.ExtractSingleValueArgument(p);

            Assert.AreEqual(numberValue, numberResult);
        }

        [TestMethod]
        public void ExtractSingleValueArgument_GivenSimpleType_GivenPropertyPath_CallsValueExtractorCorrectly()
        {
            var p = new ParameterDefinition();
            p.ParameterType = ParameterType.Text;
            p.PropertyPath = "PropertyPath";

            var context = new object();

            var extractedResult = "result";

            _valueExtractorMock
                .Setup(x => x.ExtractValue(context, p.PropertyPath))
                .Returns(extractedResult);

            var result = _target.ExtractSingleValueArgument(p, context);

            _valueExtractorMock
                .Verify(x => x.ExtractValue(context, p.PropertyPath), Times.Once);

            Assert.AreSame(extractedResult, result);
        }

        [TestMethod]
        public void ExtractSingleValueArgument_GivenArrayAggregation_WithoutAggregationMethod_ThrowsException()
        {
            var p = new ParameterDefinition();
            p.ParameterType = ParameterType.ArrayAggregation;
            p.ArrayAggregationMethod = null;

            Assert.ThrowsException<SimpleRulesEngineException>(() => _target.ExtractSingleValueArgument(p));
        }

        [TestMethod]
        public void ExtractSingleValueArgument_GivenArrayAggregate_GivenConstantValue_PassesArrayToAggregatorCorrectly()
        {
            var p = new ParameterDefinition();

            var array = Enumerable.Empty<object>();

            p.ParameterType = ParameterType.ArrayAggregation;
            p.ArrayAggregationMethod = ArrayAggregationMethod.Avg;
            p.ConstantValue = array;

            var aggregationResult = new object();

            _arrayAggregatorMock
                .Setup(x => x.AggregateArray(array, p.ArrayAggregationMethod.Value))
                .Returns(aggregationResult);

            var result = _target.ExtractSingleValueArgument(p);

            _arrayAggregatorMock
                .Verify(x => x.AggregateArray(array, p.ArrayAggregationMethod.Value), Times.Once);

            Assert.AreSame(aggregationResult, result);
        }

        [TestMethod]
        public void ExtractSingleValueArgument_GivenArrayAggregate_GivenPropertyPath_CallsValueExtractorAndAggregatorCorrectly()
        {
            var p = new ParameterDefinition();

            var array = Enumerable.Empty<object>();

            p.ParameterType = ParameterType.ArrayAggregation;
            p.ArrayAggregationMethod = ArrayAggregationMethod.Avg;
            p.PropertyPath = "PropertyPath";
            p.ArrayAggregationArrayItemPropertyPath = "ArrayAggregationArrayItemPropertyPath";

            var context = new object();

            _valueExtractorMock
                .Setup(x => x.ExtractArray(context, p.PropertyPath, p.ArrayAggregationArrayItemPropertyPath))
                .Returns(array);

            var aggregationResult = new object();

            _arrayAggregatorMock
                .Setup(x => x.AggregateArray(array, p.ArrayAggregationMethod.Value))
                .Returns(aggregationResult);

            var result = _target.ExtractSingleValueArgument(p, context);

            _valueExtractorMock
                .Verify(x => x.ExtractArray(context, p.PropertyPath, p.ArrayAggregationArrayItemPropertyPath), Times.Once);

            _arrayAggregatorMock
                .Verify(x => x.AggregateArray(array, p.ArrayAggregationMethod.Value), Times.Once);

            Assert.AreEqual(aggregationResult, result);
        }


        [TestMethod]
        public void ExtractArrayArgument_GivenSimpleType_ThrowsException()
        {
            var p = new ParameterDefinition();

            p.ParameterType = ParameterType.Text;

            Assert.ThrowsException<SimpleRulesEngineException>(() => _target.ExtractArrayArgument(p));

            p.ParameterType = ParameterType.Numeric;

            Assert.ThrowsException<SimpleRulesEngineException>(() => _target.ExtractArrayArgument(p));

            p.ParameterType = ParameterType.ArrayAggregation;

            Assert.ThrowsException<SimpleRulesEngineException>(() => _target.ExtractArrayArgument(p));
        }

        [TestMethod]
        public void ExtractArrayArgument_GivenBothConstantValueAndPropertyPath_ThrowsException()
        {
            var p = new ParameterDefinition()
            {
                ConstantValue = "ConstantValue",
                PropertyPath = "PropertyPath",
            };

            Assert.ThrowsException<SimpleRulesEngineException>(() => _target.ExtractArrayArgument(p));
        }

        [TestMethod]
        public void ExtractArrayArgument_GivenArrayConstant_ReturnsValue()
        {
            var p = new ParameterDefinition();
            p.ParameterType = ParameterType.Array;

            var array = new object[] { 1, 2, 3 };

            p.ConstantValue = array;

            var result = _target.ExtractArrayArgument(p);

            CollectionAssert.AreEqual(array, (object[])result);
        }

        [TestMethod]
        public void ExtractArrayArgument_GivenPropertyPath_CallsValueExtractorCorrectly()
        {
            var p = new ParameterDefinition();

            p.ParameterType = ParameterType.Array;
            p.PropertyPath = "PropertyPath";
            p.ArrayAggregationArrayItemPropertyPath = "ArrayAggregationArrayItemPropertyPath";

            var context = new object();

            var array = Enumerable.Empty<object>();

            _valueExtractorMock
                .Setup(x => x.ExtractArray(context, p.PropertyPath, p.ArrayAggregationArrayItemPropertyPath))
                .Returns(array);

            var result = _target.ExtractArrayArgument(p, context);

            Assert.AreEqual(array, result);
        }
    }
}