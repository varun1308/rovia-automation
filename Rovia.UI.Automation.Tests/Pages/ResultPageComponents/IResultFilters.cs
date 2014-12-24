using System;
using System.Collections.Generic;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public interface IResultFilters
    {
        bool VerifyPreSearchFilters(PreSearchFilters preSearchFilters);
        void SetPostSearchFilters(PostSearchFilters postSearchFilters);

        void ValidateFilters(PostSearchFilters postSearchFilters, Func<List<Results>> parseResults);
    }
}
