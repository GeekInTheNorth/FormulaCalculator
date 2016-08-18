using FormulaCalculator.Exceptions;
using NUnit.Framework;

namespace FormulaCalculator.Test
{
    [TestFixture]
    public class CalculatorTest
    {
        private Calculator calculator;

        [SetUp]
        public void SetUp()
        {
            calculator = new Calculator();
        }

        [Test]
        public void GivenABlankExpressionIsProvided_ThenAnArgumentExceptionShouldBeThrown()
        {
            Assert.That(() => calculator.Calculate(string.Empty), Throws.ArgumentException);
        }

        [Test]
        public void GivenANullExpressionIsProvided_ThenAnArgumentExceptionShouldBeThrown()
        {
            Assert.That(() => calculator.Calculate(null), Throws.ArgumentException);
        }

        [Test]
        public void GivenASetOfUnmatchingBraces_ThenAnInvalidFormulaExceptionShouldBeThrown()
        {
            Assert.Throws<InvalidFormulaException>(() => calculator.Calculate("("));
        }

        [Test]
        public void GivenASetOfBracesWithNoContent_ThenAnInvalidFormulaExceptionShouldBeThrown()
        {
            Assert.Throws<InvalidFormulaException>(() => calculator.Calculate("()"));
        }

        [Test]
        [TestCase("(5)", 5)]
        [TestCase("(123.456)", 123.456)]
        public void GivenASetOfBracesContainsASingleNumber_ThenTheContainedValueIsReturned(string expression, double expectedValue)
        {
            Assert.That(calculator.Calculate(expression), Is.EqualTo(expectedValue));
        }
    }
}