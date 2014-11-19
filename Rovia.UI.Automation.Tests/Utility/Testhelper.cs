using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Utility
{
    [TestClass]
    public class TestHelper
    {
        public static RoviaApp App { get; set; }
        private static SearchCriteria _criteria;
        public static IScenarioDataBinder DataBinder { get; set; }
        public static TripFolder Trip { get; set; }

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            App = new RoviaApp();
            GoToHomePage();
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            App.Dispose();
        }

        internal static void GoToHomePage()
        {
            App.Launch(ApplicationSettings.Url);
            App.HomePage.WaitForHomePage();
            App.State.CurrentPage = "HomePage";
        }

        internal static void SetCriteria(DataRow dataRow)
        {
            _criteria = DataBinder.GetCriteria(dataRow);
        }

        private static List<Results> ApplySpecialCriteria()
        {
            var selectedResults = App.ResultsPage.ParseResults();
            if (_criteria != null)
            {
                foreach (var criterium in _criteria.SpecialCriteria)
                {
                    if (criterium.Name.Equals("Supplier"))
                        selectedResults = selectedResults.Where(x => x.Supplier.SupplierName.Equals(criterium.Value)).ToList();
                    //todo add handling for other criteria
                }
            }
            return selectedResults;
        }

        internal static void EditPassengerInfoAndContinue()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("PassengerDetails-ConfirmationPage"))
                    throw new Exception("PassengerDetails-Editing is not available on " + App.State.CurrentPage);
                App.PassengerInfoPage.EditPassengerInfo();
            }
            catch (Exception exception)
            {
                throw new Exception("PassengerDetail-Submission failed", exception);
            }
        }

        internal static void Search()
        {

            try
            {
                if (!App.State.CurrentPage.Equals("HomePage"))
                    throw new Exception("Search is not available on " + App.State.CurrentPage);
                var criteria = _criteria is AirSearchCriteria ? _criteria as AirSearchCriteria : null;
                App.HomePage.Search(criteria);
                WaitForResultLoad();

            }
            catch (Exception exception)
            {

                throw new Exception("SearchFailed", exception);
            }
        }
        
        internal static void Login()
        {
            try
            {
                if (App.State.CurrentUser.IsLoggedIn)
                    LogOut();
                GoToLoginPage();
                switch (_criteria.UserType)
                {
                    case UserType.Registered:
                        App.LoginDetailsPage.LogIn("vrathod@tavisca.com", "zaq1ZAQ!");
                        App.State.CurrentUser.UserName = "vrathod@tavisca.com";
                        App.State.CurrentUser.Type = _criteria.UserType;
                        App.State.CurrentUser.IsLoggedIn = true;
                        break;
                    case UserType.Preferred:
                        App.LoginDetailsPage.LogIn("PreferredUser", "Password");
                        App.State.CurrentUser.UserName = "RegisteredUserUserName";
                        App.State.CurrentUser.Type = _criteria.UserType;
                        App.State.CurrentUser.IsLoggedIn = true;
                        break;
                    case UserType.Guest:
                        App.LoginDetailsPage.ContinueAsGuest();
                        App.State.CurrentUser.ReSetUser();
                        break;
                }
                App.HomePage.WaitForHomePage();
                App.State.CurrentPage = "HomePage";

            }
            catch (Exception exception)
            {
                
                throw new Exception("LogIn Failed", exception);
            }
        }

        internal static void AddToCart()
        {
            try
            {
                if (!App.State.CurrentPage.EndsWith("ResultsPage"))
                    throw new Exception("AddToCart is not available on " + App.State.CurrentPage);
                App.ResultsPage.AddToCart(ApplySpecialCriteria());
                App.State.CurrentPage = "TripFolderPage";
                ParseTripFolder();
            }
            catch (Exception exception)
            {
                throw new Exception("AddToCart Failed", exception);
            }
        }

        internal static void EnterPassengerDetails()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("PassengerInfoPage"))
                    throw new Exception("PassengerDetails-Submission is not available on " + App.State.CurrentPage);
                App.PassengerInfoPage.SubmitPassengerDetails(GetPassengerDetails());
                App.State.CurrentPage = "PassengerDetails-ConfirmationPage";
            }
            catch (Exception exception)
            {
                throw new Exception("PassengerDetail-Submission failed", exception);
            }
        }

        internal static void ConfirmPassengerDetails()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("PassengerDetails-ConfirmationPage"))
                    throw new Exception("PassengerDetails-Confirmation is not available on " + App.State.CurrentPage);
                App.PassengerInfoPage.ConfirmPassengers();
                App.CheckoutPage.WaitForLoad();
                App.State.CurrentPage = "CheckOutPage";
            }
            catch (Exception exception)
            {
                
                throw new Exception("Passenger Confirmation failed",exception);
            }
        }

        private static void GoToLoginPage()
        {
            try
            {
                switch (App.State.CurrentPage)
                {
                    case "HomePage": App.HomePage.GoToLoginPage();
                        break;
                }
                App.State.CurrentPage = "LogInPage";
            }
            catch (Exception exception)
            {

                throw new Exception("Login page failed to Load", exception);
            }
        }

        private static void LogOut()
        {
            try
            {
                switch (App.State.CurrentPage)
                {
                    case "HomePage": App.HomePage.LogOut();
                        App.State.CurrentPage = "LogInPage";
                        GoToHomePage();
                        break;
                }
            }
            catch (Exception exception)
            {
                throw new Exception("LogOut Failed", exception);
            }

        }

        private static PassengerDetails GetPassengerDetails()
        {
            return new PassengerDetails(_criteria.Passengers)
            {
                Country = "United States",
                IsInsuranceRequired = false
            };
        }

        private static void WaitForResultLoad()
        {
            switch (_criteria.ProductType)
            {
                case ProductType.Air: App.ResultsPage.WaitForResultLoad();
                    App.State.CurrentPage = "AirResultsPage";
                    break;
            }
        }

        #region TripFolder Calls

        private static void ParseTripFolder()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip can not be parse on " + App.State.CurrentPage);
                Trip = App.TripFolderPage.ParseTripFolder();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in parsing trip details.", exception);
            }
        }

        public static void SaveTrip()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip can not be save on " + App.State.CurrentPage);
                // to implement
                Trip.TripSettingsButton.Click();
                Trip.SaveTripButton.Click();

                //2 cases
                //1. If already logged in directly save the trip
                //2. If not, ask for login and then save the trip
            }
            catch (Exception exception)
            {
                throw new Exception("Error in saving trip.", exception);
            }
        }

        public static void TripStartOver()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip can not be start over on " + App.State.CurrentPage);
                Trip.TripSettingsButton.Click();
                Trip.StartoverButton.Click();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in trip start over", exception);
            }
        }

        public static void EditTripName()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip name can not be edit on " + App.State.CurrentPage);
                Trip.TripSettingsButton.Click();
                App.TripFolderPage.EditTripName();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in editing trip name.", exception);
            }
        }

        public static void ModifyProduct(int index)
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip product can not be modified on " + App.State.CurrentPage);
                Trip.TripProducts[index].ModifyProductButton.Click();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in modifying trip product.", exception);
            }
    }

        public static void RemoveProduct(int index)
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip product can not be removed on " + App.State.CurrentPage);
                Trip.TripProducts[index].RemoveProductButton.Click();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in removing trip product.", exception);
            }
        }

        public static void CheckoutTrip()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Trip can not be checkout on " + App.State.CurrentPage);
                Trip.CheckoutTripButton.Click();
                App.PassengerInfoPage.WaitForPageLoad();
                App.State.CurrentPage = "PassengerInfoPage";
            }
            catch (Exception exception)
            {
                throw new Exception("Error in trip checkout.", exception);
            }
        }

        public static void ContinueShopping()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("TripFolderPage"))
                    throw new Exception("Can not able to continue shopping on " + App.State.CurrentPage);
                Trip.ContinueShoppingButton.Click();
            }
            catch (Exception exception)
            {
                throw new Exception("Error in continue shopping", exception);
            }
        }

        #endregion

        #region Air Filters Call

        public static void SetAirFilters()
        {
            try
            {
                var airPostSearchFilters = new AirPostSearchFilters()
                {
                    PriceRange = new PriceRange(){Max = 20,Min = 20},
                    TakeOffTimeRange = new TakeOffTimeRange(){Min = 20,Max = 80},
                    LandingTimeRange = new LandingTimeRange(){Min = 20,Max = 80},
                    MaxTimeDurationDiff = 4,
                    Stop = "one", //field can be none/one/one-plus
                    CabinTypes = new List<string>(){"economy","business"},
                    Airlines = new List<string>(){"AA","NK","UA","US"}
                };

                App.ResultsPage.SetAirFilters(airPostSearchFilters);
            }
            catch (Exception exception)
            {
                throw new Exception("Error in setting air filters", exception);
            }
        }

        public static void SetMatrixAirline()
        {
            try
            {
                App.ResultsPage.SetMatrixAirline("Spirit Airlines");
            }
            catch (Exception exception)
            {
                throw new Exception("Error in setting airlines from matrix", exception);
        }
        }

        #endregion

        public static void PayNow()
        {
            try
            {
                if (!App.State.CurrentPage.Equals("CheckOutPage"))
                    throw new Exception("Pay-Now is not available on " + App.State.CurrentPage);
                if (_criteria.PaymentMode == PaymentMode.RoviaBucks)
                    App.CheckoutPage.PayNow(new PaymentInfo(_criteria.PaymentMode));
                else
                {
                    App.CheckoutPage.PayNow(_criteria.PaymentMode);
                    App.BFCPaymentPage.WaitForLoad();
                    App.State.CurrentPage = "BFCPaymentPage";
                    App.BFCPaymentPage.PayNow(new PaymentInfo(_criteria.PaymentMode,_criteria.CardType));
                }
                App.CheckoutPage.CheckPaymentStatus();

            }
            catch (Exception exception)
            {

                throw new Exception("Pay Now failed",exception);
            }
        }

        
    }

}
