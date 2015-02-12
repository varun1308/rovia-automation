using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Framework.Configurations;

namespace Rovia.UI.Automation.Framework.Pages
{
    /// <summary>
    /// This clas holds methods for login to site
    /// </summary>
    public class LoginDetailsPage : UIPage
    {
        /// <summary>
        /// Check if log in page visible
        /// </summary>
        /// <returns></returns>
        internal bool IsVisible()
        {
            var div = WaitAndGetBySelector("divLogInPanel", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

        /// <summary>
        /// Log in with username password to site
        /// </summary>
        /// <param name="userId">Username</param>
        /// <param name="password">Password</param>
        public void LogIn(string userId, string password)
        {
            WaitAndGetBySelector("username", ApplicationSettings.TimeOut.Slow).SendKeys(userId);

            WaitAndGetBySelector("password", ApplicationSettings.TimeOut.Fast).SendKeys(password);

            WaitAndGetBySelector("btnLogIn", ApplicationSettings.TimeOut.Fast).Click();
        }

        /// <summary>
        /// Continue as guest option method
        /// </summary>
        /// <param name="fname">FirstName</param>
        /// <param name="lname">LastName</param>
        /// <param name="emailid">EmailId</param>
        public void ContinueAsGuest(string fname,string lname, string emailid)
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
