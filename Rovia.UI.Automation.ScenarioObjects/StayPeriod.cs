using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
