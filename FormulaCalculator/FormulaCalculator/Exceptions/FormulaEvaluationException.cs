using System;

namespace FormulaCalculator.Exceptions
{
    public class FormulaEvaluationException : Exception
    {
        public FormulaEvaluationException(string message) : base(message)
        {
        }
    }
}
