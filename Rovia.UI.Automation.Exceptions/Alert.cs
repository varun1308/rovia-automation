using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.Exceptions
{
    public class Alert:BaseException
    {
        public Alert(string message) : base(message)
        {
        }
    }
}
