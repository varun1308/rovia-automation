using System;

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
