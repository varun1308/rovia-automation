
namespace Rovia.UI.Automation.ScenarioObjects
{
    public class Amount
    {
      
        public override int GetHashCode()
        {
            unchecked
            {
                return (TotalAmount.GetHashCode()*397) ^ (Currency != null ? Currency.GetHashCode() : 0);
            }
        }

        public double TotalAmount { get; set; }
        public double AmountPerPerson { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }

        public Amount(string amount)
        {
            var details = amount.Split();
            Currency = details[1];
            TotalAmount = double.Parse(details[0].Remove(0,1));
        }

        public Amount(){}

        public override bool Equals(object obj)
        {
            var amt = obj as Amount;
            if (amt == null)
                return false;
            return TotalAmount.Equals(amt.TotalAmount) && Currency.Equals(amt.Currency);
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", TotalAmount, Currency);
        }
    }
}
