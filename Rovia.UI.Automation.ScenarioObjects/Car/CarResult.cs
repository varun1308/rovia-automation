
namespace Rovia.UI.Automation.ScenarioObjects
{
    public class CarResult :Results
    {
        public string RentalAgency { get; set; }
        public string CarType { get; set; }
        public string AirConditioning { get; set; }
        public string Transmission { get; set; }
        public string Location { get; set; }
        public Amount PricePerWeek { get; set; }
        public Amount TotalPrice { get; set; }
    }
}
