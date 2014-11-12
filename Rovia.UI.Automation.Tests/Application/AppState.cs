using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rovia.UI.Automation.Tests.Pages;

namespace Rovia.UI.Automation.Tests.Application
{
    public class AppState
    {
        public string CurrentPage { get; set; }
        public User CurrentUser { get; set; }
    }
}