
using System.Collections.Generic;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class HotelResult:Results
    {

        public string RoomSupplier { get; set; }

        public string HotelName { get; set; }

        public string HotelAddress { get; set; }

        public int HotelRating { get; set; }

        public string RoomType { get; set; }

        public Amount RoomPrice { get; set; }

        public List<string> Amenities { get; set; }
    }
}
