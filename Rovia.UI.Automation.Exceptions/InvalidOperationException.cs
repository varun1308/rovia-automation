namespace Rovia.UI.Automation.Exceptions
{
    public class InvalidOperationException:BaseException
    {
       
        public InvalidOperationException(string operation, string page)
            : this(operation+" on "+page)
        {

        }
        public InvalidOperationException(string param)
            : base(param+" is not a valid operation")
        {

        }
    }
}
