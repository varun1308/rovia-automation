using System.Data;
using Rovia.UI.Automation.Criteria;

namespace Rovia.UI.Automation.DataBinder
{
    public interface ICriteriaDataBinder
    {
        SearchCriteria GetCriteria(DataRow dataRow);
    }
}
