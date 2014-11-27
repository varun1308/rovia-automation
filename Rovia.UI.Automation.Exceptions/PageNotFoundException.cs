using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.Exceptions
{
    public class PageNotFoundException : Exception
    {
        public PageNotFoundException(string page)
            : base("Page not available : " +page )
        {
        }
        public PageNotFoundException(string page,Exception innerException)
            : base("Page not available : " + page, innerException)
        {
        }
    }
}
