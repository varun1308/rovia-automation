using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class CarResultsPage : UIPage
    {
        internal bool IsWaitingVisible()
        {
            var div = WaitAndGetBySelector("divWaiting", ApplicationSettings.TimeOut.Fast);
            return div != null && div.Displayed;
        }

        internal bool IsResultsVisible()
        {
            var div = WaitAndGetBySelector("divMatrix", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }
    }
}
