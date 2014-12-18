using System;
using System.Configuration;
using System.IO;

namespace Rovia.UI.Automation.Logger
{
    class FileLogger:ILogger
    {
        private readonly string _path;

        public FileLogger(string target)
        {
            _path =GetDirectoryPath()+"\\"+ GetFileName(target);
        }
        public void Log(string message)
        {
            File.AppendAllText(_path, message + Environment.NewLine);
        }

        private static string GetFileName(string path)
        {
            var postFix = "_" + DateTime.Now.ToShortDateString().Replace('/', '_') + ".log";
            return (path + postFix);
        }

        private string GetDirectoryPath()
        {
            var directoryPath = ConfigurationManager.AppSettings["application.loggedfilepath"] + "\\Dated_" +
                                        DateTime.Now.ToShortDateString().Replace('/', '_');
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return directoryPath;
        }
    }
}
