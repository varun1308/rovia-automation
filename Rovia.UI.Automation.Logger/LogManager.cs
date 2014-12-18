using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.Logger
{
    public sealed class LogManager
    {
        private readonly List<ILogger> _detailsLoggers;
        private readonly List<ILogger> _summaryLoggers;
        private static LogManager _logManagerInstance;
        private SummaryEntry _summaryEntry;
        private DetailEntry _detailEntry;
        private Severity _severity;

        private LogManager(List<ILogger> summaryLoggers, List<ILogger> detailLoggers)
        {
            _summaryLoggers = summaryLoggers;
            _detailsLoggers = detailLoggers;
            _severity=Severity.All;
        }

        

        public static void Initialise()
        {
            var config = ConfigurationManager.GetSection("LogManager")
                 as LogManagerConfigSection;

            // Console.WriteLine(config["Tata Motors"].Code);
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

        public static LogManager GetInstance()
        {
            if (_logManagerInstance == null) throw new Exception("invalid call");
            return _logManagerInstance;
        }
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

        public void LogStatus(string step, string status = "passed")
        {
            if (_severity > Severity.Status) return;
            _summaryEntry.AddStep(step, status);
            _detailEntry.Append(step + " " + status );
        }

        public void LogDebug(string message)
        {
            if (_severity > Severity.Status) return;
            _detailEntry.Append("DebugMessage> "+message);
        }

        public void LogInformation(string information)
        {
            if (_severity > Severity.Information) return;
            _detailEntry.Append("info> "+information );
        }

        public void LogWarning(string warning)
        {
            if (_severity > Severity.Warning) return;
            _detailEntry.Append("!!!Warning> "+warning );
        }

        public void LogError(Exception exception)
        {
            if (_severity > Severity.Error) return;
            _detailEntry.Append("!!!!!Exception> "+exception );
        }

        public void LogFatal(string message)
        {
            if (_severity > Severity.Fatal) return;
            _detailEntry.Append("!!!!Fetal Error> "+message+"!!!!");
        }

        public void SubmitLog(string sessionId)
        {
            _detailEntry.Id = sessionId;
            _summaryEntry.Id = sessionId;
            _detailsLoggers.ForEach(x=>x.Log(_detailEntry.ToString()));
            _summaryLoggers.ForEach(x=>x.Log(_summaryEntry.ToString()));
        }
    }
}
