using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rovia.UI.Automation.ScenarioObjects
{
    class Infant:Passenger
    {
        public Infant()
        {
            Age = (new Random()).Next(1,2);
        }
    }
}
