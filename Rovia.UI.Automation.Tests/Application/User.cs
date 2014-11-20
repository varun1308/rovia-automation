using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Application
{
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