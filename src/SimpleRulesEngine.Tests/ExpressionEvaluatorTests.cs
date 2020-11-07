using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRulesEngine.Models;
using System.Collections.Generic;

namespace SimpleRulesEngine.Tests
{
    [TestClass]
    public class ExpressionEvaluatorTests
    {
        private ExpressionEvaluator _target;

        [TestInitialize]
        public void BeforeEach()
        {
            _target = new ExpressionEvaluator();
        }

        [TestMethod]
        public void Evaluator_GivenLeftSingleValueAndRightSingleValue_EvaluatesCorrectly()
        {
            var ed = new ExpressionDefinition();

            // text
            TestHelper(ed, TextComparison.Equal, "A", "A", true);
            TestHelper(ed, TextComparison.Equal, "A", "B", false);
            TestHelper(ed, TextComparison.NotEqual, "A", "B", true);
            TestHelper(ed, TextComparison.NotEqual, "A", "A", false);
            TestHelper(ed, TextComparison.StartsWith, "ABC", "A", true);
            TestHelper(ed, TextComparison.StartsWith, "ABC", "C", false);
            TestHelper(ed, TextComparison.EndsWith, "ABC", "C", true);
            TestHelper(ed, TextComparison.EndsWith, "ABC", "A", false);
            TestHelper(ed, TextComparison.Contains, "ABC", "B", true);
            TestHelper(ed, TextComparison.Contains, "ABC", "D", false);

            ed.TextComparison = null;

            // numeric
            TestHelper(ed, NumericComparison.Equal, 1, 1, true);
            TestHelper(ed, NumericComparison.Equal, 1, 2, false);
            TestHelper(ed, NumericComparison.NotEqual, 1, 2, true);
            TestHelper(ed, NumericComparison.NotEqual, 1, 1, false);
            TestHelper(ed, NumericComparison.GreaterThan, 1, 0, true);
            TestHelper(ed, NumericComparison.GreaterThan, 1, 1, false);
            TestHelper(ed, NumericComparison.GreaterThan, 1, 2, false);
            TestHelper(ed, NumericComparison.GreaterThanOrEqual, 1, 0, true);
            TestHelper(ed, NumericComparison.GreaterThanOrEqual, 1, 1, true);
            TestHelper(ed, NumericComparison.GreaterThanOrEqual, 1, 2, false);
            TestHelper(ed, NumericComparison.LessThan, 1, 2, true);
            TestHelper(ed, NumericComparison.LessThan, 1, 1, false);
            TestHelper(ed, NumericComparison.LessThan, 1, 0, false);
            TestHelper(ed, NumericComparison.LessThanOrEqual, 1, 2, true);
            TestHelper(ed, NumericComparison.LessThanOrEqual, 1, 1, true);
            TestHelper(ed, NumericComparison.LessThanOrEqual, 1, 0, false);

            // enum
            TestHelper(ed, NumericComparison.Equal, TestEnum.Value1, 1, true);
            TestHelper(ed, NumericComparison.Equal, TestEnum.Value1, 2, false);
            TestHelper(ed, NumericComparison.NotEqual, TestEnum.Value1, 2, true);
            TestHelper(ed, NumericComparison.NotEqual, TestEnum.Value1, 1, false);
        }

