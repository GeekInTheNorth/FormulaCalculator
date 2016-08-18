using System;
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

        [Test]
        [TestCase("2*10", 20)]
        [TestCase("1.25*5.75", 7.1875)]
        public void GivenTwoNumbersToMultiple_ThenTheCombinedValueIsReturned(string expression, double expectedValue)
        {
            Assert.That(calculator.Calculate(expression), Is.EqualTo(expectedValue));
        }

        [Test]
        public void GivenTwoNumbersToDivide_AndTheSecondNumberIsZero_ThenADivideByZeroExceptionShouldBeThrown()
        {
            Assert.Throws<DivideByZeroException>(() => calculator.Calculate("1/0"));
        }

        [Test]
        [TestCase("2/10", 0.2)]
        [TestCase("5.75/1.25", 4.6)]
        public void GivenTwoNumbersToDivide_ThenTheDividedValueIsReturned(string expression, double expectedValue)
        {
            Assert.That(calculator.Calculate(expression), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("10/2*5", 25)]
        [TestCase("10*2/5", 4)]
        [TestCase("17.53/3.2*5.61", 30.73228125)]
        public void GivenAMixOfMultiplicationAndDivision_ThenTheResultShouldBeCalculatedFromLeftToRightIrrespectiveOfOperator(string expression, double expectedValue)
        {
            Assert.That(calculator.Calculate(expression), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("(10/2)*5", 25)]
        [TestCase("10/(2*5)", 1)]
        public void GivenAMixOfBracesMultiplicationAndDivision_ThenBracesShouldBeResolvedBeforeOperators(string expression, double expectedValue)
        {
            Assert.That(calculator.Calculate(expression), Is.EqualTo(expectedValue));
        }
    }
}