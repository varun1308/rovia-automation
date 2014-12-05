using System;
using System.Configuration;
using Rovia.UI.Automation.Logger;

namespace Rovia.UI.Automation.Tests.Configuration
{
    public static class ApplicationSettings
    {
        public static string Url
        {
            get { return ConfigurationManager.AppSettings["application.url"] ?? "http://corp.rovia.com/";
        }
        }

        public static class WebUser
        {
            public static string Username
            {
                get { return ConfigurationManager.AppSettings["application.webuser.username"]; }
            }

            public static string Password
            {
                get { return ConfigurationManager.AppSettings["application.webuser.password"]; }
            }

        }

        public static int MaxSearchDepth
        {
            get { return int.Parse(ConfigurationManager.AppSettings["application.maxSearchDepth"]); }
        }

        public static class TimeOut
        {
            public static int Safe { get { return 10; } }
            public static int Slow { get { return 8; } }
            public static int Fast { get { return 5; } }
            public static int SuperFast { get { return 3; } }



        }

      
       
    }
}
