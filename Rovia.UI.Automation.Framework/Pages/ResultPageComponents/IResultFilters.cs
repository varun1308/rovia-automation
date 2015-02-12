using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Framework.Pages
{
    //This interface holds results page filters methods
    public interface IResultFilters
    {
        void VerifyPreSearchFilters(PreSearchFilters preSearchFilters);
        void SetPostSearchFilters(PostSearchFilters postSearchFilters);
        void ValidateFilters(PostSearchFilters postSearchFilters);
    }
}
