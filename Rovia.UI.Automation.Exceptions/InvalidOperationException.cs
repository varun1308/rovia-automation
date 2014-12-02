using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
