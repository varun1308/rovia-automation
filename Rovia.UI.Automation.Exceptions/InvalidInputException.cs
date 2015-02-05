namespace Rovia.UI.Automation.Exceptions
{
    using System;

    public class InvalidInputException:BaseException
    {
        public InvalidInputException(string param)
            : this(param, null)
        {
            
        }

        public InvalidInputException(string param,Exception innerException)
            : base("invalid input  " + param,innerException)
        {

        }
    }
}
