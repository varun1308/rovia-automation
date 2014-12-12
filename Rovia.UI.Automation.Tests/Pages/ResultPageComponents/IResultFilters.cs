using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public interface IResultFilters
    {
        bool VerifyPreSearchFilters(PreSearchFilters preSearchFilters);
        bool SetPostSearchFilters(PostSearchFilters postSearchFilters);
    }
}
