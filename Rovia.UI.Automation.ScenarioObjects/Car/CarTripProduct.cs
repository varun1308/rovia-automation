
using System;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class CarTripProduct : TripProduct
    {
        public string RentalAgency { get; set; }
        public string CarType { get; set; }
        public string AirConditioning { get; set; }
        public string Transmission { get; set; }
        public DateTime PickUpDateTime { get; set; }
        public DateTime DropOffDateTime { get; set; }

        public CarTripProduct()
        {
            ProductType = TripProductType.Car;
        }
    }
}
