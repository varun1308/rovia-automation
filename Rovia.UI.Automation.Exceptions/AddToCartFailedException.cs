namespace Rovia.UI.Automation.Exceptions
{
    using System;

    public class AddToCartFailedException:BaseException
    {
        public AddToCartFailedException()
            : base(" No itineries Could be added to cart ")
        {

        }
        public AddToCartFailedException(string msg, Exception exception)
            : base(msg,exception)
        {

        }
    }
}
