namespace Rovia.UI.Automation.Exceptions
{
    public class ValidationException:BaseException
    {
        public ValidationException(string param)
            : base(param+" Validation Failed")
        {

        }
    }
}
