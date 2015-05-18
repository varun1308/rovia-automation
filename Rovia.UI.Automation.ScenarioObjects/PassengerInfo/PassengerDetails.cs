using System;
using System.Collections.Generic;
using System.Linq;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class PassengerDetails
    {
        public string Country { get; set; }
        public bool IsInsuranceRequired { get; set; }
        public List<Passenger> Passengers { get; private set; }
        private static readonly string[] FirstName = { "VIKUL", "ASHISH", "POOJA", "PRIYA", "ASMITA", "GATUM", "RAMESH", "SURESH", "AARTI", "SNEHA", "ASTHA", "DIKSHA","TEJAS","ROHAN","KUNAL","SANDESH","KHUSHBOO" };
        private static readonly string[] LastName = { "AUTOMATION", "AUTOMATION", "AUTOMATION", "AUTOMATION"};
        private static readonly string[] MiddleName = { "", ""};
        private static readonly string[] EmailId = { "rpandit@tavisca.com", "vrathod@tavisca.com" };
        public PassengerDetails(Passengers passengers)
        {
            Passengers = new List<Passenger>();
            var randomGenerator = new Random();
            for (var i = 0; i < passengers.Adults; i++)
            {
                Add(new Adult()
                    {
                        FirstName = FirstName[randomGenerator.Next(FirstName.Length)],
                        MiddleName = MiddleName[randomGenerator.Next(MiddleName.Length)],
                        LastName = LastName[randomGenerator.Next(LastName.Length)],
                        Emailid = EmailId[randomGenerator.Next(EmailId.Length)],
                        Gender = DateTime.Now.Second % 2 == 0 ? "Male" : "Female"
                    });
            }
            for (var i = 0; i < passengers.Children; i++)
            {
                Add(new Child()
                {
                    FirstName = FirstName[randomGenerator.Next(FirstName.Length)],
                    MiddleName = MiddleName[randomGenerator.Next(MiddleName.Length)],
                    LastName = LastName[randomGenerator.Next(LastName.Length)],
                    Emailid = EmailId[randomGenerator.Next(EmailId.Length)],
                    Gender = DateTime.Now.Second % 2 == 0 ? "Male" : "Female"
                });
            }
            for (var i = 0; i < passengers.Infants; i++)
            {
                Add(new Infant()
                {
                    FirstName = FirstName[randomGenerator.Next(FirstName.Length)],
                    MiddleName = MiddleName[randomGenerator.Next(MiddleName.Length)],
                    LastName = LastName[randomGenerator.Next(LastName.Length)],
                    Emailid = EmailId[randomGenerator.Next(EmailId.Length)],
                    Gender = DateTime.Now.Second % 2 == 0 ? "Male" : "Female"
                });
            }
            Country = "United States";
            IsInsuranceRequired = false;
        }

        private void Add(Passenger passenger)
        {
            while (true)
            {
                if (Passengers.Any(x => x.FirstName.Equals(passenger.FirstName) && x.LastName.Equals(passenger.LastName)))
                    passenger.FirstName += "d";
                else
                    break;
            }
            Passengers.Add(passenger);
        }

        public PassengerDetails(Passengers passengers, AgeGroup adultAgeGroup, AgeGroup childAgeGroup, AgeGroup infantAgeGroup)
            : this(passengers)
        {
            var index = 0;
            var pCounter = 0;
            while (pCounter++ < passengers.Adults)
                AdjustAge(Passengers[index++], adultAgeGroup);
            pCounter = 0;
            while (pCounter++ < passengers.Children)
                AdjustAge(Passengers[index++], childAgeGroup);
            pCounter = 0;
            while (pCounter++ < passengers.Infants)
                AdjustAge(Passengers[index++], infantAgeGroup);
        }

        private static void AdjustAge(Passenger passenger, AgeGroup ageGroup)
        {
            var years = ((ageGroup.MaxAge - ageGroup.MinAge) / 2) + ageGroup.MinAge;
            passenger.BirthDate = DateTime.Now.AddYears(-1 * years).AddMonths(-2).ToString("MM/dd/yyyy");
        }
    }


}
