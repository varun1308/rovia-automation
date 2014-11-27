using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.Exceptions
{
    public class ResultsNotFoundException : Exception
    {
        public ResultsNotFoundException()
            : base("Results not found")
        {
        }
        public ResultsNotFoundException(Exception exception)
            : base("Results not found", exception)
        {
        }
    }
}
