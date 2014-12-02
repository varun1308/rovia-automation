using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.Exceptions
{
    public class PageLoadFailed:BaseException
    {
        public PageLoadFailed(string page)
            : base(page +" failed to load ")
        {

        }

        public PageLoadFailed(string page, Exception innerException)
            : base(page + " failed to load ", innerException)
        {

        }
    }
}
