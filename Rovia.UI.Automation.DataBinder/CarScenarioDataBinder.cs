using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rovia.UI.Automation.Constants;
using Rovia.UI.Automation.Criteria;

namespace Rovia.UI.Automation.DataBinder
{
    public class CarScenarioDataBinder:IScenarioDataBinder
    {
        public SearchCriteria GetCriteria(DataRow dataRow)
        {
            // todo createScenario from datarow
            return new CarSearchCriteria()
                {
                    PickUpLocationType = CarLocationType.NearAirPort,
                    PickUpLocation = "LAS",
                    DropOffLocationType = CarLocationType.NearAirPort,
                    DropOffLocation = "LAX",
                    ToDate = DateTime.Now.AddDays(25),
                    FromDate = DateTime.Now.AddDays(20),
                    DropOffRadius = 10,
                    PickUpRadius = 10
                };
        }
    }
}
