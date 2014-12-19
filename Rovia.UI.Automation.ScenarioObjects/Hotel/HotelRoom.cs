using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.Ui.Automation.ScenarioObjects.Hotel
{
    public class HotelRoom
    {
        public Amount RoomPrice { get; set; }
        public string Descriptions { get; set; }
        public int NoOfRooms { get; set; }
    }
}
