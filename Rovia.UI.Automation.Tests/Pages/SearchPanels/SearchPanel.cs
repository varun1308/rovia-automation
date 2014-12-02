using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

// ReSharper disable CheckNamespace
namespace Rovia.UI.Automation.Tests.Pages
// ReSharper restore CheckNamespace
{
    public abstract class SearchPanel:UIPage 
    {
        
        protected void SelectSearchPanel()
        {
            var navBar = WaitAndGetBySelector("navBar", ApplicationSettings.TimeOut.Slow);
            if (navBar == null || !navBar.Displayed)
                throw new UIElementNullOrNotVisible("Navigation Bar ");
            navBar.Click();

            Thread.Sleep(500);

            var searchPanel = WaitAndGetBySelector("searchPanel", ApplicationSettings.TimeOut.Slow);
            if (searchPanel == null || !searchPanel.Displayed)
                throw new UIElementNullOrNotVisible("SearchPanel");
        }

        public abstract void Search(SearchCriteria searchCriteria);
        
        protected abstract void ApplyPreSearchFilters(PreSearchFilters filters);
    }
}
