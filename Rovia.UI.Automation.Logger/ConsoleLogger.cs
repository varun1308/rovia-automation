using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.Logger
{
    class ConsoleLogger:ILogger
    {
        public ConsoleLogger(string target)
        {

        }

        public void Log(string message)
        {
            Console.WriteLine("---------------------------------------------------------------------------------");
            Console.WriteLine(message);
        }
    }
}
