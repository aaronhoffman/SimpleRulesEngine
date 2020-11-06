using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRulesEngine.Models;

namespace SimpleRulesEngine.Tests
{
    [TestClass]
    public class ArrayAggregatorTests
    {
        private ArrayAggregator _target;

        [TestInitialize]
        public void BeforeEach()
        {
            _target = new ArrayAggregator();
        }

        [TestMethod]
        public void AggregateArray_GivenArray_AggregatesCorrectly()
        {
            var array = new object[] { 3, 4, 14 };


            var aggregateResult = _target.AggregateArray(array, ArrayAggregationMethod.Avg);

            Assert.AreEqual(7m, aggregateResult);


            aggregateResult = _target.AggregateArray(array, ArrayAggregationMethod.Max);

            Assert.AreEqual(14m, aggregateResult);


            aggregateResult = _target.AggregateArray(array, ArrayAggregationMethod.Min);

            Assert.AreEqual(3m, aggregateResult);


            aggregateResult = _target.AggregateArray(array, ArrayAggregationMethod.Sum);

            Assert.AreEqual(21m, aggregateResult);
        }
    }
}
