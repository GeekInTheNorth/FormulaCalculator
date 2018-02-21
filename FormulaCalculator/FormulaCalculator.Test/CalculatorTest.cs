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
        public void GivenTwoNumbersToMultiply_ThenTheCombinedValueIsReturned(string expression, double expectedValue)
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

        [Test]
        [TestCase("3+4", 7)]
        [TestCase("7.5+10.5", 18)]
        [TestCase("127.3+10.5+19.75", 157.55)]
        public void GivenTwoOrMoreNumbersToAdd_ThenTheSumIsReturned(string expression, double expectedValue)
        {
            Assert.That(calculator.Calculate(expression), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("4-3", 1)]
        [TestCase("10.5-7.5", 3)]
        public void GivenANumberToSubtractFromALargerNumber_ThenTheRemainderIsReturned(string expression, double expectedValue)
        {
            Assert.That(calculator.Calculate(expression), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("12/6-1", 1)]
        [TestCase("12-6/3", 10)]
        [TestCase("12-(6/3)", 10)]
        [TestCase("(12-6)/3", 2)]
        public void GivenAMixOfSymbols_ThenTheCorrectResultIsReturned(string expression, double expectedValue)
        {
            Assert.That(calculator.Calculate(expression), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("6++3", 9)]
        [TestCase("6--3", 3)]
        [TestCase("6+-3", 3)]
        [TestCase("6+-3", 3)]
        public void GivenIAmSubtractingANegativeNumber_ThenIShouldAddTheAbsoluteValue(string expression, double expectedValue)
        {
            Assert.That(calculator.Calculate(expression), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("-123", -123)]
        [TestCase("-12+2", -10)]
        public void GivenALeadingNegativeValue_ThenTheNumberShouldBeRecognisedAsANegative(string expression, double expectedValue)
        {
            Assert.That(calculator.Calculate(expression), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("2*-10", -20)]
        public void GivenANegativeValueIsMultipliedByAnother_ThenTheCorrectValueShouldBeResolved(string expression, double expectedValue)
        {
            Assert.That(calculator.Calculate(expression), Is.EqualTo(expectedValue));
        }

        [Test]
        [TestCase("(9/4+4/5)/(2/5)*(1/4+7/2)", 28.59375)]
        public void ComplexFormulaExamples(string expression, double expectedValue)
        {
            Assert.That(calculator.Calculate(expression), Is.EqualTo(expectedValue));
        }
    }
}