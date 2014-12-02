using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.Exceptions
{
    public class UIElementNullOrNotVisible:BaseException
    {
       
        public UIElementNullOrNotVisible(string param)
            : base(param+" is Null or Not Visible")
        {

        }
        public UIElementNullOrNotVisible(string param, Exception innerException)
            : base(param + " is Null or Not Visible", innerException)
        {

        }
    }
}
