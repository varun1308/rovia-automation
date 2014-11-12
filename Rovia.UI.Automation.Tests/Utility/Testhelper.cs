using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Utility
{   
        [TestClass]
        public class TestHelper
        {
            public static RoviaApp App { get;  set; }

            [AssemblyInitialize]
            public static void AssemblyInitialize(TestContext testContext)
            {
                App = new RoviaApp();
                App.Launch(ApplicationSettings.Url);
            }

            [AssemblyCleanup]
            public static void AssemblyCleanup()
            {
                App.Dispose();
            }
        }
    }
