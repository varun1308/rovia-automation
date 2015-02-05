namespace Rovia.UI.Automation.Criteria
{
    using ScenarioObjects;

    public abstract class SearchCriteria
    {
        public string Description { get; set; }
        public UserType UserType { get; set; }
        public PaymentMode PaymentMode { get; set; }
        public CreditCardType CardType { get; set; }
        public Passengers Passengers { get; set; }
        public Filters Filters { get; set; }
        public string Pipeline { get; set; }

        public string Supplier { get; set; }
    }
}
