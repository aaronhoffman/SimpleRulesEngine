using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleRulesEngine.Exceptions;
using System.Linq;

namespace SimpleRulesEngine.Tests
{
    [TestClass]
    public class ValueExtractorTests
    {
        private ValueExtractor _target;

        [TestInitialize]
        public void BeforeEach()
        {
            _target = new ValueExtractor();
        }

        [TestMethod]
        public void ExtractValue_GivenEmptyPropertyPath_ReturnsContext()
        {
            var context = new object();

            var result = _target.ExtractValue(context, (string)null);

            Assert.AreSame(context, result);
        }

        [TestMethod]
        public void ExtractValue_GivenNullContext_ThrowsException()
        {
            Assert.ThrowsException<SimpleRulesEngineException>(() => _target.ExtractValue(null, "asdf"));
        }

        [TestMethod]
        public void ExtractValue_GivenNullPathArray_ReturnsContext()
        {
            var context = new object();

            var result = _target.ExtractValue(context, (string[])null);

            Assert.AreSame(context, result);
        }

        [TestMethod]
        public void ExtractValue_GivenEmptyPathArray_ReturnsContext()
        {
            var context = new object();

            var result = _target.ExtractValue(context, new string[0]);

            Assert.AreSame(context, result);
        }

        [TestMethod]
        public void ExtractValue_GivenValidPathArray_RetrievesValue()
        {
            var context = CreateTestContext();

            var result01 = _target.ExtractValue(context, "TestClass");

            Assert.AreSame(context.TestClass, result01);

            var result02 = _target.ExtractValue(context, "TestClass.TextProperty");

            Assert.AreSame(context.TestClass.TextProperty, result02);

            var result03 = _target.ExtractValue(context, "TestClass.SubClass.TextProperty");

            Assert.AreSame(context.TestClass.SubClass.TextProperty, result03);
        }

        [TestMethod]
        public void ExtractArray_GivenValueIsString_ThrowsException()
        {
            var context = CreateTestContext();

            Assert.ThrowsException<SimpleRulesEngineException>(() => _target.ExtractArray(context, "TestClass.TextProperty"));
        }

        [TestMethod]
        public void ExtractArray_GivenValueIsNotArray_ThrowsException()
        {
            var context = CreateTestContext();

            Assert.ThrowsException<SimpleRulesEngineException>(() => _target.ExtractArray(context, "TestClass.NumericProperty"));
        }

        [TestMethod]
        public void ExtractArray_GivenValidArray_GivenNoArrayItemPath_ReturnsArray()
        {
            var context = CreateTestContext();

            var result = _target.ExtractArray(context, "TestClass.ArrayOfSimpleType");

            CollectionAssert.AreEquivalent(context.TestClass.ArrayOfSimpleType, result.Cast<decimal>().ToArray());
        }

        [TestMethod]
        public void ExtractArray_GivenValidArray_GivenArrayItemPath_ReturnsCorrectArray()
        {
            var context = CreateTestContext();

            var result = _target.ExtractArray(context, "TestClass.ArrayOfSubClass", "SubClass.TextProperty");

            var expectedResult = context.TestClass.ArrayOfSubClass.Select(x => x.SubClass.TextProperty).ToList();

            CollectionAssert.AreEquivalent(expectedResult, result.Cast<string>().ToList());
        }

        private ValueExtractorTestsContextClass CreateTestContext()
        {
            var context = new ValueExtractorTestsContextClass()
            {
                TestClass = new ValueExtractorTestsClass()
                {
                    TextProperty = "A",
                    NumericProperty = 1,
                    SubClass = new ValueExtractorTestsSubClass()
                    {
                        TextProperty = "B",
                        NumericProperty = 2,
                    },
                    ArrayOfSimpleType = new[] { 3m, 4m, 14m },
                    ArrayOfSubClass = new ValueExtractorTestsArrayItemClass[]
                    {
                        new ValueExtractorTestsArrayItemClass()
                        {
                            TextProperty = "C",
                            NumericProperty = 3,
                            SubClass = new ValueExtractorTestsSubClass()
                            {
                                TextProperty = "D",
                                NumericProperty = 3,
                            }
                        },
                        new ValueExtractorTestsArrayItemClass()
                        {
                            TextProperty = "E",
                            NumericProperty = 4,
                            SubClass = new ValueExtractorTestsSubClass()
                            {
                                TextProperty = "F",
                                NumericProperty = 4,
                            }
                        },
                        new ValueExtractorTestsArrayItemClass()
                        {
                            TextProperty = "G",
                            NumericProperty = 14,
                            SubClass = new ValueExtractorTestsSubClass()
                            {
                                TextProperty = "H",
                                NumericProperty = 14,
                            }
                        },
                    }
                }
            };

            return context;
        }
    }

    public class ValueExtractorTestsContextClass
    {
        public ValueExtractorTestsClass TestClass { get; set; }
    }

    public class ValueExtractorTestsClass
    {
        public string TextProperty { get; set; }
        public decimal NumericProperty { get; set; }
        public ValueExtractorTestsSubClass SubClass { get; set; }
        public decimal[] ArrayOfSimpleType { get; set; }
        public ValueExtractorTestsArrayItemClass[] ArrayOfSubClass { get; set; }
    }

    public class ValueExtractorTestsSubClass
    {
        public string TextProperty { get; set; }
        public decimal NumericProperty { get; set; }
    }

    public class ValueExtractorTestsArrayItemClass
    {
        public string TextProperty { get; set; }
        public decimal NumericProperty { get; set; }
        public ValueExtractorTestsSubClass SubClass { get; set; }
    }
}
