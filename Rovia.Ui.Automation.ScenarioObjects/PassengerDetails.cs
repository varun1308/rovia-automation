using System;
using System.Collections.Generic;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class PassengerDetails
    {
        public string Country { get; set; }
        public bool IsInsuranceRequired { get; set; }
        public List<Passenger> Passengers { get; private set; }
        private static readonly string[] FirstName = { "Vikul", "Ashish", "Pooja", "Priya","Asmita","Gatum","Ramesh", "Suresh","Aarti","Sneha","Astha","Diksha" };
        private static readonly string[] LastName = {"Rathod", "Guliya","Singh","Roy","Singh", "Dabur", "Gupta","Sharma","Trivedi", "Paswaan","Fernandis","D'Silva","Lol"};
        private static readonly string[] MiddleName = { "", "Kumar", "", "", "Ram", "Vir", "", "Vijay", "K.", "", "", "", "" };
        private static readonly string[] EmailId = { "vrathod@tavisca.com", "aguliya@tavisca.com"};

        public PassengerDetails(Passengers passengers)
        {
            Passengers=new List<Passenger>();
            
            var randomGenerator = new Random();
            for (var i = 0; i < passengers.Adults;i++ )
            {
                Passengers.Add(new Adult()
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
                Passengers.Add(new Child()
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
                Passengers.Add(new Infant()
                {
                    FirstName = FirstName[randomGenerator.Next(FirstName.Length)],
                    MiddleName = MiddleName[randomGenerator.Next(MiddleName.Length)],
                    LastName = LastName[randomGenerator.Next(LastName.Length)],
                    Emailid = EmailId[randomGenerator.Next(EmailId.Length)],
                    Gender = DateTime.Now.Second % 2 == 0 ? "Male" : "Female"
                });
            }

        }
    }

    
}
