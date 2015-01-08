using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.ScenarioObjects.Activity
{
    public class ActivityTripProduct:TripProduct
    {
        public ActivityTripProduct()
        {
            ProductType=TripProductType.Activity;
        }

        public string ActivityProductName { get; set; }

        public string Category { get; set; }

        public DateTime Date { get; set; }
    }
}
