using System;
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

            return ConvertToDouble(expression);
        }

        private string ResolveMultiplicationAndDivision(string expression)
        {
            var symbols = new[] { "*", "/" };
            var operatorPosition = GetFirstIndex(expression, symbols);

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
                    else
                        break;
                }

                for (var loop = operatorPosition + 1; loop < expression.Length; loop++)
                {
                    var character = expression[loop].ToString();
                    if (numbers.Contains(character))
                        secondNumber = secondNumber + character;
                    else
                        break;
                }

                var expressionToReplace = string.Format("{0}{1}{2}", firstNumber, expression[operatorPosition], secondNumber);
                var operatorSymbol = expression[operatorPosition];
                var operationResult = EvaluateOperation(ConvertToDouble(firstNumber), ConvertToDouble(secondNumber), operatorSymbol);

                expression = expression.Replace(expressionToReplace, operationResult.ToString("F10"));
                operatorPosition = GetFirstIndex(expression, symbols);
            }

            return expression;
        }

        private int GetFirstIndex(string expression, string[] mathOperators)
        {
            var index = -1;
            foreach(var mathOperator in mathOperators)
            {
                if (expression.Contains(mathOperator) && (index == -1 || expression.IndexOf(mathOperator) < index))
                    index = expression.IndexOf(mathOperator);
            }

            return index;
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
    }
}