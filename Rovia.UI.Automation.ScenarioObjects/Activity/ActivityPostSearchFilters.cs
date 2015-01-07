using System;
using System.Collections.Generic;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class ActivityPostSearchFilters : PostSearchFilters
    {
        public string ActivityName { get; set; }

        public List<string> Categories { get; set; }

        public SortBy SortBy { get; set; }
    }
}
