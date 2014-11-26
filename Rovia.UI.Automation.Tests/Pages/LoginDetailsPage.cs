using System;
using System.Threading;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Model;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class LoginDetailsPage : UIPage
    {
        internal bool Login()
        {
            //uncomment if login from home page
            //var loginlink = WaitAndGetBySelector("loginlink", ApplicationSettings.TimeOut.SuperFast);
            //if (loginlink!=null&& loginlink.Displayed)
            //    loginlink.Click();

            WaitAndGetBySelector("username", ApplicationSettings.TimeOut.SuperFast).SendKeys(ApplicationSettings.WebUser.Username);
            WaitAndGetBySelector("password", ApplicationSettings.TimeOut.SuperFast).SendKeys(ApplicationSettings.WebUser.Password);
            WaitAndGetBySelector("signin", ApplicationSettings.TimeOut.SuperFast).Click();

            Thread.Sleep(1000);
            //var isLoginSuccessHome = WaitAndGetBySelector("loginsuccessHome", ApplicationSettings.TimeOut.Slow);
            var isLoginSuccessTrip = WaitAndGetBySelector("loginsuccessTrip", ApplicationSettings.TimeOut.Safe);
            return (isLoginSuccessTrip != null && isLoginSuccessTrip.Displayed);
        }

        internal bool NewUserRegistration(NewUserRegistration user)
        {
            //uncomment if login from home page
            //var loginlink = WaitAndGetBySelector("loginlink", ApplicationSettings.TimeOut.SuperFast);
            //if (loginlink!=null&& loginlink.Displayed)
            //    loginlink.Click();
            //Thread.Sleep(1000);

            ExecuteJavascript("$('input[value=\"register\"]').click()");
            Thread.Sleep(1500);
            ExecuteJavascript("$('input#Text1').val('" + user.FName + "');$('input#Text2').val('" + user.LName + "');$('input#Text3').val('" + user.EmailId + "');$('input#Text4').val('" + user.Password + "');");
            ExecuteJavascript("$('input[placeholder=\"Date of Birth\"]').val('" + user.DOB + "')");
            ExecuteJavascript("$('#chkTNC').prop('checked',true)");
            ExecuteJavascript("$('button[val-submit=\"register\"]').click()");

            var divPrefCust = WaitAndGetBySelector("divPrefCust", ApplicationSettings.TimeOut.Slow);
            if (divPrefCust != null && divPrefCust.Displayed)
                return true;
            else
                return false;
        }

        internal bool PreferedCustomerRegistration()
        {
            //need to handle if user wants to become prefered customer
            ExecuteJavascript("$('input[class=\"jbtnNo\"]').click()");
            ExecuteJavascript("$('a[val-submit=\"PrefCustomerTNC\"]').click()");
            Thread.Sleep(500);
            return true;
        }

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

            WaitAndGetBySelector("btnLogIn", ApplicationSettings.TimeOut.Slow).Click();



        }

        internal void ContinueAsGuest()
        {
            throw new NotImplementedException();
        }
    }
}
