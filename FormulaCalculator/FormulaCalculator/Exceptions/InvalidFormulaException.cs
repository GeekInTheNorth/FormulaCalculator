using System;

namespace FormulaCalculator.Exceptions
{
    public class InvalidFormulaException : Exception
    {
        public InvalidFormulaException(string message) : base(message)
        {
        }
    }
}
