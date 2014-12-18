using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rovia.UI.Automation.Criteria;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public interface IResultsTitle
    {
        bool ValidateTitle(SearchCriteria criteria);
    }
}
