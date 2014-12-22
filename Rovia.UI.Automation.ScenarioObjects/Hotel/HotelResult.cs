
using System.Collections.Generic;
using Rovia.UI.Automation.ScenarioObjects.Hotel;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class HotelResult:Results
    {

        public string RoomSupplier { get; set; }

        public string HotelName { get; set; }

        public string HotelAddress { get; set; }

        public int HotelRating { get; set; }

        public HotelRoom SelectedRoom { get; set; }

        public StayPeriod StayPeriod { get; set; }

        public List<string> Amenities { get; set; }

        public Passengers Passengers { get; set; }
    }
}
