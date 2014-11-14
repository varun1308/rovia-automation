using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rovia.Ui.Automation.ScenarioObjects
{
    class Child:Passenger
    {
        public Child()
        {
            Age = (new Random()).Next(3, 17);
        }
    }
}
