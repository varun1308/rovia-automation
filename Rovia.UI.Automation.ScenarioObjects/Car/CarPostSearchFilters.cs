using System.Collections.Generic;

namespace Rovia.UI.Automation.ScenarioObjects
{
   public class CarPostSearchFilters:PostSearchFilters
    {
       public List<string> LocationValues { get; set; }
       public List<string> CarTypes { get; set; }
       public List<string> RentalAgency { get; set; }
       public List<string> CarOptions { get; set; }
    }
}
