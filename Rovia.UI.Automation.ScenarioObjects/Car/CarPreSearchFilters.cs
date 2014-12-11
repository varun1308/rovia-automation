using System.Collections.Generic;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.Ui.Automation.ScenarioObjects.Car
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
