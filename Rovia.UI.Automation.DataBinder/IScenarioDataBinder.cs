using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rovia.UI.Automation.Criteria;

namespace Rovia.UI.Automation.DataBinder
{
    public interface IScenarioDataBinder
    {
        SearchCriteria GetCriteria(DataRow dataRow);
    }
}
