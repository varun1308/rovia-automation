using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Framework.Pages
{
    //This interface holds results page itinerary specific actions methods
    public interface IResultsHolder
    {
        bool IsVisible();
        Results AddToCart(SearchCriteria  criteria);
    }
}
