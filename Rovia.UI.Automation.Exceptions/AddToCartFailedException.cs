using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.Exceptions
{
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
