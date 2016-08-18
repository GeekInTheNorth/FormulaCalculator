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

            double returnValue = 0;
            if (!double.TryParse(expression, out returnValue))
                throw new FormulaEvaluationException("Unable to evaluate formula");

            return returnValue;
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
    }
}