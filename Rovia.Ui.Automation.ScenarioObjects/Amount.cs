using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class Amount
    {
        public double TotalAmount { get; set; }
        public double AmountPerPerson { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }

        public Amount(string amount)
        {
            var details = amount.Split();
            Currency = details[1];
            TotalAmount = double.Parse(details[0].Remove(0,1));
        }

        public Amount(){}
    }
}
