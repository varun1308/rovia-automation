using Rovia.UI.Automation.Criteria;

namespace Rovia.UI.Automation.Framework.Pages
{
    /// <summary>
    /// Interface holding search widgets methods
    /// </summary>
    public interface ISearchPanel 
    {
        void Search(SearchCriteria searchCriteria);
    }
}
