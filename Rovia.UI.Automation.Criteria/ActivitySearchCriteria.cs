using System;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Criteria
{
    public class ActivitySearchCriteria:SearchCriteria
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ShortLocation { get; set; }
        public string Location { get; set; }
        public AgeGroup ChildrenAgeGroup { get; set; }
        public AgeGroup AdultAgeGroup { get; set; }
        public AgeGroup InfantAgeGroup { get; set; }
    }
}
