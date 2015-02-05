namespace Rovia.UI.Automation.Criteria
{
    using ScenarioObjects;

    public class HotelSearchCriteria:SearchCriteria
    {
        public string Location { get; set; }
        public string ShortLocation { get; set; }
        public StayPeriod StayPeriod { get; set; }
    }
}
