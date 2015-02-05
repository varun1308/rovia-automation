namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    using Criteria;
    using ScenarioObjects;

    //This interface holds results page itinerary specific actions methods
    public interface IResultsHolder
    {
        bool IsVisible();
        Results AddToCart(SearchCriteria  criteria);
    }
}
