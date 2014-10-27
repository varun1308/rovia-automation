using System;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class HomePage : UIPage
    {
        internal bool IsVisible()
        {
            var div = WaitAndGetBySelector("divHome", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

    }



}
