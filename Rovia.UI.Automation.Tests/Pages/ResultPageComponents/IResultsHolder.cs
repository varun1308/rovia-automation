using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public interface IResultsHolder
    {
        bool IsVisible();
        List<Results> ParseResults();
        void AddToCart(List<Results> result);
    }
}
