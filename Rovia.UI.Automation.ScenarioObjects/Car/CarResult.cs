using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.ScenarioObjects.Car
{
    public class CarResult :Results
    {
        public string RentalAgency { get; set; }
        public string CarType { get; set; }
        public string AirConditioning { get; set; }
        public string Transmission { get; set; }
        public float PricePerWeek { get; set; }
        public float TotalPrice { get; set; }
    }
}
