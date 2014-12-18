using System;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class StayPeriod
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int Duration {
            get { return (CheckOutDate - CheckInDate).Days; }
        }
    }
}
