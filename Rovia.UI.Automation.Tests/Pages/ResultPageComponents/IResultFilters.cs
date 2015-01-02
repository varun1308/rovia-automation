using System;
using System.Collections.Generic;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public interface IResultFilters
    {
        void VerifyPreSearchFilters(PreSearchFilters preSearchFilters,Func<List<Results>> getParsedResults);
        void SetPostSearchFilters(PostSearchFilters postSearchFilters);

        void ValidateFilters(PostSearchFilters postSearchFilters, Func<List<Results>> parseResults);
    }
}
