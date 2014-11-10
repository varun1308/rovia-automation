using System;
using System.Threading;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class AirResultsPage : UIPage
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

        internal bool AddToCart()
        {
            var div = WaitAndGetBySelector("divResults", ApplicationSettings.TimeOut.Slow);
            if (div != null && div.Displayed)
            {
                WaitAndGetBySelector("btnaddToCart", ApplicationSettings.TimeOut.Slow).Click();
                
                var divloader = WaitAndGetBySelector("divloader", ApplicationSettings.TimeOut.Fast);
                while (divloader != null && divloader.Displayed)
                Thread.Sleep(500);
                WaitAndGetBySelector("btncheckout", ApplicationSettings.TimeOut.Slow).Click();
                while (IsTripFolderVisible())
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsTripFolderVisible()
        {
            var tfCheckout = WaitAndGetBySelector("tfCheckout", ApplicationSettings.TimeOut.Slow);
            return tfCheckout != null && tfCheckout.Displayed;
        }
    }
}
