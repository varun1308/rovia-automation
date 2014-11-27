using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Validator
{
   public interface IValidator
   {
       bool ValidatePreSearchFilters(PreSearchFilters preSearchFilters,Results parseResults);
   }
}
