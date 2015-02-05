namespace Rovia.UI.Automation.Logger
{
    using System;
    /// <summary>
    /// Write logs to console
    /// </summary>
    class ConsoleLogger:ILogger
    {
        public ConsoleLogger(string target)
        {

        }

        /// <summary>
        /// Write log message on console
        /// </summary>
        /// <param name="message">Message</param>
        public void Log(string message)
        {
            Console.WriteLine("---------------------------------------------------------------------------------");
            Console.WriteLine(message);
        }
    }
}
