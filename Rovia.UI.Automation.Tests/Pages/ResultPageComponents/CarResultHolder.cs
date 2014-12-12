using System;
using System.Collections.Generic;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
  public class CarResultHolder:UIPage,IResultsHolder
    {
        public bool IsVisible()
        {
            var div = WaitAndGetBySelector("matrixHolder", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

        public List<ScenarioObjects.Results> ParseResults()
        {
            throw new NotImplementedException();
        }

        public ScenarioObjects.Results AddToCart(string result)
        {
            throw new NotImplementedException();
        }
    }
}
