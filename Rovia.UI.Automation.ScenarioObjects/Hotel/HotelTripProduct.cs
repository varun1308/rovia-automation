
using Rovia.UI.Automation.ScenarioObjects.Hotel;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class HotelTripProduct:TripProduct
    {
        public StayPeriod StayPeriod { get; set; }
        public string KitchenType { get; set; }
        public string Address { get; set; }
        public int Rating { get; set; }
        public HotelRoom Room { get; set; }
        public HotelTripProduct()
        {
            ProductType = TripProductType.Hotel;
        }
    }
}
