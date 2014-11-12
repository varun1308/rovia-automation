using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.Tests.Model
{
   public class PassengerDetails
    {
       public Insurance InsuranceData { get; set; }
       public string FirstName { get; set; }
       public string MiddleName { get; set; }
       public string LastName { get; set; }
       public string DOB { get; set; }
       public string Emailid { get; set; }
       public string Gender { get; set; }
    }

    public class Insurance
    {
        public string Country { get; set; }
        public bool IsInsuared { get; set; }
    }
}
