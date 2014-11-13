using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Pages
{
    public interface IResultsPage 
    {
        void AddToCart(List<Results> result);
        void WaitForResultLoad();
        List<Results> ParseResults();
    }
}
