using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.Exceptions
{
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
