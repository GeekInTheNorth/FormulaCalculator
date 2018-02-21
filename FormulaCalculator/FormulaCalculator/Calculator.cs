using System;
using System.Linq;
using FormulaCalculator.Exceptions;

namespace FormulaCalculator
{
    public class Calculator
    {
        public double Calculate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                throw new ArgumentException("An expression must be provided", "expression");

            return EvaluateExpression(expression);
        }

        private double EvaluateExpression(string expression)
        {
            while (expression.Contains("("))
            {
                var openingBracePosition = expression.IndexOf('(');
                var closingBracePosition = GetClosingBracePosition(expression, openingBracePosition);

                var innerExpressionStart = openingBracePosition + 1;
                var innerExpressionEnd = closingBracePosition - 1;
                var innerExpressionLength = innerExpressionEnd - innerExpressionStart + 1;

                if (innerExpressionLength <= 0)
                    throw new InvalidFormulaException("No expression contained in braces");

                var innerExpression = expression.Substring(innerExpressionStart, innerExpressionLength);

                var evaluateResult = EvaluateExpression(innerExpression);
                var expressionToReplace = string.Format("({0})", innerExpression);

                expression = expression.Replace(expressionToReplace, evaluateResult.ToString("F10"));
            }

            expression = ResolveMultiplicationAndDivision(expression);
            expression = ResolveAdditionAndSubtraction(expression);

            return ConvertToDouble(expression);
        }

        private string ResolveMultiplicationAndDivision(string expression)
        {
            var symbols = new[] { "*", "/" };

            return ResolveOperations(symbols, expression);
        }

        private string ResolveAdditionAndSubtraction(string expression)
        {
            var symbols = new[] { "+", "-" };

            return ResolveOperations(symbols, expression);
        }

        private string ResolveOperations(string[] mathOperators, string expression)
        {
            expression = ResolveDoubleSymbols(expression);
            var operatorPosition = GetFirstIndex(expression, mathOperators);

            while (operatorPosition != -1)
            {
                var numbers = "0123456789.";
                var firstNumber = string.Empty;
                var secondNumber = string.Empty;

                for (var loop = operatorPosition - 1; loop >= 0; loop--)
                {
                    var character = expression[loop].ToString();
                    if (numbers.Contains(character))
                        firstNumber = character + firstNumber;
                    else if (loop == 0 && character.Equals("-"))
                        firstNumber = character + firstNumber;
                    else
                        break;
                }

                for (var loop = operatorPosition + 1; loop < expression.Length; loop++)
                {
                    var character = expression[loop].ToString();
                    if (numbers.Contains(character))
                        secondNumber = secondNumber + character;
                    else if (loop == (operatorPosition + 1) && character.Equals("-"))
                        secondNumber = character + secondNumber;
                    else
                        break;
                }

                var expressionToReplace = string.Format("{0}{1}{2}", firstNumber, expression[operatorPosition], secondNumber);
                var operatorSymbol = expression[operatorPosition];
                var operationResult = EvaluateOperation(ConvertToDouble(firstNumber), ConvertToDouble(secondNumber), operatorSymbol);

                expression = expression.Replace(expressionToReplace, operationResult.ToString("F10"));
                expression = ResolveDoubleSymbols(expression);
                operatorPosition = GetFirstIndex(expression, mathOperators);
            }

            return expression;
        }

        private int GetFirstIndex(string expression, string[] mathOperators)
        {
            for (var index = 1; index < expression.Length; index++)
            {
                if (Array.IndexOf(mathOperators, expression.Substring(index, 1)) > -1)
                    return index;
            }

            return -1;
        }

        private int GetClosingBracePosition(string expression, int openingBracePosition)
        {
            var openBraces = 0;
            var closingBraces = 0;
            for (var loop = openingBracePosition; loop < expression.Length; loop++)
            {
                if (expression[loop] == '(') openBraces++;
                if (expression[loop] == ')') closingBraces++;

                if (openBraces == closingBraces)
                    return loop;
            }

            return 0;
        }

        private double EvaluateOperation(double firstNumber, double secondNumber, char operatorSymbol)
        {
            switch (operatorSymbol)
            {
                case '*':
                    return firstNumber * secondNumber;
                case '/':
                    if (secondNumber == 0)
                        throw new DivideByZeroException();
                    return firstNumber / secondNumber;
                case '+':
                    return firstNumber + secondNumber;
                case '-':
                    return firstNumber - secondNumber;
                default:
                    return 0;
            }
        }

        private double ConvertToDouble(string value)
        {
            double outputValue = 0;

            if (!double.TryParse(value, out outputValue))
                throw new FormulaEvaluationException("Unable to evaluate formula");

            return outputValue;
        }

        /// <summary>
        /// Two 'pluses' make a plus, two 'minuses' make a plus. A plus and a minus make a minus.
        /// </summary>
        private string ResolveDoubleSymbols(string expression)
        {
            while (expression.Contains("++")) expression = expression.Replace("++", "+");
            while (expression.Contains("--")) expression = expression.Replace("--", "-");
            while (expression.Contains("+-")) expression = expression.Replace("+-", "-");
            while (expression.Contains("-+")) expression = expression.Replace("-+", "-");

            return expression;
        }
    }
}