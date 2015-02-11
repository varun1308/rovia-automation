namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    using ScenarioObjects;

    //This interface holds results page filters methods
    public interface IResultFilters
    {
        void VerifyPreSearchFilters(PreSearchFilters preSearchFilters);
        void SetPostSearchFilters(PostSearchFilters postSearchFilters);
        void ValidateFilters(PostSearchFilters postSearchFilters);
    }
}
