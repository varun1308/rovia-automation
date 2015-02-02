using System.Collections.Generic;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public interface IResultsHolder
    {
        bool IsVisible();
        Results AddToCart(SearchCriteria  criteria);
    }
}
