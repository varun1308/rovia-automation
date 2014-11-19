using System.Collections.Generic;
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
        public CreditCardType CardType { get; set; }
        public Passengers Passengers { get; set; }
    }
}
