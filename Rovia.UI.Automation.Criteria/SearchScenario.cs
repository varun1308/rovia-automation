using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Criteria
{
    public abstract class SearchCriteria
    {
        public ProductType ProductType { get; set; }
        public string Description { get; set; }
        public UserType UserType { get; set; }
        public List<SpecialCriteria>  SpecialCriteria { get; set; }
        public PaymentMode PaymentMode { get; set; }
    }
}
