using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Framework.Application
{
    /// <summary>
    /// Saves logged in user data
    /// </summary>
    public class User
    {
        public UserType Type { get; set; }
        public string UserName { get; set; }
        public bool IsLoggedIn { get; set; }

        public void ResetUser()
        {
            Type = UserType.Guest;
            UserName = "";
            IsLoggedIn = false;
        }
    }
}