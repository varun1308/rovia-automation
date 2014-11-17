using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Model;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Tests
{
    [TestClass]
    public class HomePageTests
    {
        public TestContext TestContext { get; set; }
        private static RoviaApp _app;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestHelper.DataBinder = new AirScenarioDataBinder();
            _app = TestHelper.App;
            
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Assert.IsTrue(_app.HomePage.IsVisible(), "Could not Load Home Page!");
            Thread.Sleep(500);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        [TestCategory("Sanity")]
        public void ShouldShowHomePage()
        {
            Assert.IsTrue(_app.HomePage.IsVisible(), "Home was unavailable");
        }


        [TestMethod]
        [TestCategory("Sanity")]
        [DataSource("AirGeneralDataSource")]
        public void PreferedCust_BookingFlow_CreditCard_Success()
        {
            TestHelper.SetCriteria(TestContext.DataRow);
            TestHelper.Login();
            TestHelper.Search();
            TestHelper.AddToCart();
            TestHelper.CheckoutTrip();
            TestHelper.EnterPassengerDetails();
            TestHelper.ConfirmPassengerDetails();
            
            Thread.Sleep(1000);

            //check if itinerary available or price not changed before actual payment
            Assert.IsTrue(_app.CheckoutPage.IsPayNowSuccess_CreditCard(), "Itinerary not available or Price has changed.");

            #region pay by credit card
            PaymentFields paymentFields = new PaymentFields()
            {
                EmailAddress = "vrathod@tavisca.com",
                CreditCard = new CreditCard()
                {
                    NameOnCard = "Vikul Rathod",
                    CardNumber = "4111111111111111",
                    SecurityCode = 999,
                    ExpirationMonth = 10,
                    ExpirationYear = 2015
                },
                Address = new Address()
                {
                    Address1 = "888 main",
                    City = "Plano",
                    Country = "US",
                    PostalCode = "77777",
                    Provinces = "TX"
                },
                PhoneNumber = new PhoneNumber()
                {
                    PhoneNumberArea = "222",
                    PhoneNumberExchange = "223",
                    PhoneNumberDigits = "7777",
                }
            };

            Assert.IsTrue(_app.CheckoutPage.MakePayment_CreditCard(paymentFields), "Payment Failed");
            #endregion

        }

        [TestMethod]
        [TestCategory("Sanity")]
        [DataSource("AirGeneralDataSource")]
        public void GuestUser_BookingFlow_CreditCard_Success()
        {
            TestHelper.SetCriteria(TestContext.DataRow);
            TestHelper.Search();
            TestHelper.AddToCart();
            TestHelper.CheckoutTrip();
            

            #region Guest User Registration
            //for guest user asks for login on checkout page and he will register here
            //please change email id before run this test case
            NewUserRegistration newUser = new NewUserRegistration()
            {
                FName = "Vikul",
                LName = "Rathod",
                EmailId = "vikulr@gmail.com",
                DOB = "09/16/1989",
                Password = "Vikul1989"
            };
            Assert.IsTrue(_app.LoginDetailsPage.NewUserRegistration(newUser), "Registration Failed");

            Assert.IsTrue(_app.LoginDetailsPage.PreferedCustomerRegistration(), "Error during pref cust registration");

            #endregion

            TestHelper.EnterPassengerDetails();
            TestHelper.ConfirmPassengerDetails();

            Thread.Sleep(1000);

            //check if itinerary available or price not changed before actual payment
            Assert.IsTrue(_app.CheckoutPage.IsPayNowSuccess_CreditCard(), "Itinerary not available or Price has changed.");

            #region pay by credit card
            PaymentFields paymentFields = new PaymentFields()
            {
                EmailAddress = "vrathod@tavisca.com",
                CreditCard = new CreditCard()
                {
                    NameOnCard = "Vikul Rathod",
                    CardNumber = "4111111111111111",
                    SecurityCode = 999,
                    ExpirationMonth = 10,
                    ExpirationYear = 2015
                },
                Address = new Address()
                {
                    Address1 = "888 main",
                    City = "Plano",
                    Country = "US",
                    PostalCode = "77777",
                    Provinces = "TX"
                },
                PhoneNumber = new PhoneNumber()
                {
                    PhoneNumberArea = "222",
                    PhoneNumberExchange = "223",
                    PhoneNumberDigits = "7777",
                }
            };

            Assert.IsTrue(_app.CheckoutPage.MakePayment_CreditCard(paymentFields), "Payment Failed");
            #endregion

        }

        [TestMethod]
        [TestCategory("Sanity")]
        [DataSource("AirGeneralDataSource")]
        public void PreferedCust_BookingFlow_RoviaBucks_Success()
        {
            TestHelper.SetCriteria(TestContext.DataRow);
            TestHelper.Login();
            TestHelper.Search();
            TestHelper.AddToCart();
            TestHelper.CheckoutTrip();
            TestHelper.EnterPassengerDetails();
        
            

            //#region submit passenger details
            //PassengerDetails pes = new PassengerDetails()
            //{
            //    InsuranceData = new Insurance() { Country = "United States", IsInsuared = false },
            //    FirstName = "Vikul",
            //    LastName = "Rathod",
            //    DOB = "09/16/1989",
            //    Gender = "Male",
            //    Emailid = "vrathod@tavisca.com"
            //};
            //_app.PassengerInfoPage.SubmitPassengerDetails(pes);
            //#endregion

            //Thread.Sleep(1000);

            #region pay by credit card
            PaymentFields paymentFields = new PaymentFields()
            {
                EmailAddress = "vrathod@tavisca.com",
                Address = new Address()
                {
                    Address1 = "888 main",
                    City = "Plano",
                    Country = "US",
                    PostalCode = "77777",
                    Provinces = "TX"
                },
                PhoneNumber = new PhoneNumber()
                {
                    PhoneNumberArea = "222",
                    PhoneNumberExchange = "223",
                    PhoneNumberDigits = "7777",
                }
            };

            Assert.IsTrue(_app.CheckoutPage.IsPayNowSuccess_RB(paymentFields), "Booking failed with Rovia Bucks");
            #endregion
        }
    }
}
