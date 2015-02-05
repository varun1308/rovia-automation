namespace Rovia.UI.Automation.Tests.Configuration
{
    using System.Configuration;
    /// <summary>
    /// Stores all the app configuration keys in properties
    /// </summary>
    public static class ApplicationSettings
    {
        public static string Url
        {
            get
            {
                return ConfigurationManager.AppSettings["application.url"] ?? "http://corp.rovia.com/";
            }
        }

        public static int MaxSearchDepth
        {
            get { return int.Parse(ConfigurationManager.AppSettings["application.maxSearchDepth"] ?? "1"); }
        }

        public static string LoggedFilePath
        {
            get { return ConfigurationManager.AppSettings["application.loggedfilepath"]; }
        }

        public static string Site
        {
            get { return ConfigurationManager.AppSettings["site"].ToUpper(); }
        }

        public static string Environment
        {
            get { return ConfigurationManager.AppSettings["application.environment"].ToUpper(); }
        }

        public static string TripsErrorUri
        {
            get { return ConfigurationManager.AppSettings["application.tripsErrorUI.url"]; }
        }

        public static string Driver
        {
            get { return ConfigurationManager.AppSettings["driver"].ToUpper(); }
        }

        public static class TimeOut
        {
            public static int Safe { get { return 10; } }
            public static int Slow { get { return 8; } }
            public static int Fast { get { return 5; } }
            public static int SuperFast { get { return 3; } }
        }

        public static class PreferredCustomer
        {
            public static string Username
            {
                get { return ConfigurationManager.AppSettings["application.preferred.username"] ?? "3285301"; }
            }
            public static string Password
            {
                get { return ConfigurationManager.AppSettings["application.preferred.password"] ?? "test"; }
            }
        }

        public static class RegisteredCustomer
        {
            public static string Username
            {
                get { return ConfigurationManager.AppSettings["application.registered.username"] ?? "vrathod@tavisca.com"; }
            }
            public static string Password
            {
                get { return ConfigurationManager.AppSettings["application.registered.password"] ?? "zaq1ZAQ!"; }
            }
        }
    }
}
