using System;
using System.IO;

namespace Rovia.UI.Automation.Logger
{
    class FileLogger:ILogger
    {
        private readonly string _path;

        public FileLogger(string target)
        {
            _path = GetFileName(target);
        }
        public void Log(string message)
        {
            File.AppendAllText(_path, message + Environment.NewLine);
        }

        private static string GetFileName(string path)
        {
            var postFix = "_" + DateTime.Now.ToString().Replace(' ', '_').Replace(':', '-').Replace('/', '-') + ".log";
            return (path + postFix);
        }
    }
}
