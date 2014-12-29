using System;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class PickUp
    {
        public PickUpType PickUpType { get; set; }
        public string PickUpLocCode { get; set; }
        public string PickUpLocation { get; set; }
        public DateTime PickUpDate { get; set; }
        public string PickUpTime { get; set; }
    }
}
