namespace Rovia.UI.Automation.Logger
{
    using System;
    using System.Configuration;
    using System.IO;

    /// <summary>
    /// Log custom messages to files
    /// </summary>
    class FileLogger:ILogger
    {
        #region Private fields
        private readonly string _path;
        #endregion

        #region Private Members

        private static string GetFileName(string path)
        {
            var postFix = "_" + DateTime.Now.ToShortDateString().Replace('/', '_') + ".log";
            return (path + postFix);
        }

        private static string GetDirectoryPath()
        {
            var directoryPath = ConfigurationManager.AppSettings["application.loggedfilepath"] + "\\Dated_" +
                                        DateTime.Now.ToShortDateString().Replace('/', '_');
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return directoryPath;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Creates log file on target directory location
        /// </summary>
        /// <param name="target">target directory path</param>
        public FileLogger(string target)
        {
            _path =GetDirectoryPath()+"\\"+ GetFileName(target);
        }

        /// <summary>
        /// Append message to log file
        /// </summary>
        /// <param name="message">Message</param>
        public void Log(string message)
        {
            File.AppendAllText(_path, message + Environment.NewLine);
        }

        #endregion
    }
}
