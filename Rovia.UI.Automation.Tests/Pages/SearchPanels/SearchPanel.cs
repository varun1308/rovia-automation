using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Pages
{
    public abstract class SearchPanel:UIPage 
    {
        protected abstract void SelectSearchPanel();

        public abstract void Search(SearchCriteria searchCriteria);
        
        protected abstract void ApplyPreSearchFilters(PreSearchFilters filters);
    }
}
