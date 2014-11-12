using System;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class LogInPage : UIPage
    {
        internal bool IsVisible()
        {
            var div = WaitAndGetBySelector("divLogInPanel", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

        internal void LogIn(string userId, string password)
        {
            //Enter Credentials

            WaitAndGetBySelector("username", ApplicationSettings.TimeOut.Fast).SendKeys(userId);
            
            WaitAndGetBySelector("password", ApplicationSettings.TimeOut.Fast).SendKeys(password);
            
            WaitAndGetBySelector("btnLogIn", ApplicationSettings.TimeOut.Fast).Click();

            
            
        }

        internal void DefaultLogIn()
        {
            LogIn(ApplicationSettings.WebUser.Username,ApplicationSettings.WebUser.Password);
        }

        
    }
}
