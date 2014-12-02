using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