        [TestMethod]
        public void Evaluator_GivenLeftSingleValueAndRightArray_EvaluatesCorrectly()
        {
            var ed = new ExpressionDefinition();

            var na = new object[] { 1, 2, 3 };
            TestHelper(ed, ArrayComparison.IsAny, 1, na, true);
            TestHelper(ed, ArrayComparison.IsAny, 4, na, false);
            TestHelper(ed, ArrayComparison.IsNotAny, 4, na, true);
            TestHelper(ed, ArrayComparison.IsNotAny, 1, na, false);

            var ta = new object[] { "A", "B", "C" };
            TestHelper(ed, ArrayComparison.IsAny, "A", ta, true);
            TestHelper(ed, ArrayComparison.IsAny, "D", ta, false);
            TestHelper(ed, ArrayComparison.IsNotAny, "D", ta, true);
            TestHelper(ed, ArrayComparison.IsNotAny, "A", ta, false);

            // the text constant "A" is not in the numeric array [ 1, 2, 3 ]
            TestHelper(ed, ArrayComparison.IsNotAny, "A", na, true);

            // the text constant "1" is not in the numeric array [ 1, 2, 3 ]
            TestHelper(ed, ArrayComparison.IsNotAny, "1", na, true);

            // the numeric constant 1 is not in the text array [ "A", "B", "C" ]
            TestHelper(ed, ArrayComparison.IsNotAny, 1, ta, true);

            // enum to enum
            var ea = new object[] { TestEnum.Value1, TestEnum.Value2 };
            TestHelper(ed, ArrayComparison.IsAny, TestEnum.Value1, ea, true);
            TestHelper(ed, ArrayComparison.IsAny, TestEnum.Value3, ea, false);
            TestHelper(ed, ArrayComparison.IsNotAny, TestEnum.Value3, ea, true);
            TestHelper(ed, ArrayComparison.IsNotAny, TestEnum.Value1, ea, false);
        }

