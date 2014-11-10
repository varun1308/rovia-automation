using System;
using System.Threading;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Pages
{
   public class TripFolderPage : UIPage
    {
       internal bool Checkout()
       {
           var btncheckout = WaitAndGetBySelector("tfCheckout", ApplicationSettings.TimeOut.Slow);
           if (btncheckout != null && btncheckout.Displayed)
           {
               btncheckout.Click();
               return true;
           }
           else
           {
               return false;
           }
       }
    }
}
