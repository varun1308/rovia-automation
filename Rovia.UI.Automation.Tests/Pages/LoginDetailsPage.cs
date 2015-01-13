using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class LoginDetailsPage : UIPage
    {
        internal bool IsVisible()
        {
            var div = WaitAndGetBySelector("divLogInPanel", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

        internal void LogIn(string userId, string password)
        {
            WaitAndGetBySelector("username", ApplicationSettings.TimeOut.Slow).SendKeys(userId);

            WaitAndGetBySelector("password", ApplicationSettings.TimeOut.Fast).SendKeys(password);

            WaitAndGetBySelector("btnLogIn", ApplicationSettings.TimeOut.Fast).Click();
        }

        internal void ContinueAsGuest(string fname,string lname, string emailid)
        {
            WaitAndGetBySelector("guestOption", 120).Click();
            WaitAndGetBySelector("guestOptionContinue", ApplicationSettings.TimeOut.Safe).Click();
            WaitAndGetBySelector("guestFName", ApplicationSettings.TimeOut.SuperFast).SendKeys(fname);
            WaitAndGetBySelector("guestLName", ApplicationSettings.TimeOut.SuperFast).SendKeys(lname);
            WaitAndGetBySelector("guestEmailId", ApplicationSettings.TimeOut.SuperFast).SendKeys(emailid);
            ExecuteJavascript("$('button[val-submit=\"guest\"]').click()");
        }
    }
}
