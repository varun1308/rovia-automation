using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public interface IResultFilters
    {
        bool VerifyPreSearchFilters(PreSearchFilters preSearchFilters);
        void SetPostSearchFilters(PostSearchFilters postSearchFilters);

        void ValidateFilters(PostSearchFilters postSearchFilters, System.Collections.Generic.List<Results> list);
    }
}
