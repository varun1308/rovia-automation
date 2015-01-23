using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class AirTripProduct : TripProduct
    {
        public List<FlightLeg> FlightLegs { get; set; }
        public List<string> Airlines { get; set; } 
        public AirTripProduct()
        {
            ProductType = TripProductType.Air;
        }
    }
}