        [TestMethod]
        public void Evaluator_GivenLeftArrayAndRightSingleValue_EvaluatesCorrectly()
        {
            var ed = new ExpressionDefinition();

            var a1 = new object[] { 1, 1, 1 };
            ed.ExpressionAggregationMethod = ExpressionAggregationMethod.All;
            TestHelper(ed, NumericComparison.Equal, a1, 1, true);
            TestHelper(ed, NumericComparison.Equal, a1, 2, false);
            TestHelper(ed, NumericComparison.NotEqual, a1, 2, true);
            TestHelper(ed, NumericComparison.NotEqual, a1, 1, false);
            TestHelper(ed, NumericComparison.GreaterThan, a1, 0, true);
            TestHelper(ed, NumericComparison.GreaterThan, a1, 1, false);
            TestHelper(ed, NumericComparison.GreaterThan, a1, 2, false);
            TestHelper(ed, NumericComparison.GreaterThanOrEqual, a1, 0, true);
            TestHelper(ed, NumericComparison.GreaterThanOrEqual, a1, 1, true);
            TestHelper(ed, NumericComparison.GreaterThanOrEqual, a1, 2, false);
            TestHelper(ed, NumericComparison.LessThan, a1, 2, true);
            TestHelper(ed, NumericComparison.LessThan, a1, 1, false);
            TestHelper(ed, NumericComparison.LessThan, a1, 0, false);
            TestHelper(ed, NumericComparison.LessThanOrEqual, a1, 2, true);
            TestHelper(ed, NumericComparison.LessThanOrEqual, a1, 1, true);
            TestHelper(ed, NumericComparison.LessThanOrEqual, a1, 0, false);

            var a2 = new object[] { 1, 2, 3 };
            ed.ExpressionAggregationMethod = ExpressionAggregationMethod.Any;
            TestHelper(ed, NumericComparison.Equal, a2, 1, true);
            TestHelper(ed, NumericComparison.Equal, a2, 4, false);
            TestHelper(ed, NumericComparison.NotEqual, a2, 4, true);
            TestHelper(ed, NumericComparison.NotEqual, new object[] { 1 }, 1, false);
            TestHelper(ed, NumericComparison.GreaterThan, a2, 0, true);
            TestHelper(ed, NumericComparison.GreaterThan, a2, 1, true);
            TestHelper(ed, NumericComparison.GreaterThan, a2, 3, false);
            TestHelper(ed, NumericComparison.GreaterThan, a2, 4, false);
            TestHelper(ed, NumericComparison.GreaterThanOrEqual, a2, 0, true);
            TestHelper(ed, NumericComparison.GreaterThanOrEqual, a2, 1, true);
            TestHelper(ed, NumericComparison.GreaterThanOrEqual, a2, 3, true);
            TestHelper(ed, NumericComparison.GreaterThanOrEqual, a2, 4, false);
            TestHelper(ed, NumericComparison.LessThan, a2, 4, true);
            TestHelper(ed, NumericComparison.LessThan, a2, 3, true);
            TestHelper(ed, NumericComparison.LessThan, a2, 1, false);
            TestHelper(ed, NumericComparison.LessThan, a2, 0, false);
            TestHelper(ed, NumericComparison.LessThanOrEqual, a2, 4, true);
            TestHelper(ed, NumericComparison.LessThanOrEqual, a2, 3, true);
            TestHelper(ed, NumericComparison.LessThanOrEqual, a2, 1, true);
            TestHelper(ed, NumericComparison.LessThanOrEqual, a2, 0, false);

            // clear NumericComparison for Text tests
            ed.NumericComparison = null;

            var aa1 = new object[] { "ABC", "ABC", "ABC" };
            ed.ExpressionAggregationMethod = ExpressionAggregationMethod.All;
            TestHelper(ed, TextComparison.Equal, aa1, "ABC", true);
            TestHelper(ed, TextComparison.Equal, aa1, "DEF", false);
            TestHelper(ed, TextComparison.NotEqual, aa1, "DEF", true);
            TestHelper(ed, TextComparison.NotEqual, aa1, "ABC", false);
            TestHelper(ed, TextComparison.StartsWith, aa1, "A", true);
            TestHelper(ed, TextComparison.StartsWith, aa1, "C", false);
            TestHelper(ed, TextComparison.EndsWith, aa1, "C", true);
            TestHelper(ed, TextComparison.EndsWith, aa1, "A", false);
            TestHelper(ed, TextComparison.Contains, aa1, "B", true);
            TestHelper(ed, TextComparison.Contains, aa1, "D", false);

            var aa2 = new object[] { "ABC", "DEF", "GHI" };
            ed.ExpressionAggregationMethod = ExpressionAggregationMethod.Any;
            TestHelper(ed, TextComparison.Equal, aa2, "ABC", true);
            TestHelper(ed, TextComparison.Equal, aa2, "XYZ", false);
            TestHelper(ed, TextComparison.NotEqual, aa2, "ABC", true);
            TestHelper(ed, TextComparison.NotEqual, new object[] { "ABC" }, "ABC", false);
            TestHelper(ed, TextComparison.StartsWith, aa2, "A", true);
            TestHelper(ed, TextComparison.StartsWith, aa2, "X", false);
            TestHelper(ed, TextComparison.EndsWith, aa2, "C", true);
            TestHelper(ed, TextComparison.EndsWith, aa2, "Z", false);
            TestHelper(ed, TextComparison.Contains, aa2, "B", true);
            TestHelper(ed, TextComparison.Contains, aa2, "Y", false);
        }

