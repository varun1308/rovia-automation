using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class Child:Passenger
    {
        public Child()
        {
            BirthDate = DateTime.Now.AddYears(-1 * (new Random()).Next(3,12)).AddMonths(3).ToString("MM/dd/yyyy");
        }
        public Child(IList<string> details)
        {
            BirthDate = details[details.IndexOf("BIRTHDATE") + 1];
            FirstName = details[details.IndexOf("FIRST/GIVEN NAME") + 1];
            MiddleName =
                details.IndexOf("MIDDLE") == -1
                    ? ""
                    : details[details.IndexOf("MIDDLE") + 1];
            LastName = details[details.IndexOf("LAST/FAMILY NAME") + 1];
            Gender = details[details.IndexOf("GENDER") + 1];
            Emailid = details[details.IndexOf("EMAIL ADDRESS") + 1];
        }

        
    }
}
