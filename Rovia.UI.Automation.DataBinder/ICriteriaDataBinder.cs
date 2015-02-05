namespace Rovia.UI.Automation.DataBinder
{
    using System.Data;
    using Criteria;

    //Interface for binding search criteria from input datasheet
    public interface ICriteriaDataBinder
    {
        SearchCriteria GetCriteria(DataRow dataRow);
    }
}