        [TestMethod]
        public void Evaluator_GivenLeftArrayAndRightArray_EvaluatesCorrectly()
        {
            var ed = new ExpressionDefinition();
            var nla = new object[] { 1, 2, 3 };
            var nra1 = new object[] { 1, 2, 3 };
            var nra2 = new object[] { 1, 2 };
            var nra3 = new object[] { 4, 5, 6 };
            var nra4 = new object[] { 1, 2, 4 };

            TestHelper(ed, ArrayComparison.ContainsAny, nla, nra1, true);
            TestHelper(ed, ArrayComparison.ContainsAny, nla, nra2, true);
            TestHelper(ed, ArrayComparison.ContainsAny, nla, nra3, false);
            TestHelper(ed, ArrayComparison.DoesNotContainAny, nla, nra3, true);
            TestHelper(ed, ArrayComparison.DoesNotContainAny, nla, nra2, false);
            TestHelper(ed, ArrayComparison.DoesNotContainAny, nla, nra1, false);
            TestHelper(ed, ArrayComparison.ContainsAll, nla, nra1, true);
            TestHelper(ed, ArrayComparison.ContainsAll, nla, nra2, true);
            TestHelper(ed, ArrayComparison.ContainsAll, nla, nra3, false);
            TestHelper(ed, ArrayComparison.ContainsAll, nla, nra4, false);

            var ala = new object[] { "A", "B", "C" };
            var ara1 = new object[] { "A", "B", "C" };
            var ara2 = new object[] { "A", "B" };
            var ara3 = new object[] { "D", "E", "F" };
            var ara4 = new object[] { "A", "B", "D" };

            TestHelper(ed, ArrayComparison.ContainsAny, ala, ara1, true);
            TestHelper(ed, ArrayComparison.ContainsAny, ala, ara2, true);
            TestHelper(ed, ArrayComparison.ContainsAny, ala, ara3, false);
            TestHelper(ed, ArrayComparison.DoesNotContainAny, ala, ara3, true);
            TestHelper(ed, ArrayComparison.DoesNotContainAny, ala, ara2, false);
            TestHelper(ed, ArrayComparison.DoesNotContainAny, ala, ara1, false);
            TestHelper(ed, ArrayComparison.ContainsAll, ala, ara1, true);
            TestHelper(ed, ArrayComparison.ContainsAll, ala, ara2, true);
            TestHelper(ed, ArrayComparison.ContainsAll, ala, ara3, false);
            TestHelper(ed, ArrayComparison.ContainsAll, ala, ara4, false);
        }

        private void TestHelper(ExpressionDefinition ed, NumericComparison numericComparison, object leftArgument, object rightArgument, bool expectedResult)
        {
            ed.ArrayComparison = null;
            ed.TextComparison = null;
            ed.NumericComparison = numericComparison;
            Assert.AreEqual(expectedResult, _target.EvaluateLeftSingleValueRightSingleValueExpression(ed, leftArgument, rightArgument));
        }

        private void TestHelper(ExpressionDefinition ed, TextComparison textComparison, object leftArgument, object rightArgument, bool expectedResult)
        {
            ed.ArrayComparison = null;
            ed.NumericComparison = null;
            ed.TextComparison = textComparison;
            Assert.AreEqual(expectedResult, _target.EvaluateLeftSingleValueRightSingleValueExpression(ed, leftArgument, rightArgument));
        }

        private void TestHelper(ExpressionDefinition ed, NumericComparison numericComparison, IEnumerable<object> leftArgument, object rightArgument, bool expectedResult)
        {
            ed.NumericComparison = numericComparison;
            Assert.AreEqual(expectedResult, _target.EvaluateLeftArrayRightSingleValueExpression(ed, leftArgument, rightArgument));
        }

        private void TestHelper(ExpressionDefinition ed, TextComparison textComparison, IEnumerable<object> leftArgument, object rightArgument, bool expectedResult)
        {
            ed.TextComparison = textComparison;
            Assert.AreEqual(expectedResult, _target.EvaluateLeftArrayRightSingleValueExpression(ed, leftArgument, rightArgument));
        }

        private void TestHelper(ExpressionDefinition ed, ArrayComparison arrayComparison, object leftArgument, IEnumerable<object> rightArgument, bool expectedResult)
        {
            ed.ArrayComparison = arrayComparison;
            Assert.AreEqual(expectedResult, _target.EvaluateLeftSingleValueRightArrayExpression(ed, leftArgument, rightArgument));
        }

        private void TestHelper(ExpressionDefinition ed, ArrayComparison arrayComparison, IEnumerable<object> leftArgument, IEnumerable<object> rightArgument, bool expectedResult)
        {
            ed.ArrayComparison = arrayComparison;
            Assert.AreEqual(expectedResult, _target.EvaluateLeftArrayRightArrayExpression(ed, leftArgument, rightArgument));
        }
    }

    public enum TestEnum
    {
        NotSet = 0,
        Value1 = 1,
        Value2 = 2,
        Value3 = 3,
    }
}
