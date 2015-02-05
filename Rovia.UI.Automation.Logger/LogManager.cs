namespace Rovia.UI.Automation.Logger
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Custom log files generator
    /// </summary>
    public sealed class LogManager
    {
        #region Private Fields

        private readonly List<ILogger> _detailsLoggers;
        private readonly List<ILogger> _summaryLoggers;
        private static LogManager _logManagerInstance;
        private SummaryEntry _summaryEntry;
        private DetailEntry _detailEntry;
        private Severity _severity;

        #endregion

        #region Private Members
        //Initializes a LogManager Object
        private LogManager(List<ILogger> summaryLoggers, List<ILogger> detailLoggers)
        {
            _summaryLoggers = summaryLoggers;
            _detailsLoggers = detailLoggers;
            _severity=Severity.All;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Intializer to log files
        /// </summary>
        public static void Initialise()
        {
            var config = ConfigurationManager.GetSection("LogManager")
                 as LogManagerConfigSection;

            var summaryLoggers = new List<ILogger>();
            var detailLoggers = new List<ILogger>();
            if (config == null)
                throw new Exception("Invalid Valies in rovia.automation.logManager section in configuration");

            summaryLoggers.AddRange(from LoggerElement e in config.SummaryLoggers select (ILogger)Activator.CreateInstance(Assembly.Load(e.Assembly).GetType(e.Namespace), e.Target));
            detailLoggers.AddRange(from LoggerElement e in config.DetailLoggers select (ILogger)Activator.CreateInstance(Assembly.Load(e.Assembly).GetType(e.Namespace), e.Target));
            _logManagerInstance = new LogManager(summaryLoggers, detailLoggers) { _severity = config.Severity.Value };
        }

        public static LogManager GetInstance(string path1,string path2)
        {
            if (_logManagerInstance==null)
                throw new Exception("LogManager not Initialised");
            return _logManagerInstance;

        }

        /// <summary>
        /// Create instance to LogManager Object
        /// </summary>
        /// <returns>LogManager Object</returns>
        public static LogManager GetInstance()
        {
            if (_logManagerInstance == null) throw new Exception("invalid call");
            return _logManagerInstance;
        }

        /// <summary>
        /// Initiates for Summary and Details log files 
        /// </summary>
        /// <param name="description">Test Description</param>
        public void StartNewLog(String description)
        {
            _summaryEntry=new SummaryEntry()
                {
                    Description = description
                };
            _detailEntry=new DetailEntry()
                {
                    Description = description
                };
        }

        /// <summary>
        /// Log test execution status with excution step and its result status
        /// </summary>
        /// <param name="step">Execution step in pipeline</param>
        /// <param name="status">Status Passed/Failed</param>
        public void LogStatus(string step, string status = "passed")
        {
            if (_severity > Severity.Status) return;
            _summaryEntry.AddStep(step, status);
            _detailEntry.Append(step + " " + status );
        }

        /// <summary>
        /// Log a debug message during test execution flow
        /// </summary>
        /// <param name="message">Debug message</param>
        public void LogDebug(string message)
        {
            if (_severity > Severity.Status) return;
            _detailEntry.Append("DebugMessage> "+message);
        }

        /// <summary>
        /// Log a message as information during test execution flow
        /// </summary>
        /// <param name="information">Suitable descriptive message</param>
        public void LogInformation(string information)
        {
            if (_severity > Severity.Information) return;
            _detailEntry.Append("info> "+information );
        }

        /// <summary>
        /// Log a message as warning during test execution flow
        /// </summary>
        /// <param name="warning">warning text message</param>
        public void LogWarning(string warning)
        {
            if (_severity > Severity.Warning) return;
            _detailEntry.Append("!!!Warning> "+warning );
        }

        /// <summary>
        /// Log a message as error during test execution flow
        /// </summary>
        /// <param name="exception">Error message</param>
        public void LogError(Exception exception)
        {
            if (_severity > Severity.Error) return;
            _detailEntry.Append("!!!!!Exception> "+exception );
        }

        /// <summary>
        /// Log a message as fatal info during test execution flow
        /// </summary>
        /// <param name="message">Message</param>
        public void LogFatal(string message)
        {
            if (_severity > Severity.Fatal) return;
            _detailEntry.Append("!!!!Fetal Error> "+message+"!!!!");
        }

        /// <summary>
        /// Write all logs to files at the end test execution
        /// </summary>
        /// <param name="sessionLogs">Test executed session infos</param>
        public void SubmitLog(string sessionLogs)
        {
            _detailEntry.Id = sessionLogs;
            _summaryEntry.Id = sessionLogs;
            _detailsLoggers.ForEach(x=>x.Log(_detailEntry.ToString()));
            _summaryLoggers.ForEach(x=>x.Log(_summaryEntry.ToString()));
        }

        #endregion
    }
}
