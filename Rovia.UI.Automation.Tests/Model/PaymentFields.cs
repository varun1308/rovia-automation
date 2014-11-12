using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.Tests.Model
{
   public class PaymentFields
    {
       public CreditCard CreditCard { get; set; }
       public Address Address { get; set; }
       public PhoneNumber PhoneNumber { get; set; }
       public string EmailAddress { get; set; }
    }

    public class CreditCard
    {
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public int SecurityCode { get; set; }
        public int ExpirationMonth { get; set; }
        public int ExpirationYear { get; set; }
    }

    public class Address
    {
        public string Address1 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Provinces { get; set; }
    }

    public class PhoneNumber
    {
        public string PhoneNumberArea { get; set; }
        public string PhoneNumberExchange { get; set; }
        public string PhoneNumberDigits { get; set; }
    }
}
