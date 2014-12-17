using System.Collections.Generic;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class CarPreSearchFilters : PreSearchFilters
    {
        public string RentalAgency { get; set; }
        public string CarType { get; set; }
        public int AirConditioning { get; set; }
        public int Transmission { get; set; }
        public List<CorporateDiscount> CorporateDiscount { get; set; }
    }
}
