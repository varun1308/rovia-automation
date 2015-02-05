namespace Rovia.UI.Automation.Exceptions
{
    using System;

    public abstract class BaseException:Exception
    {
       
        protected BaseException(string message)
            : base(message)
        {

        }
        protected BaseException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        public override string ToString()
        {
            return Message + (InnerException == null ? "." : "{ " + InnerException + " }");
        }
    }
}
